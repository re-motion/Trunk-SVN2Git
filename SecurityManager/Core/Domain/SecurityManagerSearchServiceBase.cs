using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// Base-Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="BaseSecurityManagerObject"/> type.
  /// </summary>
  /// <remarks>
  /// Inherit from this type and add search delegates for the properties the specified <typeparam name="T"/> using the <see cref="AddSearchDelegate"/> 
  /// method from the constructor.
  /// </remarks>
  public abstract class SecurityManagerSearchServiceBase<T> : ISearchAvailableObjectsService
      where T: BaseSecurityManagerObject
  {
    private readonly Dictionary<string, Func<T, IBusinessObjectReferenceProperty, string, IBusinessObject[]>> _searchDelegates = new Dictionary<string, Func<T, IBusinessObjectReferenceProperty, string, IBusinessObject[]>>();

    protected void AddSearchDelegate (string propertyName, Func<T, IBusinessObjectReferenceProperty, string, IBusinessObject[]> searchDelegate)
    {
      _searchDelegates.Add (propertyName, searchDelegate);
    }

    public bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      return _searchDelegates.ContainsKey (property.Identifier);
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement)
    {
      T referencingSecurityManagerObject = ArgumentUtility.CheckNotNullAndType<T> ("referencingObject", referencingObject);
      ArgumentUtility.CheckNotNull ("property", property);

      Func<T, IBusinessObjectReferenceProperty, string, IBusinessObject[]> searchDelegate;
      if (_searchDelegates.TryGetValue (property.Identifier, out searchDelegate))
        return searchDelegate (referencingSecurityManagerObject, property, searchStatement);

      throw new ArgumentException (string.Format ("The property '{0}' is not supported by the '{1}' type.", property.DisplayName, GetType().FullName));
    }
  }
}