using OrderProcessing.Domain.Exceptions;
using System;

namespace OrderProcessing.Domain.Models
{
    public record ProductCode
    {
        public string Value { get; }

        public ProductCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidProductCodeException("Product code cannot be empty");
            
            if (value.Length < 3 || value.Length > 20)
                throw new InvalidProductCodeException("Product code must be between 3 and 20 characters");

            Value = value.ToUpperInvariant();
        }

        public override string ToString() => Value;
    }
}
