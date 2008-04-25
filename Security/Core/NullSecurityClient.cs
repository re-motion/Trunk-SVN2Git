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