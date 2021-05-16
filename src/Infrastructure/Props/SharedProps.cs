using Amazon.CDK;
using Amazon.CDK.AWS.APIGatewayv2;

namespace Infrastructure.Props
{
    public class SharedProps : StackProps
    {
        public string Name { get; set; }

        public string NameWithSpaces { get; set; }

        public string NameWithDashes { get; set; }

        public HttpApi SharedApi { get; set; }
    }
}
