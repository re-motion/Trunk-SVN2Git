using System;
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Security.ExecutionEngine
{
  public class WxeDemandMethodPermissionAttributeHelper
  {
    // types

    // static members

    // member fields

    private Type _functionType;
    private WxeDemandTargetPermissionAttribute _attribute;

    // construction and disposing

    public WxeDemandMethodPermissionAttributeHelper (Type functionType, WxeDemandTargetPermissionAttribute attribute)
    {
      ArgumentUtility.CheckNotNull ("functionType", functionType);
      ArgumentUtility.CheckNotNull ("attribute", attribute);

      switch (attribute.MethodType)
      {
        case MethodType.Instance:
          CheckMethodNameNotNullOrEmpty (functionType, attribute.MethodName);
          break;
        case MethodType.Static:
          CheckSecurabeClassNotNull (functionType, attribute.SecurableClass);
          CheckMethodNameNotNullOrEmpty (functionType, attribute.MethodName);
          break;
        case MethodType.Constructor:
          CheckSecurabeClassNotNull (functionType, attribute.SecurableClass);
          break;
      }

      _functionType = functionType;
      _attribute = attribute;
    }

    // methods and properties

    public Type FunctionType
    {
      get { return _functionType; }
    }

    public MethodType MethodType
    {
      get { return _attribute.MethodType; }
    }

    public string MethodName
    {
      get { return _attribute.MethodName; }
    }

    public Type SecurableClass
    {
      get { return _attribute.SecurableClass; }
    }

    public Type GetTypeOfSecurableObject ()
    {
      WxeParameterDeclaration[] parameterDeclarations = WxeFunction.GetParameterDeclarations (_functionType);
      WxeParameterDeclaration parameterDeclaration = GetParameterDeclaration (parameterDeclarations);
      if (!typeof (ISecurableObject).IsAssignableFrom (parameterDeclaration.Type))
      {
        throw new WxeException (string.Format (
            "The parameter '{1}' specified by the {0} applied to WxeFunction '{2}' does not implement interface '{3}'.",
            _attribute.GetType ().Name, parameterDeclaration.Name, _functionType.FullName, typeof (ISecurableObject).FullName));
      }

      if (SecurableClass == null)
        return parameterDeclaration.Type;

      if (!parameterDeclaration.Type.IsAssignableFrom (SecurableClass))
      {
        throw new WxeException (string.Format (
            "The parameter '{1}' specified by the {0} applied to WxeFunction '{2}' is of type '{3}', which is not a base type of type '{4}'.",
            _attribute.GetType ().Name, parameterDeclaration.Name, _functionType.FullName, parameterDeclaration.Type.FullName, SecurableClass.FullName));
      }

      return SecurableClass;
    }

    public ISecurableObject GetSecurableObject (WxeFunction function)
    {
      ArgumentUtility.CheckNotNullAndType ("function", function, _functionType);
      
      WxeParameterDeclaration parameterDeclaration = GetParameterDeclaration (function.ParameterDeclarations);
      object parameterValue = function.Variables[parameterDeclaration.Name];
      if (parameterValue == null)
      {
        throw new WxeException (string.Format (
           "The parameter '{1}' specified by the {0} applied to WxeFunction '{2}' is null.",
           _attribute.GetType ().Name, parameterDeclaration.Name, _functionType.FullName));
      }

      ISecurableObject securableObject = parameterValue as ISecurableObject;
      if (securableObject == null)
      {
        throw new WxeException (string.Format (
            "The parameter '{1}' specified by the {0} applied to WxeFunction '{2}' does not implement interface '{3}'.",
            _attribute.GetType ().Name, parameterDeclaration.Name, _functionType.FullName, typeof (ISecurableObject).FullName));
      }

      if (SecurableClass != null && !parameterDeclaration.Type.IsAssignableFrom (SecurableClass))
      {
        throw new WxeException (string.Format (
            "The parameter '{1}' specified by the {0} applied to WxeFunction '{2}' is not derived from type '{3}'.",
            _attribute.GetType ().Name, parameterDeclaration.Name, _functionType.FullName, SecurableClass.FullName));
      }

      return securableObject;
    }

    private WxeParameterDeclaration GetParameterDeclaration (WxeParameterDeclaration[] parameterDeclarations)
    {
      if (parameterDeclarations.Length == 0)
      {
        throw new WxeException (string.Format (
            "WxeFunction '{1}' has a {0} applied, but does not define any parameters to supply the 'this-object'.",
            _attribute.GetType ().Name, _functionType.FullName));
      }

      if (StringUtility.IsNullOrEmpty (_attribute.ParameterName))
        return parameterDeclarations[0];

      for (int i = 0; i < parameterDeclarations.Length; i++)
      {
        if (string.Equals (parameterDeclarations[i].Name, _attribute.ParameterName, StringComparison.Ordinal))
          return parameterDeclarations[i];
      }

      throw new WxeException (string.Format (
          "The parameter '{1}' specified by the {0} applied to WxeFunction '{2}' is not a valid parameter of this function.",
          _attribute.GetType ().Name, _attribute.ParameterName, _functionType.FullName));
    }

    private void CheckMethodNameNotNullOrEmpty (Type functionType, string methodName)
    {
      if (StringUtility.IsNullOrEmpty (methodName))
      {
        throw new WxeException (string.Format (
            "The {0} applied to WxeFunction '{1}' does not specify the method to get the required permissions from.",
            _attribute.GetType ().Name, functionType.FullName));
      }
    }

    private void CheckSecurabeClassNotNull (Type functionType, Type securableClass)
    {
      if (securableClass == null)
      {
        throw new WxeException (string.Format (
            "The {0} applied to WxeFunction '{1}' does not specify a type implementing interface '{2}'.",
            _attribute.GetType ().Name, functionType.FullName, typeof (ISecurableObject).FullName));
      }
    }
  }
}