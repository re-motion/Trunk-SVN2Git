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
  /// <typeparam name="TReferencingObject">The <see cref="Type"/> of the object that exposes the property.</typeparam>
  public abstract class SecurityManagerSearchServiceBase<TReferencingObject> : ISearchAvailableObjectsService
      where TReferencingObject: BaseSecurityManagerObject
  {
    protected delegate IQueryable<IBusinessObject> SearchDelegate (
        TReferencingObject referencingObject,
        IBusinessObjectReferenceProperty property,
        SecurityManagerSearchArguments searchArguments);

    public abstract bool SupportsProperty (IBusinessObjectReferenceProperty property);

    protected abstract SearchDelegate GetSearchDelegate (IBusinessObjectReferenceProperty property);

    public IBusinessObject[] Search (
        IBusinessObject referencingObject,
        IBusinessObjectReferenceProperty property,
        ISearchAvailableObjectsArguments searchArguments)
    {
      var referencingSecurityManagerObject = ArgumentUtility.CheckType<TReferencingObject> ("referencingObject", referencingObject);
      ArgumentUtility.CheckNotNull ("property", property);

      var searchDelegate = GetSearchDelegate(property);

      var securityManagerSearchArguments = CreateSearchArguments (searchArguments);
      return CreateQuery (searchDelegate, referencingSecurityManagerObject, property, securityManagerSearchArguments).ToArray();
    }

    private IQueryable<IBusinessObject> CreateQuery (
        SearchDelegate searchDelegate,
        TReferencingObject referencingSecurityManagerObject,
        IBusinessObjectReferenceProperty property,
        SecurityManagerSearchArguments searchArguments)
    {
      var query = searchDelegate (referencingSecurityManagerObject, property, searchArguments);
      // ReSharper disable RedundantCast
      return query.Apply ((IResultSizeConstraint) searchArguments);
      // ReSharper restore RedundantCast
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