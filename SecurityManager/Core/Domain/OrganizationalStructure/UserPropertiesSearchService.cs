using System;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="User"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="User.OwningGroup"/> property via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public sealed class UserPropertiesSearchService : ISearchAvailableObjectsService
  {
    private const string c_owningGroupName = "OwningGroup";

    public UserPropertiesSearchService ()
    {
    }

    public bool SupportsIdentity (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      if (property.Identifier == c_owningGroupName)
        return true;
      else
        return false;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      User user = ArgumentUtility.CheckNotNullAndType<User> ("referencingObject", referencingObject);
      ArgumentUtility.CheckNotNull ("property", property);

      switch (property.Identifier)
      {
        case c_owningGroupName:
          if (user.Tenant == null)
            return new IBusinessObject[0];
          return (IBusinessObject[]) ArrayUtility.Convert (Group.FindByTenantID (user.Tenant.ID), typeof (IBusinessObject));
        default:
          throw new ArgumentException (
              string.Format (
                  "The property '{0}' is not supported by the '{1}' type.", property.DisplayName, typeof (UserPropertiesSearchService).FullName));
      }
    }
  }
}