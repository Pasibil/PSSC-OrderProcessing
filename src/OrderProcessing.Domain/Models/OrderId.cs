using System;

namespace OrderProcessing.Domain.Models
{
    public record OrderId
    {
        public Guid Value { get; }

        private OrderId(Guid value)
        {
            Value = value;
        }

        public static OrderId NewOrderId() => new(Guid.NewGuid());
        
        public static OrderId From(Guid value) => new(value);

        public override string ToString() => Value.ToString();
    }
}
