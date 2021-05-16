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
        public async Task<Response> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            LambdaLogger.Log($"Function Started: {context.FunctionName}");

            var records = new List<Record>();
            var dimensions = new List<Dimension>();

            try
            {
                var input = JsonSerializer.Deserialize<Request>(request.Body);

                dimensions.Add(new Dimension
                {
                    Name = "project",
                    Value = "Project Shield"
                });

                foreach (var stepData in input.Values)
                {
                    var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(stepData.Time);
                    var time = dateTimeOffset.ToUnixTimeMilliseconds().ToString();

                    var step = new Record
                    {
                        Dimensions = dimensions,
                        MeasureName = "steps",
                        MeasureValue = stepData.Steps.ToString(),
                        MeasureValueType = MeasureValueType.DOUBLE,
                        Time = time
                    };

                    records.Add(step);
                }

                var writeClient = new AmazonTimestreamWriteClient(new AmazonTimestreamWriteConfig
                {
                    RegionEndpoint = RegionEndpoint.USWest2,
                    Timeout = TimeSpan.FromSeconds(20),
                    MaxErrorRetry = 10
                });

                var response = await writeClient.WriteRecordsAsync(new WriteRecordsRequest
                {
                    DatabaseName = "ProjectShieldTimestreamDatabase",
                    TableName = "ProjectShieldTimestreamTable",
                    Records = records
                });

                return new Response
                {
                    Data = records,
                    Success = true
                };
            }
            catch (RejectedRecordsException ex)
            {
                LambdaLogger.Log($"RejectedRecordsException: {ex.Message}");

                foreach (var rejectedRecord in ex.RejectedRecords)
                {
                    LambdaLogger.Log($"RejectedRecordsException : RejectedRecordIndex {rejectedRecord.RecordIndex} : {rejectedRecord.Reason}");
                }

                return new Response
                {
                    Data = records,
                    Message = $"Rejected Records Indexes: {string.Join(",", ex.RejectedRecords.Select(r => r.RecordIndex))}",
                    Exception = ex
                };
            }
            catch (Exception ex)
            {
                LambdaLogger.Log($"Exception: {ex.Message}");

                return new Response
                {
                    Data = records,
                    Message = ex.Message,
                    Exception = ex
                };
            }
            finally
            {
                LambdaLogger.Log($"Function Finished: {context.FunctionName}");
            }
        }
    }
}
