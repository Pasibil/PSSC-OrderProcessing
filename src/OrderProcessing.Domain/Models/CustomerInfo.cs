using System;
using System.Text.RegularExpressions;

namespace OrderProcessing.Domain.Models
{
    public record CustomerInfo
    {
        public string Name { get; }
        public string Email { get; }

        public CustomerInfo(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                throw new ArgumentException("Invalid email address", nameof(email));

            Name = name;
            Email = email;
        }

        private static bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

        public override string ToString() => $"{Name} ({Email})";
    }
}
