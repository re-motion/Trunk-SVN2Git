using System;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.Web.Security.ExecutionEngine
{
  public enum MethodType
  {
    Instance,
    Static,
    Constructor
  }

  //// dirty - optional, irgendwann, vielleicht doch -> nie
  //[WxeDemandObjectPermission (AccessTypes.Edit)] // default: 1st parameter
  //[WxeDemandObjectPermission (AccessTypes.Edit, ParameterName = "obj")]
  //[WxeDemandClassPermission (AccessTypes.Search, typeof (Akt))]

  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public abstract class WxeDemandTargetPermissionAttribute : Attribute
  {
    // types

    // static members

    // member fields

    private MethodType _methodType;
    private Type _securableClass;
    private string _parameterName;
    private string _methodName;

    // construction and disposing

    public WxeDemandTargetPermissionAttribute (MethodType type)
    {
      _methodType = type;
    }

    // methods and properties

    public MethodType MethodType
    {
      get { return _methodType; }
    }

    public Type SecurableClass
    {
      get
      {
        return _securableClass;
      }
      protected set
      {
        ArgumentUtility.CheckTypeIsAssignableFrom ("SecurableClass", value, typeof (ISecurableObject));
        _securableClass = value;
      }
    }

    public string ParameterName
    {
      get { return _parameterName; }
      protected set { _parameterName = value; }
    }

    public string MethodName
    {
      get { return _methodName; }
      protected set { _methodName = value; }
    }

    protected void CheckDeclaringTypeOfMethodNameEnum (Enum methodNameEnum)
    {
      ArgumentUtility.CheckNotNull ("methodNameEnum", methodNameEnum);

      Type enumType = methodNameEnum.GetType ();

      if (enumType.DeclaringType == null)
        throw new ArgumentException (string.Format ("Enumerated type '{0}' is not declared as a nested type.", enumType.FullName), "methodNameEnum");

      if (!typeof (ISecurableObject).IsAssignableFrom (enumType.DeclaringType))
      {
        throw new ArgumentException (string.Format (
                "The declaring type of enumerated type '{0}' does not implement interface '{1}'.",
                enumType.FullName,
                typeof (ISecurableObject).FullName),
            "methodNameEnum");
      }
    }

    protected void CheckDeclaringTypeOfMethodNameEnum (Enum enumValue, Type securableClass)
    {
      CheckDeclaringTypeOfMethodNameEnum (enumValue);

      Type enumType = enumValue.GetType ();
      if (!enumType.DeclaringType.IsAssignableFrom (securableClass))
      {
        throw new ArgumentException (
            string.Format ("Type '{0}' cannot be assigned to the declaring type of enumerated type '{1}'.", securableClass, enumType),
            "securableClass");
      }
    }
  }
}