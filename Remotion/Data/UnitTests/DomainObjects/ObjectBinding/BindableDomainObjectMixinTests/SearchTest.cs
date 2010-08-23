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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class SearchTest : ObjectBindingBaseTest
  {
    private SearchServiceTestHelper _searchServiceTestHelper;
    private string _stubbedQueryID;

    private IBusinessObject _referencingBusinessObject;
    private IBusinessObjectReferenceProperty _property;

    private ClientTransactionScope _transactionScope;

    public override void SetUp ()
    {
      base.SetUp();

      _searchServiceTestHelper = new SearchServiceTestHelper();
      _stubbedQueryID = "FakeQuery";

      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (
          typeof (ISearchAvailableObjectsService), new BindableDomainObjectCompoundSearchService());

      var transaction = _searchServiceTestHelper.CreateStubbableTransaction<ClientTransaction>();
      _transactionScope = transaction.EnterDiscardingScope();

      var fakeResultDataContainer = _searchServiceTestHelper.CreateFakeResultDataContainer();
      _searchServiceTestHelper.StubQueryResult (_stubbedQueryID, new[] { fakeResultDataContainer });

      var referencingObject = SampleBindableMixinDomainObject.NewObject();
      _referencingBusinessObject = (IBusinessObject) referencingObject;
      _property = (IBusinessObjectReferenceProperty) _referencingBusinessObject.BusinessObjectClass.GetPropertyDefinition ("Relation");
    }

    public override void TearDown ()
    {
      _transactionScope.Leave();
      base.TearDown();
    }

    [Test]
    public void SearchViaReferencePropertyWithIdentity ()
    {
      Assert.IsTrue (_property.SupportsSearchAvailableObjects);
      var results =
          (IBusinessObjectWithIdentity[]) _property.SearchAvailableObjects (_referencingBusinessObject, new DefaultSearchArguments (_stubbedQueryID));
      Assert.That (
          results,
          Is.EqualTo (ClientTransaction.Current.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration (_stubbedQueryID)).ToArray()));
    }

    [Test]
    public void SearchViaReferencePropertyWithoutIdentity ()
    {
      Assert.IsTrue (_property.SupportsSearchAvailableObjects);
      IBusinessObject[] results = _property.SearchAvailableObjects (_referencingBusinessObject, new DefaultSearchArguments (_stubbedQueryID));
      Assert.That (
          results,
          Is.EqualTo (ClientTransaction.Current.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration (_stubbedQueryID)).ToArray()));
    }

    [Test]
    public void SearchAvailableObjectsUsesCurrentTransaction ()
    {
      var outerTransaction = ClientTransaction.Current;

      var transaction = _searchServiceTestHelper.CreateTransactionWithStubbedQuery<ClientTransaction> (_stubbedQueryID);
      using (transaction.EnterDiscardingScope())
      {
        IBusinessObject[] results = _property.SearchAvailableObjects (_referencingBusinessObject, new DefaultSearchArguments (_stubbedQueryID));

        Assert.IsNotNull (results);
        Assert.IsTrue (results.Length > 0);

        var resultDomainObject = (DomainObject) results[0];
        Assert.IsFalse (outerTransaction.IsEnlisted (resultDomainObject));
        Assert.IsTrue (ClientTransaction.Current.IsEnlisted (resultDomainObject));
      }
    }

    [Test]
    public void SearchAvailableObjectsUsesBindingTransaction ()
    {
      var bindingTransaction = _searchServiceTestHelper.CreateTransactionWithStubbedQuery<BindingClientTransaction> (_stubbedQueryID);

      IBusinessObject boundObject;
      using (bindingTransaction.EnterNonDiscardingScope())
      {
        boundObject = (IBusinessObject) SampleBindableMixinDomainObject.NewObject();
      }

      IBusinessObject[] results = _property.SearchAvailableObjects (boundObject, new DefaultSearchArguments (_stubbedQueryID));
      Assert.IsNotNull (results);
      Assert.IsTrue (results.Length > 0);

      var resultDomainObject = (DomainObject) results[0];
      Assert.That (ClientTransaction.Current.IsEnlisted (resultDomainObject), Is.False);
      Assert.That (bindingTransaction.IsEnlisted (resultDomainObject), Is.True);
    }

    [Test]
    public void SearchAvailableObjectsWithDifferentObject ()
    {
      IBusinessObject[] businessObjects =
          _property.SearchAvailableObjects (SampleBindableDomainObject.NewObject(), new DefaultSearchArguments (_stubbedQueryID));

      Assert.IsNotNull (businessObjects);
      Assert.IsTrue (businessObjects.Length > 0);
    }

    [Test]
    public void SearchAvailableObjectsWithNullSearchArguments ()
    {
      var fakeResultDataContainer = _searchServiceTestHelper.CreateFakeResultDataContainer();
      _searchServiceTestHelper.StubSearchAllObjectsQueryResult (typeof (OppositeBidirectionalBindableDomainObject), new[] { fakeResultDataContainer });

      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_referencingBusinessObject, null);

      Assert.IsNotNull (businessObjects);
      Assert.That (((DomainObject) businessObjects[0]).ID, Is.EqualTo (fakeResultDataContainer.ID));
    }

    [Test]
    public void SearchAvailableObjectsWithNullSearchArguments_BindingTransaction ()
    {
      var transaction = _searchServiceTestHelper.CreateStubbableTransaction<BindingClientTransaction>();

      var fakeResultDataContainer = _searchServiceTestHelper.CreateFakeResultDataContainer();
      _searchServiceTestHelper.StubSearchAllObjectsQueryResult (typeof (OppositeBidirectionalBindableDomainObject), new[] { fakeResultDataContainer });

      IBusinessObject boundReferencingObject;
      using (transaction.EnterNonDiscardingScope())
      {
        boundReferencingObject = (IBusinessObject) SampleBindableMixinDomainObject.NewObject();
      }

      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (boundReferencingObject, null);

      Assert.IsNotNull (businessObjects);
      Assert.That (((DomainObject) businessObjects[0]).GetBindingTransaction(), Is.SameAs (transaction));
    }

    [Test]
    public void SearchAvailableObjectsWithNullQuery ()
    {
      var fakeResultDataContainer = _searchServiceTestHelper.CreateFakeResultDataContainer();
      _searchServiceTestHelper.StubSearchAllObjectsQueryResult (typeof (OppositeBidirectionalBindableDomainObject), new[] { fakeResultDataContainer });

      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_referencingBusinessObject, new DefaultSearchArguments (null));

      Assert.IsNotNull (businessObjects);
      Assert.That (((DomainObject) businessObjects[0]).ID, Is.EqualTo (fakeResultDataContainer.ID));
    }

    [Test]
    public void SearchAvailableObjectsWithEmptyQuery ()
    {
      var fakeResultDataContainer = _searchServiceTestHelper.CreateFakeResultDataContainer();
      _searchServiceTestHelper.StubSearchAllObjectsQueryResult (typeof (OppositeBidirectionalBindableDomainObject), new[] { fakeResultDataContainer });

      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_referencingBusinessObject, new DefaultSearchArguments (""));

      Assert.IsNotNull (businessObjects);
      Assert.That (((DomainObject) businessObjects[0]).ID, Is.EqualTo (fakeResultDataContainer.ID));
    }
  }
}