/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class MixinTypeGeneratorTest
  {
    private IModuleManager _moduleMock;
    private IClassEmitter _classEmitterMock;
    private ITypeGenerator _typeGeneratorMock;

    private TargetClassDefinition _simpleClassDefinition;
    private MixinDefinition _simpleMixinDefinition;
    private MixinDefinition _signedMixinDefinition;
    private MixinDefinition _unsignedMixinDefinition;

    [SetUp]
    public void SetUp ()
    {
      _moduleMock = MockRepository.GenerateMock<IModuleManager> ();
      _classEmitterMock = MockRepository.GenerateMock<IClassEmitter> ();
      _moduleMock.Stub (
          mock =>
          mock.CreateClassEmitter (
              Arg<string>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything, Arg<TypeAttributes>.Is.Anything, Arg<bool>.Is.Anything)).Return (_classEmitterMock);
      _typeGeneratorMock = MockRepository.GenerateMock<ITypeGenerator> ();

      _simpleClassDefinition = new TargetClassDefinition (new ClassContext (typeof (BaseType1), typeof (BT1Mixin1)));
      _simpleMixinDefinition = new MixinDefinition (MixinKind.Extending, typeof (BT1Mixin1), _simpleClassDefinition, false);
      _signedMixinDefinition = new MixinDefinition (MixinKind.Extending, typeof (object), _simpleClassDefinition, false);
      _unsignedMixinDefinition = _simpleMixinDefinition;
      PrivateInvoke.InvokeNonPublicMethod (_simpleClassDefinition.Mixins, "Add", _simpleMixinDefinition);
    }

    [Test]
    public void Initialization ()
    {
      MixinTypeGenerator generator = new MixinTypeGenerator (_moduleMock, _typeGeneratorMock, _simpleMixinDefinition, GuidNameProvider.Instance);
      Assert.That (generator.Configuration, Is.SameAs (_simpleMixinDefinition));
      Assert.That (generator.Emitter, Is.Not.Null);
      Assert.That (generator.Emitter, Is.SameAs (_classEmitterMock));
    }

    [Test]
    public void Initialization_ForceUnsignedFalse ()
    {
      Assert.That (ReflectionUtility.IsAssemblySigned (_signedMixinDefinition.Type.Assembly), Is.True);

      _typeGeneratorMock.Stub (mock => mock.IsAssemblySigned).Return (true);
      new MixinTypeGenerator (_moduleMock, _typeGeneratorMock, _signedMixinDefinition, GuidNameProvider.Instance);

      _moduleMock.AssertWasCalled (
          mock =>
          mock.CreateClassEmitter (
              Arg<string>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything, Arg<TypeAttributes>.Is.Anything, Arg<bool>.Is.Equal (false)));
    }

    [Test]
    public void Initialization_ForceUnsignedTrue_BecauseOfTargetType ()
    {
      Assert.That (ReflectionUtility.IsAssemblySigned (_signedMixinDefinition.Type.Assembly), Is.True);
      
      _typeGeneratorMock.Stub (mock => mock.IsAssemblySigned).Return (false);
      new MixinTypeGenerator (_moduleMock, _typeGeneratorMock, _signedMixinDefinition, GuidNameProvider.Instance);

      _moduleMock.AssertWasCalled (
          mock =>
          mock.CreateClassEmitter (
              Arg<string>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything, Arg<TypeAttributes>.Is.Anything, Arg<bool>.Is.Equal (true)));
    }

  }
}