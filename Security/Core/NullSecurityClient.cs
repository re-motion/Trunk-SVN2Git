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
using System.Security.Principal;
using Remotion.Collections;
using Remotion.Security.Metadata;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a nullable <see cref="SecurityClient"/> according to the "Null Object Pattern".
  /// </summary>
  public class NullSecurityClient : SecurityClient, INullObject
  {
    public NullSecurityClient ()
        : base (
            new NullSecurityProvider(), 
            new PermissionReflector(), 
            new NullUserProvider(), 
            new FunctionalSecurityStrategy (new SecurityStrategy (new NullCache<string, AccessType[]>(), new NullGlobalAccessTypeCacheProvider())))
    {
    }

    public override bool HasAccess (ISecurableObject securableObject, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return true;
    }

    public override bool HasStatelessAccess (Type securableClass, IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return true;
    }

    public override bool HasMethodAccess (ISecurableObject securableObject, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("user", user);

      return true;
    }

    public override bool HasPropertyReadAccess (ISecurableObject securableObject, string propertyName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("user", user);

      return true;
    }

    public override bool HasPropertyWriteAccess (ISecurableObject securableObject, string propertyName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("user", user);

      return true;
    }

    public override bool HasConstructorAccess (Type securableClass, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("user", user);

      return true;
    }

    public override bool HasStaticMethodAccess (Type securableClass, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("user", user);

      return true;
    }

    public override bool HasStatelessMethodAccess (Type securableClass, string methodName, IPrincipal user)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("user", user);

      return true;
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}
