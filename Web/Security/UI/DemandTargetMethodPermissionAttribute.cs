using System;

namespace Remotion.Web.Security.UI
{
  public class DemandTargetMethodPermissionAttribute : DemandTargetPermissionAttribute
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public DemandTargetMethodPermissionAttribute (object methodEnum)
      : base (methodEnum)
    {
    }

    public DemandTargetMethodPermissionAttribute (object methodEnum, Type securableClass)
      : base (methodEnum, securableClass)
    {
    }

    public DemandTargetMethodPermissionAttribute (string methodName)
      : base (methodName)
    {
    }

    public DemandTargetMethodPermissionAttribute (string methodName, Type securableClass)
      : base (methodName, securableClass)
    {
    }

    // methods and properties
  }
}