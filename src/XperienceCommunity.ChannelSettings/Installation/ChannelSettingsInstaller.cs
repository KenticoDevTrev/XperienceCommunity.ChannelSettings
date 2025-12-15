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

        private static void InitializeChannelCustomSettingsInfo(ResourceInfo resource)
        {
            var info = DataClassInfoProvider.GetDataClassInfo(ChannelCustomSettingInfo.OBJECT_TYPE) ?? DataClassInfo.New(ChannelCustomSettingInfo.OBJECT_TYPE);
            var formInfo = info.ClassID > 0 ? new FormInfo(info.ClassFormDefinition) : FormHelper.GetBasicFormDefinition(nameof(ChannelCustomSettingInfo.ChannelCustomSettingID));

            info.ClassName = "XperienceCommunity.ChannelCustomSetting";
            info.ClassTableName = "XperienceCommunity_ChannelCustomSetting";
            info.ClassDisplayName = "Channel Custom Settings";
            info.ClassType = ClassType.OTHER;
            info.ClassResourceID = resource.ResourceID;

            var formItem = GetFormField(formInfo, nameof(ChannelCustomSettingInfo.ChannelCustomSettingGuid));
            formItem.AllowEmpty = false;
            formItem.Visible = true;
            formItem.Precision = 0;
            formItem.DataType = "guid";
            formItem.Enabled = true;
            SetFormField(formInfo, formItem);

            formItem = GetFormField(formInfo, nameof(ChannelCustomSettingInfo.ChannelCustomSettingLastModified));
            formItem.AllowEmpty = false;
            formItem.Visible = true;
            formItem.Precision = 7;
            formItem.DataType = "datetime";
            formItem.Enabled = true;
            SetFormField(formInfo, formItem);

            formItem = GetFormField(formInfo, nameof(ChannelCustomSettingInfo.ChannelCustomSettingKeyChannelID));
            formItem.AllowEmpty = false;
            formItem.Visible = true;
            formItem.Precision = 0;
            formItem.DataType = "integer";
            formItem.ReferenceType = ObjectDependencyEnum.Required;
            formItem.ReferenceToObjectType = ChannelInfo.OBJECT_TYPE;
            formItem.Enabled = true;
            SetFormField(formInfo, formItem);

            formItem = GetFormField(formInfo, nameof(ChannelCustomSettingInfo.ChannelCustomSettingKeyName));
            formItem.AllowEmpty = false;
            formItem.Visible = true;
            formItem.Precision = 0;
            formItem.Size = 100;
            formItem.DataType = "text";
            formItem.Enabled = true;
            SetFormField(formInfo, formItem);

            formItem = GetFormField(formInfo, nameof(ChannelCustomSettingInfo.ChannelCustomSettingKeyValue));
            formItem.AllowEmpty = true;
            formItem.Visible = true;
            formItem.Precision = 0;
            formItem.DataType = "longtext";
            formItem.Enabled = true;
            SetFormField(formInfo, formItem);

            info.ClassFormDefinition = formInfo.GetXmlDefinition();

            if (info.HasChanged) {
                DataClassInfoProvider.SetDataClassInfo(info);
                try {
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
                } catch (Exception ) {
                    //_eventLogService.LogException("ChannelSettingsInstaller", "InitializeChannelCustomSettings Error", ex);
                }
            }
        }

        private static FormFieldInfo GetFormField(FormInfo formInfo, string fieldName)
        {
            return formInfo.FieldExists(fieldName) ? formInfo.GetFormField(fieldName) : new FormFieldInfo() { Name = fieldName };
        }

        private static void SetFormField(FormInfo formInfo, FormFieldInfo field)
        {
            if (formInfo.FieldExists(field.Name)) {
                formInfo.UpdateFormField(field.Name, field);
            } else {
                formInfo.AddFormItem(field);
            }
        }
    }
}
