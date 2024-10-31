using CMS.DataEngine;

namespace XperienceCommunity.ChannelSettings
{
    /// <summary>
    /// Declares members for <see cref="ChannelCustomSettingInfo"/> management.
    /// </summary>
    public partial interface IChannelCustomSettingInfoProvider : IInfoProvider<ChannelCustomSettingInfo>, IInfoByIdProvider<ChannelCustomSettingInfo>, IInfoByGuidProvider<ChannelCustomSettingInfo>
    {
    }
}