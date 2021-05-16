namespace TimestreamHandler.Models
{
    using System;
    using System.Collections.Generic;
    using Amazon.TimestreamWrite.Model;

    public class Response
    {
        public List<Record> Data { get; set; }

        public bool Success { get; set; } = false;

        public string Message { get; set; }

        public Exception Exception { get; set; } = null;
    }
}
