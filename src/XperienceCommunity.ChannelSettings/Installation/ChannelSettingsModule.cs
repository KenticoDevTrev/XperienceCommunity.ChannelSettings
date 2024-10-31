using CMS;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.DbDataManager;
using CMS.Helpers;
using Microsoft.Extensions.DependencyInjection;
using XperienceCommunity.ChannelSettings.Events;
using XperienceCommunity.ChannelSettings.Installation;


[assembly: RegisterModule(typeof(ChannelSettingsInitializationModule))]
namespace XperienceCommunity.ChannelSettings.Events
{
    public class ChannelSettingsInitializationModule : Module
    {

        private IServiceProvider? _services;
        private ChannelSettingsInstaller? _installer;


        protected override void OnInit(ModuleInitParameters parameters)
        {
            base.OnInit();

            _services = parameters.Services;
            _installer = _services.GetRequiredService<ChannelSettingsInstaller>();
            ApplicationEvents.Initialized.Execute += InitializeModule;

            // Handle cache key clearing by key name, since they don't have a code name.
            ChannelCustomSettingInfo.TYPEINFO.Events.Update.After += UpdateInsert_After;
            ChannelCustomSettingInfo.TYPEINFO.Events.Insert.After += UpdateInsert_After;
            ChannelCustomSettingInfo.TYPEINFO.Events.Delete.Before += Delete_Before;

        }

        public ChannelSettingsInitializationModule() : base("ChannelSettingsInitializationModule")
        {
         
        }

        private void Delete_Before(object? sender, ObjectEventArgs e)
        {
            if (e.Object is ChannelCustomSettingInfo customSettingsInfo) {
                CacheHelper.TouchKey($"{ChannelCustomSettingInfo.OBJECT_TYPE}|bykey|{customSettingsInfo.ChannelCustomSettingKeyName}");
                CacheHelper.TouchKey($"{ChannelCustomSettingInfo.OBJECT_TYPE}|byname|{customSettingsInfo.ChannelCustomSettingKeyName}");
            }
        }

        private void UpdateInsert_After(object? sender, ObjectEventArgs e)
        {
            if (e.Object is ChannelCustomSettingInfo customSettingsInfo) {
                CacheHelper.TouchKey($"{ChannelCustomSettingInfo.OBJECT_TYPE}|bykey|{customSettingsInfo.ChannelCustomSettingKeyName}");
                CacheHelper.TouchKey($"{ChannelCustomSettingInfo.OBJECT_TYPE}|byname|{customSettingsInfo.ChannelCustomSettingKeyName}");
            }
        }

        private void InitializeModule(object? sender, EventArgs e)
        {
            _installer?.Install();
        }
    }
}
