namespace XperienceCommunity.ChannelSettings;

public partial class ChannelCustomSettingInfo
{
    static ChannelCustomSettingInfo()
    {
        TYPEINFO.ContinuousIntegrationSettings.Enabled = true;
        TYPEINFO.ContinuousIntegrationSettings.ObjectFileNameFields.Add(nameof(ChannelCustomSettingKeyName));
    }
}
