namespace OrderProcessing.Events.Models
{
    public enum EventProcessingResult
    {
        Completed,
        Abandoned,
        DeadLettered
    }
}
