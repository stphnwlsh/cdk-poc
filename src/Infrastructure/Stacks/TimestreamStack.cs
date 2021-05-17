namespace Infrastructure.Stacks
{
    using Amazon.CDK;
    using Amazon.CDK.AWS.APIGatewayv2;
    using Amazon.CDK.AWS.APIGatewayv2.Integrations;
    using Amazon.CDK.AWS.IAM;
    using Amazon.CDK.AWS.Lambda;
    using Amazon.CDK.AWS.Timestream;
    using Infrastructure.Props;

    public class TimestreamStack : Stack
    {
        internal TimestreamStack(Construct scope, string id, SharedProps props = null) : base(scope, id, props)
        {
            // Timestream Lambda //
            var handler = new Function(this, $"{props.NameWithDashes}-Timestream-Handler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("./../TimestreamHandler/bin/Release/netcoreapp3.1/publish"),
                Handler = "TimestreamHandler::TimestreamHandler.Function::FunctionHandler",
                Timeout = Duration.Seconds(15)
            });

            // Timestream Lambda - Execution Policy //
            handler.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Resources = new[] { "*" },
                Actions = new[] { "timestream:*" }
            }));

            // Timestream Database //
            var db = new CfnDatabase(this, $"{props.NameWithDashes}-Timestream-Database", new CfnDatabaseProps
            {
                DatabaseName = $"{props.Name}TimestreamDatabase",
            });

            // Timestream Database Table //
            var table = new CfnTable(this, "Project-Shield-Table", new CfnTableProps()
            {
                TableName = $"{props.Name}TimestreamTable",
                DatabaseName = db.DatabaseName
            });
            table.AddDependsOn(db);

            // Timestream Lambda API Integration //
            var lambdaProxyIntegration = new LambdaProxyIntegration(new LambdaProxyIntegrationProps()
            {
                Handler = handler,
                PayloadFormatVersion = PayloadFormatVersion.VERSION_2_0
            });

            // Timestream Lambda API Route //
            _ = props.SharedApi.AddRoutes(new AddRoutesOptions()
            {
                Path = "/timestream",
                Integration = lambdaProxyIntegration,
                Methods = new[] { HttpMethod.POST }
            });
        }
    }
}
