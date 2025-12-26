using System;

namespace OrderProcessing.Domain.Models
{
    public record Quantity
    {
        public int Value { get; }

        public Quantity(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(value));

            Value = value;
        }

        public static Quantity operator +(Quantity a, Quantity b) 
            => new(a.Value + b.Value);

        public override string ToString() => Value.ToString();
    }
}
