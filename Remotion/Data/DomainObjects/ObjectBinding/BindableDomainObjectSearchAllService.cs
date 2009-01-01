// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectSearchAllService : ISearchAvailableObjectsService
  {
    private static readonly QueryCache s_queryCache = new QueryCache ();
    private static readonly MethodInfo s_getQueryMethod = 
        typeof (BindableDomainObjectSearchAllService).GetMethod ("GetQuery", BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

    public bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      var domainObjectType = GetDomainObjectType (property);
      return domainObjectType != null;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      if (!SupportsProperty (property))
      {
        var message = string.Format ("The property '{0}' on type '{1}' is not supported by the BindableDomainObjectSearchAllService: The service "
            + "only supports relation properties (ie. references to other DomainObject instances).", property.Identifier, 
            property.ReflectedClass.Identifier);
        throw new ArgumentException (message, "property");
      }

      var domainObjectType = GetDomainObjectType (property);
      return GetAllObjects (domainObjectType);
    }

    private Type GetDomainObjectType (IBusinessObjectReferenceProperty property)
    {
      if (typeof (DomainObject).IsAssignableFrom (property.PropertyType))
        return property.PropertyType;
      else if (property.IsList && typeof (DomainObject).IsAssignableFrom (property.ListInfo.ItemType))
        return property.ListInfo.ItemType;
      else
        return null;
    }

    public IBusinessObject[] GetAllObjects (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      var query = GetQuery (type);
      return ClientTransaction.Current.QueryManager.GetCollection (query).Cast<IBusinessObject>().ToArray();
    }

    private IQuery GetQuery (Type type)
    {
      if (!typeof (DomainObject).IsAssignableFrom (type))
        throw new ArgumentException ("This service only supports queries for DomainObject types.", "type");
      if (!BindableDomainObjectProvider.IsBindableObjectImplementation (type))
      {
        var message = string.Format ("This service only supports queries for bindable DomainObject types, the given type '{0}' is not a bindable "
            + "type. Derive from BindableDomainObject or apply the BindableDomainObjectAttribute.", type.FullName);
        throw new ArgumentException (message, "type");
      }

      Assertion.IsNotNull (s_getQueryMethod);

      return (IQuery) s_getQueryMethod.MakeGenericMethod (type).Invoke (this, null);
    }

// ReSharper disable UnusedPrivateMember
    private IQuery GetQuery<T> () where T : DomainObject
    {
      return s_queryCache.GetQuery<T> (typeof (T).AssemblyQualifiedName, source => from x in source select x);
    }
// ReSharper restore UnusedPrivateMember
  }
}