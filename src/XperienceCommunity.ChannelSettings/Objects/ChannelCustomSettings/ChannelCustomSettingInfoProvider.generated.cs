using CMS.DataEngine;

namespace XperienceCommunity.ChannelSettings
{
    /// <summary>
    /// Class providing <see cref="ChannelCustomSettingInfo"/> management.
    /// </summary>
    [ProviderInterface(typeof(IChannelCustomSettingInfoProvider))]
    public partial class ChannelCustomSettingInfoProvider : AbstractInfoProvider<ChannelCustomSettingInfo, ChannelCustomSettingInfoProvider>, IChannelCustomSettingInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelCustomSettingInfoProvider"/> class.
        /// </summary>
        public ChannelCustomSettingInfoProvider()
            : base(ChannelCustomSettingInfo.TYPEINFO)
        {
        }
    }
}