namespace XperienceCommunity.ChannelSettings.Repositories
{
    public interface IChannelCustomSettingsRepository
    {
        Task<string> GetStringValueAsync(string key, string defaultValue = "", string? channelName = null);

        Task<bool> GetBoolValueAsync(string key, bool defaultValue = false, string? channelName = null);

        Task<int> GetIntegerValueAsync(string key, int defaultValue = 0, string? channelName = null);

        Task<T?> GetValueAsync<T>(string key, T? defaultValue = default, string? channelName = null);

        Task<T> GetSettingsModel<T>(string? channelName = null) where T : new();
        
        Task<T> GetSettingsModel<T>(int channelId) where T : new();

        string[] GetSettingModelDependencyKeys<T>() where T : new();
    }
}
