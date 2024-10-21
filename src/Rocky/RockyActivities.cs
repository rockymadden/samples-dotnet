namespace TemporalioSamples.Rocky;

using Temporalio.Activities;

public static class RockyActivities
{
    [Activity]
    public static bool Always() => true;

    [Activity]
    public static bool Never() => false;
}