namespace TemporalioSamples.Rocky;

using Temporalio.Workflows;

[Workflow]
public class RockyWorkflow
{
    [WorkflowRun]
    public async Task<bool> RunAsync()
    {
        var result = await Workflow.ExecuteActivityAsync(
            () => RockyActivities.Always(),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
            });

        return result;
    }
}