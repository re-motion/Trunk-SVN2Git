// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration
{
  public interface IMethodEmitter : IAttributableEmitter
  {
    MethodBuilder MethodBuilder { get; }
    ILGenerator ILGenerator { get; }
    string Name { get; }
    ArgumentReference[] ArgumentReferences { get; }
    Type ReturnType { get; }
    Type[] ParameterTypes { get; }
    Expression[] GetArgumentExpressions ();
    CustomMethodEmitter SetParameterTypes (params Type[] parameters);
    CustomMethodEmitter SetReturnType (Type returnType);
    CustomMethodEmitter CopyParametersAndReturnType (MethodInfo method);
    CustomMethodEmitter ImplementByReturning (Expression result);
    CustomMethodEmitter ImplementByReturningVoid ();
    CustomMethodEmitter ImplementByReturningDefault ();
    CustomMethodEmitter ImplementByDelegating (TypeReference implementer, MethodInfo methodToCall);
    CustomMethodEmitter ImplementByBaseCall (MethodInfo baseMethod);
    CustomMethodEmitter ImplementByThrowing (Type exceptionType, string message);
    CustomMethodEmitter AddStatement (Statement statement);
    LocalReference DeclareLocal (Type type);
  }
}