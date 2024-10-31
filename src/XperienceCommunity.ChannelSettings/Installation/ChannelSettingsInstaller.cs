using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Modules;

namespace XperienceCommunity.ChannelSettings.Installation
{
    public class ChannelSettingsInstaller(IInfoProvider<ResourceInfo> resourceInfoProvider, IEventLogService eventLogService)
    {
        private readonly IInfoProvider<ResourceInfo> _resourceInfoProvider = resourceInfoProvider;
        private readonly IEventLogService _eventLogService = eventLogService;

        public void Install()
        {
            var resource = _resourceInfoProvider.Get("XperienceCommunity.ChannelSettings") ?? new ResourceInfo();
            InitializeChannelSettings(resource);
            InitializeChannelCustomSettingsInfo(resource);
        }

        private void InitializeChannelSettings(ResourceInfo resource)
        {

            resource.ResourceDisplayName = "XperienceCommunity.ChannelSettings";
            resource.ResourceName = "XperienceCommunity.ChannelSettings";
            resource.ResourceDescription = "Allows for Custom Settings to be declared and then set on a per-channel basis. v1.0.0";
            resource.ResourceIsInDevelopment = false;

            if (resource.HasChanged)
            {
                _resourceInfoProvider.Set(resource);
            }
        }

        private void InitializeChannelCustomSettingsInfo(ResourceInfo resource)
        {
            var info = DataClassInfoProvider.GetDataClassInfo(ChannelCustomSettingInfo.OBJECT_TYPE) ?? DataClassInfo.New(ChannelCustomSettingInfo.OBJECT_TYPE);

            info.ClassName = "XperienceCommunity.ChannelCustomSetting";
            info.ClassTableName = "XperienceCommunity_ChannelCustomSetting";
            info.ClassDisplayName = "Channel Custom Settings";
            info.ClassType = ClassType.OTHER;
            info.ClassResourceID = resource.ResourceID;

            var formInfo = FormHelper.GetBasicFormDefinition("ChannelCustomSettingID");

            var formItem = new FormFieldInfo
            {
                Name = nameof(ChannelCustomSettingInfo.ChannelCustomSettingGuid),
                AllowEmpty = false,
                Visible = true,
                Precision = 0,
                DataType = "guid",
                Enabled = true
            };
            formInfo.AddFormItem(formItem);
            formItem = new FormFieldInfo
            {
                Name = nameof(ChannelCustomSettingInfo.ChannelCustomSettingLastModified),
                AllowEmpty = false,
                Visible = true,
                Precision = 7,
                DataType = "datetime",
                Enabled = true
            };
            formInfo.AddFormItem(formItem);

            formItem = new FormFieldInfo
            {
                Name = nameof(ChannelCustomSettingInfo.ChannelCustomSettingKeyChannelID),
                AllowEmpty = false,
                Visible = true,
                Precision = 0,
                DataType = "integer",
                ReferenceType = ObjectDependencyEnum.Required,
                ReferenceToObjectType = ChannelInfo.OBJECT_TYPE,
                Enabled = true
            };
            formInfo.AddFormItem(formItem);

            formItem = new FormFieldInfo
            {
                Name = nameof(ChannelCustomSettingInfo.ChannelCustomSettingKeyName),
                AllowEmpty = false,
                Visible = true,
                Precision = 0,
                Size = 100,
                DataType = "text",
                Enabled = true
            };
            formInfo.AddFormItem(formItem);

            formItem = new FormFieldInfo
            {
                Name = nameof(ChannelCustomSettingInfo.ChannelCustomSettingKeyValue),
                AllowEmpty = false,
                Visible = true,
                Precision = 0,
                DataType = "longtext",
                Enabled = true
            };
            formInfo.AddFormItem(formItem);

            SetFormDefinition(info, formInfo);

            if (info.HasChanged)
            {
                DataClassInfoProvider.SetDataClassInfo(info);
                try
                {
                    // run SQL to set foreign keys
                    var foreignKeySql =
    @"
IF(OBJECT_ID('FK_XperienceCommunity_ChannelCustomSetting_CMS_Channel', 'F') IS NULL)
BEGIN
ALTER TABLE [dbo].[XperienceCommunity_ChannelCustomSetting]  WITH CHECK ADD  CONSTRAINT [FK_XperienceCommunity_ChannelCustomSetting_CMS_Channel] FOREIGN KEY([ChannelCustomSettingKeyChannelID])
REFERENCES [dbo].[CMS_Channel] ([ChannelID])
ON UPDATE CASCADE
ON DELETE CASCADE


ALTER TABLE [dbo].[XperienceCommunity_ChannelCustomSetting] CHECK CONSTRAINT [FK_XperienceCommunity_ChannelCustomSetting_CMS_Channel]
END
";
                    ConnectionHelper.ExecuteNonQuery(foreignKeySql, [], QueryTypeEnum.SQLQuery);
                }
                catch (Exception ex)
                {
                    _eventLogService.LogException("ChannelSettingsInstaller", "InitializeChannelCustomSettings Error", ex);
                }
            }
        }

        private static void SetFormDefinition(DataClassInfo info, FormInfo form)
        {
            if (info.ClassID > 0)
            {
                var existingForm = new FormInfo(info.ClassFormDefinition);
                existingForm.CombineWithForm(form, new());
                info.ClassFormDefinition = existingForm.GetXmlDefinition();
            }
            else
            {
                info.ClassFormDefinition = form.GetXmlDefinition();
            }
        }
    }
}
