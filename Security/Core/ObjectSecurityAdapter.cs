using System;

using Remotion.Utilities;

namespace Remotion.Security
{
  public class ObjectSecurityAdapter : IObjectSecurityAdapter
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public ObjectSecurityAdapter ()
    {
    }

    // methods and properties

    public bool HasAccessOnGetAccessor (ISecurableObject securableObject, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);

      if (SecurityFreeSection.IsActive)
        return true;

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return securityClient.HasPropertyReadAccess (securableObject, propertyName);
    }

    public bool HasAccessOnSetAccessor (ISecurableObject securableObject, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);

      if (SecurityFreeSection.IsActive)
        return true;

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return securityClient.HasPropertyWriteAccess (securableObject, propertyName);
    }
  }
}