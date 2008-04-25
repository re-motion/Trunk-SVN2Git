using System;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.Web.Security.ExecutionEngine
{
  public class WxeDemandTargetMethodPermissionAttribute : WxeDemandTargetPermissionAttribute
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeDemandTargetMethodPermissionAttribute (object methodNameEnum)
      : base (MethodType.Instance)
    {
      Enum enumValue = ArgumentUtility.CheckNotNullAndType<Enum> ("methodNameEnum", methodNameEnum);
      Type enumType = enumValue.GetType ();

      CheckDeclaringTypeOfMethodNameEnum (enumValue);

      MethodName = enumValue.ToString ();
      SecurableClass = enumType.DeclaringType;
    }

    public WxeDemandTargetMethodPermissionAttribute (object methodNameEnum, Type securableClass)
      : base (MethodType.Instance)
    {
      Enum enumValue = ArgumentUtility.CheckNotNullAndType<Enum> ("methodNameEnum", methodNameEnum);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      CheckDeclaringTypeOfMethodNameEnum (enumValue, securableClass);

      MethodName = enumValue.ToString ();
      SecurableClass = securableClass;
    }

    public WxeDemandTargetMethodPermissionAttribute (string methodName)
      : base (MethodType.Instance)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      MethodName = methodName;
    }

    public WxeDemandTargetMethodPermissionAttribute (string methodName, Type securableClass)
      : base (MethodType.Instance)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      MethodName = methodName;
      SecurableClass = securableClass;
    }

    // methods and properties

    public new string ParameterName
    {
      get { return base.ParameterName; }
      set { base.ParameterName = value; }
    }
  }
}