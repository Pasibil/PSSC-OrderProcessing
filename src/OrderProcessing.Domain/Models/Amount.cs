using System;

namespace OrderProcessing.Domain.Models
{
    public record Amount
    {
        public decimal Value { get; }

        public Amount(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(value));

            Value = value;
        }

        public static Amount operator +(Amount a, Amount b) 
            => new(a.Value + b.Value);

        public override string ToString() => $"{Value:F2}";
    }
}
