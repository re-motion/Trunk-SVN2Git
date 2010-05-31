﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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

namespace Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests
{
  [TestFixture]
  public class NotSupportedScenarios : TestBase
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'value' of the wrappedMethod is an out parameter, but out parameters are not supported by the MethodWrapperGenerator.\r\n"
        + "Parameter name: wrappedMethod")]
    public void EmitMethodBody_OutParameter ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithOutParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (object).MakeByRefType() };
      GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);
      BuildType ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'value' of the wrappedMethod is a by-ref parameter, but by-ref parameters are not supported by the MethodWrapperGenerator.\r\n"
        + "Parameter name: wrappedMethod")]
    public void EmitMethodBody_ByRefParameter ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithByRefParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (object).MakeByRefType() };
      GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);
      BuildType ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'value' of the wrappedMethod is an optional parameter, but optional parameters are not supported by the MethodWrapperGenerator.\r\n"
        + "Parameter name: wrappedMethod")]
    public void EmitMethodBody_OptionalParameter ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithOptionalParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (void);
      Type[] parameterTypes = new[] { typeof (object), typeof (object) };
      GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod (), parameterTypes, returnType, methodInfo);
      BuildType ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Open generic method definitions are not supported by the MethodWrapperGenerator.\r\n"
        + "Parameter name: wrappedMethod")]
    public void EmitMethodBody_OpenGeneric ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("GenericInstanceMethod", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object), typeof (object) };
      GetWrapperMethodFromEmitter (MethodInfo.GetCurrentMethod(), parameterTypes, returnType, methodInfo);
      BuildType ();
    }
  }
}