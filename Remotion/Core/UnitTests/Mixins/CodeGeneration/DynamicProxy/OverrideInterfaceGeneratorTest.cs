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
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class OverrideInterfaceGeneratorTest
  {
    private IClassEmitter _classEmitterMock;

    [SetUp]
    public void SetUp ()
    {
      _classEmitterMock = MockRepository.GenerateMock<IClassEmitter> ();
    }

    [Test]
    public void CreateTopLevelGenerator ()
    {
      var moduleMock = MockRepository.GenerateMock<ICodeGenerationModule> ();
      moduleMock
          .Stub (mock => mock.CreateClassEmitter (
              "TestType",
              null,
              Type.EmptyTypes,
              TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract,
              false))
          .Return (_classEmitterMock);

      var generator = OverrideInterfaceGenerator.CreateTopLevelGenerator (moduleMock, "TestType");
      Assert.That (PrivateInvoke.GetNonPublicField (generator, "_emitter"), Is.SameAs (_classEmitterMock));

      moduleMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateNestedGenerator ()
    {
      var outerTypeMock = MockRepository.GenerateMock<IClassEmitter> ();
      outerTypeMock
          .Stub (mock => mock.CreateNestedClass (
              "TestType",
              null,
              Type.EmptyTypes,
              TypeAttributes.NestedPublic | TypeAttributes.Interface | TypeAttributes.Abstract))
          .Return (_classEmitterMock);

      var generator = OverrideInterfaceGenerator.CreateNestedGenerator (outerTypeMock, "TestType");
      Assert.That (PrivateInvoke.GetNonPublicField (generator, "_emitter"), Is.SameAs (_classEmitterMock));

      outerTypeMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetBuiltType ()
    {
      _classEmitterMock.Expect (mock => mock.BuildType ()).Return (typeof (string));

      var generator = new OverrideInterfaceGenerator (_classEmitterMock);
      var type = generator.GetBuiltType ();

      Assert.That (type, Is.SameAs (typeof (string)));
      _classEmitterMock.VerifyAllExpectations ();
    }

    [Test]
    public void AddOverriddenMethod ()
    {
      var overriddenMethod = typeof (MixinWithAbstractMembers).GetMethod ("AbstractMethod", BindingFlags.NonPublic | BindingFlags.Instance);
      var methodEmitterMock = MockRepository.GenerateMock<IMethodEmitter> ();
      var fakeMethodBuilder = (MethodBuilder) FormatterServices.GetSafeUninitializedObject (typeof (MethodBuilder));

      _classEmitterMock
          .Expect (mock => mock.CreateMethod ("AbstractMethod", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract))
          .Return (methodEmitterMock);
      methodEmitterMock.Expect (mock => mock.CopyParametersAndReturnType (overriddenMethod)).Return (methodEmitterMock);
      methodEmitterMock.Expect (mock => mock.MethodBuilder).Return (fakeMethodBuilder);

      var generator = new OverrideInterfaceGenerator (_classEmitterMock);
      var result = generator.AddOverriddenMethod (overriddenMethod);

      Assert.That (result, Is.SameAs (fakeMethodBuilder));
      _classEmitterMock.VerifyAllExpectations ();
    }
  }
}