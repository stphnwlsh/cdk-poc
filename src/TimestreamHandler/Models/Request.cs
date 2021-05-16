namespace TimestreamHandler.Models
{
    using System.Text.Json.Serialization;

    public class Request
    {
        [JsonPropertyName("values")]
        public StepData[] Values { get; set; }
    }
}
