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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class DomainObjectCollectionExtensionsTest : ClientTransactionBaseTest
  {
    private DomainObjectCollection _collection;
    private Customer _customer1;
    private Customer _customer2;

    public override void SetUp ()
    {
      base.SetUp ();

      _customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      _customer2 = Customer.GetObject (DomainObjectIDs.Customer2);

      _collection = new DomainObjectCollection (typeof (Customer)) { _customer1, _customer2 };
    }

    [Test]
    public void UnionWith ()
    {
      var secondCollection = new DomainObjectCollection (_collection, false);
      secondCollection.Add (Customer.GetObject (DomainObjectIDs.Customer3));

      _collection.UnionWith (secondCollection);

      Assert.That (_collection.Count, Is.EqualTo (3));
      Assert.That (_collection.ContainsObject (_customer1), Is.True);
      Assert.That (_collection.ContainsObject (_customer2), Is.True);
      Assert.That (_collection.Contains (DomainObjectIDs.Customer3), Is.True);
      Assert.That (_collection.IsReadOnly, Is.False);
    }

    [Test]
    public void UnionWith_WithIdenticalID_AndDifferentReference ()
    {
      var customer1FromOtherTransaction = new ClientTransactionMock ().GetObject (_customer1.ID);
      var secondCollection = new DomainObjectCollection ();
      secondCollection.Add (customer1FromOtherTransaction);

      secondCollection.UnionWith (_collection);

      Assert.That (secondCollection, Is.EqualTo (new[] { customer1FromOtherTransaction, _customer2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemTypeException))]
    public void UnionWith_ChecksItems ()
    {
      var secondCollection = new DomainObjectCollection ();
      secondCollection.Add (Order.GetObject (DomainObjectIDs.Order1));

      _collection.UnionWith (secondCollection);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "A read-only collection cannot be combined with another collection.")]
    public void UnionWith_ChecksNotReadOnly ()
    {
      var readOnlyCollection = new DomainObjectCollection (_collection, true);
      readOnlyCollection.UnionWith (_collection);

      Assert.Fail ();
    }

    [Test]
    public void CheckNotReadOnly_NotReadOnly ()
    {
      _collection.CheckNotReadOnly ("Test");
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Test")]
    public void CheckNotReadOnly_ReadOnly ()
    {
      var readOnlyCollection = new DomainObjectCollection (_collection, true);
      readOnlyCollection.CheckNotReadOnly ("Test");
    }
  }
}