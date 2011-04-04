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
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.UnitTests.Mixins.TestDomain;
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

      Assert.AreSame (thisMock, mixin.Target);
      Assert.AreSame (baseMock, mixin.Next);
    }

    [Test]
    public void Mock_ThisConfig ()
    {
      var repository = new MockRepository ();

      var thisMock = repository.StrictMock<IBaseType32> ();
      var mixin = new BT3Mixin2 ();

      MixinTargetMockUtility.MockMixinTarget (mixin, thisMock);
      Assert.AreSame (thisMock, mixin.Target);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin has not been initialized yet.")]
    public void UninitializedMixin_This ()
    {
      var mixin = new BT3Mixin1 ();
      Dev.Null = mixin.Target;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Mixin has not been initialized yet.")]
    public void UninitializedMixin_Base ()
    {
      var mixin = new BT3Mixin1 ();
      Dev.Null = mixin.Next;
    }

    [Test]
    [ExpectedException (typeof (MemberAccessException), ExpectedMessage = "Cannot create an instance of "
        + "Remotion.UnitTests.Mixins.TestDomain.MixinWithAbstractMembers because it is an abstract class.")]
    public void CreateMixinWithMockedTarget_AbstractMixin ()
    {
      var thisMock = new ClassOverridingMixinMembers();
      var baseMock = new object();

      MixinTargetMockUtility.CreateMixinWithMockedTarget<MixinWithAbstractMembers, object, object> (thisMock, baseMock);
    }

    [Test]
    public void CreateMixinWithMockedTarget_ThisBase ()
    {
      var repository = new MockRepository ();

      var thisMock = repository.StrictMock<IBaseType31> ();
      var baseMock = repository.StrictMock<IBaseType31> ();

      BT3Mixin1 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin1, IBaseType31, IBaseType31> (thisMock, baseMock);
      Assert.AreSame (thisMock, mixin.Target);
      Assert.AreSame (baseMock, mixin.Next);
    }

    [Test]
    public void CreateMixinWithMockedTarget_This ()
    {
      var repository = new MockRepository ();

      var thisMock = repository.StrictMock<IBaseType32> ();

      BT3Mixin2 mixin =
          MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin2, IBaseType32> (thisMock);
      Assert.AreSame (thisMock, mixin.Target);
    }

    [Test]
    public void CreateMixinWithMockedTarget_NonPublicCtor ()
    {
      var repository = new MockRepository ();

      var thisMock = repository.StrictMock<IBaseType32> ();

      BT3Mixin2 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin2, IBaseType32> (thisMock, 7);
      Assert.AreSame (thisMock, mixin.Target);
      Assert.That (mixin.I, Is.EqualTo (7));
    }

    [Test]
    public void SignalOnDeserialized_This ()
    {
      var thisMock = new SerializableBaseType32Mock ();

      BT3Mixin2 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin2, IBaseType32> (thisMock);
      var deserializedData = Serializer.SerializeAndDeserialize (Tuple.Create (thisMock, mixin));

      MixinTargetMockUtility.MockMixinTargetAfterDeserialization (deserializedData.Item2, deserializedData.Item1);
      Assert.That (deserializedData.Item2.Target, Is.Not.Null);
      Assert.That (deserializedData.Item2.Target, Is.SameAs (deserializedData.Item1));
    }

    [Test]
    public void SignalOnDeserialized_ThisBase ()
    {
      var thisMock = new SerializableBaseType31Mock ();
      var baseMock = new SerializableBaseType31Mock ();

      BT3Mixin1 mixin = MixinTargetMockUtility.CreateMixinWithMockedTarget<BT3Mixin1, IBaseType31, IBaseType31> (thisMock, baseMock);
      var deserializedData = Serializer.SerializeAndDeserialize (Tuple.Create (thisMock, baseMock, mixin));

      MixinTargetMockUtility.MockMixinTargetAfterDeserialization (deserializedData.Item3, deserializedData.Item1, deserializedData.Item2);
      Assert.That (deserializedData.Item3.Target, Is.Not.Null);
      Assert.That (deserializedData.Item3.Target, Is.SameAs (deserializedData.Item1));
      Assert.That (deserializedData.Item3.Next, Is.Not.Null);
      Assert.That (deserializedData.Item3.Next, Is.SameAs (deserializedData.Item2));
    }
  }
}
