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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;

namespace Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests
{
  public class MethodWrapperEmitterTestBase
  {    
    private static int s_counter;

    private ClassEmitter _classEmitter;
    private bool _hasBeenBuilt;

    [SetUp]
    public virtual void SetUp ()
    {
      var uniqueName = GetType().Name + "." + s_counter;
      s_counter++;

      _classEmitter = new ClassEmitter (SetUpFixture.Scope, uniqueName, typeof (object), new Type[0], TypeAttributes.Class | TypeAttributes.Public, true);
      _hasBeenBuilt = false;
    }

    [TearDown]
    public virtual void TearDown ()
    {
      if (!_hasBeenBuilt)
        _classEmitter.BuildType();
    }

    protected MethodEmitter GetWrapperMethodFromEmitter (
        MethodBase executingTestMethod, Type[] publicParameterTypes, Type publicReturnType, MethodInfo innerMethod)
    {
      var methodName = executingTestMethod.DeclaringType.Name + "_" + executingTestMethod.Name;
      var method = _classEmitter.CreateMethod (methodName, MethodAttributes.Public | MethodAttributes.Static, publicReturnType, publicParameterTypes);

      var statement = new ILStatement ((memberEmitter, ilGenerator) =>
      {
        var emitter = new MethodWrapperEmitter (ilGenerator, innerMethod, publicParameterTypes, publicReturnType);
        emitter.EmitStaticMethodBody ();
      });
      method.CodeBuilder.AddStatement (statement);

      return method;
    }

    protected object BuildTypeAndInvokeMethod (MethodEmitter method, params object[] arguments)
    {
      _hasBeenBuilt = true;
      Type builtType = _classEmitter.BuildType();
      var methodInfo = builtType.GetMethod (method.MethodBuilder.Name);
      return methodInfo.Invoke (null, arguments);
    }
  }
}