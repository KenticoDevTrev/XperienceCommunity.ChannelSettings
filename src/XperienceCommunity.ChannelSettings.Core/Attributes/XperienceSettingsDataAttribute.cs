namespace XperienceCommunity.ChannelSettings.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class XperienceSettingsDataAttribute(string name, object? defaultValue = null) : Attribute
    {
        public string Name { get; set; } = name;

        public object? DefaultValue { get; set; } = defaultValue;
    }
}