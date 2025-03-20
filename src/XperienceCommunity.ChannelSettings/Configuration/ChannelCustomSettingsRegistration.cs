using XperienceCommunity.ChannelSettings.Repositories;
using XperienceCommunity.ChannelSettings.Repositories.Implementation;
using Microsoft.Extensions.DependencyInjection;
using XperienceCommunity.ChannelSettings.Installation;

namespace XperienceCommunity.ChannelSettings.Configuration
{
    public static class ChannelCustomSettingsRegistration
    {
        public static IServiceCollection AddChannelCustomSettings(this IServiceCollection services)
        {
            return services
                .AddSingleton<ChannelSettingsInstaller>()
                .AddSingleton<IChannelSettingsInternalHelper, ChannelSettingsInternalHelper>()
                .AddSingleton<IChannelCustomSettingsRepository, ChannelCustomSettingsRepository>();
        }
    }
}