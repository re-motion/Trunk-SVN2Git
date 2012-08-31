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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class InterceptedDomainObjectCreatorTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private InterceptedDomainObjectCreator _interceptedDomainObjectCreator;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();
      _interceptedDomainObjectCreator = InterceptedDomainObjectCreator.Instance;
    }

    [Test]
    public void CreateObjectReference ()
    {
      var order = _interceptedDomainObjectCreator.CreateObjectReference (DomainObjectIDs.Order1, _transaction);

      Assert.That (order, Is.InstanceOf (typeof (Order)));
      Assert.That (order.ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void CreateObjectReference_UsesFactoryGeneratedType ()
    {
      var order = _interceptedDomainObjectCreator.CreateObjectReference (DomainObjectIDs.Order1, _transaction);

      var factory = _interceptedDomainObjectCreator.Factory;
      Assert.That (factory.WasCreatedByFactory ((((object) order).GetType())), Is.True);
    }

    [Test]
    public void CreateObjectReference_CallsNoCtor ()
    {
      var order = (Order) _interceptedDomainObjectCreator.CreateObjectReference (DomainObjectIDs.Order1, _transaction);
      Assert.That (order.CtorCalled, Is.False);
    }

    [Test]
    public void CreateObjectReference_PreparesMixins ()
    {
      var objectID = new ObjectID (typeof (TargetClassForPersistentMixin), Guid.NewGuid());
      var instance = _interceptedDomainObjectCreator.CreateObjectReference (objectID, _transaction);
      Assert.That (Mixin.Get<MixinAddingPersistentProperties> (instance), Is.Not.Null);
    }

    [Test]
    public void CreateObjectReference_InitializesObjectID ()
    {
      var instance = _interceptedDomainObjectCreator.CreateObjectReference (DomainObjectIDs.Order1, _transaction);
      Assert.That (instance.ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void CreateObjectReference_EnlistsObjectInTransaction ()
    {
      var instance = _interceptedDomainObjectCreator.CreateObjectReference (DomainObjectIDs.Order1, _transaction);
      Assert.That (_transaction.GetEnlistedDomainObject (instance.ID), Is.SameAs (instance));
    }

    [Test]
    public void CreateObjectReference_BindingTransaction ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction();

      var instance = _interceptedDomainObjectCreator.CreateObjectReference (DomainObjectIDs.Order1, bindingTransaction);

      Assert.That (instance.HasBindingTransaction, Is.True);
      Assert.That (instance.GetBindingTransaction(), Is.SameAs (bindingTransaction));
    }

    [Test]
    public void CreateObjectReference_NoBindingTransaction ()
    {
      var instance = _interceptedDomainObjectCreator.CreateObjectReference (DomainObjectIDs.Order1, _transaction);
      Assert.That (instance.HasBindingTransaction, Is.False);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "mixin", MatchType = MessageMatch.Contains)]
    public void CreateObjectReference_ValidatesMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        var objectID = new ObjectID (typeof (TargetClassForPersistentMixin), Guid.NewGuid());
        _interceptedDomainObjectCreator.CreateObjectReference (objectID, _transaction);
      }
    }

    [Test]
    public void CreateObjectReference_CallsReferenceInitializing ()
    {
      var domainObject = (Order) _interceptedDomainObjectCreator.CreateObjectReference (DomainObjectIDs.Order1, _transaction);
      Assert.That (domainObject.OnReferenceInitializingCalled, Is.True);
    }

    [Test]
    public void CreateObjectReference_CallsReferenceInitializing_InRightTransaction ()
    {
      var domainObject = (Order) _interceptedDomainObjectCreator.CreateObjectReference (DomainObjectIDs.Order1, _transaction);
      Assert.That (domainObject.OnReferenceInitializingTx, Is.SameAs (_transaction));
    }

    [Test]
    public void CreateNewObject ()
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        var result = _interceptedDomainObjectCreator.CreateNewObject (typeof (OrderItem), ParamList.Create ("A product"));

        Assert.That (_interceptedDomainObjectCreator.Factory.WasCreatedByFactory (((object) result).GetType ()), Is.True);
        Assert.That (result, Is.AssignableTo<OrderItem> ());
        Assert.That (((OrderItem) result).Product, Is.EqualTo ("A product"));
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "mixin", MatchType = MessageMatch.Contains)]
    public void CreateNewObject_ValidatesMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        _interceptedDomainObjectCreator.CreateNewObject (typeof (TargetClassForPersistentMixin), ParamList.Empty);
      }
    }

    [Test]
    public void CreateNewObject_InitializesMixins ()
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        var result = _interceptedDomainObjectCreator.CreateNewObject (typeof (ClassWithAllDataTypes), ParamList.Empty);
      
        var mixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>> (result);
        Assert.That (mixin, Is.Not.Null);
        Assert.That (mixin.OnDomainObjectCreatedCalled, Is.True);
        Assert.That (mixin.OnDomainObjectCreatedTx, Is.SameAs (_transaction));
      }
    }

    [Test]
    public void CreateNewObject_AllowsPublicAndNonPublicCtors ()
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        Assert.That (_interceptedDomainObjectCreator.CreateNewObject (typeof (DomainObjectWithPublicCtor), ParamList.Empty), Is.Not.Null);
        Assert.That (_interceptedDomainObjectCreator.CreateNewObject (typeof (DomainObjectWithProtectedCtor), ParamList.Empty), Is.Not.Null);
      }
    }

    [DBTable]
    public class DomainObjectWithPublicCtor : DomainObject
    {
      public DomainObjectWithPublicCtor ()
      {
      }
    }

    [DBTable]
    public class DomainObjectWithProtectedCtor : DomainObject
    {
      protected DomainObjectWithProtectedCtor ()
      {
      }
    }
  }
}