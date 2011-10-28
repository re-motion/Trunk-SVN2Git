// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure
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
    protected delegate IQueryable<IBusinessObject> SearchDelegate (
        T referencingObject,
        IBusinessObjectReferenceProperty property,
        SecurityManagerSearchArguments searchArguments);

    private readonly Dictionary<string, SearchDelegate> _searchDelegates = new Dictionary<string, SearchDelegate>();

    protected void AddSearchDelegate (string propertyName, SearchDelegate searchDelegate)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("searchDelegate", searchDelegate);

      _searchDelegates.Add (propertyName, searchDelegate);
    }

    public bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      return _searchDelegates.ContainsKey (property.Identifier);
    }

    public IBusinessObject[] Search (
        IBusinessObject referencingObject,
        IBusinessObjectReferenceProperty property,
        ISearchAvailableObjectsArguments searchArguments)
    {
      T referencingSecurityManagerObject = ArgumentUtility.CheckType<T> ("referencingObject", referencingObject);
      ArgumentUtility.CheckNotNull ("property", property);

      SearchDelegate searchDelegate;
      if (!_searchDelegates.TryGetValue (property.Identifier, out searchDelegate))
      {
        throw new ArgumentException (
            string.Format ("The property '{0}' is not supported by the '{1}' type.", property.DisplayName, GetType().FullName));
      }

      var securityManagerSearchArguments = CreateSearchArguments (searchArguments);
      return CreateQuery (searchDelegate, referencingSecurityManagerObject, property, securityManagerSearchArguments).ToArray();
    }

    private IQueryable<IBusinessObject> CreateQuery (
        SearchDelegate searchDelegate,
        T referencingSecurityManagerObject,
        IBusinessObjectReferenceProperty property,
        SecurityManagerSearchArguments searchArguments)
    {
      var query = searchDelegate (referencingSecurityManagerObject, property, searchArguments);

      var resultSizeConstraint = ((IResultSizeConstraint) searchArguments);
      if (resultSizeConstraint != null && resultSizeConstraint.Value.HasValue)
        query = query.Take (resultSizeConstraint.Value.Value);

      return query;
    }

    private SecurityManagerSearchArguments CreateSearchArguments (ISearchAvailableObjectsArguments searchArguments)
    {
      var defaultSearchArguments = searchArguments as DefaultSearchArguments;
      if (defaultSearchArguments != null)
      {
        if (string.IsNullOrEmpty (defaultSearchArguments.SearchStatement))
          return null;
        return new SecurityManagerSearchArguments (ObjectID.Parse (defaultSearchArguments.SearchStatement), null, null);
      }
      return ArgumentUtility.CheckType<SecurityManagerSearchArguments> ("searchArguments", searchArguments);
    }
  }
}