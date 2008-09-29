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
using System.Collections.Generic;
using System.Linq;
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
    private MixinTypeGenerator _mixinTypeGenerator;
    private MethodInfo _referenceMethodWrapper;
    private IEnumerable<MethodInfo> _methodsToBeWrapped;

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
      PrivateInvoke.InvokeNonPublicMethod (_simpleClassDefinition.Mixins, "Add", _simpleMixinDefinition);

      _mixinTypeGenerator = new MockRepository().PartialMock<MixinTypeGenerator> (_moduleMock, _typeGeneratorMock, _simpleMixinDefinition, GuidNameProvider.Instance);

      _referenceMethodWrapper = typeof (DateTime).GetMethod ("get_Now");
      _methodsToBeWrapped = (from m in _simpleMixinDefinition.Type.GetMethods (BindingFlags.Instance | BindingFlags.NonPublic)
                             where m.IsFamily
                             select m);
      foreach (var m in _methodsToBeWrapped)
        _classEmitterMock.Stub (mock => mock.GetPublicMethodWrapper (m)).Return (_referenceMethodWrapper);

    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_mixinTypeGenerator.Configuration, Is.SameAs (_simpleMixinDefinition));
      Assert.That (_mixinTypeGenerator.Emitter, Is.Not.Null);
      Assert.That (_mixinTypeGenerator.Emitter, Is.SameAs (_classEmitterMock));
    }

    [Test]
    public void Initialization_ForceUnsignedFalse ()
    {
      Assert.That (ReflectionUtility.IsAssemblySigned (_signedMixinDefinition.Type.Assembly), Is.True);

      IModuleManager moduleMock = MockRepository.GenerateMock<IModuleManager> ();
      moduleMock.Expect (
          mock =>
          mock.CreateClassEmitter (
              Arg<string>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything, Arg<TypeAttributes>.Is.Anything, Arg<bool>.Is.Equal (false)))
              .Return (_classEmitterMock);

      _typeGeneratorMock.Stub (mock => mock.IsAssemblySigned).Return (true);
      new MixinTypeGenerator (moduleMock, _typeGeneratorMock, _signedMixinDefinition, GuidNameProvider.Instance);

      moduleMock.VerifyAllExpectations ();
    }

    [Test]
    public void Initialization_ForceUnsignedTrue_BecauseOfTargetType ()
    {
      Assert.That (ReflectionUtility.IsAssemblySigned (_signedMixinDefinition.Type.Assembly), Is.True);

      IModuleManager moduleMock = MockRepository.GenerateMock<IModuleManager> ();
      moduleMock.Expect (
          mock =>
          mock.CreateClassEmitter (
              Arg<string>.Is.Anything, Arg<Type>.Is.Anything, Arg<Type[]>.Is.Anything, Arg<TypeAttributes>.Is.Anything, Arg<bool>.Is.Equal (true)))
              .Return (_classEmitterMock);

      _typeGeneratorMock.Stub (mock => mock.IsAssemblySigned).Return (false);
      new MixinTypeGenerator (moduleMock, _typeGeneratorMock, _signedMixinDefinition, GuidNameProvider.Instance);

      moduleMock.VerifyAllExpectations();
    }

    [Test]
    public void GetBuiltType_Type ()
    {
      DisableGenerate();

      _classEmitterMock.Stub (mock => mock.BuildType()).Return (typeof (string));
      ConcreteMixinType mixinType = _mixinTypeGenerator.GetBuiltType();
      Assert.That (mixinType.GeneratedType, Is.SameAs (typeof (string)));
    }

    [Test]
    public void GetBuiltType_ReturnsWrappersForAllProtectedMethods ()
    {
      DisableGenerate ();

      _classEmitterMock.Stub (mock => mock.BuildType ()).Return (typeof (string));

      var result = _mixinTypeGenerator.GetBuiltType ();
      foreach (var m in _methodsToBeWrapped)
        Assert.That (result.GetMethodWrapper (m), Is.SameAs (_referenceMethodWrapper));
    }

    private void DisableGenerate ()
    {
      _mixinTypeGenerator.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "Generate"));
    }
  }
}