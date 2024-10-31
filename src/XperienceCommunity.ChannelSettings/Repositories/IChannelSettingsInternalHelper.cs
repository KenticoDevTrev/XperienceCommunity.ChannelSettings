using System.Reflection;

namespace XperienceCommunity.ChannelSettings.Repositories
{
    /// <summary>
    /// Helpers that are used for admin and repository data handling
    /// </summary>
    public interface IChannelSettingsInternalHelper
    {
        Task<ChannelCustomSettingInfo> InsertOrUpdatedSettingsKey(string key, string? value, int channelId);

        Task<ChannelCustomSettingInfo?> GetSettingsKey(string key, int channelId);

        object? ParseValue(Type type, string value, object? defaultValue = default);

        string? ParseObject(Type type, object? value);

        IEnumerable<PropertyInfo>? GetAllPropertiesWithAttribute(Type objectType, Type attributeType);

        void SetPropertyValue(object obj, string propertyName, object? val);

        object? GetPropertyValue(object obj, string propertyName);

        Task<int> GetChannelId(string? channelName);
        Task<Dictionary<int, Dictionary<string, ChannelCustomSettingInfo>>> GetChannelToSettingsKeyToInfo();
        Task<Dictionary<int, string>> GetChannelIdToName();
    }
}
