using Temporalio.Client;
using Temporalio.Worker;
using TemporalioSamples.Rocky;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var client = await TemporalClient.ConnectAsync(new("localhost:7233"));
        await Subcommand(args.ElementAtOrDefault(0)!)(client);
    }

    private static async Task<bool> ExecuteWorkflowAsync(TemporalClient client)
    {
        await client.ExecuteWorkflowAsync(
            (RockyWorkflow wf) => wf.RunAsync(),
            new(id: "rocky-id", taskQueue: "rocky"));

        return true;
    }

    private static async Task<bool> RunWorkerAsync(TemporalClient client)
    {
         using var tokenSource = new CancellationTokenSource();
         Console.CancelKeyPress += (_, eventArgs) =>
         {
              tokenSource.Cancel();
              eventArgs.Cancel = true;
         };

         using var worker = new TemporalWorker(
             client,
             new TemporalWorkerOptions(taskQueue: "rocky").
                 AddActivity(RockyActivities.Always).
                 AddActivity(RockyActivities.Never).
                 AddWorkflow<RockyWorkflow>());

         try
         {
             await worker.ExecuteAsync(tokenSource.Token);
             return true;
         }
         catch (OperationCanceledException)
         {
             Console.WriteLine("Worker cancelled");
             return false;
         }
    }

    private static Func<TemporalClient, Task<bool>> Subcommand(string subcommand)
    {
        switch (subcommand)
        {
            case "worker":
                return RunWorkerAsync;
            case "workflow":
                return ExecuteWorkflowAsync;
            default:
                throw new ArgumentException("Must pass 'worker' or 'workflow' as the single argument");
        }
    }
}