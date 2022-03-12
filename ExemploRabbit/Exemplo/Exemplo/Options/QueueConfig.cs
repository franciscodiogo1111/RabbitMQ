namespace Exemplo.Options
{
    public class QueueConfig
    {
        public string PrefixForRetriable { get; set; }
        public int MaxRetries { get; set; }
        public int TtlTimeout { get; set; }
    }
}
