using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using XperienceCommunity.ChannelSettings;

[assembly: RegisterObjectType(typeof(ChannelCustomSettingInfo), ChannelCustomSettingInfo.OBJECT_TYPE)]

namespace XperienceCommunity.ChannelSettings
{
    /// <summary>
    /// Data container class for <see cref="ChannelCustomSettingInfo"/>.
    /// </summary>
    public partial class ChannelCustomSettingInfo : AbstractInfo<ChannelCustomSettingInfo, IChannelCustomSettingInfoProvider>, IInfoWithId, IInfoWithName
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "xperiencecommunity.channelcustomsetting";


        /// <summary>
        /// Type information.
        /// </summary>
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(ChannelCustomSettingInfoProvider), OBJECT_TYPE, "XperienceCommunity.ChannelCustomSetting", "ChannelCustomSettingID", "ChannelCustomSettingLastModified", "ChannelCustomSettingGuid", "ChannelCustomSettingKeyName", null, null, null, null)
        {
            TouchCacheDependencies = true,
            LogEvents = true,
            DependsOn = new List<ObjectDependency>()
            {
                new ObjectDependency("ChannelCustomSettingKeyChannelID", "cms.channel", ObjectDependencyEnum.Required),
            },
        };

        /// <summary>
        /// Channel custom setting ID.
        /// </summary>
        [DatabaseField]
        public virtual int ChannelCustomSettingID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(ChannelCustomSettingID)), 0);
            set => SetValue(nameof(ChannelCustomSettingID), value);
        }

        /// <summary>
        /// Channel custom setting Guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid ChannelCustomSettingGuid
        {
            get => ValidationHelper.GetGuid(GetValue(nameof(ChannelCustomSettingGuid)), Guid.Empty);
            set => SetValue(nameof(ChannelCustomSettingGuid), value);
        }

        /// <summary>
        /// Channel custom setting Last Modified.
        /// </summary>
        [DatabaseField]
        public virtual DateTime ChannelCustomSettingLastModified
        {
            get => ValidationHelper.GetDateTime(GetValue(nameof(ChannelCustomSettingLastModified)), DateTime.MinValue);
            set => SetValue(nameof(ChannelCustomSettingLastModified), value);
        }


        /// <summary>
        /// Channel custom settings key channel ID.
        /// </summary>
        [DatabaseField]
        public virtual int ChannelCustomSettingKeyChannelID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(ChannelCustomSettingKeyChannelID)), 0);
            set => SetValue(nameof(ChannelCustomSettingKeyChannelID), value);
        }


        /// <summary>
        /// Channel custom setting key name.
        /// </summary>
        [DatabaseField]
        public virtual string ChannelCustomSettingKeyName
        {
            get => ValidationHelper.GetString(GetValue(nameof(ChannelCustomSettingKeyName)), String.Empty);
            set => SetValue(nameof(ChannelCustomSettingKeyName), value);
        }


        /// <summary>
        /// Channel custom setting key value.
        /// </summary>
        [DatabaseField]
        public virtual string ChannelCustomSettingKeyValue
        {
            get => ValidationHelper.GetString(GetValue(nameof(ChannelCustomSettingKeyValue)), String.Empty);
            set => SetValue(nameof(ChannelCustomSettingKeyValue), value, String.Empty);
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            Provider.Delete(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            Provider.Set(this);
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="ChannelCustomSettingInfo"/> class.
        /// </summary>
        public ChannelCustomSettingInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="ChannelCustomSettingInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public ChannelCustomSettingInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}