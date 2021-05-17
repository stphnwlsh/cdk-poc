namespace TimestreamHandler.Models
{
    using System.Collections.Generic;
    using Amazon.TimestreamWrite.Model;

    public class Response
    {
        public List<Record> Data { get; set; } = null;

        public bool Success { get; set; } = false;

        public string Message { get; set; } = null;

        public string StackTrace { get; set; } = null;
    }
}
