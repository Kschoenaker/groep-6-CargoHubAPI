using System.Text.Json;
using System.Text.RegularExpressions;
using Npgsql;
using NpgsqlTypes;

namespace MyApi.Data;

public static partial class BusinessDataSeeder
{
    private sealed record Dataset(
        string FileName,
        string TableName,
        string[] ConflictColumns);

    private static readonly Dataset[] Datasets =
    [
        new("client.json", "clients", ["id"]),
        new("warehouse.json", "warehouses", ["id"]),
        new("supplier.json", "suppliers", ["id"]),
        new("item_group.json", "item_groups", ["id"]),
        new("item_line.json", "item_lines", ["id"]),
        new("item_type.json", "item_types", ["id"]),
        new("item.json", "items", ["id"]),
        new("location.json", "locations", ["id"]),
        new("inventory.json", "inventories", ["item_id", "location_id"]),
        new("order.json", "orders", ["id"]),
        new("order_item.json", "order_items", ["id"]),
        new("shipment.json", "shipments", ["id"]),
        new("shipment_item.json", "shipment_items", ["id"]),
        new("transfer.json", "transfers", ["id"]),
        new("transfer_item.json", "transfer_items", ["id"])
    ];

    public static async Task SeedAsync(
        string connectionString,
        string contentRoot,
        CancellationToken cancellationToken = default)
    {
        var sourceDirectory = Path.Combine(contentRoot, "Data", "JsonBronData");
        if (!Directory.Exists(sourceDirectory))
        {
            throw new DirectoryNotFoundException(
                $"De map met JSON-brondata bestaat niet: {sourceDirectory}");
        }

        var datasets = Datasets.Select(dataset =>
        {
            var filePath = Path.Combine(sourceDirectory, dataset.FileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    $"Het bronbestand '{dataset.FileName}' bestaat niet.",
                    filePath);
            }

            using var document = JsonDocument.Parse(File.ReadAllText(filePath));
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidDataException(
                    $"Het bronbestand '{dataset.FileName}' moet een lijst records bevatten.");
            }

            var firstRecord = document.RootElement
                .EnumerateArray()
                .FirstOrDefault();
            if (firstRecord.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidDataException(
                    $"Het bronbestand '{dataset.FileName}' bevat geen records.");
            }

            var columns = firstRecord
                .EnumerateObject()
                .Select(property => property.Name)
                .ToArray();

            return (Dataset: dataset, FilePath: filePath, Columns: columns);
        }).ToArray();

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        foreach (var (dataset, filePath, columns) in datasets)
        {
            await ValidateTableAsync(
                connection,
                transaction,
                dataset,
                columns,
                cancellationToken);
        }

        var totalRecords = 0;
        foreach (var (dataset, filePath, columns) in datasets)
        {
            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            var updateColumns = columns
                .Except(dataset.ConflictColumns, StringComparer.Ordinal)
                .ToArray();

            var insertColumnsSql = string.Join(", ", columns.Select(QuoteIdentifier));
            var conflictColumnsSql = string.Join(", ", dataset.ConflictColumns.Select(QuoteIdentifier));
            var updateSql = updateColumns.Length == 0
                ? "DO NOTHING"
                : $"DO UPDATE SET {string.Join(", ", updateColumns.Select(column =>
                    $"{QuoteIdentifier(column)} = EXCLUDED.{QuoteIdentifier(column)}"))}";

            var sql = $"""
                       INSERT INTO {QuoteIdentifier(dataset.TableName)} ({insertColumnsSql})
                       SELECT {string.Join(", ", columns.Select(column => $"record.{QuoteIdentifier(column)}"))}
                       FROM jsonb_populate_recordset(
                           NULL::{QuoteIdentifier(dataset.TableName)},
                           @records) AS record
                       ON CONFLICT ({conflictColumnsSql}) {updateSql};
                       """;

            await using var command = new NpgsqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("records", NpgsqlDbType.Jsonb, json);
            var affected = await command.ExecuteNonQueryAsync(cancellationToken);
            totalRecords += affected;
            Console.WriteLine($"{dataset.FileName}: {affected} records verwerkt.");
        }

        await transaction.CommitAsync(cancellationToken);
        Console.WriteLine(
            $"Import voltooid: {totalRecords} records verwerkt uit {datasets.Length} bedrijfsbestanden. user.json is overgeslagen.");
    }

    private static async Task ValidateTableAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        Dataset dataset,
        IReadOnlyCollection<string> sourceColumns,
        CancellationToken cancellationToken)
    {
        await using var command = new NpgsqlCommand(
            """
            SELECT column_name
            FROM information_schema.columns
            WHERE table_schema = current_schema()
              AND table_name = @table_name;
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("table_name", NpgsqlDbType.Text, dataset.TableName);

        var tableColumns = new HashSet<string>(StringComparer.Ordinal);
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                tableColumns.Add(reader.GetString(0));
            }
        }

        if (tableColumns.Count == 0)
        {
            throw new InvalidOperationException(
                $"De tabel '{dataset.TableName}' bestaat niet in de huidige PostgreSQL-schema.");
        }

        var missingColumns = sourceColumns
            .Where(column => !tableColumns.Contains(column))
            .ToArray();
        if (missingColumns.Length > 0)
        {
            throw new InvalidOperationException(
                $"De tabel '{dataset.TableName}' mist kolommen uit '{dataset.FileName}': {string.Join(", ", missingColumns)}.");
        }

        var missingConflictColumns = dataset.ConflictColumns
            .Where(column => !tableColumns.Contains(column))
            .ToArray();
        if (missingConflictColumns.Length > 0)
        {
            throw new InvalidOperationException(
                $"De tabel '{dataset.TableName}' mist de conflictkolommen: {string.Join(", ", missingConflictColumns)}.");
        }
    }

    private static string QuoteIdentifier(string identifier)
    {
        if (!IdentifierPatternRegex.IsMatch(identifier))
        {
            throw new InvalidOperationException($"Ongeldige database-identificatie: {identifier}");
        }

        return $"\"{identifier}\"";
    }

    private static readonly Regex IdentifierPatternRegex =
        new("^[a-z_][a-z0-9_]*$", RegexOptions.CultureInvariant);
}
