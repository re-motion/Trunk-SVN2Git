// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests
{
  [TestFixture]
  public class ReturnValues : TestBase
  {
    [Test]
    public void EmitMethodBody_ForInstanceMethodWithReferenceTypeReturnValue ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeReturnValue", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      var obj = new ClassWithMethods { InstanceReferenceTypeValue = new SimpleReferenceType() };

      Assert.That (BuildTypeAndInvokeMethod (method, obj), Is.SameAs (obj.InstanceReferenceTypeValue));
    }

    [Test]
    public void EmitMethodBody_ForInstanceMethodWithReferenceTypeReturnValue_WithNull ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeReturnValue", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod (), parameterTypes, returnType, methodInfo);

      var obj = new ClassWithMethods { InstanceReferenceTypeValue = null };

      Assert.That (BuildTypeAndInvokeMethod (method, obj), Is.Null);
    }

    [Test]
    public void EmitMethodBody_ForInstanceMethodWithValueTypeReturnValue ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithValueTypeReturnValue", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      var obj = new ClassWithMethods { InstanceValueTypeValue = 100 };

      Assert.That (BuildTypeAndInvokeMethod (method, obj), Is.EqualTo (obj.InstanceValueTypeValue));
    }

    [Test]
    public void EmitMethodBody_ForStaticMethodWithReferenceTypeReturnValue ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("StaticMethodWithReferenceTypeReturnValue", BindingFlags.Public | BindingFlags.Static);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      ClassWithMethods.StaticReferenceTypeValue = new SimpleReferenceType();

      Assert.That (BuildTypeAndInvokeMethod (method, new object[] { null }), Is.SameAs (ClassWithMethods.StaticReferenceTypeValue));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "The ReturnType of the wrappedMethod cannot be assigned to the wrapperReturnType.\r\nParameter name: wrappedMethod")]
    public void EmitMethodBody_ReturnTypesDoNotMatch ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeReturnValue", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object) };
      var method = ClassEmitter.CreateMethod (MethodInfo.GetCurrentMethod ().Name, MethodAttributes.Public | MethodAttributes.Static)
          .SetParameterTypes (parameterTypes)
          .SetReturnType (returnType);

      var emitter = new MethodWrapperEmitter ();
      emitter.EmitStaticMethodBody (method.ILGenerator, methodInfo, typeof (string), parameterTypes);
    }
  }
}