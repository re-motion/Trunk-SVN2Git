using System;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.Web.Security.ExecutionEngine
{
  public class WxeDemandCreatePermissionAttribute : WxeDemandTargetPermissionAttribute
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeDemandCreatePermissionAttribute (Type securableClass)
      : base (MethodType.Constructor)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));
      SecurableClass = securableClass;
    }

    // methods and properties
  }
}