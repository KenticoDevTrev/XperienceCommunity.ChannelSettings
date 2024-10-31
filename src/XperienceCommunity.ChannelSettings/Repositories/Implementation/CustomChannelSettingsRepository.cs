using System.Reflection;
using XperienceCommunity.ChannelSettings.Attributes;

namespace XperienceCommunity.ChannelSettings.Repositories.Implementation
{
    public class ChannelCustomSettingsRepository(IChannelSettingsInternalHelper channelSettingsInternalHelper) : IChannelCustomSettingsRepository
    {
        private readonly IChannelSettingsInternalHelper _channelSettingsInternalHelper = channelSettingsInternalHelper;

        public async Task<bool> GetBoolValueAsync(string key, bool defaultValue = false, string? channelName = null)
        {
            var value = await GetSettingsItemAsync(key, (await _channelSettingsInternalHelper.GetChannelId(channelName)));
            if (!string.IsNullOrWhiteSpace(value)) {
                return ((bool?)_channelSettingsInternalHelper.ParseValue(typeof(bool), value, defaultValue)) ?? defaultValue;
            } else {
                return defaultValue;
            }
        }

        public async Task<int> GetIntegerValueAsync(string key, int defaultValue = 0, string? channelName = null)
        {
            var value = await GetSettingsItemAsync(key, (await _channelSettingsInternalHelper.GetChannelId(channelName)));
            if (!string.IsNullOrWhiteSpace(value)) {
                return ((int?)_channelSettingsInternalHelper.ParseValue(typeof(int), value, defaultValue)) ?? defaultValue;
            } else {
                return defaultValue;
            }
        }

        public async Task<string> GetStringValueAsync(string key, string defaultValue = "", string? channelName = null) => (await GetSettingsItemAsync(key, (await _channelSettingsInternalHelper.GetChannelId(channelName)))) ?? defaultValue;


        public async Task<T?> GetValueAsync<T>(string key, T? defaultValue = default, string? channelName = null)
        {
            var value = await GetSettingsItemAsync(key, (await _channelSettingsInternalHelper.GetChannelId(channelName)));
            if (!string.IsNullOrWhiteSpace(value)) {
                return ((T?)_channelSettingsInternalHelper.ParseValue(typeof(T), value, defaultValue)) ?? defaultValue;
            } else {
                return defaultValue;
            }
        }


        public async Task<T> GetSettingsModel<T>(string? channelName = null) where T : new() => await GetSettingsModel<T>(await _channelSettingsInternalHelper.GetChannelId(channelName));
        

        public async Task<T> GetSettingsModel<T>(int channelId) where T : new()
        {
            T model = new();
            if (model != null) {
                var settingsProperties = _channelSettingsInternalHelper.GetAllPropertiesWithAttribute(model.GetType(), typeof(XperienceSettingsDataAttribute));
                if (settingsProperties?.Any() ?? false) {
                    foreach (var prop in settingsProperties) {
                        var settingsKey = prop.GetCustomAttribute<XperienceSettingsDataAttribute>();
                        if (settingsKey == null) {
                            continue;
                        }

                        var value = await GetSettingsItemAsync(settingsKey.Name, channelId);
                        if (value != null) {
                            _channelSettingsInternalHelper.SetPropertyValue(model, prop.Name, _channelSettingsInternalHelper.ParseValue(prop.PropertyType, value, settingsKey.DefaultValue));
                        } else if (settingsKey.DefaultValue != null) {
                            if (settingsKey.DefaultValue.GetType().Equals(prop.PropertyType)) {
                                _channelSettingsInternalHelper.SetPropertyValue(model, prop.Name, settingsKey.DefaultValue);
                            } else {
                                _channelSettingsInternalHelper.SetPropertyValue(model, prop.Name, _channelSettingsInternalHelper.ParseValue(prop.PropertyType, settingsKey.DefaultValue?.ToString() ?? string.Empty));
                            }
                        }
                    }
                }
            }

            return model;
        }

        private async Task<string?> GetSettingsItemAsync(string key, int channelId)
        {
            return (await _channelSettingsInternalHelper.GetChannelToSettingsKeyToInfo()).TryGetValue(channelId, out var keyToValue) && keyToValue.TryGetValue(key.ToLowerInvariant(), out var value) ? value.GetValue(nameof(ChannelCustomSettingInfo.ChannelCustomSettingKeyValue))?.ToString() ?? null : null;
        }

        public string[] GetSettingModelDependencyKeys<T>() where T : new()
        {
            List<string> keyNames = [];
            T model = new();
            if (model != null) {
                var settingsProperties = _channelSettingsInternalHelper.GetAllPropertiesWithAttribute(model.GetType(), typeof(XperienceSettingsDataAttribute));
                if (settingsProperties?.Any() ?? false) {
                    foreach (var prop in settingsProperties) {
                        var settingsKey = prop.GetCustomAttribute<XperienceSettingsDataAttribute>();
                        if (settingsKey == null) {
                            continue;
                        }
                        keyNames.Add(settingsKey.Name);
                    }
                }
            }

            return keyNames.Select(x => $"{ChannelCustomSettingInfo.OBJECT_TYPE}|bykey|{x}").ToArray();
        }
    }
}
