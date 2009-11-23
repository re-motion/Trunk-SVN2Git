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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class DomainObjectCollectionsWithDifferentClientTransactionsTest : ClientTransactionBaseTest
  {
    private DomainObjectCollection _collection;
    private Customer _customer1;
    private Customer _customer2;

    private ClientTransaction _secondClientTransaction;
    private DomainObjectCollection _secondCollection;
    private Customer _secondCustomer1;

    public override void SetUp ()
    {
      base.SetUp ();

      _customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      _customer2 = Customer.GetObject (DomainObjectIDs.Customer2);

      _collection = CreateCustomerCollection ();

      _secondClientTransaction = ClientTransaction.CreateRootTransaction();
      _secondCollection = new DomainObjectCollection ();
      using (_secondClientTransaction.EnterDiscardingScope ())
      {
        _secondCustomer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The collection already contains an object with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid'.")]
    public void ReplaceObjectWithDifferentClientTransaction ()
    {
      _secondCollection.Add (_secondCustomer1);

      _secondCollection[0] = _customer1;
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The collection already contains an object with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid'.\r\n"
        + "Parameter name: domainObject")]
    public void AddSameObjectWithDifferentClientTransaction ()
    {
      _secondCollection.Add (_secondCustomer1);

      _secondCollection.Add (_customer1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The collection already contains an object with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid'.\r\n"
        + "Parameter name: domainObject")]
    public void InsertSameObjectWithDifferentClientTransaction ()
    {
      _secondCollection.Add (_secondCustomer1);

      _secondCollection.Insert (0, _customer1);
    }

    [Test]
    public void CombineWithIdenticalIDAndDifferentDomainObjects ()
    {
      _secondCollection.Add (_secondCustomer1);

      _secondCollection.Combine (_collection);

      Assert.AreEqual (2, _secondCollection.Count);
      Assert.AreSame (_secondCustomer1, _secondCollection[_secondCustomer1.ID]);
      Assert.AreSame (_customer2, _secondCollection[_customer2.ID]);
    }

    private DomainObjectCollection CreateCustomerCollection ()
    {
      var collection = new DomainObjectCollection (typeof (Customer));
      collection.Add (_customer1);
      collection.Add (_customer2);

      return collection;
    }
  }
}
