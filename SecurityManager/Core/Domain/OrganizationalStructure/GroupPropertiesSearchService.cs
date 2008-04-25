using System;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="Group"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="Group.Parent"/> property via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public sealed class GroupPropertiesSearchService : ISearchAvailableObjectsService
  {
    private const string c_parentName = "Parent";

    public GroupPropertiesSearchService ()
    {
    }

    public bool SupportsIdentity (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      if (property.Identifier == c_parentName)
        return true;
      else
        return false;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      Group group = ArgumentUtility.CheckNotNullAndType<Group> ("referencingObject", referencingObject);
      ArgumentUtility.CheckNotNull ("property", property);

      switch (property.Identifier)
      {
        case c_parentName:
          if (group.Tenant == null)
            return new IBusinessObject[0];
          return (IBusinessObject[]) ArrayUtility.Convert (group.GetPossibleParentGroups (group.Tenant.ID), typeof (IBusinessObject));
        default:
          throw new ArgumentException (
              string.Format (
                  "The property '{0}' is not supported by the '{1}' type.", property.DisplayName, typeof (GroupPropertiesSearchService).FullName));
      }
    }
  }
}