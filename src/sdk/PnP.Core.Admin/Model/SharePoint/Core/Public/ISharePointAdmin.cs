﻿using PnP.Core.Model.Security;
using PnP.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PnP.Core.Admin.Model.SharePoint
{
    /// <summary>
    /// SharePoint Admin features
    /// </summary>
    public interface ISharePointAdmin
    {
        /// <summary>
        /// Returns the SharePoint tenant admin center url (e.g. https://contoso-admin.sharepoint.com)
        /// </summary>
        /// <returns>SharePoint tenant admin center url</returns>
        Task<Uri> GetTenantAdminCenterUriAsync();

        /// <summary>
        /// Returns the SharePoint tenant admin center url (e.g. https://contoso-admin.sharepoint.com)
        /// </summary>
        /// <returns>SharePoint tenant admin center url</returns>
        Uri GetTenantAdminCenterUri();

        /// <summary>
        /// Returns the SharePoint tenant portal url (e.g. https://contoso.sharepoint.com)
        /// </summary>
        /// <returns>SharePoint tenant portal url</returns>
        Task<Uri> GetTenantPortalUriAsync();

        /// <summary>
        /// Returns the SharePoint tenant portal url (e.g. https://contoso.sharepoint.com)
        /// </summary>
        /// <returns>SharePoint tenant portal url</returns>
        Uri GetTenantPortalUri();

        /// <summary>
        /// Returns the SharePoint tenant my site host url (e.g. https://contoso-my.sharepoint.com)
        /// </summary>
        /// <returns>SharePoint tenant my site host url</returns>
        Task<Uri> GetTenantMySiteHostUriAsync();

        /// <summary>
        /// Returns the SharePoint tenant my site host url (e.g. https://contoso-my.sharepoint.com)
        /// </summary>
        /// <returns>SharePoint tenant my site host url</returns>
        Uri GetTenantMySiteHostUri();

        /// <summary>
        /// Returns a <see cref="PnPContext"/> for the tenant's SharePoint admin center site
        /// </summary>
        /// <returns><see cref="PnPContext"/> for the tenant's SharePoint admin center</returns>
        Task<PnPContext> GetTenantAdminCenterContextAsync();

        /// <summary>
        /// Returns a <see cref="PnPContext"/> for the tenant's SharePoint admin center site
        /// </summary>
        /// <returns><see cref="PnPContext"/> for the tenant's SharePoint admin center</returns>
        PnPContext GetTenantAdminCenterContext();

        /// <summary>
        /// Returns a list of <see cref="ISharePointUser"/>s who are SharePoint Online Tenant admin
        /// </summary>
        /// <returns>List of SharePoint Online Tenant admins</returns>
        Task<List<ISharePointUser>> GetTenantAdminsAsync();

        /// <summary>
        /// Returns a list of <see cref="ISharePointUser"/>s who are SharePoint Online Tenant admin
        /// </summary>
        /// <returns>List of SharePoint Online Tenant admins</returns>
        List<ISharePointUser> GetTenantAdmins();

        /// <summary>
        /// Checks if the current user is SharePoint Online tenant admin
        /// </summary>
        /// <returns>True if the user is a SharePoint Online tenant admin, false otherwise</returns>
        Task<bool> IsCurrentUserTenantAdminAsync();

        /// <summary>
        /// Checks if the current user is SharePoint Online tenant admin
        /// </summary>
        /// <returns>True if the user is a SharePoint Online tenant admin, false otherwise</returns>
        bool IsCurrentUserTenantAdmin();

        /// <summary>
        /// Gets the properties of this tenant
        /// </summary>
        /// <returns>Properties of the tenant</returns>
        Task<ITenantProperties> GetTenantPropertiesAsync();

        /// <summary>
        /// Gets the properties of this tenant
        /// </summary>
        /// <returns>Properties of the tenant</returns>
        ITenantProperties GetTenantProperties();
    }
}
