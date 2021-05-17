namespace Infrastructure
{
    using Amazon.CDK;
    using Infrastructure.Props;
    using Infrastructure.Stacks;

    internal static class Program
    {
        public static void Main(string[] args)
        {
            var props = new SharedProps
            {
                Name = "ProjectShield",
                NameWithSpaces = "Project Shield",
                NameWithDashes = "Project-Shield",
            };

            var app = new App();

            // Stack - Shared //
            var shared = new SharedStack(app, $"{props.Name}SharedStack", props);

            // Stack - Integrations
            _ = new TimestreamStack(app, $"{props.Name}TimestreamStack", shared.SharedProps);

            _ = app.Synth();
        }
    }
}
