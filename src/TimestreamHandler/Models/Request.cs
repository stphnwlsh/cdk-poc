namespace TimestreamHandler.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class Request
    {
        [JsonPropertyName("values")]
        public List<StepData> Values { get; set; }
    }
}
