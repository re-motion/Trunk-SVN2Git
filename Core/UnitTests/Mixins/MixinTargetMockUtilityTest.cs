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
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class MixinTargetMockUtilityTest
  {
    [SetUp]
    public void SetUp ()
    {
      ConcreteTypeBuilder.SetCurrent (null);
      // ensure compatibility with Rhino.Mocks
      ((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope = new ModuleScope (false);
    }

    [TearDown]
    public void TearDown ()
    {
      ConcreteTypeBuilder.SetCurrent (null);
    }

    [Test]
    public void Mock_ThisBaseConfig ()
    {
      MockRepository repository = new MockRepository();

      IBaseType31 thisMock = repository.CreateMock<IBaseType31>();
      IBaseType31 baseMock = repository.CreateMock<IBaseType31>();

      BT3Mixin1 mixin = new BT3Mixin1();

      MixinTargetMockUtility.MockMixinTarget (mixin, thisMock, baseMock);

      Assert.AreSame (thisMock, mixin.This);
      Assert.AreSame (baseMock, mixin.Base);
    }

    [Test]
    public void Mock_ThisConfig ()
    {
      MockRepository repository = new MockRepository ();

      IBaseType32 thisMock = repository.CreateMock<IBaseType32> ();
      BT3Mixin2 mixin = new BT3Mixin2 ();

      MixinTargetMockUtility.MockMixinTarget (mixin, thisMock);
      Assert.AreSame (thisMock, mixin.This);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin has not been initialized yet.")]
    public void UninitializedMixin_This ()
    {
      BT3Mixin1 mixin = new BT3Mixin1 ();
      Dev.Null = mixin.This;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin has not been initialized yet.")]
    public void UninitializedMixin_Base ()
    {
      BT3Mixin1 mixin = new BT3Mixin1 ();
      Dev.Null = mixin.Base;
    }

    [Test]
    public void CreateMixinWithMockedTarget_ThisBase_WithGeneratedMixin ()
    {
      MockRepository repository = new MockRepository ();

      ClassOverridingMixinMembers thisMock = repository.CreateMock<ClassOverridingMixinMembers>();
      object baseMock = new object();

      Expect.Call (thisMock.AbstractMethod (25)).Return ("Mocked!");

      repository.ReplayAll();

      MixinWithAbstractMembers mixin =
          MixinTargetMockUtility.CreateMixinWithMockedTarget<MixinWithAbstractMembers, object, object> (thisMock, baseMock);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedMethod-Mocked!", mixin.ImplementedMethod ());

      repository.VerifyAll ();
    }

    [Test]
    public void CreateMixinWithMockedTarget_This_WithGeneratedMixin ()
    {
      MockRepository repository = new MockRepository ();

      ClassOverridingSpecificMixinMember thisMock = repository.CreateMock<ClassOverridingSpecificMixinMember> ();

      Expect.Call (thisMock.VirtualMethod ()).Return ("Mocked, bastard!");

      repository.ReplayAll ();

      MixinWithVirtualMethod mixin =
          MixinTargetMockUtility.CreateMixinWithMockedTarget<MixinWithVirtualMethod, object> (thisMock);
      Assert.AreEqual ("Mocked, bastard!", mixin.VirtualMethod ());

      repository.VerifyAll ();
    }

    [Test]
    public void CreateMixinWithMockedTarget_ThisBase_WithNonGeneratedMixin ()
    {
      MockRepository repository = new MockRepository ();

      IBaseType31 thisMock = repository.CreateMock<IBaseType31> ();
      IBaseType31 baseMock = repository.CreateMock<IBaseType31> ();

      BT3Mixin1 mixin =
          MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin1, IBaseType31, IBaseType31> (thisMock, baseMock);
      Assert.AreSame (thisMock, mixin.This);
      Assert.AreSame (baseMock, mixin.Base);
    }

    [Test]
    public void CreateMixinWithMockedTarget_This_WithNonGeneratedMixin ()
    {
      MockRepository repository = new MockRepository ();

      IBaseType32 thisMock = repository.CreateMock<IBaseType32> ();

      BT3Mixin2 mixin =
          MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin2, IBaseType32> (thisMock);
      Assert.AreSame (thisMock, mixin.This);
    }
  }
}
