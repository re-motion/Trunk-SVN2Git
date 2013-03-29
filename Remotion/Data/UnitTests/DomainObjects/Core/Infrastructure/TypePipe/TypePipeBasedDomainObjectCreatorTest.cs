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
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.TypePipe
{
  [TestFixture]
  public class TypePipeBasedDomainObjectCreatorTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private TypePipeBasedDomainObjectCreator _interceptedDomainObjectCreator;

    private IObjectInitializationContext _order1InitializationContext;
    private IObjectInitializationContext _targetClassForPersistentMixinInitializationContext;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();
      // TODO 5500: Refactor to be unit test.
      var domainObjectParticipant = new DomainObjectParticipant(new TypeDefinitionProvider(), new InterceptedPropertyCollectorAdapter());
      var objectFactory = Pipeline.Create ("TypePipeBasedDomainObjectCreatorTest", domainObjectParticipant);
      _interceptedDomainObjectCreator = new TypePipeBasedDomainObjectCreator (objectFactory);

      _order1InitializationContext = CreateFakeInitializationContext (DomainObjectIDs.Order1, _transaction);

      var objectID = new ObjectID (typeof (TargetClassForPersistentMixin), Guid.NewGuid ());
      _targetClassForPersistentMixinInitializationContext = CreateFakeInitializationContext (objectID, _transaction);
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

      Assert.That (((object) order).GetType().Name, Is.StringMatching (@"_Proxy\d"));
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

      var initializationContextMock = MockRepository.GenerateStrictMock<IObjectInitializationContext> ();
      initializationContextMock.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      initializationContextMock.Stub (stub => stub.RootTransaction).Return (_transaction);
      initializationContextMock
          .Expect (mock => mock.RegisterObject (Arg<DomainObject>.Matches (obj => obj.ID == DomainObjectIDs.Order1)))
          .WhenCalled (mi => registeredObject = (DomainObject) mi.Arguments[0]);

      var instance = _interceptedDomainObjectCreator.CreateObjectReference (initializationContextMock, _transaction);

      initializationContextMock.VerifyAllExpectations ();
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
    public void CreateObjectReference_CallsReferenceInitializing_InRightTransaction_WithActivatedInactiveTransaction ()
    {
      ClientTransactionTestHelper.MakeInactive (_transaction);

      var domainObject = (Order) _interceptedDomainObjectCreator.CreateObjectReference (_order1InitializationContext, _transaction);
      Assert.That (domainObject.OnReferenceInitializingTx, Is.SameAs (_transaction));
      Assert.That (domainObject.OnReferenceInitializingActiveTx, Is.SameAs (_transaction));
    }

    [Test]
    public void CreateNewObject ()
    {
      var initializationContext = CreateNewObjectInitializationContext (DomainObjectIDs.OrderItem1, _transaction);
      var result = _interceptedDomainObjectCreator.CreateNewObject (initializationContext, ParamList.Create ("A product"), _transaction);

      Assert.That (((object) result).GetType().Name, Is.StringMatching (@"_Proxy\d"));
      Assert.That (result, Is.AssignableTo<OrderItem>());
      Assert.That (result.ID, Is.EqualTo (DomainObjectIDs.OrderItem1));
      Assert.That (result.RootTransaction, Is.SameAs (_transaction));
      Assert.That (_transaction.IsDiscarded, Is.False);
      Assert.That (_transaction.IsEnlisted (result), Is.True);
      Assert.That (_transaction.ExecuteInScope (() => ((OrderItem) result).Product), Is.EqualTo ("A product"));
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
      var initializationContext = CreateNewObjectInitializationContext (DomainObjectIDs.ClassWithAllDataTypes1, _transaction);

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
              CreateNewObjectInitializationContext (DomainObjectIDs.OrderItem1, _transaction), ParamList.Empty, _transaction),
          Is.Not.Null);

      Assert.That (
          () =>
          _interceptedDomainObjectCreator.CreateNewObject (
              CreateNewObjectInitializationContext (DomainObjectIDs.OrderItem2, _transaction), ParamList.Empty, _transaction),
          Is.Not.Null);
    }

    private IObjectInitializationContext CreateFakeInitializationContext (ObjectID objectID, ClientTransaction rootTransaction)
    {
      var initializationContextStub = MockRepository.GenerateStub<IObjectInitializationContext> ();

      initializationContextStub.Stub (stub => stub.ObjectID).Return (objectID);
      initializationContextStub.Stub (stub => stub.RootTransaction).Return (rootTransaction);
      return initializationContextStub;
    }

    private NewObjectInitializationContext CreateNewObjectInitializationContext (ObjectID objectID, ClientTransaction rootTransaction)
    {
      return new NewObjectInitializationContext (
          objectID,
          rootTransaction,
          ClientTransactionTestHelper.GetEnlistedDomainObjectManager (rootTransaction),
          ClientTransactionTestHelper.GetIDataManager (rootTransaction));
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