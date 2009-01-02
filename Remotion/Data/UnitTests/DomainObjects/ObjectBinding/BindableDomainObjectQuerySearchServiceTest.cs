// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.ObjectBinding;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectQuerySearchServiceTest : ObjectBindingBaseTest
  {
    private BindableDomainObjectQuerySearchService _service;

    private IBusinessObject _orderItem;
    private IBusinessObjectReferenceProperty _property;

    private IDisposable _mixinConfiguration;

    public override void SetUp ()
    {
      base.SetUp ();
      _mixinConfiguration = MixinConfiguration.BuildFromActive ()
          .ForClass<Order> ().Clear ().AddMixin<BindableDomainObjectMixin> ()
          .ForClass<OrderItem> ().Clear ().AddMixin<BindableDomainObjectMixin> ()
          .EnterScope();

      _service = new BindableDomainObjectQuerySearchService ();

      _orderItem = (IBusinessObject) OrderItem.NewObject();
      _property = (IBusinessObjectReferenceProperty) _orderItem.BusinessObjectClass.GetPropertyDefinition ("Order");
    }

    public override void TearDown ()
    {
      _mixinConfiguration.Dispose ();
      base.TearDown ();
    }

    [Test]
    public void SearchViaReferencePropertyWithIdentity ()
    {
      Assert.IsTrue (_property.SupportsSearchAvailableObjects);
      IBusinessObject[] results = _service.Search (_orderItem, _property, new DefaultSearchArguments("QueryWithSpecificCollectionType"));
      Assert.That (results, Is.EqualTo (ClientTransactionMock.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("QueryWithSpecificCollectionType"))));
    }

    [Test]
    public void SearchViaReferencePropertyWithoutIdentity ()
    {
      Assert.IsTrue (_property.SupportsSearchAvailableObjects);
      IBusinessObject[] results = _service.Search (_orderItem, _property, new DefaultSearchArguments ("QueryWithSpecificCollectionType"));
      Assert.That (results, Is.EqualTo (ClientTransactionMock.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("QueryWithSpecificCollectionType"))));
    }

    [Test]
    public void SearchAvailableObjectsUsesCurrentTransaction_NullObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        IBusinessObject[] results = _service.Search (null, _property, new DefaultSearchArguments ("QueryWithSpecificCollectionType"));

        Assert.IsNotNull (results);
        Assert.IsTrue (results.Length > 0);

        var order = (Order) results[0];
        Assert.IsFalse (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
        Assert.IsTrue (order.TransactionContext[ClientTransaction.Current].CanBeUsedInTransaction);
      }
    }

    [Test]
    public void SearchAvailableObjectsUsesCurrentTransaction_NonDomainObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var nonDomainObject = new BindableNonDomainObjectReferencingDomainObject();
        var property = (IBusinessObjectReferenceProperty) nonDomainObject.BusinessObjectClass.GetPropertyDefinition ("Order");
        IBusinessObject[] results = _service.Search (nonDomainObject, property, new DefaultSearchArguments ("QueryWithSpecificCollectionType"));

        Assert.IsNotNull (results);
        Assert.IsTrue (results.Length > 0);

        var order = (Order) results[0];
        Assert.IsFalse (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
        Assert.IsTrue (order.TransactionContext[ClientTransaction.Current].CanBeUsedInTransaction);
      }
    }

    [Test]
    public void SearchAvailableObjectsUsesCurrentTransaction_UnboundObject ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        IBusinessObject[] results = _service.Search (_orderItem, _property, new DefaultSearchArguments ("QueryWithSpecificCollectionType"));

        Assert.IsNotNull (results);
        Assert.IsTrue (results.Length > 0);

        var order = (Order) results[0];
        Assert.IsFalse (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
        Assert.IsTrue (order.TransactionContext[ClientTransaction.Current].CanBeUsedInTransaction);
      }
    }

    [Test]
    public void SearchAvailableObjectsUsesBindingTransaction_BoundObject ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();

      IBusinessObject boundOrderItem;
      using (bindingTransaction.EnterNonDiscardingScope ())
      {
        boundOrderItem = (IBusinessObject) OrderItem.NewObject();
      }

      IBusinessObject[] results = _service.Search (boundOrderItem, _property, new DefaultSearchArguments ("QueryWithSpecificCollectionType"));

      Assert.IsNotNull (results);
      Assert.IsTrue (results.Length > 0);

      var order = (Order) results[0];
      Assert.That (order.TransactionContext[ClientTransaction.Current].CanBeUsedInTransaction, Is.False);
      Assert.That (order.TransactionContext[bindingTransaction].CanBeUsedInTransaction, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread or "
         + "the referencing object.")]
    public void SearchAvailableObjects_NoTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope ())
      {
        _service.Search (null, _property, new DefaultSearchArguments ("QueryWithSpecificCollectionType"));
      }
    }

    [Test]
    public void SearchAvailableObjectsWithDifferentObject ()
    {
      IBusinessObject[] businessObjects = _service.Search (_orderItem, _property, new DefaultSearchArguments("QueryWithSpecificCollectionType"));

      Assert.IsNotNull (businessObjects);
      Assert.IsTrue (businessObjects.Length > 0);
    }

    [Test]
    public void SearchAvailableObjectsWithNullSearchArguments ()
    {
      IBusinessObject[] businessObjects = _service.Search (_orderItem, _property, null);

      Assert.IsNotNull (businessObjects);
      Assert.That (businessObjects, Is.Empty);
    }

    [Test]
    public void SearchAvailableObjectsWithNullQuery ()
    {
      IBusinessObject[] businessObjects = _service.Search (_orderItem, _property, new DefaultSearchArguments (null));

      Assert.IsNotNull (businessObjects);
      Assert.That (businessObjects, Is.Empty);
    }

    [Test]
    public void SearchAvailableObjectsWithEmptyQuery ()
    {
      IBusinessObject[] businessObjects = _service.Search (_orderItem, _property, new DefaultSearchArguments (""));

      Assert.IsNotNull (businessObjects);
      Assert.That (businessObjects, Is.Empty);
    }
  }
}