// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class MixedDomainTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetSetCommitRollbackPersistentProperties ()
    {
      using (TestableClientTransaction.CreateSubTransaction ().EnterNonDiscardingScope ())
      {
        IMixinAddingPersistentProperties properties = TargetClassForPersistentMixin.NewObject () as IMixinAddingPersistentProperties;
        Assert.IsNotNull (properties);

        properties.ExtraPersistentProperty = 10;
        properties.PersistentProperty = 11;
        properties.NonPersistentProperty = 12;

        Assert.AreEqual (10, properties.ExtraPersistentProperty);
        Assert.AreEqual (11, properties.PersistentProperty);
        Assert.AreEqual (12, properties.NonPersistentProperty);

        ClientTransaction.Current.Commit ();

        Assert.AreEqual (10, properties.ExtraPersistentProperty);
        Assert.AreEqual (11, properties.PersistentProperty);
        Assert.AreEqual (12, properties.NonPersistentProperty);

        properties.ExtraPersistentProperty = 13;
        properties.PersistentProperty = 14;
        properties.NonPersistentProperty = 15;

        Assert.AreEqual (13, properties.ExtraPersistentProperty);
        Assert.AreEqual (14, properties.PersistentProperty);
        Assert.AreEqual (15, properties.NonPersistentProperty);

        ClientTransaction.Current.Rollback ();

        Assert.AreEqual (10, properties.ExtraPersistentProperty);
        Assert.AreEqual (11, properties.PersistentProperty);
        Assert.AreEqual (15, properties.NonPersistentProperty);
      }
    }

    [Test]
    public void LoadStoreMixedDomainObject ()
    {
      SetDatabaseModifyable();

      TargetClassForPersistentMixin mixedInstance;
      MixinAddingPersistentProperties mixin;
      RelationTargetForPersistentMixin relationTarget1;
      RelationTargetForPersistentMixin relationTarget2;
      RelationTargetForPersistentMixin relationTarget3;
      RelationTargetForPersistentMixin relationTarget4;
      RelationTargetForPersistentMixin relationTarget5;

      using (ClientTransaction.CreateBindingTransaction ().EnterNonDiscardingScope())
      {
        mixedInstance = TargetClassForPersistentMixin.NewObject();
        mixin = Mixin.Get<MixinAddingPersistentProperties> (mixedInstance);
        relationTarget1 = RelationTargetForPersistentMixin.NewObject();
        relationTarget2 = RelationTargetForPersistentMixin.NewObject();
        relationTarget3 = RelationTargetForPersistentMixin.NewObject();
        relationTarget4 = RelationTargetForPersistentMixin.NewObject ();
        relationTarget5 = RelationTargetForPersistentMixin.NewObject ();
      }

      mixin.PersistentProperty = 10;
      mixin.NonPersistentProperty = 100;
      mixin.ExtraPersistentProperty = 1000;
      mixin.RelationProperty = relationTarget1;
      mixin.VirtualRelationProperty = relationTarget2;
      mixin.CollectionProperty1Side.Add (relationTarget3);
      mixin.CollectionPropertyNSide = relationTarget4;
      mixin.UnidirectionalRelationProperty = relationTarget5;

      mixedInstance.GetBindingTransaction().Commit();

      TargetClassForPersistentMixin loadedInstance;
      MixinAddingPersistentProperties loadedMixin;
      using (ClientTransaction.CreateBindingTransaction ().EnterNonDiscardingScope ())
      {
        loadedInstance = TargetClassForPersistentMixin.GetObject (mixedInstance.ID);
        loadedMixin = Mixin.Get<MixinAddingPersistentProperties> (loadedInstance);
      }

      Assert.AreNotSame (mixedInstance, loadedInstance);
      Assert.AreNotSame (mixin, loadedMixin);

      Assert.AreEqual (10, loadedMixin.PersistentProperty);
      Assert.AreEqual (0, loadedMixin.NonPersistentProperty);
      Assert.AreEqual (1000, loadedMixin.ExtraPersistentProperty);
      Assert.AreEqual (mixin.RelationProperty.ID, loadedMixin.RelationProperty.ID);
      Assert.AreEqual (mixin.VirtualRelationProperty.ID, loadedMixin.VirtualRelationProperty.ID);
      Assert.AreEqual (mixin.CollectionProperty1Side.Count, loadedMixin.CollectionProperty1Side.Count);
      Assert.AreEqual (mixin.CollectionProperty1Side[0].ID, loadedMixin.CollectionProperty1Side[0].ID);
      Assert.AreEqual (mixin.CollectionPropertyNSide.ID, loadedMixin.CollectionPropertyNSide.ID);
      Assert.AreEqual (mixin.UnidirectionalRelationProperty.ID, loadedMixin.UnidirectionalRelationProperty.ID);
    }

    [Test]
    public void OneDomainClassTwoMixins ()
    {
      SetDatabaseModifyable ();

      TargetClassWithTwoUnidirectionalMixins mixedInstance;
      MixinAddingUnidirectionalRelation1 mixin1;
      MixinAddingUnidirectionalRelation2 mixin2;
      Computer relationTarget1;
      Computer relationTarget2;

      using (ClientTransaction.CreateBindingTransaction ().EnterNonDiscardingScope ())
      {
        mixedInstance = TargetClassWithTwoUnidirectionalMixins.NewObject ();
        mixin1 = Mixin.Get<MixinAddingUnidirectionalRelation1> (mixedInstance);
        mixin2 = Mixin.Get<MixinAddingUnidirectionalRelation2> (mixedInstance);

        relationTarget1 = Computer.NewObject ();
        relationTarget2 = Computer.NewObject ();
      }

      mixin1.Computer = relationTarget1;
      mixin2.Computer = relationTarget2;
      mixedInstance.GetBindingTransaction().Commit ();

      TargetClassWithTwoUnidirectionalMixins loadedInstance;
      MixinAddingUnidirectionalRelation1 loadedMixin1;
      MixinAddingUnidirectionalRelation2 loadedMixin2;

      using (ClientTransaction.CreateBindingTransaction ().EnterNonDiscardingScope ())
      {
        loadedInstance = TargetClassWithTwoUnidirectionalMixins.GetObject (mixedInstance.ID);
        loadedMixin1 = Mixin.Get<MixinAddingUnidirectionalRelation1> (loadedInstance);
        loadedMixin2 = Mixin.Get<MixinAddingUnidirectionalRelation2> (loadedInstance);
      }

      Assert.AreEqual (mixin1.Computer.ID, loadedMixin1.Computer.ID);
      Assert.AreEqual (mixin2.Computer.ID, loadedMixin2.Computer.ID);
    }

    [Test]
    public void OneMixinTwoDomainClasses ()
    {
      SetDatabaseModifyable ();

      TargetClassWithUnidirectionalMixin1 mixedInstance1;
      TargetClassWithUnidirectionalMixin2 mixedInstance2;
      MixinAddingUnidirectionalRelation1 mixin1;
      MixinAddingUnidirectionalRelation1 mixin2;
      Computer relationTarget1;
      Computer relationTarget2;

      using (ClientTransaction.CreateBindingTransaction ().EnterNonDiscardingScope ())
      {
        mixedInstance1 = TargetClassWithUnidirectionalMixin1.NewObject ();
        mixin1 = Mixin.Get<MixinAddingUnidirectionalRelation1> (mixedInstance1);
        mixedInstance2 = TargetClassWithUnidirectionalMixin2.NewObject ();
        mixin2 = Mixin.Get<MixinAddingUnidirectionalRelation1> (mixedInstance2);

        relationTarget1 = Computer.NewObject ();
        relationTarget2 = Computer.NewObject ();
      }

      mixin1.Computer = relationTarget1;
      mixin2.Computer = relationTarget2;
      mixedInstance1.GetBindingTransaction().Commit();

      TargetClassWithUnidirectionalMixin1 loadedInstance1;
      TargetClassWithUnidirectionalMixin2 loadedInstance2;
      MixinAddingUnidirectionalRelation1 loadedMixin1;
      MixinAddingUnidirectionalRelation1 loadedMixin2;

      using (ClientTransaction.CreateBindingTransaction ().EnterNonDiscardingScope ())
      {
        loadedInstance1 = TargetClassWithUnidirectionalMixin1.GetObject (mixedInstance1.ID);
        loadedMixin1 = Mixin.Get<MixinAddingUnidirectionalRelation1> (loadedInstance1);
        loadedInstance2 = TargetClassWithUnidirectionalMixin2.GetObject (mixedInstance2.ID);
        loadedMixin2 = Mixin.Get<MixinAddingUnidirectionalRelation1> (loadedInstance2);
      }

      Assert.AreEqual (mixin1.Computer.ID, loadedMixin1.Computer.ID);
      Assert.AreEqual (mixin2.Computer.ID, loadedMixin2.Computer.ID);
    }
  }
}
