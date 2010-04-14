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
using System.ComponentModel;
using Remotion.Security.Configuration;
using Remotion.Security.Metadata;
using Remotion.Utilities;

namespace Remotion.Security
{
  public class SecurityClient : INullObject
  {
    public static readonly SecurityClient Null = new NullSecurityClient();

    public static SecurityClient CreateSecurityClientFromConfiguration ()
    {
      ISecurityProvider securityProvider = SecurityConfiguration.Current.SecurityProvider;

      if (securityProvider.IsNull)
        return SecurityClient.Null;

      return new SecurityClient (
          securityProvider,
          SecurityConfiguration.Current.PermissionProvider,
          SecurityConfiguration.Current.PrincipalProvider,
          SecurityConfiguration.Current.FunctionalSecurityStrategy);
    }

    private readonly ISecurityProvider _securityProvider;
    private readonly IPermissionProvider _permissionProvider;
    private readonly IPrincipalProvider _principalProvider;
    private readonly IFunctionalSecurityStrategy _functionalSecurityStrategy;

    public SecurityClient (
        ISecurityProvider securityProvider,
        IPermissionProvider permissionProvider,
        IPrincipalProvider principalProvider,
        IFunctionalSecurityStrategy functionalSecurityStrategy)
    {
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.CheckNotNull ("permissionProvider", permissionProvider);
      ArgumentUtility.CheckNotNull ("userProvider", principalProvider);
      ArgumentUtility.CheckNotNull ("functionalSecurityStrategy", functionalSecurityStrategy);

      _securityProvider = securityProvider;
      _permissionProvider = permissionProvider;
      _principalProvider = principalProvider;
      _functionalSecurityStrategy = functionalSecurityStrategy;
    }


    public bool HasAccess (ISecurableObject securableObject, params AccessType[] requiredAccessTypes)
    {
      return HasAccess (securableObject, _principalProvider.GetPrincipal(), requiredAccessTypes);
    }

    public virtual bool HasAccess (ISecurableObject securableObject, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("requiredAccessTypes", requiredAccessTypes);

      if (SecurityFreeSection.IsActive)
        return true;

      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy();
      if (objectSecurityStrategy == null)
        throw new InvalidOperationException ("The securableObject did not return an IObjectSecurityStrategy.");

      return objectSecurityStrategy.HasAccess (_securityProvider, principal, requiredAccessTypes);
    }

    public void CheckAccess (ISecurableObject securableObject, params AccessType[] requiredAccessTypes)
    {
      CheckAccess (securableObject, _principalProvider.GetPrincipal(), requiredAccessTypes);
    }

    public void CheckAccess (ISecurableObject securableObject, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("requiredAccessTypes", requiredAccessTypes);

      if (!HasAccess (securableObject, principal, requiredAccessTypes))
        throw CreatePermissionDeniedException ("Access has been denied.");
    }


    public bool HasStatelessAccess (Type securableClass, params AccessType[] requiredAccessTypes)
    {
      return HasStatelessAccess (securableClass, _principalProvider.GetPrincipal(), requiredAccessTypes);
    }

    public virtual bool HasStatelessAccess (Type securableClass, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("requiredAccessTypes", requiredAccessTypes);

      if (SecurityFreeSection.IsActive)
        return true;

      return _functionalSecurityStrategy.HasAccess (securableClass, _securityProvider, principal, requiredAccessTypes);
    }

    public void CheckStatelessAccess (Type securableClass, params AccessType[] requiredAccessTypes)
    {
      CheckStatelessAccess (securableClass, _principalProvider.GetPrincipal(), requiredAccessTypes);
    }

    public void CheckStatelessAccess (Type securableClass, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("requiredAccessTypes", requiredAccessTypes);

      if (!HasStatelessAccess (securableClass, principal, requiredAccessTypes))
        throw CreatePermissionDeniedException ("Access has been denied.");
    }


    public bool HasMethodAccess (ISecurableObject securableObject, string methodName)
    {
      return HasMethodAccess (securableObject, methodName, _principalProvider.GetPrincipal());
    }

    public virtual bool HasMethodAccess (ISecurableObject securableObject, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredMethodPermissions (securableObject.GetSecurableType(), methodName);
      if (requiredAccessTypeEnums == null)
        throw new InvalidOperationException ("IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.");

      return HasAccess (securableObject, methodName, requiredAccessTypeEnums, principal);
    }

    public void CheckMethodAccess (ISecurableObject securableObject, string methodName)
    {
      CheckMethodAccess (securableObject, methodName, _principalProvider.GetPrincipal());
    }

    public void CheckMethodAccess (ISecurableObject securableObject, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      if (!HasMethodAccess (securableObject, methodName, principal))
      {
        throw CreatePermissionDeniedException (
            "Access to method '{0}' on type '{1}' has been denied.", methodName, securableObject.GetSecurableType().FullName);
      }
    }


    public bool HasPropertyReadAccess (ISecurableObject securableObject, string propertyName)
    {
      return HasPropertyReadAccess (securableObject, propertyName, _principalProvider.GetPrincipal());
    }

    public virtual bool HasPropertyReadAccess (ISecurableObject securableObject, string propertyName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredPropertyReadPermissions (securableObject.GetSecurableType(), propertyName);
      if (requiredAccessTypeEnums == null)
        throw new InvalidOperationException ("IPermissionProvider.GetRequiredPropertyReadPermissions evaluated and returned null.");

      if (requiredAccessTypeEnums.Length == 0)
        requiredAccessTypeEnums = new Enum[] { GeneralAccessTypes.Read };

      return HasAccess (securableObject, propertyName, requiredAccessTypeEnums, principal);
    }

    public void CheckPropertyReadAccess (ISecurableObject securableObject, string propertyName)
    {
      CheckPropertyReadAccess (securableObject, propertyName, _principalProvider.GetPrincipal());
    }

    public void CheckPropertyReadAccess (ISecurableObject securableObject, string propertyName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      if (!HasPropertyReadAccess (securableObject, propertyName, principal))
      {
        throw CreatePermissionDeniedException (
            "Access to get-accessor of property '{0}' on type '{1}' has been denied.", propertyName, securableObject.GetSecurableType().FullName);
      }
    }

    public bool HasPropertyWriteAccess (ISecurableObject securableObject, string propertyName)
    {
      return HasPropertyWriteAccess (securableObject, propertyName, _principalProvider.GetPrincipal());
    }

    public virtual bool HasPropertyWriteAccess (ISecurableObject securableObject, string propertyName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredPropertyWritePermissions (securableObject.GetSecurableType(), propertyName);
      if (requiredAccessTypeEnums == null)
        throw new InvalidOperationException ("IPermissionProvider.GetRequiredPropertyWritePermissions evaluated and returned null.");

      if (requiredAccessTypeEnums.Length == 0)
        requiredAccessTypeEnums = new Enum[] { GeneralAccessTypes.Edit };

      return HasAccess (securableObject, propertyName, requiredAccessTypeEnums, principal);
    }

    public void CheckPropertyWriteAccess (ISecurableObject securableObject, string propertyName)
    {
      CheckPropertyWriteAccess (securableObject, propertyName, _principalProvider.GetPrincipal());
    }

    public void CheckPropertyWriteAccess (ISecurableObject securableObject, string propertyName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      if (!HasPropertyWriteAccess (securableObject, propertyName, principal))
      {
        throw CreatePermissionDeniedException (
            "Access to set-accessor of property '{0}' on type '{1}' has been denied.", propertyName, securableObject.GetSecurableType().FullName);
      }
    }


    public bool HasConstructorAccess (Type securableClass)
    {
      return HasConstructorAccess (securableClass, _principalProvider.GetPrincipal());
    }

    public virtual bool HasConstructorAccess (Type securableClass, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("principal", principal);

      if (SecurityFreeSection.IsActive)
        return true;

      AccessType[] requiredAccessTypes = new[] { AccessType.Get (GeneralAccessTypes.Create) };

      return _functionalSecurityStrategy.HasAccess (securableClass, _securityProvider, principal, requiredAccessTypes);
    }

    public void CheckConstructorAccess (Type securableClass)
    {
      CheckConstructorAccess (securableClass, _principalProvider.GetPrincipal());
    }

    public void CheckConstructorAccess (Type securableClass, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNull ("principal", principal);

      if (!HasConstructorAccess (securableClass, principal))
        throw CreatePermissionDeniedException ("Access to constructor of type '{0}' has been denied.", securableClass.FullName);
    }


    public bool HasStaticMethodAccess (Type securableClass, string methodName)
    {
      return HasStaticMethodAccess (securableClass, methodName, _principalProvider.GetPrincipal());
    }

    public virtual bool HasStaticMethodAccess (Type securableClass, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredStaticMethodPermissions (securableClass, methodName);
      if (requiredAccessTypeEnums == null)
        throw new InvalidOperationException ("IPermissionProvider.GetRequiredStaticMethodPermissions evaluated and returned null.");

      return HasStatelessAccess (securableClass, methodName, requiredAccessTypeEnums, principal);
    }

    public void CheckStaticMethodAccess (Type securableClass, string methodName)
    {
      CheckStaticMethodAccess (securableClass, methodName, _principalProvider.GetPrincipal());
    }

    public void CheckStaticMethodAccess (Type securableClass, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      if (!HasStaticMethodAccess (securableClass, methodName, principal))
        throw CreatePermissionDeniedException ("Access to static method '{0}' on type '{1}' has been denied.", methodName, securableClass.FullName);
    }


    [EditorBrowsable (EditorBrowsableState.Never)]
    public bool HasStatelessMethodAccess (Type securableClass, string methodName)
    {
      return HasStatelessMethodAccess (securableClass, methodName, _principalProvider.GetPrincipal());
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public virtual bool HasStatelessMethodAccess (Type securableClass, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull ("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("principal", principal);

      Enum[] requiredAccessTypeEnums = _permissionProvider.GetRequiredMethodPermissions (securableClass, methodName);
      if (requiredAccessTypeEnums == null)
        throw new InvalidOperationException ("IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.");

      return HasStatelessAccess (securableClass, methodName, requiredAccessTypeEnums, principal);
    }


    private bool HasAccess (ISecurableObject securableObject, string memberName, Enum[] requiredAccessTypeEnums, ISecurityPrincipal principal)
    {
      if (requiredAccessTypeEnums.Length == 0)
        throw new ArgumentException (string.Format ("The member '{0}' does not define required permissions.", memberName), "requiredAccessTypeEnums");

      return HasAccess (securableObject, principal, ConvertRequiredAccessTypeEnums (requiredAccessTypeEnums));
    }

    private bool HasStatelessAccess (Type securableClass, string memberName, Enum[] requiredAccessTypeEnums, ISecurityPrincipal principal)
    {
      if (requiredAccessTypeEnums.Length == 0)
        throw new ArgumentException (string.Format ("The member '{0}' does not define required permissions.", memberName), "requiredAccessTypeEnums");

      return HasStatelessAccess (securableClass, principal, ConvertRequiredAccessTypeEnums (requiredAccessTypeEnums));
    }


    private AccessType[] ConvertRequiredAccessTypeEnums (Enum[] requiredAccessTypeEnums)
    {
      return Array.ConvertAll (requiredAccessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
    }

    private PermissionDeniedException CreatePermissionDeniedException (string message, params object[] args)
    {
      return new PermissionDeniedException (string.Format (message, args));
    }


    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
