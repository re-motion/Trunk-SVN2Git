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
using Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests
{
  [TestFixture]
  public class ArgumentChecks : TestBase
  {
    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "The wrapperReturnType ('String') cannot be assigned from the return type ('SimpleReferenceType') of the wrappedMethod.\r\n"
        + "Parameter name: wrapperReturnType")]
    public void EmitMethodBody_ReturnTypesDoNotMatch ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeReturnValue", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (string);
      Type[] parameterTypes = new[] { typeof (object) };
      GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "The wrapperParameterType #1 ('String') cannot be assigned to parameter type #0 ('SimpleReferenceType') of the wrappedMethod.\r\n"
        + "Parameter name: wrapperParameterTypes")]
    public void EmitMethodBody_ParameterTypesDoNotMatch ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object), typeof (string) };
      GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The number of elements in the wrapperParameterTypes array (3) does not match the number of parameters required for invoking the wrappedMethod (5).\r\n"
        + "Parameter name: wrapperParameterTypes")]
    public void EmitMethodBody_ParameterCountDoNotMatch ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithMultipleParameters", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object), typeof (object), typeof (object) };
      GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);
    }
  }
}