//Deze unit tests controleren of de validatie van een shipment werkt de happy en unhappy path

using System.ComponentModel.DataAnnotations; // onderdeel van .NET
using MyApi.Models;

namespace MyApi.Tests;

public class UnitTestShipments
{
    [Fact]
    public void Shipment_WithStatus_IsValid()
    {
        // maak een shipment met een ingevulde status
        var shipment = CreateShipment(shipment_status: "Pending");
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(shipment); //geeft de validator informatie over het object dat gecontroleerd wordt

        // controleer de validatieregels op het model, dus in models shipments.cs
        var isValid = Validator.TryValidateObject( //voert de validatie uit
            shipment,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // een ingevulde status moet geldig zijn
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void Shipment_WithoutStatus_IsInvalid()
    {
        // maak een shipment zonder status
        var shipment = CreateShipment(shipment_status: "");
        var validationResults = new List<ValidationResult>(); // een lijst met validatiefouten
        var validationContext = new ValidationContext(shipment);

        // controleer de validatieregels op het model
        var isValid = Validator.TryValidateObject(
            shipment,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // de test slaagt als er geen status is ingevoerd
        Assert.False(isValid);
        Assert.Contains(
            validationResults,
            result => result.MemberNames.Contains(nameof(Shipments.shipment_status)));
    }

    private static Shipments CreateShipment(string shipment_status)
    {
        // vult de overige velden in
        return new Shipments
        {
            id = 1,
            reference = "TEST-001",
            order_id = 10,
            shipment_date = DateTime.UtcNow,
            shipment_type = "Outbound",
            shipment_status = shipment_status,
            carrier_name = "Test Carrier",
            shipping_method = "Standard",
            payment_type = "Prepaid",
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }
}