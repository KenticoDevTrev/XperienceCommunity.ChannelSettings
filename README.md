# XperienceCommunity.ChannelSettings
This package allows you to easily create strongly-typed Settings models that can be edited per-channel in the Xperience Admin, and retrieved easily, including default fallback values.

Huge shout out to [Dragoljub and his repository](https://github.com/drilic/xperience-ui-customsettings) which laid the groundwork for this module.

## Requirements
* **Kentico.Xperience.Admin 29.6.0** or newer version to use latest Xperience by Kentico
* **net8.0** as a long-term support (LTS) release

## Installation
1. Install the Nuget Packages into your project:
    * There are 3 packages, each with different dependencies for Kentico Libraries, so you can install them seprately on different projects to keep concerns separated
    * `XperienceCommunity.ChannelSettings.Core` = Kentico Agnostic, contains `IChannelCustomSettingsRepository` (settings retrieval) and `XperienceSettingsData` (Model decoration)
    * `XperienceCommunity.ChannelSettings` = Kentico.Xperience.Core dependent, contains actual logic.  Install this on your MVC Site itself.
    * `XperienceCommunity.ChannelSettings.Admin` = Kentico.Xperience.Admin dependent, contains `ChannelCustomSettingsPage` Class which you inherit from and use when creating your `UIPage` for editing your settings.
2. In your startup code, add `builder.Services.AddChannelCustomSettings();`

## Create your Custom Settings Model(s)

Next step is to create your Channel Settings Model class, and decorate the properties with the `[XperienceSettingsData("The.Key", defaultValue)]`.  The system will read the properties and attributes and use this when saving to the database and retrieving/building your model from settings.

``` csharp
    public class SEOChannelSettings
    {
        [XperienceSettingsData("SEO.ShowRobots", true)]
        public virtual bool ShowRobots { get; set; } = false;

        [XperienceSettingsData("SEO.Favicon", "~/images/favicon.png")]
        public virtual string Favicon { get; set; } = string.Empty;
    }
```

## Create (or append) Xperience's Form Annotation Attributes

Xperience will need information on how to display the fields and the form editing of them.  This is done through the Xperience FormAnnotation Attributes.  You can append them to your Model directly (requires Kentico.Xperience.Admin package), or simply make an inherited class off of your Kentico Agnostic Class.

``` csharp
    // This is a special class only for the Admin, overriding the fields and adding the Form Annotations to them.
    public class SEOChannelSettingsFormAnnotated : SEOChannelSettings
    {

        [CheckBoxComponent(Label = "Show Robots", Order = 101)]
        public override bool ShowRobots { get; set; } = false;

        [UrlSelectorComponent(Label = "Favicon", Order = 102)]
        public override string Favicon { get; set; } = "";
    }

```

## Add a UI Page to edit the settings

Lastly, you need to define your UI Page to edit this.  Copy and paste the code snippet below, Editing the `slug`, `uiPageType`, `name`, and putting your FormAnnotated model as the type for the `ChannelCustomSettingsPage<YourModel>`

``` csharp

[assembly: UIPage(parentType: typeof(Kentico.Xperience.Admin.Base.UIPages.ChannelEditSection),
    slug: "seo-channel-custom-settings",
    uiPageType: typeof(SEOChannelSettingsExtender),
    name: "SEO Channel settings",
    templateName: TemplateNames.EDIT,
    order: UIPageOrder.NoOrder)]
namespace XperienceCommunity.ChannelSettings.Admin
{
    public class SEOChannelSettingsExtender(Kentico.Xperience.Admin.Base.Forms.Internal.IFormItemCollectionProvider formItemCollectionProvider,
                                                                     IFormDataBinder formDataBinder,
                                                                     IChannelCustomSettingsRepository customChannelSettingsRepository,
                                                                     IChannelSettingsInternalHelper channelCustomSettingsInfoHandler,
                                                                     IInfoProvider<ChannelInfo> channelInfoProvider) 
        // Change type below to your settings
        : ChannelCustomSettingsPage<SEOChannelSettingsFormAnnotated>(formItemCollectionProvider, formDataBinder, customChannelSettingsRepository, channelCustomSettingsInfoHandler, channelInfoProvider)
    {
    }

```


## Edit Properties in the Admin

With the above, you can now go to `/admin/channels/list` (Admin -> Configuration -> Channels), select your channel, and you should now see your UI Page on the section when you edit.

![image](https://github.com/user-attachments/assets/42b71fa7-aa5b-47d5-b188-f8a63a040294)


## Getting your Settings

Retrieiving your settings is quite simple.  Inject the `IChannelCustomSettingsRepository` into your class, then call the `await _channelCustomSettingsRepository.GetSettingsModel<YourModel>()`.  This will:

1. Analyze the model for `XperienceSettingsData` attributes
2. Attempt to retrieve and parse values found in the Settings table for the given keys
3. Add in Defaults (if defined) where there is no value in the database.
4. Return the model.

``` csharp
[ViewComponent]
public class HomeViewComponent(IChannelCustomSettingsRepository channelCustomSettingsRepository) : ViewComponent
{
    private readonly IChannelCustomSettingsRepository _channelCustomSettingsRepository = channelCustomSettingsRepository;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var settings = await _channelCustomSettingsRepository.GetSettingsModel<SEOChannelSettings>();

        // Any retrieval here
        var model = new HomeViewModel(settings.ShowRobots)
        {
            // Properties here
        };
        return View("/Features/Home/Home.cshtml", model);
    }
}

```

## Dependency Keys
The `IChannelCustomSettingsRepository` already has data caching applied to make sure retrieval is quick and doesn't require multiple database queries, and it has it's own Cache Dependency Key settings to make sure data is cleared upon updates.

However, if you are using `<cache>` tags, you can use the `IChannelCustomSettingsRepository.GetSettingModelDependencyKeys<YourModel>();` to get a list of all the cache dependency keys that went into making the settings model.

## Migration from KX13

You should be able to migrate your CMS_SettingsKeys into the new database table and use the Attributes to make similar Settings Models.  I would simply write SQL once this is installed to move things over.

KeyName => ChannelCustomSettingKeyName
KeyValue => ChannelCustomSettingKeyValue
KeySiteID => ChannelCustomSettingKeyChannelID
KeyGUID => ChannelCustomSettingGuid

The following should be coded as part of the Form Data Annotation / Model:
KeyDisplayName => Use FormAnnotation Order
KeyType (Just the property type)
KeyCategoryID => You can either split it into different Models, or use the `FormCategory` attribute on your FormAnnotated class
KeyOrder => Use FormAnnotation Order

## Global Settings

For settings that are global, you should probably consider using AppSettings.json and transformations for almost all of those needs, or a simple Custom Module Class.

## Contributions and Support

Feel free to fork and submit pull requests or report issues to contribute. Either this way or another one, we will look into them as soon as possible. 
