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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class SearchTest : ObjectBindingBaseTest
  {
    private IBusinessObject _orderItem;
    private IBusinessObjectReferenceProperty _property;

    private IDisposable _mixinConfiguration;

    public override void SetUp ()
    {
      base.SetUp ();
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (typeof (ISearchAvailableObjectsService), new BindableDomainObjectSearchService ());

      _mixinConfiguration = MixinConfiguration.BuildFromActive ()
          .ForClass<Order> ().Clear ().AddMixin<BindableDomainObjectMixin> ()
          .ForClass<OrderItem> ().Clear ().AddMixin<BindableDomainObjectMixin> ()
          .EnterScope();

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
      IBusinessObjectWithIdentity[] results = (IBusinessObjectWithIdentity[]) _property.SearchAvailableObjects (_orderItem, new DefaultSearchArguments("QueryWithSpecificCollectionType"));
      Assert.That (results, Is.EqualTo (ClientTransactionMock.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("QueryWithSpecificCollectionType"))));
    }

    [Test]
    public void SearchViaReferencePropertyWithoutIdentity ()
    {
      Assert.IsTrue (_property.SupportsSearchAvailableObjects);
      IBusinessObject[] results = _property.SearchAvailableObjects (_orderItem, new DefaultSearchArguments ("QueryWithSpecificCollectionType"));
      Assert.That (results, Is.EqualTo (ClientTransactionMock.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("QueryWithSpecificCollectionType"))));
    }

    [Test]
    public void SearchAvailableObjectsUsesCurrentTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        IBusinessObject[] results = _property.SearchAvailableObjects (_orderItem, new DefaultSearchArguments ("QueryWithSpecificCollectionType"));

        Assert.IsNotNull (results);
        Assert.IsTrue (results.Length > 0);

        Order order = (Order) results[0];
        Assert.IsFalse (order.TransactionContext[ClientTransactionMock].CanBeUsedInTransaction);
        Assert.IsTrue (order.CanBeUsedInTransaction);
      }
    }

    [Test]
    public void SearchAvailableObjectsWithDifferentObject ()
    {
      IBusinessObject[] businessObjects =
          _property.SearchAvailableObjects ((IBusinessObject) Order.NewObject(), new DefaultSearchArguments ("QueryWithSpecificCollectionType"));

      Assert.IsNotNull (businessObjects);
      Assert.IsTrue (businessObjects.Length > 0);
    }

    [Test]
    public void SearchAvailableObjectsWithNullSearchArguments ()
    {
      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_orderItem, null);

      Assert.IsNotNull (businessObjects);
      Assert.That (businessObjects, List.Contains (Order.GetObject (DomainObjectIDs.Order1)));
      Assert.That (businessObjects, List.Contains (Order.GetObject (DomainObjectIDs.Order2)));
    }

    [Test]
    public void SearchAvailableObjectsWithNullQuery ()
    {
      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_orderItem, new DefaultSearchArguments (null));

      Assert.IsNotNull (businessObjects);
      Assert.That (businessObjects, List.Contains (Order.GetObject (DomainObjectIDs.Order1)));
      Assert.That (businessObjects, List.Contains (Order.GetObject (DomainObjectIDs.Order2)));
    }

    [Test]
    public void SearchAvailableObjectsWithEmptyQuery ()
    {
      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_orderItem, new DefaultSearchArguments (""));

      Assert.IsNotNull (businessObjects);
      Assert.IsNotNull (businessObjects);
      Assert.That (businessObjects, List.Contains (Order.GetObject (DomainObjectIDs.Order1)));
      Assert.That (businessObjects, List.Contains (Order.GetObject (DomainObjectIDs.Order2)));
    }
  }
}
