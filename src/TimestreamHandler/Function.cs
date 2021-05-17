// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace TimestreamHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Amazon;
    using Amazon.Lambda.APIGatewayEvents;
    using Amazon.Lambda.Core;
    using Amazon.TimestreamWrite;
    using Amazon.TimestreamWrite.Model;
    using TimestreamHandler.Models;


    public class Function
    {
        /// <summary>
        /// A function that takes an <see cref="APIGatewayProxyRequest"/> and inserts the data to Timestream
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<Response> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var response = new Response();
            var records = new List<Record>();
            var dimensions = new List<Dimension>();

            try
            {
                LambdaLogger.Log($"Function Started: {context.FunctionName}");

                var request = JsonSerializer.Deserialize<Request>(input.Body);

                dimensions.Add(new Dimension
                {
                    Name = "project",
                    Value = "Project Shield"
                });

                foreach (var stepData in request.Values)
                {
                    var step = new Record
                    {
                        Dimensions = dimensions,
                        MeasureName = "steps",
                        MeasureValue = stepData.Steps.ToString(),
                        MeasureValueType = MeasureValueType.DOUBLE,
                        Time = stepData.Time.ToString(),
                        TimeUnit = TimeUnit.SECONDS
                    };

                    records.Add(step);
                }

                var client = new AmazonTimestreamWriteClient(new AmazonTimestreamWriteConfig
                {
                    RegionEndpoint = RegionEndpoint.USWest2,
                    Timeout = TimeSpan.FromSeconds(20),
                    MaxErrorRetry = 10
                });

                _ = await client.WriteRecordsAsync(new WriteRecordsRequest
                {
                    DatabaseName = "ProjectShieldTimestreamDatabase",
                    TableName = "ProjectShieldTimestreamTable",
                    Records = records
                });

                response.Data = records;
                response.Success = true;
            }
            catch (RejectedRecordsException ex)
            {
                var rejectedRecords = $"Rejected Records: {string.Join(",", ex.RejectedRecords.Select(r => $"{r.RecordIndex}:{r.Reason}"))}";

                LambdaLogger.Log($"RejectedRecordsException: {ex.Message}");
                LambdaLogger.Log($"RejectedRecordsException: {rejectedRecords}");

                response.Data = records;
                response.Success = false;
                response.Message = $"{rejectedRecords}{Environment.NewLine}{ex.Message}";
                response.StackTrace = ex.StackTrace;
            }
            catch (Exception ex)
            {
                LambdaLogger.Log($"Exception: {ex.Message}");

                response.Data = records;
                response.Success = false;
                response.Message = ex.Message;
                response.StackTrace = ex.StackTrace;
            }
            finally
            {
                LambdaLogger.Log($"Function Finished: {context.FunctionName}");
            }

            return response;
        }
    }
}
