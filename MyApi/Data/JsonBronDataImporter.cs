using System.Text.Json;
using Npgsql;
using NpgsqlTypes;

namespace MyApi.Data;

public static class JsonBronDataImporter
{
    public static async Task ImportAsync(
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

        var files = Directory.GetFiles(sourceDirectory, "*.json");
        if (files.Length == 0)
        {
            throw new InvalidDataException(
                $"Er zijn geen JSON-bestanden gevonden in '{sourceDirectory}'.");
        }

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        await using (var createTable = new NpgsqlCommand(
                         """
                         CREATE TABLE IF NOT EXISTS json_brondata (
                             source_file text NOT NULL,
                             record_index integer NOT NULL,
                             data jsonb NOT NULL,
                             PRIMARY KEY (source_file, record_index)
                         );
                         """,
                         connection,
                         transaction))
        {
            await createTable.ExecuteNonQueryAsync(cancellationToken);
        }

        var totalRecords = 0;
        foreach (var file in files)
        {
            var json = await File.ReadAllTextAsync(file, cancellationToken);
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidDataException(
                    $"Het JSON-bestand '{Path.GetFileName(file)}' moet een lijst records bevatten.");
            }

            var fileName = Path.GetFileName(file);
            await using (var clearRecords = new NpgsqlCommand(
                """
                DELETE FROM json_brondata
                WHERE source_file = @source_file;
                """,
                connection,
                transaction))
            {
                clearRecords.Parameters.AddWithValue("source_file", NpgsqlDbType.Text, fileName);
                await clearRecords.ExecuteNonQueryAsync(cancellationToken);
            }

            await using var importRecords = new NpgsqlCommand(
                """
                INSERT INTO json_brondata (source_file, record_index, data)
                SELECT @source_file, (record.ordinality - 1)::integer, record.data
                FROM jsonb_array_elements(@records) WITH ORDINALITY AS record(data, ordinality);
                """,
                connection,
                transaction);
            importRecords.Parameters.AddWithValue("source_file", NpgsqlDbType.Text, fileName);
            importRecords.Parameters.AddWithValue("records", NpgsqlDbType.Jsonb, json);
            var importedRecords = await importRecords.ExecuteNonQueryAsync(cancellationToken);
            totalRecords += importedRecords;
            Console.WriteLine($"{fileName}: {importedRecords} records geïmporteerd.");
        }

        await transaction.CommitAsync(cancellationToken);
        Console.WriteLine($"Import voltooid: {totalRecords} records uit {files.Length} bestanden.");
    }
}
