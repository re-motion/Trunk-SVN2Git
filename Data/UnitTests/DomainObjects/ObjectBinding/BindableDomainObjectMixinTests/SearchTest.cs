/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionMock));
        Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
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
      Assert.AreEqual (0, businessObjects.Length);
    }

    [Test]
    public void SearchAvailableObjectsWithNullQuery ()
    {
      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_orderItem, new DefaultSearchArguments (null));

      Assert.IsNotNull (businessObjects);
      Assert.AreEqual (0, businessObjects.Length);
    }

    [Test]
    public void SearchAvailableObjectsWithEmptyQuery ()
    {
      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_orderItem, new DefaultSearchArguments (""));

      Assert.IsNotNull (businessObjects);
      Assert.AreEqual (0, businessObjects.Length);
    }
  }
}
