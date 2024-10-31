using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using XperienceCommunity.ChannelSettings.Repositories;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using System.Reflection;
using XperienceCommunity.ChannelSettings.Attributes;
using CMS.Helpers;

namespace XperienceCommunity.ChannelSettings.Admin.UI.ChannelCustomSettings
{
    public class ChannelCustomSettingsPage<T>(Kentico.Xperience.Admin.Base.Forms.Internal.IFormItemCollectionProvider formItemCollectionProvider,
                                                                     IFormDataBinder formDataBinder,
                                                                     IChannelCustomSettingsRepository customChannelSettingsRepository,
                                                                     IChannelSettingsInternalHelper channelSettingsInternalHelper,
                                                                     IInfoProvider<ChannelInfo> channelInfoProvider) : ModelEditPage<T>(formItemCollectionProvider, formDataBinder) where T : new()
    {
        private readonly IChannelCustomSettingsRepository _channelCustomSettingsRepository = customChannelSettingsRepository;
        private readonly IChannelSettingsInternalHelper _channelSettingsInternalHelper = channelSettingsInternalHelper;
        private readonly IInfoProvider<ChannelInfo> _channelInfoProvider = channelInfoProvider;

        private T _model = new();

        protected override T Model
        {
            get
            {
                _model ??= new();

                return _model;
            }
        }

        [PageParameter(typeof(IntPageModelBinder))]
        public int ObjectId { get; set; }

        public override Task ConfigurePage()
        {
            PageConfiguration.SubmitConfiguration.Label = "Save";

            var channel = _channelInfoProvider.Get(ObjectId);
            if (channel?.ChannelType != ChannelType.Website) {
                PageConfiguration.SubmitConfiguration.Visible = false;
                PageConfiguration.Disabled = true;
                PageConfiguration.Headline = "Channel custom settings is not supported.";

                PageConfiguration.Callouts =
                [
                    new()
                    {
                        Headline = "NOT SUPPORTED!",
                        Content = "<p>Channel custom settings is supported only for <strong>Website</strong> channels!</p>",
                        ContentAsHtml = true,
                        Type = CalloutType.FriendlyWarning
                    }
                ];
            } else {
                PopulateData();
            }

            return base.ConfigurePage();
        }

        protected override async Task<ICommandResponse> ProcessFormData(T model, ICollection<IFormItem> formItems)
        {
            if (model == null) {
                return GetErrorResponse("Something went wrong. Please try later!");
            }

            try {
                await SaveDataAsync();
            } catch (Exception ex) {
                EventLogService.LogException($"ChannelCustomSettingsPage -> UNEXPECTED ERROR", $"ChannelCustomSettingsPage", ex);

                return GetErrorResponse($"Unable to save data. Check event logs for more details!");
            }

            return GetSuccessResponse("Settings saved successfully!");
        }

        private async Task SaveDataAsync()
        {
            if (Model != null) {
                var settingsProperties = _channelSettingsInternalHelper.GetAllPropertiesWithAttribute(Model.GetType(), typeof(XperienceSettingsDataAttribute));
                if (settingsProperties?.Any() ?? false) {
                    foreach (var prop in settingsProperties) {
                        var settingsKey = prop.GetCustomAttribute<XperienceSettingsDataAttribute>();
                        if (settingsKey == null) {
                            continue;
                        }

                        await _channelSettingsInternalHelper.InsertOrUpdatedSettingsKey(settingsKey.Name, _channelSettingsInternalHelper.ParseObject(prop.PropertyType, _channelSettingsInternalHelper.GetPropertyValue(Model, prop.Name)), ObjectId);
                    }
                }
            }
        }

        private async void PopulateData()
        {
            if (Model != null) {

                _model = await _channelCustomSettingsRepository.GetSettingsModel<T>(ObjectId);

            }
        }

        private ICommandResponse<FormSubmissionResult> GetErrorResponse(params string[] messages)
        {
            var response = ResponseFrom(new FormSubmissionResult(FormSubmissionStatus.Error));

            foreach (var message in messages) {
                response.AddErrorMessage(message);
            }

            return response;
        }

        private ICommandResponse<FormSubmissionResult> GetSuccessResponse(string message)
        {
            var response = ResponseFrom(new FormSubmissionResult(FormSubmissionStatus.ValidationSuccess));

            response.AddSuccessMessage(message);

            return response;
        }

   
    }
}
