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
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
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
      var repository = new MockRepository();

      var thisMock = repository.StrictMock<IBaseType31>();
      var baseMock = repository.StrictMock<IBaseType31>();

      var mixin = new BT3Mixin1();

      MixinTargetMockUtility.MockMixinTarget (mixin, thisMock, baseMock);

      Assert.AreSame (thisMock, mixin.This);
      Assert.AreSame (baseMock, mixin.Base);
    }

    [Test]
    public void Mock_ThisConfig ()
    {
      var repository = new MockRepository ();

      var thisMock = repository.StrictMock<IBaseType32> ();
      var mixin = new BT3Mixin2 ();

      MixinTargetMockUtility.MockMixinTarget (mixin, thisMock);
      Assert.AreSame (thisMock, mixin.This);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin has not been initialized yet.")]
    public void UninitializedMixin_This ()
    {
      var mixin = new BT3Mixin1 ();
      Dev.Null = mixin.This;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin has not been initialized yet.")]
    public void UninitializedMixin_Base ()
    {
      var mixin = new BT3Mixin1 ();
      Dev.Null = mixin.Base;
    }

    [Test]
    [ExpectedException (typeof (MemberAccessException), ExpectedMessage = "Cannot create an instance of "
        + "Remotion.UnitTests.Mixins.SampleTypes.MixinWithAbstractMembers because it is an abstract class.")]
    public void CreateMixinWithMockedTarget_AbstractMixin ()
    {
      var thisMock = new ClassOverridingMixinMembers();
      var baseMock = new object();

      MixinTargetMockUtility.CreateMixinWithMockedTarget<MixinWithAbstractMembers, object, object> (thisMock, baseMock);
    }

    [Test]
    public void CreateMixinWithMockedTarget_ThisBase_WithNonGeneratedMixin ()
    {
      var repository = new MockRepository ();

      var thisMock = repository.StrictMock<IBaseType31> ();
      var baseMock = repository.StrictMock<IBaseType31> ();

      BT3Mixin1 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin1, IBaseType31, IBaseType31> (thisMock, baseMock);
      Assert.AreSame (thisMock, mixin.This);
      Assert.AreSame (baseMock, mixin.Base);
    }

    [Test]
    public void CreateMixinWithMockedTarget_This_WithNonGeneratedMixin ()
    {
      var repository = new MockRepository ();

      var thisMock = repository.StrictMock<IBaseType32> ();

      BT3Mixin2 mixin =
          MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin2, IBaseType32> (thisMock);
      Assert.AreSame (thisMock, mixin.This);
    }

    [Test]
    public void CreateMixinWithMockedTarget_NonPublicCtor ()
    {
      var repository = new MockRepository ();

      var thisMock = repository.StrictMock<IBaseType32> ();

      BT3Mixin2 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin2, IBaseType32> (thisMock, 7);
      Assert.AreSame (thisMock, mixin.This);
      Assert.That (mixin.I, Is.EqualTo (7));
    }

    [Test]
    public void SignalOnDeserialized_This ()
    {
      var thisMock = new SerializableBaseType32Mock ();

      BT3Mixin2 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin2, IBaseType32> (thisMock);
      var deserializedData = Serializer.SerializeAndDeserialize (Tuple.NewTuple (thisMock, mixin));

      MixinTargetMockUtility.MockMixinTargetAfterDeserialization (deserializedData.B, deserializedData.A);
      Assert.That (deserializedData.B.This, Is.Not.Null);
      Assert.That (deserializedData.B.This, Is.SameAs (deserializedData.A));
    }

    [Test]
    public void SignalOnDeserialized_ThisBase ()
    {
      var thisMock = new SerializableBaseType31Mock ();
      var baseMock = new SerializableBaseType31Mock ();

      BT3Mixin1 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin1, IBaseType31, IBaseType31> (thisMock, baseMock);
      var deserializedData = Serializer.SerializeAndDeserialize (Tuple.NewTuple (thisMock, baseMock, mixin));

      MixinTargetMockUtility.MockMixinTargetAfterDeserialization (deserializedData.C, deserializedData.A, deserializedData.B);
      Assert.That (deserializedData.C.This, Is.Not.Null);
      Assert.That (deserializedData.C.This, Is.SameAs (deserializedData.A));
      Assert.That (deserializedData.C.Base, Is.Not.Null);
      Assert.That (deserializedData.C.Base, Is.SameAs (deserializedData.B));
    }
  }
}
