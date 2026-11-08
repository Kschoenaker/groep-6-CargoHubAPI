using System.ComponentModel.DataAnnotations;
using MyApi.Models;

namespace MyApi.Tests;

public class UnitTestShipmentItems
{
    [Fact]
    public void ShipmentItem_WithPositiveAmount_IsValid()
    {
        // Arrange: maak een shipment-item met een hoeveelheid van minimaal 1.
        var shipmentItem = CreateShipmentItem(amount: 1);
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(shipmentItem);

        // Act: controleer de validatieregels op het model.
        var isValid = Validator.TryValidateObject(
            shipmentItem,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Assert: een positieve hoeveelheid moet geldig zijn.
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void ShipmentItem_WithZeroAmount_IsInvalid()
    {
        // Arrange: maak een shipment-item met een ongeldige hoeveelheid van 0.
        var shipmentItem = CreateShipmentItem(amount: 0);
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(shipmentItem);

        // Act: controleer de validatieregels op het model.
        var isValid = Validator.TryValidateObject(
            shipmentItem,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Assert: de validatie moet de hoeveelheid afkeuren.
        Assert.False(isValid);
        Assert.Contains(
            validationResults,
            result => result.MemberNames.Contains(nameof(ShipmentItems.amount)));
    }

    private static ShipmentItems CreateShipmentItem(int amount)
    {
        return new ShipmentItems
        {
            id = 1,
            shipment_id = 10,
            item_id = 20,
            amount = amount
        };
    }
}