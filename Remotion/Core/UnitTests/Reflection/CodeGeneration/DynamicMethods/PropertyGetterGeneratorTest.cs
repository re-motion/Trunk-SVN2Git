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
using Remotion.Reflection.CodeGeneration.DynamicMethods;
using Remotion.UnitTests.Reflection.CodeGeneration.DynamicMethods.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.CodeGeneration.DynamicMethods
{
  [TestFixture]
  public class PropertyGetterGeneratorTest
  {
    [Test]
    public void Build_ForPublicPropertyGetter_ForReferenceType ()
    {
      var obj = new ClassWithReferenceTypeProperties { PropertyWithPublicGetterAndSetter = new SimpleReferenceType() };
      var propertyInfo = typeof (ClassWithReferenceTypeProperties).GetProperty (
          "PropertyWithPublicGetterAndSetter", BindingFlags.Public | BindingFlags.Instance);
      var methodInfo = propertyInfo.GetGetMethod();

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object) };
      var dynamicMethod = new DynamicMethod ("", returnType, parameterTypes, typeof (ClassWithReferenceTypeProperties), false);
      var ilGenerator = dynamicMethod.GetILGenerator();

      var generator = new PropertyGetterGenerator (ilGenerator);
      generator.BuildWrapperMethod (methodInfo, returnType, parameterTypes);

      var propertyGetter = (Func<object, object>) dynamicMethod.CreateDelegate (typeof (Func<object, object>));

      Assert.That (propertyGetter (obj), Is.SameAs (obj.PropertyWithPublicGetterAndSetter));
    }

    [Test]
    public void Build_ForOverriddenPropertyGetter_ForReferenceType ()
    {
      var obj = new DerivedClassWithReferenceTypeProperties { PropertyWithPublicGetterAndSetter = new SimpleReferenceType() };
      var propertyInfo = typeof (ClassWithReferenceTypeProperties).GetProperty (
          "PropertyWithPublicGetterAndSetter", BindingFlags.Public | BindingFlags.Instance);
      var methodInfo = propertyInfo.GetGetMethod();

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object) };
      var dynamicMethod = new DynamicMethod ("", returnType, parameterTypes, typeof (ClassWithReferenceTypeProperties), false);
      var ilGenerator = dynamicMethod.GetILGenerator();

      var generator = new PropertyGetterGenerator (ilGenerator);
      generator.BuildWrapperMethod (methodInfo, returnType, parameterTypes);

      var propertyGetter = (Func<object, object>) dynamicMethod.CreateDelegate (typeof (Func<object, object>));

      Assert.That (propertyGetter (obj), Is.SameAs (obj.PropertyWithPublicGetterAndSetter));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "The ReturnType of the wrappedGetMethod cannot be assigned to the wrapperReturnType.\r\nParameter name: wrappedGetMethod")]
    public void Build_ReturnTypesDoNotMatch ()
    {
      var propertyInfo = typeof (ClassWithReferenceTypeProperties).GetProperty (
          "PropertyWithPublicGetterAndSetter", BindingFlags.Public | BindingFlags.Instance);
      var methodInfo = propertyInfo.GetGetMethod();

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object) };
      var dynamicMethod = new DynamicMethod ("", returnType, parameterTypes, typeof (ClassWithReferenceTypeProperties), false);
      var ilGenerator = dynamicMethod.GetILGenerator();

      var generator = new PropertyGetterGenerator (ilGenerator);
      generator.BuildWrapperMethod (methodInfo, typeof (string), parameterTypes);
    }
  }
}