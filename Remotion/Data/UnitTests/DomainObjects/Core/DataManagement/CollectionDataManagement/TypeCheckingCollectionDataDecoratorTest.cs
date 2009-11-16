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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class TypeCheckingCollectionDataDecoratorTest : ClientTransactionBaseTest
  {
    private IDomainObjectCollectionData _wrappedDataMock;
    private TypeCheckingCollectionDataDecorator _typeCheckingDecorator;
    private TypeCheckingCollectionDataDecorator _typeCheckingDecoratorWithoutType;

    private Order _order1;
    private OrderItem _orderItem1;

    public override void SetUp ()
    {
      base.SetUp ();

      _wrappedDataMock = MockRepository.GenerateMock<IDomainObjectCollectionData> ();
      _typeCheckingDecorator = new TypeCheckingCollectionDataDecorator (_wrappedDataMock, typeof (Order));
      _typeCheckingDecoratorWithoutType = new TypeCheckingCollectionDataDecorator (_wrappedDataMock, null);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);

      _orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
    }

    [Test]
    public void GetUndecoratedDataStore ()
    {
      var dataStoreStub = new DomainObjectCollectionData ();
      _wrappedDataMock.Stub (mock => mock.GetUndecoratedDataStore ()).Return (dataStoreStub);
      
      Assert.That (_typeCheckingDecorator.GetUndecoratedDataStore (), Is.SameAs (dataStoreStub));
    }

    [Test]
    public void TrivialMembers_Delegated ()
    {
      CheckDelegation (data => data.GetEnumerator ());
      CheckDelegation (data => Dev.Null = data.Count);
      CheckDelegation (data => Dev.Null = data.IsReadOnly);
      CheckDelegation (data => Dev.Null = data.AssociatedEndPoint);
      CheckDelegation (data => data.ContainsObjectID (DomainObjectIDs.Order1));
      CheckDelegation (data => data.GetObject (0));
      CheckDelegation (data => data.GetObject (DomainObjectIDs.Order1));
      CheckDelegation (data => data.IndexOf (DomainObjectIDs.Order1));
      CheckDelegation (data => data.Clear());
      CheckDelegation (data => data.Remove (DomainObjectIDs.Order1));
      CheckDelegation (data => data.Remove (_order1));
    }

    [Test]
    public void Insert ()
    {
      _typeCheckingDecorator.Insert (0, _order1);

      _wrappedDataMock.AssertWasCalled (mock => mock.Insert (0, _order1));
    }

    [Test]
    public void Insert_WrongType ()
    {
      CheckThrows<ArgumentTypeException> (
          () => _typeCheckingDecorator.Insert (0, _orderItem1), 
          "Values of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem'" 
          + " cannot be added to this collection. Values must be of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' or derived from "
          + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order'.\r\nParameter name: domainObject");

      _wrappedDataMock.AssertWasNotCalled (mock => mock.Insert (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Insert_RequiredItemTypeNull ()
    {
      _typeCheckingDecoratorWithoutType.Insert (0, _order1);
      _typeCheckingDecoratorWithoutType.Insert (0, _orderItem1);

      _wrappedDataMock.AssertWasCalled (mock => mock.Insert (0, _order1));
      _wrappedDataMock.AssertWasCalled (mock => mock.Insert (0, _orderItem1));
    }

    [Test]
    public void Replace ()
    {
      _typeCheckingDecorator.Replace (0, _order1);

      _wrappedDataMock.AssertWasCalled (mock => mock.Replace (0, _order1));
    }

    [Test]
    public void Replace_WrongType ()
    {
      CheckThrows<ArgumentTypeException> (
          () => _typeCheckingDecorator.Replace (0, _orderItem1),
          "Values of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem'"
          + " cannot be added to this collection. Values must be of type 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order' or derived from "
          + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order'.\r\nParameter name: newDomainObject");

      _wrappedDataMock.AssertWasNotCalled (mock => mock.Replace (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Replace_RequiredItemTypeNull ()
    {
      _typeCheckingDecoratorWithoutType.Replace (0, _order1);
      _typeCheckingDecoratorWithoutType.Replace (0, _orderItem1);

      _wrappedDataMock.AssertWasCalled (mock => mock.Replace (0, _order1));
      _wrappedDataMock.AssertWasCalled (mock => mock.Replace (0, _orderItem1));
    }

    [Test]
    public void Serializable ()
    {
      var decorator = new TypeCheckingCollectionDataDecorator (new DomainObjectCollectionData(new[] { _order1 }), typeof (Order));
      var deserializedDecorator = Serializer.SerializeAndDeserialize (decorator);

      Assert.That (deserializedDecorator.Count(), Is.EqualTo (1));
    }

    private void CheckDelegation (Action<IDomainObjectCollectionData> action)
    {
      action (_typeCheckingDecorator);
      _wrappedDataMock.AssertWasCalled (action);
    }

    private void CheckThrows<T> (Action action, string expectedMessage) where T : Exception
    {
      try
      {
        action ();
      }
      catch (T ex)
      {
        Assert.That (ex.Message, Is.EqualTo (expectedMessage), "Exception message doesn't match.");
        return;
      }
      catch (Exception ex)
      {
        Assert.Fail ("Expected " + typeof (T) + ", got " + ex);
      }
      Assert.Fail ("Expected " + typeof (T));
    }
  }
}
