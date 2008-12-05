// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  public class CustomMethodEmitter : IMethodEmitter
  {
    private readonly MethodEmitter _innerEmitter;
    private readonly CustomClassEmitter _declaringType;
    private readonly string _name;
    
    private Type _returnType;
    private Type[] _parameterTypes;

    public CustomMethodEmitter (CustomClassEmitter declaringType, string name, MethodAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("attributes", attributes);

      _declaringType = declaringType;
      _innerEmitter = _declaringType.InnerEmitter.CreateMethod (name, attributes);
      _name = name;
      _returnType = null;
      _parameterTypes = new Type[0];
      SetReturnType (typeof (void));
    }

    public MethodBuilder MethodBuilder
    {
      get { return _innerEmitter.MethodBuilder; }
    }

    internal MethodEmitter InnerEmitter
    {
      get { return _innerEmitter; }
    }

    public ILGenerator ILGenerator
    {
      get { return _innerEmitter.CodeBuilder.Generator; }
    }

    public string Name
    {
      get { return _name; }
    }

    public ArgumentReference[] ArgumentReferences
    {
      get { return InnerEmitter.Arguments; }
    }

    public Type ReturnType
    {
      get { return _returnType; }
    }

    public Type[] ParameterTypes
    {
      get { return _parameterTypes; }
    }

    public Expression[] GetArgumentExpressions ()
    {
      Expression[] argumentExpressions = new Expression[ArgumentReferences.Length];
      for (int i = 0; i < argumentExpressions.Length; ++i)
        argumentExpressions[i] = ArgumentReferences[i].ToExpression ();
      return argumentExpressions;
    }

    public CustomMethodEmitter SetParameterTypes (params Type[] parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      InnerEmitter.SetParameters (parameters);
      _parameterTypes = parameters;
      return this;
    }

    public CustomMethodEmitter SetReturnType (Type returnType)
    {
      ArgumentUtility.CheckNotNull ("returnType", returnType);
      InnerEmitter.SetReturnType (returnType);
      _returnType = returnType;
      return this;
    }

    public CustomMethodEmitter CopyParametersAndReturnType (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      _innerEmitter.CopyParametersAndReturnTypeFrom (method, _declaringType.InnerEmitter);
      return this;
    }

    public CustomMethodEmitter ImplementByReturning (Expression result)
    {
      ArgumentUtility.CheckNotNull ("result", result);
      return AddStatement (new ReturnStatement (result));
    }

    public CustomMethodEmitter ImplementByReturningVoid ()
    {
      return AddStatement (new ReturnStatement ());
    }

    public CustomMethodEmitter ImplementByReturningDefault ()
    {
      if (ReturnType == typeof (void))
        return ImplementByReturningVoid ();
      else
      {
        return ImplementByReturning (new InitObjectExpression (this, ReturnType));
      }
    }

    public CustomMethodEmitter ImplementByDelegating (TypeReference implementer, MethodInfo methodToCall)
    {
      AddDelegatingCallStatements (methodToCall, implementer, true);
      return this;
    }

    public CustomMethodEmitter ImplementByBaseCall (MethodInfo baseMethod)
    {
      ArgumentUtility.CheckNotNull ("baseMethod", baseMethod);

      if (baseMethod.IsAbstract)
        throw new ArgumentException (string.Format ("The given method {0}.{1} is abstract.", baseMethod.DeclaringType.FullName, baseMethod.Name),
            "baseMethod");
      
      AddDelegatingCallStatements (baseMethod, new TypeReferenceWrapper (SelfReference.Self, _declaringType.TypeBuilder), false);
      return this;
    }

    private void AddDelegatingCallStatements (MethodInfo methodToCall, TypeReference owner, bool callVirtual)
    {
      Expression[] argumentExpressions = GetArgumentExpressions();

      TypedMethodInvocationExpression delegatingCall;
      if (callVirtual)
        delegatingCall = new AutomaticMethodInvocationExpression (owner, methodToCall, argumentExpressions);
      else
        delegatingCall = new TypedMethodInvocationExpression (owner, methodToCall, argumentExpressions);

      AddStatement (new ReturnStatement (delegatingCall));
    }

    public CustomMethodEmitter ImplementByThrowing (Type exceptionType, string message)
    {
      ArgumentUtility.CheckNotNull ("exceptionType", exceptionType);
      ArgumentUtility.CheckNotNull ("message", message);
      AddStatement (new ThrowStatement (exceptionType, message));
      return this;
    }

    public CustomMethodEmitter AddStatement (Statement statement)
    {
      ArgumentUtility.CheckNotNull ("statement", statement);
      InnerEmitter.CodeBuilder.AddStatement (statement);
      return this;
    }

    public LocalReference DeclareLocal (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return InnerEmitter.CodeBuilder.DeclareLocal (type);
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      ArgumentUtility.CheckNotNull ("customAttribute", customAttribute);
      _innerEmitter.MethodBuilder.SetCustomAttribute (customAttribute);
    }
  }
}
