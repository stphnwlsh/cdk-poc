namespace Infrastructure.Stacks
{
    using Amazon.CDK;
    using Amazon.CDK.AWS.APIGatewayv2;
    using Infrastructure.Props;

    public class SharedStack : Stack
    {
        public SharedProps SharedProps { get; }

        internal SharedStack(Construct scope, string id, SharedProps props = null) : base(scope, id, props)
        {
            var api = new HttpApi(this, $"{props.NameWithDashes}-Service", new HttpApiProps
            {
                ApiName = $"{props.NameWithSpaces} Service",
                Description = $"This API is shared within {id}"
            });

            props.SharedApi = api;

            this.SharedProps = props;
        }
    }
}
