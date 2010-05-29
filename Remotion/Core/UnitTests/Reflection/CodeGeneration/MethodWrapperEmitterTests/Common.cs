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
using System.Reflection.Emit;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests
{
  [TestFixture]
  public class Common : TestBase
  {
    [Test]
    public void EmitMethodBody_ForPrivatePropertyGetter_ForReferenceType ()
    {
      Type declaringType = typeof (ClassWithReferenceTypeProperties);
      var propertyInfo = declaringType.GetProperty ("PropertyWithPrivateGetterAndSetter", BindingFlags.NonPublic | BindingFlags.Instance);
      var methodInfo = propertyInfo.GetGetMethod (true);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object) };
      var dynamicMethod = new DynamicMethod ("", returnType, parameterTypes, declaringType, false);
      var ilGenerator = dynamicMethod.GetILGenerator();

      var emitter = new MethodWrapperEmitter();
      emitter.EmitStaticMethodBody (ilGenerator, methodInfo, parameterTypes, returnType);

      var propertyGetter = (Func<object, object>) dynamicMethod.CreateDelegate (typeof (Func<object, object>));

      var expectedValue = new SimpleReferenceType();
      var obj = new ClassWithReferenceTypeProperties();
      obj.SetPropertyWithPrivateGetterAndSetter (expectedValue);
      Assert.That (propertyGetter (obj), Is.SameAs (expectedValue));
    }

    [Test]
    public void EmitMethodBody_ForOverriddenMethod ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeReturnValue", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);

      var obj = new DerivedClassWithMethods { DerivedInstanceReferenceTypeValue = new SimpleReferenceType() };

      Assert.That (BuildTypeAndInvokeMethod (method, obj), Is.SameAs (obj.DerivedInstanceReferenceTypeValue));
    }

    [Test]
    public void EmitMethodBody_ForValueTypeInstance ()
    {
      Type declaringType = typeof (StructWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeReturnValue", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object) };
      var method = GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod (), parameterTypes, returnType, methodInfo);

      var obj = new StructWithMethods { InstanceReferenceTypeValue = new SimpleReferenceType () };

      Assert.That (BuildTypeAndInvokeMethod (method, obj), Is.SameAs (obj.InstanceReferenceTypeValue));
    }

    public static object Test1 (object A_0)
    {
      return ((StructWithMethods) A_0).InstanceReferenceTypeValue;
    }

  }
}