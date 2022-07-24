var serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddWorkflow()
    .AddWorkflowDSL()
    .BuildServiceProvider();

var host = serviceProvider.GetService<IWorkflowHost>();
if (host == null)
    throw new Exception("Host not initialized");

LoadDefinition(serviceProvider);

host.Start();

host.StartWorkflow("ProcessPaymentWorkflow");

Console.ReadLine();

host.Stop();

static void LoadDefinition(IServiceProvider serviceProvider)
{
    var type = Deserializers.Yaml;
    var file = File.ReadAllText($"Workflows/ProcessPayment/ProcessPaymentWorkflow.{(type == Deserializers.Yaml ? "yml" : "json")}");

    var loader = serviceProvider.GetService<IDefinitionLoader>();
    if (loader == null)
        throw new Exception("Loader not initialized");

    loader.LoadDefinition(file, type);
}