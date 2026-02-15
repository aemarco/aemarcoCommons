namespace aemarcoCommons.ToolboxVlc.Mapping;

internal static class VlcMappings
{
    public static VlcStatus ToVlcStatus(this VlcClient.StatusResponse response)
    {
        return new VlcStatus(
            response.Time,
            response.Volume,
            response.Length,
            response.State);
    }

}
