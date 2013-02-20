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
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class InterceptedDomainObjectCreatorTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private InterceptedDomainObjectCreator _interceptedDomainObjectCreator;

    private IObjectInitializationContext _order1InitializationContext;
    private IObjectInitializationContext _targetClassForPersistentMixinInitializationContext;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();
      _interceptedDomainObjectCreator = InterceptedDomainObjectCreator.Instance;

      _order1InitializationContext = CreateFakeInitializationContext (DomainObjectIDs.Order1, _transaction, null);

      var objectID = new ObjectID (typeof (TargetClassForPersistentMixin), Guid.NewGuid ());
      _targetClassForPersistentMixinInitializationContext = CreateFakeInitializationContext (objectID, _transaction, null);
    }

    [Test]
    public void CreateObjectReference ()
    {
      var order = _interceptedDomainObjectCreator.CreateObjectReference (_order1InitializationContext, _transaction);

      Assert.That (order, Is.InstanceOf (typeof (Order)));
      Assert.That (order.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (order.RootTransaction, Is.EqualTo (_transaction));
    }

    [Test]
    public void CreateObjectReference_UsesFactoryGeneratedType ()
    {
      var order = _interceptedDomainObjectCreator.CreateObjectReference (_order1InitializationContext, _transaction);

      var factory = _interceptedDomainObjectCreator.Factory;
      Assert.That (factory.WasCreatedByFactory ((((object) order).GetType())), Is.True);
    }

    [Test]
    public void CreateObjectReference_CallsNoCtor ()
    {
      var order = (Order) _interceptedDomainObjectCreator.CreateObjectReference (_order1InitializationContext, _transaction);
      Assert.That (order.CtorCalled, Is.False);
    }

    [Test]
    public void CreateObjectReference_PreparesMixins ()
    {
      var instance = _interceptedDomainObjectCreator.CreateObjectReference (_targetClassForPersistentMixinInitializationContext, _transaction);
      Assert.That (Mixin.Get<MixinAddingPersistentProperties> (instance), Is.Not.Null);
    }

    [Test]
    public void CreateObjectReference_InitializesObjectID ()
    {
      var instance = _interceptedDomainObjectCreator.CreateObjectReference (_order1InitializationContext, _transaction);
      Assert.That (instance.ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void CreateObjectReference_EnlistsObjectInTransaction ()
    {
      DomainObject registeredObject = null;

      var initializationContextMock = MockRepository.GenerateStrictMock<IObjectInitializationContext>();
      initializationContextMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      initializationContextMock.Stub (stub => stub.RootTransaction).Return (_transaction);
      initializationContextMock.Stub (stub => stub.BindingTransaction).Return (null);
      initializationContextMock
          .Expect (mock => mock.RegisterObject (Arg<DomainObject>.Matches (obj => obj.ID == DomainObjectIDs.Order1)))
          .WhenCalled (mi => registeredObject = (DomainObject) mi.Arguments[0]);

      var instance = _interceptedDomainObjectCreator.CreateObjectReference (initializationContextMock, _transaction);

      initializationContextMock.VerifyAllExpectations();
      Assert.That (instance, Is.SameAs (registeredObject));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "mixin", MatchType = MessageMatch.Contains)]
    public void CreateObjectReference_ValidatesMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        _interceptedDomainObjectCreator.CreateObjectReference (_targetClassForPersistentMixinInitializationContext, _transaction);
      }
    }

    [Test]
    public void CreateObjectReference_CallsReferenceInitializing ()
    {
      var domainObject = (Order) _interceptedDomainObjectCreator.CreateObjectReference (_order1InitializationContext, _transaction);
      Assert.That (domainObject.OnReferenceInitializingCalled, Is.True);
    }

    [Test]
    public void CreateObjectReference_CallsReferenceInitializing_InRightTransaction ()
    {
      var domainObject = (Order) _interceptedDomainObjectCreator.CreateObjectReference (_order1InitializationContext, _transaction);
      Assert.That (domainObject.OnReferenceInitializingTx, Is.SameAs (_transaction));
    }

    [Test]
    public void CreateNewObject ()
    {
      var initializationContext = CreateNewObjectInitializationContext (_transaction, DomainObjectIDs.OrderItem1);
      var result = _interceptedDomainObjectCreator.CreateNewObject (initializationContext, ParamList.Create ("A product"), _transaction);

      Assert.That (_interceptedDomainObjectCreator.Factory.WasCreatedByFactory (((object) result).GetType()), Is.True);
      Assert.That (result, Is.AssignableTo<OrderItem> ());
      Assert.That (result.ID, Is.EqualTo (DomainObjectIDs.OrderItem1));
      Assert.That (result.RootTransaction, Is.SameAs (_transaction));
      Assert.That (_transaction.IsDiscarded, Is.False);
      Assert.That (_transaction.IsEnlisted (result), Is.True);
      Assert.That (_transaction.Execute (() => ((OrderItem) result).Product), Is.EqualTo ("A product"));
      Assert.That (ObjectInititalizationContextScope.CurrentObjectInitializationContext, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "mixin", MatchType = MessageMatch.Contains)]
    public void CreateNewObject_ValidatesMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        _interceptedDomainObjectCreator.CreateNewObject (_targetClassForPersistentMixinInitializationContext, ParamList.Empty, _transaction);
      }
    }

    [Test]
    public void CreateNewObject_InitializesMixins ()
    {
      var initializationContext = CreateNewObjectInitializationContext (_transaction, DomainObjectIDs.ClassWithAllDataTypes1);

      var result = _interceptedDomainObjectCreator.CreateNewObject (initializationContext, ParamList.Empty, _transaction);

      var mixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>> (result);
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin.OnDomainObjectCreatedCalled, Is.True);
      Assert.That (mixin.OnDomainObjectCreatedTx, Is.SameAs (_transaction));
    }

    [Test]
    public void CreateNewObject_AllowsPublicAndNonPublicCtors ()
    {
      Assert.That (
          () =>
          _interceptedDomainObjectCreator.CreateNewObject (
              CreateNewObjectInitializationContext (_transaction, DomainObjectIDs.OrderItem1), ParamList.Empty, _transaction),
          Is.Not.Null);

      Assert.That (
          () =>
          _interceptedDomainObjectCreator.CreateNewObject (
              CreateNewObjectInitializationContext (_transaction, DomainObjectIDs.OrderItem2), ParamList.Empty, _transaction),
          Is.Not.Null);
    }

    private IObjectInitializationContext CreateFakeInitializationContext (ObjectID objectID, ClientTransaction rootTransaction, ClientTransaction bindingTransaction)
    {
      var initializationContextStub = MockRepository.GenerateStub<IObjectInitializationContext> ();

      initializationContextStub.Stub (stub => stub.ObjectID).Return (objectID);
      initializationContextStub.Stub (stub => stub.RootTransaction).Return (rootTransaction);
      initializationContextStub.Stub (stub => stub.BindingTransaction).Return (bindingTransaction);
      return initializationContextStub;
    }

    private NewObjectInitializationContext CreateNewObjectInitializationContext (ClientTransaction rootTransaction, ObjectID objectID)
    {
      return new NewObjectInitializationContext (
          objectID,
          rootTransaction,
          ClientTransactionTestHelper.GetEnlistedDomainObjectManager (rootTransaction),
          ClientTransactionTestHelper.GetIDataManager (rootTransaction));
    }

    [DBTable]
    public class DomainObjectWithPublicCtor2 : DomainObject
    {
      public DomainObjectWithPublicCtor2 ()
      {
      }
    }

    [DBTable]
    public class DomainObjectWithProtectedCtor2 : DomainObject
    {
      protected DomainObjectWithProtectedCtor2 ()
      {
      }
    }
  }
}