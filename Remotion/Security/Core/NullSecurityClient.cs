// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using Remotion.Collections;
using Remotion.Security.Metadata;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a nullable <see cref="SecurityClient"/> according to the "Null Object Pattern".
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class NullSecurityClient : SecurityClient, INullObject
  {
    public NullSecurityClient ()
        : base (
            new NullSecurityProvider(),
            new PermissionReflector(),
            new NullPrincipalProvider(),
            new FunctionalSecurityStrategy (
                new SecurityStrategy (new NullCache<ISecurityPrincipal, AccessType[]>(), new NullGlobalAccessTypeCacheProvider())),
            new ReflectionBasedMemberResolver())
    {
    }

    public override bool HasAccess (ISecurableObject securableObject, ISecurityPrincipal user, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return true;
    }

    public override bool HasStatelessAccess (Type securableClass, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("user", principal);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("requiredAccessTypes", requiredAccessTypes);

      return true;
    }

    public override bool HasMethodAccess (ISecurableObject securableObject, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasMethodAccess (ISecurableObject securableObject, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      ArgumentUtility.CheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasPropertyReadAccess (ISecurableObject securableObject, string propertyName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasPropertyReadAccess (ISecurableObject securableObject, PropertyInfo propertyInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasPropertyWriteAccess (ISecurableObject securableObject, string propertyName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasPropertyWriteAccess (ISecurableObject securableObject, PropertyInfo propertyInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasConstructorAccess (Type securableClass, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasStaticMethodAccess (Type securableClass, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasStaticMethodAccess (Type securableClass, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      ArgumentUtility.CheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasStatelessMethodAccess (Type securableClass, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasStatelessMethodAccess (Type securableClass, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      ArgumentUtility.CheckNotNull ("principal", principal);

      return true;
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}
