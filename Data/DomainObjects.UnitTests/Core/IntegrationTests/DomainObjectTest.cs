using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.Core.MockConstraints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.Data.DomainObjects.UnitTests.Core.IntegrationTests
{
  [TestFixture]
  public class DomainObjectTest: ClientTransactionBaseTest
  {
    private DataContainer _orderDataContainer;
    private PropertyValueCollection _orderPropertyValues;
    private PropertyValue _orderDeliveryDateProperty;

    private DomainObjectEventReceiver _orderDomainObjectEventReceiver;
    private PropertyValueContainerEventReceiver _orderDataContainerEventReceiver;
    private PropertyValueContainerEventReceiver _orderPropertyValuesEventReceiver;

    public override void TestFixtureSetUp()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    [Test]
    public void RelationEventTestWithMockObject()
    {
      Customer newCustomer1 = Customer.NewObject();
      newCustomer1.Name = "NewCustomer1";

      Customer newCustomer2 = Customer.NewObject();
      newCustomer2.Name = "NewCustomer2";

      Official official2 = Official.GetObject (DomainObjectIDs.Official2);
      Ceo newCeo1 = Ceo.NewObject();
      Ceo newCeo2 = Ceo.NewObject();
      Order newOrder1 = Order.NewObject();
      newOrder1.DeliveryDate = new DateTime (2006, 1, 1);

      Order newOrder2 = Order.NewObject();
      newOrder2.DeliveryDate = new DateTime (2006, 2, 2);

      OrderItem newOrderItem1 = OrderItem.NewObject();
      OrderItem newOrderItem2 = OrderItem.NewObject();

      MockRepository mockRepository = new MockRepository();

      DomainObjectCollection newCustomer1Orders = newCustomer1.Orders;
      DomainObjectCollection newCustomer2Orders = newCustomer2.Orders;
      DomainObjectCollection official2Orders = official2.Orders;
      DomainObjectCollection newOrder1OrderItems = newOrder1.OrderItems;
      DomainObjectCollection newOrder2OrderItems = newOrder2.OrderItems;

      DomainObjectMockEventReceiver newCustomer1EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newCustomer1);
      DomainObjectMockEventReceiver newCustomer2EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newCustomer2);
      DomainObjectMockEventReceiver official2EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (official2);
      DomainObjectMockEventReceiver newCeo1EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newCeo1);
      DomainObjectMockEventReceiver newCeo2EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newCeo2);
      DomainObjectMockEventReceiver newOrder1EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrder1);
      DomainObjectMockEventReceiver newOrder2EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrder2);
      DomainObjectMockEventReceiver newOrderItem1EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrderItem1);
      DomainObjectMockEventReceiver newOrderItem2EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrderItem2);

      DomainObjectCollectionMockEventReceiver newCustomer1OrdersEventReceiver =
          mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (newCustomer1.Orders);
      DomainObjectCollectionMockEventReceiver newCustomer2OrdersEventReceiver =
          mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (newCustomer2.Orders);
      DomainObjectCollectionMockEventReceiver official2OrdersEventReceiver =
          mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (official2.Orders);
      DomainObjectCollectionMockEventReceiver newOrder1OrderItemsEventReceiver =
          mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (newOrder1.OrderItems);
      DomainObjectCollectionMockEventReceiver newOrder2OrderItemsEventReceiver =
          mockRepository.CreateMock<DomainObjectCollectionMockEventReceiver> (newOrder2.OrderItems);

      IClientTransactionExtension extension = mockRepository.CreateMock<IClientTransactionExtension>();

      using (mockRepository.Ordered())
      {
        //1
        //newCeo1.Company = newCustomer1;
        extension.RelationChanging (ClientTransactionMock, newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", null, newCustomer1);
        extension.RelationChanging (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo", null, newCeo1);

        newCeo1EventReceiver.RelationChanging (newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", null, newCustomer1);

        newCustomer1EventReceiver.RelationChanging (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo", null, newCeo1);

        extension.RelationChanged (ClientTransactionMock, newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company");
        extension.RelationChanged (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");

        newCeo1EventReceiver.RelationChanged (newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company");

        newCustomer1EventReceiver.RelationChanged (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");


        //2
        //newCeo2.Company = newCustomer1;
        extension.RelationChanging (ClientTransactionMock, newCeo2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", null, newCustomer1);
        extension.RelationChanging (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo", newCeo1, newCeo2);
        extension.RelationChanging (ClientTransactionMock, newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", newCustomer1, null);

        newCeo2EventReceiver.RelationChanging (newCeo2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", null, newCustomer1);

        newCustomer1EventReceiver.RelationChanging (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo", newCeo1, newCeo2);

        newCeo1EventReceiver.RelationChanging (newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", newCustomer1, null);

        extension.RelationChanged (ClientTransactionMock, newCeo2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company");
        extension.RelationChanged (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");
        extension.RelationChanged (ClientTransactionMock, newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company");

        newCeo2EventReceiver.RelationChanged (newCeo2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company");

        newCustomer1EventReceiver.RelationChanged (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");

        newCeo1EventReceiver.RelationChanged (newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company");


        //3
        //newCeo1.Company = newCustomer2;
        extension.RelationChanging (ClientTransactionMock, newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", null, newCustomer2);
        extension.RelationChanging (ClientTransactionMock, newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo", null, newCeo1);

        newCeo1EventReceiver.RelationChanging (newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", null, newCustomer2);

        newCustomer2EventReceiver.RelationChanging (newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo", null, newCeo1);

        extension.RelationChanged (ClientTransactionMock, newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company");
        extension.RelationChanged (ClientTransactionMock, newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");

        newCeo1EventReceiver.RelationChanged (newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company");

        newCustomer2EventReceiver.RelationChanged (newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");


        //4
        //newCeo1.Company = null;
        extension.RelationChanging (ClientTransactionMock, newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", newCustomer2, null);
        extension.RelationChanging (ClientTransactionMock, newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo", newCeo1, null);

        newCeo1EventReceiver.RelationChanging (newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", newCustomer2, null);

        newCustomer2EventReceiver.RelationChanging (newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo", newCeo1, null);

        extension.RelationChanged (ClientTransactionMock, newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company");
        extension.RelationChanged (ClientTransactionMock, newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");

        newCeo1EventReceiver.RelationChanged (newCeo1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company");

        newCustomer2EventReceiver.RelationChanged (newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo");


        //5
        //newCustomer1.Orders.Add (newOrder1);
        extension.RelationReading (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", ValueAccess.Current);
        extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionScope.CurrentTransaction), 
            Mocks_Is.Same (newCustomer1),
            Mocks_Is.Equal ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"),
            Mocks_Property.Value ("Count", 0),
            Mocks_Is.Equal (ValueAccess.Current));

        extension.RelationChanging (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, newCustomer1);
        extension.RelationChanging (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, newOrder1);

        newOrder1EventReceiver.RelationChanging (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, newCustomer1);

        newCustomer1OrdersEventReceiver.Adding (newCustomer1Orders, newOrder1);

        newCustomer1EventReceiver.RelationChanging (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, newOrder1);

        extension.RelationChanged (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
        extension.RelationChanged (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");

        newOrder1EventReceiver.RelationChanged (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");

        newCustomer1OrdersEventReceiver.Added (newCustomer1Orders, newOrder1);

        newCustomer1EventReceiver.RelationChanged (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");


        //6
        //newCustomer1.Orders.Add (newOrder2);
        extension.RelationReading (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", ValueAccess.Current);
        extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionScope.CurrentTransaction), 
            Mocks_Is.Same (newCustomer1),
            Mocks_Is.Equal ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"),
            Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (newOrder1),
            Mocks_Is.Equal (ValueAccess.Current));

        extension.RelationChanging (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, newCustomer1);
        extension.RelationChanging (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, newOrder2);

        newOrder2EventReceiver.RelationChanging (newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, newCustomer1);

        newCustomer1OrdersEventReceiver.Adding (newCustomer1Orders, newOrder2);

        newCustomer1EventReceiver.RelationChanging (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, newOrder2);

        extension.RelationChanged (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
        extension.RelationChanged (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");

        newOrder2EventReceiver.RelationChanged (newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");

        newCustomer1OrdersEventReceiver.Added (newCustomer1Orders, newOrder2);

        newCustomer1EventReceiver.RelationChanged (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");


        //7
        //newCustomer1.Orders.Remove (newOrder2);
        extension.RelationReading (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", ValueAccess.Current);
        extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionScope.CurrentTransaction), 
            Mocks_Is.Same (newCustomer1),
            Mocks_Is.Equal ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"),
            Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (newOrder1) & Mocks_List.IsIn (newOrder2),
            Mocks_Is.Equal (ValueAccess.Current));

        extension.RelationChanging (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", newCustomer1, null);
        extension.RelationChanging (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", newOrder2, null);

        newOrder2EventReceiver.RelationChanging (newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", newCustomer1, null);

        newCustomer1OrdersEventReceiver.Removing (newCustomer1Orders, newOrder2);
        newCustomer1EventReceiver.RelationChanging (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", newOrder2, null);

        extension.RelationChanged (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
        extension.RelationChanged (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");

        newOrder2EventReceiver.RelationChanged (newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
        newCustomer1OrdersEventReceiver.Removed (newCustomer1Orders, newOrder2);
        newCustomer1EventReceiver.RelationChanged (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");


        //8
        //newOrderItem1.Order = newOrder1;
        extension.RelationChanging (ClientTransactionMock, newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", null, newOrder1);
        extension.RelationChanging (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", null, newOrderItem1);

        newOrderItem1EventReceiver.RelationChanging (
            newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", null, newOrder1);
        newOrder1OrderItemsEventReceiver.Adding (newOrder1OrderItems, newOrderItem1);
        newOrder1EventReceiver.RelationChanging (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", null, newOrderItem1);

        extension.RelationChanged (ClientTransactionMock, newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
        extension.RelationChanged (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

        newOrderItem1EventReceiver.RelationChanged (newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
        newOrder1OrderItemsEventReceiver.Added (newOrder1OrderItems, newOrderItem1);
        newOrder1EventReceiver.RelationChanged (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");


        //9
        //newOrderItem2.Order = newOrder1;
        extension.RelationChanging (ClientTransactionMock, newOrderItem2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", null, newOrder1);
        extension.RelationChanging (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", null, newOrderItem2);

        newOrderItem2EventReceiver.RelationChanging (
            newOrderItem2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", null, newOrder1);
        newOrder1OrderItemsEventReceiver.Adding (newOrder1OrderItems, newOrderItem2);
        newOrder1EventReceiver.RelationChanging (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", null, newOrderItem2);

        extension.RelationChanged (ClientTransactionMock, newOrderItem2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
        extension.RelationChanged (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

        newOrderItem2EventReceiver.RelationChanged (newOrderItem2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
        newOrder1OrderItemsEventReceiver.Added (newOrder1OrderItems, newOrderItem2);
        LastCall.Constraints (Mocks_Is.Same (newOrder1OrderItems), Mocks_Property.Value ("DomainObject", newOrderItem2));
        newOrder1EventReceiver.RelationChanged (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");


        //10
        //newOrderItem1.Order = null;
        extension.RelationChanging (ClientTransactionMock, newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", newOrder1, null);
        extension.RelationChanging (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", newOrderItem1, null);

        newOrderItem1EventReceiver.RelationChanging (
            newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", newOrder1, null);
        newOrder1OrderItemsEventReceiver.Removing (newOrder1OrderItems, newOrderItem1);
        newOrder1EventReceiver.RelationChanging (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", newOrderItem1, null);

        extension.RelationChanged (ClientTransactionMock, newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
        extension.RelationChanged (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

        newOrderItem1EventReceiver.RelationChanged (newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
        newOrder1OrderItemsEventReceiver.Removed (newOrder1OrderItems, newOrderItem1);
        newOrder1EventReceiver.RelationChanged (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");


        //11
        //newOrderItem1.Order = newOrder2;
        extension.RelationChanging (ClientTransactionMock, newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", null, newOrder2);
        extension.RelationChanging (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", null, newOrderItem1);

        newOrderItem1EventReceiver.RelationChanging (
            newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", null, newOrder2);
        newOrder2OrderItemsEventReceiver.Adding (newOrder2OrderItems, newOrderItem1);
        newOrder2EventReceiver.RelationChanging (newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", null, newOrderItem1);

        extension.RelationChanged (ClientTransactionMock, newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
        extension.RelationChanged (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");

        newOrderItem1EventReceiver.RelationChanged (newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
        newOrder2OrderItemsEventReceiver.Added (newOrder2OrderItems, newOrderItem1);
        newOrder2EventReceiver.RelationChanged (newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");


        //12
        //newOrder1.Official = official2;
        extension.RelationChanging (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official", null, official2);
        extension.RelationChanging (ClientTransactionMock, official2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Official.Orders", null, newOrder1);

        newOrder1EventReceiver.RelationChanging (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official", null, official2);
        official2OrdersEventReceiver.Adding (official2Orders, newOrder1);
        official2EventReceiver.RelationChanging (official2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Official.Orders", null, newOrder1);

        extension.RelationChanged (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official");
        extension.RelationChanged (ClientTransactionMock, official2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Official.Orders");

        newOrder1EventReceiver.RelationChanged (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official");
        official2OrdersEventReceiver.Added (official2Orders, newOrder1);
        official2EventReceiver.RelationChanged (official2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Official.Orders");


        //13
        //OrderTicket newOrderTicket1 = OrderTicket.NewObject (newOrder1);

        extension.NewObjectCreating (ClientTransactionMock, typeof (OrderTicket));

        extension.RelationChanging (null, null, null, null, null);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionScope.CurrentTransaction),
            Mocks_Is.TypeOf<OrderTicket> (),
            Mocks_Is.Equal ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"),
            Mocks_Is.Null(),
            Mocks_Is.Same (newOrder1));
        extension.RelationChanging (null, null, null, null, null);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionScope.CurrentTransaction), 
            Mocks_Is.Same (newOrder1),
            Mocks_Is.Equal ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"),
            Mocks_Is.Null(),
            Mocks_Is.TypeOf<OrderTicket>());

        newOrder1EventReceiver.RelationChanging (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (newOrder1),
            Mocks_Property.Value ("PropertyName", "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket")
            & Mocks_Property.Value ("OldRelatedObject", null) & Mocks_Property.ValueConstraint ("NewRelatedObject", Mocks_Is.TypeOf<OrderTicket>()));

        extension.RelationChanged (null, null, null);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionScope.CurrentTransaction),
            Mocks_Is.TypeOf<OrderTicket>(),
            Mocks_Is.Equal ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
        extension.RelationChanged (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

        newOrder1EventReceiver.RelationChanged (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      }

      ClientTransactionScope.CurrentTransaction.Extensions.Add ("Extension", extension);
      mockRepository.ReplayAll();

      //1
      newCeo1.Company = newCustomer1;
      //2
      newCeo2.Company = newCustomer1;
      //3
      newCeo1.Company = newCustomer2;
      //4
      newCeo1.Company = null;
      //5
      newCustomer1.Orders.Add (newOrder1);
      //6
      newCustomer1.Orders.Add (newOrder2);
      //7
      newCustomer1.Orders.Remove (newOrder2);
      //8
      newOrderItem1.Order = newOrder1;
      //9
      newOrderItem2.Order = newOrder1;
      //10
      newOrderItem1.Order = null;
      //11
      newOrderItem1.Order = newOrder2;
      //12
      newOrder1.Official = official2;
      //13
      OrderTicket newOrderTicket1 = OrderTicket.NewObject (newOrder1);

      mockRepository.VerifyAll();

      BackToRecord (
          mockRepository,
          extension,
          newCustomer1EventReceiver,
          newCustomer2EventReceiver,
          official2EventReceiver,
          newCeo1EventReceiver,
          newCeo2EventReceiver,
          newOrder1EventReceiver,
          newOrder2EventReceiver,
          newOrderItem1EventReceiver,
          newOrderItem2EventReceiver,
          newCustomer1OrdersEventReceiver,
          newCustomer2OrdersEventReceiver,
          official2OrdersEventReceiver,
          newOrder1OrderItemsEventReceiver,
          newOrder2OrderItemsEventReceiver);

      DomainObjectMockEventReceiver newOrderTicket1EventReceiver = mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrderTicket1);

      using (mockRepository.Ordered())
      {
        //14
        //newOrderTicket1.Order = newOrder2;
        extension.RelationChanging (ClientTransactionMock, newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", newOrder1, newOrder2);
        extension.RelationChanging (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", newOrderTicket1, null);
        extension.RelationChanging (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", null, newOrderTicket1);

        newOrderTicket1EventReceiver.RelationChanging (
            newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", newOrder1, newOrder2);
        newOrder1EventReceiver.RelationChanging (
            newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", newOrderTicket1, null);
        newOrder2EventReceiver.RelationChanging (
            newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", null, newOrderTicket1);

        extension.RelationChanged (ClientTransactionMock, newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
        extension.RelationChanged (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
        extension.RelationChanged (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

        newOrderTicket1EventReceiver.RelationChanged (newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
        newOrder1EventReceiver.RelationChanged (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
        newOrder2EventReceiver.RelationChanged (newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");


        //15a
        //newOrder2.Customer = newCustomer1;
        extension.RelationChanging (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, newCustomer1);
        extension.RelationChanging (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, newOrder2);

        newOrder2EventReceiver.RelationChanging (newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", null, newCustomer1);
        newCustomer1OrdersEventReceiver.Adding (newCustomer1Orders, newOrder2);
        newCustomer1EventReceiver.RelationChanging (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, newOrder2);

        extension.RelationChanged (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
        extension.RelationChanged (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");

        newOrder2EventReceiver.RelationChanged (newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
        newCustomer1OrdersEventReceiver.Added (newCustomer1Orders, newOrder2);
        newCustomer1EventReceiver.RelationChanged (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");


        //15b
        //newOrder2.Customer = newCustomer2;
        extension.RelationChanging (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", newCustomer1, newCustomer2);
        extension.RelationChanging (ClientTransactionMock, newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, newOrder2);
        extension.RelationChanging (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", newOrder2, null);

        newOrder2EventReceiver.RelationChanging (
            newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", newCustomer1, newCustomer2);
        newCustomer2OrdersEventReceiver.Adding (newCustomer2Orders, newOrder2);
        newCustomer2EventReceiver.RelationChanging (newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", null, newOrder2);
        newCustomer1OrdersEventReceiver.Removing (newCustomer1Orders, newOrder2);
        newCustomer1EventReceiver.RelationChanging (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", newOrder2, null);

        extension.RelationChanged (ClientTransactionMock, newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
        extension.RelationChanged (ClientTransactionMock, newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
        extension.RelationChanged (ClientTransactionMock, newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");

        newOrder2EventReceiver.RelationChanged (newOrder2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
        newCustomer2OrdersEventReceiver.Added (newCustomer2Orders, newOrder2);
        newCustomer2EventReceiver.RelationChanged (newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
        newCustomer1OrdersEventReceiver.Removed (newCustomer1Orders, newOrder2);
        newCustomer1EventReceiver.RelationChanged (newCustomer1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");


        //16
        //newOrder2.Delete ();
        extension.ObjectDeleting (ClientTransactionMock, newOrder2);

        using (mockRepository.Unordered())
        {
          extension.RelationChanging (ClientTransactionMock, newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", newOrder2, null);
          extension.RelationChanging (ClientTransactionMock, newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", newOrder2, null);
          extension.RelationChanging (ClientTransactionMock, newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", newOrder2, null);
        }

        newOrder2EventReceiver.Deleting (null, null);
        LastCall.Constraints (Mocks_Is.Same (newOrder2), Mocks_Is.NotNull());

        using (mockRepository.Unordered())
        {
          newCustomer2OrdersEventReceiver.Removing (newCustomer2Orders, newOrder2);
          newCustomer2EventReceiver.RelationChanging (
              newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", newOrder2, null);
          newOrderTicket1EventReceiver.RelationChanging (
              newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", newOrder2, null);
          newOrderItem1EventReceiver.RelationChanging (
              newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", newOrder2, null);
        }

        using (mockRepository.Unordered())
        {
          extension.RelationChanged (ClientTransactionMock, newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
          extension.RelationChanged (ClientTransactionMock, newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
          extension.RelationChanged (ClientTransactionMock, newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
        }

        extension.ObjectDeleted (ClientTransactionMock, newOrder2);

        using (mockRepository.Unordered())
        {
          newCustomer2OrdersEventReceiver.Removed (newCustomer2Orders, newOrder2);
          newCustomer2EventReceiver.RelationChanged (newCustomer2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
          newOrderTicket1EventReceiver.RelationChanged (newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
          newOrderItem1EventReceiver.RelationChanged (newOrderItem1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
        }

        newOrder2EventReceiver.Deleted (null, null);
        LastCall.Constraints (Mocks_Is.Same (newOrder2), Mocks_Is.NotNull());

        //17
        //newOrderTicket1.Order = newOrder1;
        extension.RelationChanging (ClientTransactionMock, newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", null, newOrder1);
        extension.RelationChanging (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", null, newOrderTicket1);

        newOrderTicket1EventReceiver.RelationChanging (
            newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", null, newOrder1);
        newOrder1EventReceiver.RelationChanging (
            newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", null, newOrderTicket1);

        extension.RelationChanged (ClientTransactionMock, newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
        extension.RelationChanged (ClientTransactionMock, newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");

        newOrderTicket1EventReceiver.RelationChanged (newOrderTicket1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order");
        newOrder1EventReceiver.RelationChanged (newOrder1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");


        //cleanup for commit
        //18
        //newCustomer2.Delete ();
        extension.ObjectDeleting (ClientTransactionMock, newCustomer2);

        newCustomer2EventReceiver.Deleting (null, null);
        LastCall.Constraints (Mocks_Is.Same (newCustomer2), Mocks_Is.NotNull());

        extension.ObjectDeleted (ClientTransactionMock, newCustomer2);

        newCustomer2EventReceiver.Deleted (null, null);
        LastCall.Constraints (Mocks_Is.Same (newCustomer2), Mocks_Is.NotNull());


        //19
        //newCeo1.Delete ();
        extension.ObjectDeleting (ClientTransactionMock, newCeo1);

        newCeo1EventReceiver.Deleting (null, null);
        LastCall.Constraints (Mocks_Is.Same (newCeo1), Mocks_Is.NotNull());

        extension.ObjectDeleted (ClientTransactionMock, newCeo1);

        newCeo1EventReceiver.Deleted (null, null);
        LastCall.Constraints (Mocks_Is.Same (newCeo1), Mocks_Is.NotNull());


        //20
        //newOrderItem1.Delete ();
        extension.ObjectDeleting (ClientTransactionMock, newOrderItem1);

        newOrderItem1EventReceiver.Deleting (null, null);
        LastCall.Constraints (Mocks_Is.Same (newOrderItem1), Mocks_Is.NotNull());

        extension.ObjectDeleted (ClientTransactionMock, newOrderItem1);

        newOrderItem1EventReceiver.Deleted (null, null);
        LastCall.Constraints (Mocks_Is.Same (newOrderItem1), Mocks_Is.NotNull());


        //21
        //ClientTransactionScope.CurrentTransaction.Commit ();
        using (mockRepository.Unordered())
        {
          newCustomer1EventReceiver.Committing (null, null);
          LastCall.Constraints (Mocks_Is.Same (newCustomer1), Mocks_Is.NotNull());

          official2EventReceiver.Committing (null, null);
          LastCall.Constraints (Mocks_Is.Same (official2), Mocks_Is.NotNull());

          newCeo2EventReceiver.Committing (null, null);
          LastCall.Constraints (Mocks_Is.Same (newCeo2), Mocks_Is.NotNull());

          newOrder1EventReceiver.Committing (null, null);
          LastCall.Constraints (Mocks_Is.Same (newOrder1), Mocks_Is.NotNull());

          newOrderItem2EventReceiver.Committing (null, null);
          LastCall.Constraints (Mocks_Is.Same (newOrderItem2), Mocks_Is.NotNull());

          newOrderTicket1EventReceiver.Committing (null, null);
          LastCall.Constraints (Mocks_Is.Same (newOrderTicket1), Mocks_Is.NotNull());
        }
        extension.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (ClientTransactionScope.CurrentTransaction),
            new ContainsConstraint (newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1));

        using (mockRepository.Unordered())
        {
          newCustomer1EventReceiver.Committed (null, null);
          LastCall.Constraints (Mocks_Is.Same (newCustomer1), Mocks_Is.Anything());

          official2EventReceiver.Committed (null, null);
          LastCall.Constraints (Mocks_Is.Same (official2), Mocks_Is.NotNull());

          newCeo2EventReceiver.Committed (null, null);
          LastCall.Constraints (Mocks_Is.Same (newCeo2), Mocks_Is.NotNull());

          newOrder1EventReceiver.Committed (null, null);
          LastCall.Constraints (Mocks_Is.Same (newOrder1), Mocks_Is.NotNull());

          newOrderItem2EventReceiver.Committed (null, null);
          LastCall.Constraints (Mocks_Is.Same (newOrderItem2), Mocks_Is.NotNull());

          newOrderTicket1EventReceiver.Committed (null, null);
          LastCall.Constraints (Mocks_Is.Same (newOrderTicket1), Mocks_Is.NotNull());
        }
        extension.Committed (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (ClientTransactionScope.CurrentTransaction), 
            Mocks_Property.Value ("Count", 6) & new ContainsConstraint (newCustomer1, official2, newCeo2, newOrder1, newOrderItem2, newOrderTicket1));
      }

      mockRepository.ReplayAll();

      //14
      newOrderTicket1.Order = newOrder2;
      //15a
      newOrder2.Customer = newCustomer1;
      //15b
      newOrder2.Customer = newCustomer2;
      //16
      newOrder2.Delete();
      //17
      newOrderTicket1.Order = newOrder1;
      //cleanup for commit
      //18
      newCustomer2.Delete();
      //19
      newCeo1.Delete();
      //20
      newOrderItem1.Delete();

      //21
      ClientTransactionScope.CurrentTransaction.Commit();

      mockRepository.VerifyAll();
    }

    [Test]
    public void SetValuesAndAccessOriginalValuesTest()
    {
      OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

			DataContainer dataContainer = orderItem.InternalDataContainer;

      dataContainer.SetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product", "newProduct");

      Assert.AreNotEqual(
          "newProduct",
          dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product"].OriginalValue);
      Assert.AreEqual ("newProduct", orderItem.Product);

      ClientTransactionMock.Commit();
      orderItem.Product = "newProduct2";

      Assert.AreEqual (
          "newProduct",
          dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product"].OriginalValue);
      Assert.AreEqual ("newProduct2", orderItem.Product);
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException))]
    public void NewCustomerAndCEOTest()
    {
      IndustrialSector industrialSector = IndustrialSector.NewObject();
      Customer customer = Customer.NewObject();
      customer.Ceo = Ceo.NewObject();

      industrialSector.Companies.Add (customer);

      Order order1 = Order.NewObject();
      OrderTicket.NewObject (order1);

      //getting an SQL Exception without this line
      order1.DeliveryDate = DateTime.Now;

      OrderItem orderItem = OrderItem.NewObject();
      order1.OrderItems.Add (orderItem);
      order1.Official = Official.GetObject (DomainObjectIDs.Official2);
      customer.Orders.Add (order1);

      try
      {
        ClientTransactionMock.Commit();
      }
      catch (MandatoryRelationNotSetException)
      {
        Assert.Fail ("MandatoryRelationNotSetException was thrown when none was expected.");
      }

      customer.Delete();
      ClientTransactionScope.CurrentTransaction.Commit();
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void AddInvalidPropertyValueTest()
    {
      Employee employee = Employee.NewObject();

      PropertyDefinition propertyDefinition =
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition((ReflectionBasedClassDefinition) employee.InternalDataContainer.ClassDefinition, "testproperty", "testproperty", typeof (string), true, 10);
			PropertyValueCollection propertyValues = employee.InternalDataContainer.PropertyValues;

      Assert.IsFalse (propertyValues.Contains ("testproperty"));

      propertyValues.Add (new PropertyValue (propertyDefinition));

      Assert.IsTrue (propertyValues.Contains ("testproperty"));
      Assert.IsNotNull (propertyValues["testproperty"]);

      ClientTransactionMock.Commit();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void AddPropertyValueWithExistingNameTest()
    {
      Employee employee = Employee.NewObject();

      PropertyDefinition propertyDefinition =
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition((ReflectionBasedClassDefinition) employee.InternalDataContainer.ClassDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Name", "Name", typeof (string), true, 10);
      PropertyValueCollection propertyValues = employee.InternalDataContainer.PropertyValues;

      Assert.IsTrue (propertyValues.Contains ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Employee.Name"));

      propertyValues.Add (new PropertyValue (propertyDefinition));
    }

    [Test]
    public void PropertyEventsOfNewObjectPropertyChangeTest()
    {
      Order newOrder = Order.NewObject();

      InitializeEventReceivers (newOrder);
      CheckNoEvents ();

      newOrder.DeliveryDate = DateTime.Now;

      CheckEvents (_orderDeliveryDateProperty);
    }

    [Test]
    public void PropertyEventsOfNewObjectRelationChangeTest()
    {
      Order newOrder = Order.NewObject();

      InitializeEventReceivers (newOrder);
      CheckNoEvents ();

      newOrder.Customer = null;

      CheckNoEvents ();
    }

    [Test]
    public void PropertyEventsOfExistingObjectPropertyChangeTest()
    {
      Order order2 = Order.GetObject (DomainObjectIDs.Order2);

      InitializeEventReceivers (order2);
      CheckNoEvents ();

      order2.DeliveryDate = DateTime.Now;

      CheckEvents (_orderDeliveryDateProperty);
    }

    [Test]
    public void PropertyEventsOfExistingObjectRelationChangeTest()
    {
      Order order2 = Order.GetObject (DomainObjectIDs.Order2);

      InitializeEventReceivers (order2);
      CheckNoEvents ();

      order2.Customer = null;

      CheckMixedEvents (order2.InternalDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Order), "Customer")]);
    }

    [Test]
    public void SaveObjectWithNonMandatoryOneToManyRelation()
    {
      Customer newCustomer = Customer.NewObject();
      newCustomer.Ceo = Ceo.NewObject();

      Customer existingCustomer = Customer.GetObject (DomainObjectIDs.Customer3);
      Assert.AreEqual (1, existingCustomer.Orders.Count);
      Assert.IsNotNull (existingCustomer.Orders[0].OrderTicket);
      Assert.AreEqual (1, existingCustomer.Orders[0].OrderItems.Count);

      existingCustomer.Orders[0].OrderTicket.Delete();
      existingCustomer.Orders[0].OrderItems[0].Delete();
      existingCustomer.Orders[0].Delete();

      ClientTransactionScope.CurrentTransaction.Commit();
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        newCustomer = Customer.GetObject (newCustomer.ID);
        existingCustomer = Customer.GetObject (DomainObjectIDs.Customer3);

        Assert.AreEqual (0, newCustomer.Orders.Count);
        Assert.AreEqual (0, existingCustomer.Orders.Count);
      }
    }

    private void InitializeEventReceivers (Order order)
    {
			_orderDataContainer = order.InternalDataContainer;
      _orderPropertyValues = _orderDataContainer.PropertyValues;
      _orderDeliveryDateProperty = _orderPropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"];

      _orderDomainObjectEventReceiver = new DomainObjectEventReceiver (order);
      _orderDataContainerEventReceiver = new PropertyValueContainerEventReceiver (_orderDataContainer, false);
      _orderPropertyValuesEventReceiver = new PropertyValueContainerEventReceiver (_orderPropertyValues, false);
    }

    private void CheckNoEvents ()
    {
      Assert.IsNull (_orderPropertyValuesEventReceiver.ChangingPropertyValue);
      Assert.IsNull (_orderPropertyValuesEventReceiver.ChangedPropertyValue);
      Assert.IsNull (_orderDataContainerEventReceiver.ChangingPropertyValue);
      Assert.IsNull (_orderDataContainerEventReceiver.ChangedPropertyValue);
      Assert.IsFalse (_orderDomainObjectEventReceiver.HasChangingEventBeenCalled);
      Assert.IsFalse (_orderDomainObjectEventReceiver.HasChangedEventBeenCalled);
      Assert.IsNull (_orderDomainObjectEventReceiver.ChangingPropertyValue);
      Assert.IsNull (_orderDomainObjectEventReceiver.ChangedPropertyValue);
    }

    private void CheckMixedEvents (PropertyValue propertyValue)
    {
      Assert.AreSame (propertyValue, _orderPropertyValuesEventReceiver.ChangingPropertyValue);
      Assert.AreSame (propertyValue, _orderPropertyValuesEventReceiver.ChangedPropertyValue);
      Assert.IsNull (_orderDataContainerEventReceiver.ChangingPropertyValue);
      Assert.IsNull (_orderDataContainerEventReceiver.ChangedPropertyValue);
      Assert.IsFalse (_orderDomainObjectEventReceiver.HasChangingEventBeenCalled);
      Assert.IsFalse (_orderDomainObjectEventReceiver.HasChangedEventBeenCalled);
      Assert.IsNull (_orderDomainObjectEventReceiver.ChangingPropertyValue);
      Assert.IsNull (_orderDomainObjectEventReceiver.ChangedPropertyValue);
    }

    private void CheckEvents (PropertyValue propertyValue)
    {
      Assert.AreSame (propertyValue, _orderPropertyValuesEventReceiver.ChangingPropertyValue);
      Assert.AreSame (propertyValue, _orderPropertyValuesEventReceiver.ChangedPropertyValue);
      Assert.AreSame (propertyValue, _orderDataContainerEventReceiver.ChangingPropertyValue);
      Assert.AreSame (propertyValue, _orderDataContainerEventReceiver.ChangedPropertyValue);
      Assert.IsTrue (_orderDomainObjectEventReceiver.HasChangingEventBeenCalled);
      Assert.IsTrue (_orderDomainObjectEventReceiver.HasChangedEventBeenCalled);
      Assert.AreSame (propertyValue, _orderDomainObjectEventReceiver.ChangingPropertyValue);
      Assert.AreSame (propertyValue, _orderDomainObjectEventReceiver.ChangedPropertyValue);
    }

    private void BackToRecord (MockRepository mockRepository, params object[] objects)
    {
      foreach (object obj in objects)
        mockRepository.BackToRecord (obj);
    }
  }
}