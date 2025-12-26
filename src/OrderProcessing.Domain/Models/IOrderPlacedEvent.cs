namespace OrderProcessing.Domain.Models
{
    public interface IOrderPlacedEvent { }

    public record OrderPlacedSuccessEvent(PlacedOrder Order) : IOrderPlacedEvent;
    
    public record OrderPlacedFailedEvent(string Reason) : IOrderPlacedEvent;
}
