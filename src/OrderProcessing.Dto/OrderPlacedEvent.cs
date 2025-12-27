using System;
using System.Collections.Generic;

namespace OrderProcessing.Dto
{
    public record OrderPlacedEvent(
        Guid OrderId,
        string CustomerName,
        string CustomerEmail,
        List<OrderPlacedLineDto> OrderLines,
        decimal TotalAmount)
    {
        public override string ToString()
        {
            return $"OrderPlacedEvent {{ OrderId = {OrderId}, CustomerName = {CustomerName}, CustomerEmail = {CustomerEmail}, TotalAmount = {TotalAmount}, OrderLines = [{string.Join(", ", OrderLines)}] }}";
        }
    }

    public record OrderPlacedLineDto(
        string ProductCode,
        int Quantity,
        decimal UnitPrice)
    {
        public override string ToString()
        {
            return $"OrderPlacedLineDto {{ ProductCode = {ProductCode}, Quantity = {Quantity}, UnitPrice = {UnitPrice} }}";
        }
    }
}
