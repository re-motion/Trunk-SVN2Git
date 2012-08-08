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
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions
{
  public class InactiveTransactionsTestBase : StandardMappingTest
  {
    private ClientTransaction _inactiveRootTransaction;
    private ClientTransaction _inactiveMiddleTransaction;
    private ClientTransaction _activeSubTransaction;

    protected ClientTransaction InactiveRootTransaction
    {
      get { return _inactiveRootTransaction; }
    }

    protected ClientTransaction InactiveMiddleTransaction
    {
      get { return _inactiveMiddleTransaction; }
    }

    protected ClientTransaction ActiveSubTransaction
    {
      get { return _activeSubTransaction; }
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _inactiveRootTransaction = ClientTransaction.CreateRootTransaction ();
      _inactiveRootTransaction.Execute (InitializeInactiveRootTransaction);
      _inactiveMiddleTransaction = InactiveRootTransaction.CreateSubTransaction ();
      _inactiveMiddleTransaction.Execute (InitializeInactiveMiddleTransaction);
      _activeSubTransaction = InactiveMiddleTransaction.CreateSubTransaction ();
      _activeSubTransaction.Execute (InitializeActiveSubTransaction);
    }

    protected virtual void InitializeInactiveRootTransaction ()
    {
    }

    protected virtual void InitializeInactiveMiddleTransaction ()
    {
    }

    protected virtual void InitializeActiveSubTransaction ()
    {
    }

    protected void CheckDataLoaded (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      Assert.That (ClientTransactionTestHelper.GetIDataManager (clientTransaction).DataContainers[domainObject.ID], Is.Not.Null);
      CheckState (clientTransaction, domainObject, StateType.Unchanged);
    }

    protected void CheckDataNotLoaded (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      CheckDataNotLoaded(clientTransaction, domainObject.ID);
      CheckState(clientTransaction, domainObject, StateType.NotLoadedYet);
    }

    protected void CheckState (ClientTransaction clientTransaction, DomainObject domainObject, StateType expectedState)
    {
      Assert.That (domainObject.TransactionContext[clientTransaction].State, Is.EqualTo (expectedState));
    }

    protected void CheckDataNotLoaded (ClientTransaction clientTransaction, ObjectID objectID)
    {
      Assert.That (ClientTransactionTestHelper.GetIDataManager (clientTransaction).DataContainers[objectID], Is.Null);
    }

    protected void CheckEndPointNull (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      Assert.That (ClientTransactionTestHelper.GetIDataManager (clientTransaction).RelationEndPoints[relationEndPointID], Is.Null);
    }

    protected void CheckEndPointComplete (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      var relationEndPoint = ClientTransactionTestHelper.GetIDataManager (clientTransaction).RelationEndPoints[relationEndPointID];
      Assert.That (relationEndPoint, Is.Not.Null);
      Assert.That (relationEndPoint.IsDataComplete, Is.True);
    }

    protected void CheckForbidden (Action func, string operation)
    {
      var expectedMessage = string.Format (
          "The operation cannot be executed because the ClientTransaction is read-only. Offending transaction modification: {0}.", operation);
      Assert.That (() => func (), Throws.TypeOf<ClientTransactionReadOnlyException> ().With.Message.EqualTo (expectedMessage));
    }

    protected void CheckProperty<TDomainObject, TValue> (
        ClientTransaction clientTransaction,
        TDomainObject domainObject,
        Expression<Func<TDomainObject, TValue>> propertyExpression,
        TValue expectedCurrentValue,
        TValue expectedOriginalValue)
        where TDomainObject : DomainObject
    {
      var isReadOnlyTransaction = clientTransaction.IsReadOnly;
      if (isReadOnlyTransaction)
        ClientTransactionTestHelper.SetIsReadOnly (clientTransaction, false);
      
      try
      {
        var propertyIndexer = new PropertyIndexer (domainObject);
        var propertyAccessorData = domainObject.ID.ClassDefinition.PropertyAccessorDataCache.ResolveMandatoryPropertyAccessorData (propertyExpression);
        Assert.That (
            propertyIndexer[propertyAccessorData.PropertyIdentifier, clientTransaction].GetValueWithoutTypeCheck(), Is.EqualTo (expectedCurrentValue));
        Assert.That (
            propertyIndexer[propertyAccessorData.PropertyIdentifier, clientTransaction].GetOriginalValueWithoutTypeCheck(),
            Is.EqualTo (expectedOriginalValue));
      }
      finally
      {
        if (isReadOnlyTransaction)
          ClientTransactionTestHelper.SetIsReadOnly (clientTransaction, true);
      }
    }

    protected void CheckPropertyEquivalent<TDomainObject, TValue> (
        ClientTransaction clientTransaction,
        TDomainObject domainObject,
        Expression<Func<TDomainObject, IEnumerable<TValue>>> propertyExpression,
        IEnumerable<TValue> expectedCurrentValue,
        IEnumerable<TValue> expectedOriginalValue)
        where TDomainObject : DomainObject
    {
      var isReadOnlyTransaction = clientTransaction.IsReadOnly;
      if (isReadOnlyTransaction)
        ClientTransactionTestHelper.SetIsReadOnly (clientTransaction, false);

      try
      {
        var propertyIndexer = new PropertyIndexer (domainObject);
        var propertyAccessorData = domainObject.ID.ClassDefinition.PropertyAccessorDataCache.ResolveMandatoryPropertyAccessorData (propertyExpression);
        Assert.That (
            propertyIndexer[propertyAccessorData.PropertyIdentifier, clientTransaction].GetValueWithoutTypeCheck (), Is.EquivalentTo (expectedCurrentValue));
        Assert.That (
            propertyIndexer[propertyAccessorData.PropertyIdentifier, clientTransaction].GetOriginalValueWithoutTypeCheck (),
            Is.EquivalentTo (expectedOriginalValue));
      }
      finally
      {
        if (isReadOnlyTransaction)
          ClientTransactionTestHelper.SetIsReadOnly (clientTransaction, true);
      }
    }
  }
}