using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.BindableDomainObjectMixinTests
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
      Assert.IsTrue (_property.SupportsSearchAvailableObjects (true));
      IBusinessObjectWithIdentity[] results = (IBusinessObjectWithIdentity[]) _property.SearchAvailableObjects (_orderItem, true, "QueryWithSpecificCollectionType");
      Assert.That (results, Is.EqualTo (ClientTransactionMock.QueryManager.GetCollection (new Query ("QueryWithSpecificCollectionType"))));
    }

    [Test]
    public void SearchViaReferencePropertyWithoutIdentity ()
    {
      Assert.IsTrue (_property.SupportsSearchAvailableObjects (false));
      IBusinessObject[] results = _property.SearchAvailableObjects (_orderItem, false, "QueryWithSpecificCollectionType");
      Assert.That (results, Is.EqualTo (ClientTransactionMock.QueryManager.GetCollection (new Query ("QueryWithSpecificCollectionType"))));
    }

    [Test]
    public void SearchAvailableObjectsUsesCurrentTransaction ()
    {
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        IBusinessObject[] results = _property.SearchAvailableObjects (_orderItem, true, "QueryWithSpecificCollectionType");

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
          _property.SearchAvailableObjects ((IBusinessObject) Order.NewObject (),
          true, "QueryWithSpecificCollectionType");

      Assert.IsNotNull (businessObjects);
      Assert.IsTrue (businessObjects.Length > 0);
    }

    [Test]
    public void SearchAvailableObjectsWithNullQuery ()
    {
      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_orderItem, true, null);

      Assert.IsNotNull (businessObjects);
      Assert.AreEqual (0, businessObjects.Length);
    }

    [Test]
    public void SearchAvailableObjectsWithEmptyQuery ()
    {
      IBusinessObject[] businessObjects = _property.SearchAvailableObjects (_orderItem, true, "");

      Assert.IsNotNull (businessObjects);
      Assert.AreEqual (0, businessObjects.Length);
    }
  }
}