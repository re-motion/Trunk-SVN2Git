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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionWrapperTest : ClientTransactionBaseTest
  {
    private ITransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = TestableClientTransaction.ToITransaction();
    }

    [Test]
    public void To_ClientTransaction ()
    {
      var actual = _transaction.To<TestableClientTransaction>();

      Assert.That (actual, Is.SameAs (TestableClientTransaction));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "Argument TTransaction is a Remotion.Data.DomainObjects.DomainObject, "
        + "which cannot be assigned to type Remotion.Data.DomainObjects.ClientTransaction.\r\nParameter name: TTransaction")]
    public void To_InvalidType ()
    {
      _transaction.To<DomainObject>();
    }

    [Test]
    public void CanCreateChild ()
    {
      Assert.That (_transaction.CanCreateChild, Is.True);
    }

    [Test]
    public void CreateChild ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.That (child, Is.Not.Null);
      Assert.IsInstanceOf (typeof (ClientTransactionWrapper), child);
      Assert.IsInstanceOf (typeof (ClientTransaction), ((ClientTransactionWrapper) child).WrappedInstance);

      var persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy (((ClientTransactionWrapper) child).WrappedInstance);
      Assert.IsInstanceOf (typeof (SubPersistenceStrategy), persistenceStrategy);
    }

    [Test]
    public void IsChild ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.That (child.IsChild, Is.True);
      Assert.That (_transaction.IsChild, Is.False);
      Assert.That (child.CreateChild().IsChild, Is.True);
    }

    [Test]
    public void Parent ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.That (((ClientTransactionWrapper) child.Parent).WrappedInstance, Is.SameAs (((ClientTransactionWrapper) _transaction).WrappedInstance));
      Assert.That (((ClientTransactionWrapper) child.CreateChild().Parent).WrappedInstance, Is.SameAs (((ClientTransactionWrapper) child).WrappedInstance));
    }

    [Test]
    public void Release ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.That (((ClientTransactionWrapper) _transaction).WrappedInstance.IsActive, Is.False);
      Assert.That (((ClientTransactionWrapper) child).WrappedInstance.IsDiscarded, Is.False);
      child.Release();
      Assert.That (((ClientTransactionWrapper) _transaction).WrappedInstance.IsActive, Is.True);
      Assert.That (((ClientTransactionWrapper) child).WrappedInstance.IsDiscarded, Is.True);
    }

    [Test]
    public void EnterScope ()
    {
      ITransaction transaction = ClientTransaction.CreateRootTransaction().ToITransaction();

      ClientTransactionScope.ResetActiveScope();
      Assert.That (ClientTransactionScope.ActiveScope, Is.Null);

      ITransactionScope transactionScope = transaction.EnterScope();

      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (transactionScope));
      Assert.That (ClientTransactionScope.ActiveScope.ScopedTransaction, Is.SameAs (((ClientTransactionWrapper) transaction).WrappedInstance));
      Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.None));
      ClientTransactionScope.ResetActiveScope();
    }

    [Test]
    public void RegisterObjects ()
    {
      ClientTransaction firstClientTransaction = ClientTransaction.CreateRootTransaction();

      var domainObject1 = LifetimeService.GetObject (firstClientTransaction, DomainObjectIDs.ClassWithAllDataTypes2, false);
      var domainObject2 = LifetimeService.GetObject (firstClientTransaction, DomainObjectIDs.Partner1, false);

      var secondClientTransaction = TestableClientTransaction;
      ITransaction secondTransaction = secondClientTransaction.ToITransaction();
      Assert.That (secondClientTransaction.IsEnlisted (domainObject1), Is.False);
      Assert.That (secondClientTransaction.IsEnlisted (domainObject2), Is.False);

      secondTransaction.RegisterObjects (new object[] { null, domainObject1, 1, domainObject2, domainObject2 });

      Assert.That (secondClientTransaction.IsEnlisted (domainObject1), Is.True);
      Assert.That (secondClientTransaction.Execute (() => domainObject1.State), Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (secondClientTransaction.IsEnlisted (domainObject2), Is.True);
      Assert.That (secondClientTransaction.Execute (() => domainObject2.State), Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void RegisterObjects_WithNewObject ()
    {
      ClientTransaction firstClientTransaction = ClientTransaction.CreateRootTransaction();

      var domainObject = LifetimeService.NewObject (firstClientTransaction, typeof (Partner), ParamList.Empty);

      var secondClientTransaction = TestableClientTransaction;
      ITransaction secondTransaction = secondClientTransaction.ToITransaction();

      secondTransaction.RegisterObjects (new object[] { domainObject });

      Assert.That (secondClientTransaction.Execute (() => domainObject.State), Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (() => secondClientTransaction.Execute (domainObject.EnsureDataAvailable), Throws.TypeOf<ObjectsNotFoundException>());
    }

    [Test]
    public void CanBeDerivedFrom ()
    {
      var ctor =  typeof (ClientTransactionWrapper).GetConstructor (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, 
          new[] {typeof (ClientTransaction)}, null);
      Assert.That (typeof (ClientTransactionWrapper).IsSealed, Is.False);
      Assert.That (ctor.IsFamilyOrAssembly);
    }
  }
}
