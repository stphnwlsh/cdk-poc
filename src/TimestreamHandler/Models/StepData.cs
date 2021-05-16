namespace TimestreamHandler.Models
{
    using System.Text.Json.Serialization;

    public class StepData
    {
        [JsonPropertyName("steps")]
        public int Steps { get; set; }

        [JsonPropertyName("time")]
        public long Time { get; set; }
    }
}
