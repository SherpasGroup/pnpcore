# Working with groups

In Microsoft 365 there's the concept of Microsoft 365 group with owners and members and there are SharePoint groups. The latter is the main topic of this page as SharePoint groups is the common model to used to grant users or other groups access to a SharePoint site.

In the remainder of this article you'll see a lot of `context` use: in this case this is a `PnPContext` which was obtained via the `PnPContextFactory` as explained in the [overview article](readme.md) and show below:

```csharp
using (var context = await pnpContextFactory.CreateAsync("SiteToWorkWith"))
{
    // See next chapter on how to use the PnPContext for working with lists
}
```

## SharePoint Groups

SharePoint groups are defined at the web level and by default does each web has an owners group, a members group and a visitors group. Adding users to those default created groups is a commonly used model, but you can also add a new group with it's role definitions and users.

### Getting groups

To get the groups on a web you query the `SiteGroups` property which return a collection of `ISharePointGroup` instances. If you want to work with the default groups you can get a reference to these groups via the `AssociatedOwnerGroup`, `AssociatedMemberGroup` or `AssociatedVisitorGroup` properties of an `IWeb`.

```csharp
// Option A: Load the default groups
await context.Web.LoadAsync(p => p.AssociatedMemberGroup, p => p.AssociatedOwnerGroup, p => p.AssociatedVisitorGroup);

await foreach(var user in context.Web.AssociatedOwnerGroup.Users.AsAsyncEnumerable())
{
    // do something with the user/group
}

// Option B: load all the groups with their users
await context.Web.LoadAsync(p => p.SiteGroups.QueryProperties(u => u.Users));

foreach(var group in context.Web.SiteGroups.AsRequested())
{
    // do something with the group
}
```

### Adding a group with a custom role definition

Adding a new SharePoint group is done via the usual `Add` methods. When a group is added you also need to assign a role to it (see [Configuring roles](./security-intro.md#configuring-roles)) otherwise the users added to group will not have any permissions on the SharePoint site.

```csharp
// Add a new role definition for our new group, a limited reader role
var roleDefinition = await context.Web.RoleDefinitions.AddAsync(roleDefName, Model.SharePoint.RoleType.Reader, new Model.SharePoint.PermissionKind[] { Model.SharePoint.PermissionKind.Open });

// Add the new SharePoint group
var siteGroup = await context.Web.SiteGroups.AddAsync("Limited readers");

// Add the role definition to our group
await siteGroup.AddRoleDefinitionsAsync(roleDefinition.Name);
```

### Editing a group

To update a group you first change the needed properties and then call one of the `Update` methods.

> [!Note]
> As a group's description cannot contain html characters the provided html is turned into text automatically. Also is the description length automatically truncated at 511 characters.

```csharp
// First get the group to update
var siteGroup = await context.Web.SiteGroups.FirstOrDefaultAsync(g => g.Title == "Limited readers");

// Update the group, e.g. the Description property
siteGroup.Description = "Group for users with limited read access to this site";
await siteGroup.UpdateAsync();
```

### Deleting a group

To delete a group use one of the `Delete` methods.

```csharp
// First get the group to delete
var siteGroup = await context.Web.SiteGroups.FirstOrDefaultAsync(g => g.Title == "Limited readers");

// Delete the group
await siteGroup.DeleteAsync();
```

### Adding users/groups to a group

Once a group has been created adding users or other groups is a common task which can be done using one of the `AddUser` methods.

```csharp
// First get the group to add users to
var siteGroup = await context.Web.SiteGroups.FirstOrDefaultAsync(g => g.Title == "Limited readers");
// Get the user to add
var currentUser = await context.Web.GetCurrentUserAsync();
// Add the user
await addedGroup.AddUserAsync(currentUser.LoginName);
```

### Listing the users in a group

To query the users in a group you need to load/get the `Users` property of the group.

```csharp
// Notice how the users are loaded
var myGroup = await context.Web.SiteGroups.QueryProperties(p => p.Users).FirstOrDefaultAsync(g => g.Title == "Limited readers");

foreach(var user in myGroup.Users.AsRequested())
{
    // do something with the group user
}
```

### Removing users/groups from a group

Removing users from a group is done using the `RemoveUser` methods.

```csharp
// First get the group to remove users from
var siteGroup = await context.Web.SiteGroups.FirstOrDefaultAsync(g => g.Title == "Limited readers");
// Get the user to remove
var currentUser = await context.Web.GetCurrentUserAsync();
// Remove the user
await addedGroup.RemoveUserAsync(currentUser.Id);
```

## Microsoft 365 groups

When a site is connected to a Microsoft 365 group then the Microsoft 365 group's owners are also site collection administrators and are part of the site's default "Owners" SharePoint group. The Microsoft's 365 group members will be automatically part of the site's default "Members" SharePoint group. Important to understand is that the Microsoft 365 group's owners and members complement the existing SharePoint security model, meaning it's perfectly possible to define add a SharePoint group with members in a Microsoft 365 group connected site (e.g. a team site). Important to understand is the permissions set of the SharePoint site via SharePoint groups do not apply to the Microsoft 365 group and it's other connected resources (e.g. a mailbox, Yammer group etc). Go [here](https://docs.microsoft.com/en-us/sharepoint/dev/transform/modernize-connect-to-office365-group) to learn more.
