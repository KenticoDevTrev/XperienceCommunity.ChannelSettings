using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Websites.Routing;
using System.Reflection;
using System.Text.Json;

namespace XperienceCommunity.ChannelSettings.Repositories.Implementation
{
    public class ChannelSettingsInternalHelper(
        IInfoProvider<ChannelInfo> channelInfoProvider,
        IInfoProvider<ChannelCustomSettingInfo> channelCustomSettingInfoProvider,
        IWebsiteChannelContext websiteChannelContext,
        IProgressiveCache progressiveCache) : IChannelSettingsInternalHelper
    {
        private readonly IInfoProvider<ChannelInfo> _channelInfoProvider = channelInfoProvider;
        private readonly IInfoProvider<ChannelCustomSettingInfo> _channelCustomSettingInfoProvider = channelCustomSettingInfoProvider;
        private readonly IWebsiteChannelContext _websiteChannelContext = websiteChannelContext;
        private readonly IProgressiveCache _progressiveCache = progressiveCache;

        public async Task<ChannelCustomSettingInfo> InsertOrUpdatedSettingsKey(string key, string? value, int channelId)
        {
            var settings = (await _channelCustomSettingInfoProvider.Get()
                .WhereEquals(nameof(ChannelCustomSettingInfo.ChannelCustomSettingKeyName), key)
                .WhereEquals(nameof(ChannelCustomSettingInfo.ChannelCustomSettingKeyChannelID), channelId)
                .GetEnumerableTypedResultAsync()).FirstOrDefault() ?? new ChannelCustomSettingInfo()
                {
                    ChannelCustomSettingKeyChannelID = channelId,
                    ChannelCustomSettingKeyName = key,
                    ChannelCustomSettingKeyValue = value
                };

            // Set value
            settings.ChannelCustomSettingKeyValue = value;

            // If changed, save
            if (settings.HasChanged) {
                _channelCustomSettingInfoProvider.Set(settings);
            }

            return settings;
        }

        public async Task<ChannelCustomSettingInfo?> GetSettingsKey(string key, int channelId) => (await GetChannelToSettingsKeyToInfo()).TryGetValue(channelId, out var settings) && settings.TryGetValue(key.ToLowerInvariant(), out var value) ? value : default;

        private async Task<IEnumerable<ChannelCustomSettingInfo>> GetSettingsItemsAsync() => await _channelCustomSettingInfoProvider.Get().GetEnumerableTypedResultAsync();

        public object? ParseValue(Type type, string value, object? defaultValue = default)
        {
            return Type.GetTypeCode(type) switch {
                TypeCode.Boolean => bool.TryParse(value, out var val) ? val : bool.TryParse(defaultValue?.ToString() ?? "", out var parsedDefaultVal) ? parsedDefaultVal : default,
                TypeCode.Int16 => Int16.TryParse(value, out var val) ? val : Int16.TryParse(defaultValue?.ToString() ?? "", out var parsedDefaultVal) ? parsedDefaultVal : default,
                TypeCode.Int32 => Int32.TryParse(value, out var val) ? val : Int32.TryParse(defaultValue?.ToString() ?? "", out var parsedDefaultVal) ? parsedDefaultVal : default,
                TypeCode.Int64 => Int64.TryParse(value, out var val) ? val : Int64.TryParse(defaultValue?.ToString() ?? "", out var parsedDefaultVal) ? parsedDefaultVal : default,
                TypeCode.UInt16 => UInt16.TryParse(value, out var val) ? val : UInt16.TryParse(defaultValue?.ToString() ?? "", out var parsedDefaultVal) ? parsedDefaultVal : default,
                TypeCode.UInt32 => UInt32.TryParse(value, out var val) ? val : UInt32.TryParse(defaultValue?.ToString() ?? "", out var parsedDefaultVal) ? parsedDefaultVal : default,
                TypeCode.UInt64 => UInt64.TryParse(value, out var val) ? val : UInt64.TryParse(defaultValue?.ToString() ?? "", out var parsedDefaultVal) ? parsedDefaultVal : default,
                TypeCode.Char => char.TryParse(value, out var val) ? val : char.TryParse(defaultValue?.ToString() ?? "", out var parsedDefaultVal) ? parsedDefaultVal : default,
                TypeCode.Double => double.TryParse(value, out var val) ? val : double.TryParse(defaultValue?.ToString() ?? "", out var parsedDefaultVal) ? parsedDefaultVal : default,
                TypeCode.Decimal => decimal.TryParse(value, out var val) ? val : decimal.TryParse(defaultValue?.ToString() ?? "", out var parsedDefaultVal) ? parsedDefaultVal : default,
                TypeCode.DateTime => DateTime.TryParse(value, out var val) ? val : DateTime.TryParse(defaultValue?.ToString() ?? "", out var parsedDefaultVal) ? parsedDefaultVal : default,
                TypeCode.String => !string.IsNullOrWhiteSpace(value) ? value : defaultValue?.ToString() ?? string.Empty,
                TypeCode.Empty => string.Empty,
                TypeCode.DBNull => null,
                TypeCode.Object => type.Name.Equals("Guid") && Guid.TryParse(value, out var val) ? (object?)val : JsonSerializer.Deserialize(value, type, options: new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }),
                _ or TypeCode.SByte or TypeCode.Byte or TypeCode.Single => throw new NotImplementedException("Cannot parse this type, please use basic elements such as boolean, int, double, decimal, DateTime, Guid or string."),
            };
        }

        public string? ParseObject(Type type, object? value)
        {
            if(value == null) {
                return null;
            }

            return Type.GetTypeCode(type) switch {
                TypeCode.Boolean 
                or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64
                or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64
                or TypeCode.Char or TypeCode.Double or TypeCode.Decimal or TypeCode.DateTime
                or TypeCode.String => value.ToString(),
                TypeCode.Empty => string.Empty,
                TypeCode.DBNull => null,
                TypeCode.Object => (value is Guid || value is Guid?) ? value.ToString() : JsonSerializer.Serialize(value, type, options: new JsonSerializerOptions()),
                _ or TypeCode.SByte or TypeCode.Byte or TypeCode.Single => throw new NotImplementedException("Cannot parse this type, please use basic elements such as boolean, int, double, decimal, DateTime, Guid or string."),
            };
        }

        public IEnumerable<PropertyInfo>? GetAllPropertiesWithAttribute(Type objectType, Type attributeType) => objectType?.GetProperties()?.Where(prop => prop.IsDefined(attributeType)) ?? [];

        public void SetPropertyValue(object obj, string propertyName, object? val)
        => obj?.GetType()?.GetProperty(propertyName)?.SetValue(obj, val);

        public object? GetPropertyValue(object obj, string propertyName)
            => obj?.GetType()?.GetProperty(propertyName)?.GetValue(obj);



        public async Task<int> GetChannelId(string? channelName)
        {
            var properName = !string.IsNullOrEmpty(channelName) ? channelName : _websiteChannelContext.WebsiteChannelName;

            return (await GetChannelNameToId()).TryGetValue(properName.ToLowerInvariant(), out var channelId) ? channelId : 0;
        }

        private async Task<Dictionary<string, int>> GetChannelNameToId()
        {
            return await _progressiveCache.LoadAsync(async cs => {
                if (cs.Cached) {
                    cs.CacheDependency = CacheHelper.GetCacheDependency([$"{ChannelInfo.OBJECT_TYPE}|all"]);
                }
                return (await _channelInfoProvider.Get()
                .Columns(nameof(ChannelInfo.ChannelName), nameof(ChannelInfo.ChannelID))
                .GetEnumerableTypedResultAsync())
                .ToDictionary(key => key.ChannelName.ToLowerInvariant(), value => value.ChannelID);
            }, new CacheSettings(360, "CustomSettings_GetChannelNameToId"));
        }

        public async Task<Dictionary<int, string>> GetChannelIdToName()
        {
            return await _progressiveCache.LoadAsync(async cs => {
                if (cs.Cached) {
                    cs.CacheDependency = CacheHelper.GetCacheDependency([$"{ChannelInfo.OBJECT_TYPE}|all"]);
                }
                return (await _channelInfoProvider.Get()
                .Columns(nameof(ChannelInfo.ChannelName), nameof(ChannelInfo.ChannelID))
                .GetEnumerableTypedResultAsync())
                .ToDictionary(key => key.ChannelID,  value => value.ChannelName.ToLowerInvariant());
            }, new CacheSettings(360, "CustomSettings_GetChannelIdToName"));
        }

        public async Task<Dictionary<int, Dictionary<string, ChannelCustomSettingInfo>>> GetChannelToSettingsKeyToInfo()
        {
            return await _progressiveCache.LoadAsync(async cs => {
                if (cs.Cached) {
                    cs.CacheDependency = CacheHelper.GetCacheDependency([$"{ChannelCustomSettingInfo.OBJECT_TYPE}|all"]);
                }
                return (await _channelCustomSettingInfoProvider.Get()
                .GetEnumerableTypedResultAsync())
                .GroupBy(x => x.ChannelCustomSettingKeyChannelID)
                .ToDictionary(key => key.Key, value => value.ToDictionary(key2 => key2.ChannelCustomSettingKeyName.ToLowerInvariant(), value2 => value2));
            }, new CacheSettings(360, "CustomSettings_GetChannelToSettingsKeyToInfo"));
        }
    }
}