using System.ComponentModel.DataAnnotations;
using MyApi.Models;

namespace MyApi.Tests;

public class UnitTestOrderItems
{
    [Fact]
    public void OrderItem_WithPositiveAmount_IsValid()
    {
        // Arrange: maak een order-item met een hoeveelheid van minimaal 1.
        var orderItem = CreateOrderItem(amount: 1);
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(orderItem);

        // Act: controleer de validatieregels op het model.
        var isValid = Validator.TryValidateObject(
            orderItem,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Assert: een positieve hoeveelheid moet geldig zijn.
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void OrderItem_WithZeroAmount_IsInvalid()
    {
        // Arrange: maak een order-item met een ongeldige hoeveelheid van 0.
        var orderItem = CreateOrderItem(amount: 0);
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(orderItem);

        // Act: controleer de validatieregels op het model.
        var isValid = Validator.TryValidateObject(
            orderItem,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Assert: de validatie moet de hoeveelheid afkeuren.
        Assert.False(isValid);
        Assert.Contains(
            validationResults,
            result => result.MemberNames.Contains(nameof(OrderItem.amount)));
    }

    private static OrderItem CreateOrderItem(int amount)
    {
        return new OrderItem
        {
            id = 1,
            order_id = 10,
            item_id = 20,
            amount = amount
        };
    }
}