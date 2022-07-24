# Workflow Core with DSL

Check out the CodeOmelet blog post for this project.

Link: https://codeomelet.com/posts/workflow-core-with-dsl

---

#### Initial Setup

The WorkflowCore package required .NET Core 2.0 framework.
```
dotnet new console -o workflow-start
dotnet run
```

#### Nuget Packages
```
dotnet add package WorkflowCore
dotnet add package WorkflowCore.DSL
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add package Microsoft.Extensions.Logging
```

#### Starting Up
One have to create the steps first then the workflow and then finally register the workflow to the host on the main file:

1. Create Steps
2. Create Workflow
3. Register and start the Workflow from the main program file

#### 1. Create Steps
Create steps folder and start adding step files: 
```
public class Initialize : StepBody
{
    public double BaseAmount { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        Console.WriteLine("Initialize");
        return ExecutionResult.Next();
    }
}
```

#### 2. Create Workflow (.yml)
```
Id: ProcessPaymentWorkflow
Version: 1
Steps:
  - Id: Initialize
    StepType: Initialize, workflow-start-dsl
    NextStepId: ApplyDiscount
  - Id: ApplyDiscount
    StepType: ApplyDiscount, workflow-start-dsl
    NextStepId: ApplyShipping
  - Id: ApplyShipping
    StepType: ApplyShipping, workflow-start-dsl
    NextStepId: Finalize
  - Id: Finalize
    StepType: Finalize, workflow-start-dsl
```

#### 3. Register and start the Workflow from the main program file
```
var serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddWorkflow()
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
```