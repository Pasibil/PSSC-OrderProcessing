using System;

namespace OrderProcessing.Domain.Models
{
    public record Price
    {
        public decimal Value { get; }

        public Price(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Price cannot be negative", nameof(value));

            Value = value;
        }

        public static Price operator *(Price price, int quantity) 
            => new(price.Value * quantity);

        public override string ToString() => $"{Value:F2}";
    }
}
