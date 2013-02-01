// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

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
      WxeParameterDeclaration[] parameterDeclarations = WxeVariablesContainer.GetParameterDeclarations (_functionType);
      WxeParameterDeclaration parameterDeclaration = GetParameterDeclaration (parameterDeclarations);

      var actualParameterType = GetActualParameterType (parameterDeclaration.Type);
      if (!typeof (ISecurableObject).IsAssignableFrom (actualParameterType))
      {
        throw new WxeException (string.Format (
            "The parameter '{1}' specified by the {0} applied to WxeFunction '{2}' does not implement interface '{3}'.",
            _attribute.GetType ().Name, parameterDeclaration.Name, _functionType.FullName, typeof (ISecurableObject).FullName));
      }

      if (SecurableClass == null)
        return actualParameterType;

      CheckParameterDeclarationMatchesSecurableClass (actualParameterType, parameterDeclaration.Name);

      return SecurableClass;
    }

    public ISecurableObject GetSecurableObject (WxeFunction function)
    {
      ArgumentUtility.CheckNotNullAndType ("function", function, _functionType);
      
      WxeParameterDeclaration parameterDeclaration = GetParameterDeclaration (function.VariablesContainer.ParameterDeclarations);
      object parameterValue = function.Variables[parameterDeclaration.Name];
      if (parameterValue == null)
      {
        throw new WxeException (string.Format (
           "The parameter '{1}' specified by the {0} applied to WxeFunction '{2}' is null.",
           _attribute.GetType ().Name, parameterDeclaration.Name, _functionType.FullName));
      }

      // TODO 4405: First, check for ISecurableObjectHandle attribute - if yes, use this to get the ISecurableObject. Otherwise, get object as below.

      ISecurableObject securableObject = parameterValue as ISecurableObject;
      if (securableObject == null)
      {
        throw new WxeException (string.Format (
            "The parameter '{1}' specified by the {0} applied to WxeFunction '{2}' does not implement interface '{3}'.",
            _attribute.GetType ().Name, parameterDeclaration.Name, _functionType.FullName, typeof (ISecurableObject).FullName));
      }

      if (SecurableClass != null)
        CheckParameterDeclarationMatchesSecurableClass (parameterDeclaration.Type, parameterDeclaration.Name);
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

    private void CheckParameterDeclarationMatchesSecurableClass (Type parameterType, string parameterName)
    {
      if (!parameterType.IsAssignableFrom (SecurableClass))
      {
        throw new WxeException (
            string.Format (
                "The parameter '{1}' specified by the {0} applied to WxeFunction '{2}' is of type '{3}', which is not a base type of type '{4}'.",
                _attribute.GetType ().Name,
                parameterName,
                _functionType.FullName,
                parameterType.FullName,
                SecurableClass.FullName));
      }
    }

    private static Type GetActualParameterType (Type declaredParameterType)
    {
      var handleAttributes = (IHandleAttribute[]) declaredParameterType.GetCustomAttributes (typeof (IHandleAttribute), true);
      if (handleAttributes.Any())
        return handleAttributes.First().GetReferencedType (declaredParameterType);
      
      return declaredParameterType;
    }
  }
}
