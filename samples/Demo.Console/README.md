# PnP Core SDK - Console Sample

This solution demonstrates how the PnP Core SDK can be used in a console application. In this sample we're querying a modern group connected SharePoint site which also has Teams. If you're testing this code against a modern communication site or another classic site then please comment out the "teams" parts.

## Source code

You can find the sample source code here: [/samples/Demo.Console](https://github.com/pnp/pnpcore/tree/dev/samples/Demo.Console)

# Run the sample

## Register and configure an AAD app

In order for the user to authenticate on the App, A new app registration should be created on Azure Portal

- Go to [Azure Active Directory Portal](https://aad.portal.azure.com)

- In App registrations, click __New registration__

- Enter a name for your new app, make sure *Accounts in this organizational directory only* is selected. As the Redirect URI, change from Web Platform to "Mobile and Desktop Applications" use __http://localhost__ for the redirect URI (only needed if you want use an interactive authentication flow)

- Under __Implicit grant__ section, check __ID tokens__ and __Access tokens__

- Under __Advanced settings__ section, set __Allow public client flows__ to __yes__

- Go to __API permissions__ section , click __Add a permission__
  - Select __Microsoft Graph__ > __Delegated permissions__ > select __Directory.Read.All__
  - Select __Microsoft Graph__ > __Delegated permissions__ > select __User.Read__
  - Select __Microsoft Graph__ > __Delegated permissions__ > select __ChannelMessage.Read.All__
  - Select __Microsoft Graph__ > __Delegated permissions__ > select __ChannelMessage.Send__
  - Select __Microsoft Graph__ > __Delegated permissions__ > select __TeamSettings.ReadWrite.All__
  - Select __Microsoft Graph__ > __Delegated permissions__ > select __TeamsTab.ReadWrite.All__
  - Select __Microsoft Graph__ > __Delegated permissions__ > select __Sites.Read.All__
  - Select __Microsoft Graph__ > __Delegated permissions__ > select __Files.Read.All__
  - Select __SharePoint__ > __Delegated permissions__ > select __AllSites.Read__

- Click __Grand admin consent for {tenant}__

- From __Overview__,
  - copy the value of __Directory (tenant) ID__
  - copy the value of __Application (client) ID__

## Configure your application

- This demo application comes with code for 2 different authentication providers, the `CredentialManagerAuthenticationProvider` or the `InteractiveAuthenticationProvider` can be used. The latter is the default value. To configure the app update the `appsettings.json` file with:

- Configure the Tenant ID of your app as the value of `CustomSettings:TenantId` in appsettings.json setting
- Configure the Client ID of your app as the value of `CustomSettings:ClientId` in appsettings.json setting
- Configure the URL of a target Microsoft SharePoint Online modern team site collection as the value of `CustomSettings:DemoSiteUrl` in appsettings.json setting
- Configure the URL of a target Microsoft SharePoint Online sub site as the value of `CustomSettings:DemoSubSiteUrl` in appsettings.json setting

Be sure to have a Team in Microsoft Teams backing the modern team site in the above site collection

## Execute

Hit F5 in Visual studio to execute the console app. The app will prompt for an interactive login (via a browser window).

![preview image of the running app](preview.png)
