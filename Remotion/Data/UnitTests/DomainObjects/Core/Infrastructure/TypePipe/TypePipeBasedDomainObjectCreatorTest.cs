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
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectLifetime;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.TypePipe
{
  [TestFixture]
  public class TypePipeBasedDomainObjectCreatorTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private TypePipeBasedDomainObjectCreator _interceptedDomainObjectCreator;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();
      _interceptedDomainObjectCreator = new TypePipeBasedDomainObjectCreator (SafeServiceLocator.Current.GetInstance<IObjectFactory>());
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

      Assert.That (((object) order).GetType().Name, Is.StringMatching (@"_Proxy\d"));
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
      var objectID = new ObjectID (typeof (TargetClassForPersistentMixin), Guid.NewGuid ());
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
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();

      var instance = _interceptedDomainObjectCreator.CreateObjectReference (DomainObjectIDs.Order1, bindingTransaction);

      Assert.That (instance.HasBindingTransaction, Is.True);
      Assert.That (instance.GetBindingTransaction (), Is.SameAs (bindingTransaction));
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
      var result =
          CallWithInitializationContext (
              () => _interceptedDomainObjectCreator.CreateNewObject (typeof (OrderItem), ParamList.Create ("A product"), _transaction),
              DomainObjectIDs.OrderItem1);

      Assert.That (((object) result).GetType().Name, Is.StringMatching (@"_Proxy\d"));
      Assert.That (result, Is.AssignableTo<OrderItem>());
      Assert.That (_transaction.IsDiscarded, Is.False);
      Assert.That (_transaction.IsEnlisted (result), Is.True);
      Assert.That (_transaction.Execute (() => ((OrderItem) result).Product), Is.EqualTo ("A product"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "mixin", MatchType = MessageMatch.Contains)]
    public void CreateNewObject_ValidatesMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        CallWithInitializationContext (
            () => _interceptedDomainObjectCreator.CreateNewObject (typeof (TargetClassForPersistentMixin), ParamList.Empty, _transaction),
            DomainObjectIDs.OrderItem1);
      }
    }

    [Test]
    public void CreateNewObject_InitializesMixins ()
    {
      var result = CallWithInitializationContext (
          () => _interceptedDomainObjectCreator.CreateNewObject (typeof (ClassWithAllDataTypes), ParamList.Empty, _transaction),
          DomainObjectIDs.OrderItem1);

      var mixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>> (result);
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin.OnDomainObjectCreatedCalled, Is.True);
      Assert.That (mixin.OnDomainObjectCreatedTx, Is.SameAs (_transaction));
    }

    [Test]
    public void CreateNewObject_AllowsPublicAndNonPublicCtors ()
    {
      Assert.That (
          CallWithInitializationContext (
              () => _interceptedDomainObjectCreator.CreateNewObject (typeof (DomainObjectWithPublicCtor), ParamList.Empty, _transaction),
              DomainObjectIDs.OrderItem1),
          Is.Not.Null);

      Assert.That (
          CallWithInitializationContext (
              () => _interceptedDomainObjectCreator.CreateNewObject (typeof (DomainObjectWithProtectedCtor), ParamList.Empty, _transaction),
              DomainObjectIDs.OrderItem2),
          Is.Not.Null);
    }

    private T CallWithInitializationContext<T> (Func<T> func, ObjectID objectID)
    {
      ObjectLifetimeAgentTestHelper.StubCurrentObjectInitializationContext (_transaction, objectID);
      try
      {
        return func ();
      }
      finally
      {
        ObjectLifetimeAgentTestHelper.SetCurrentInitializationContext (null);
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