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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance
{
  [TestFixture]
  public class DomainObjectTest : TableInheritanceMappingTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    [Test]
    public void OneToManyRelationToAbstractClass ()
    {
      Client client = Client.GetObject (DomainObjectIDs.Client);

      DomainObjectCollection assignedObjects = client.AssignedObjects;

      Assert.AreEqual (4, assignedObjects.Count);
      Assert.AreEqual (DomainObjectIDs.OrganizationalUnit, assignedObjects[0].ID);
      Assert.AreEqual (DomainObjectIDs.Person, assignedObjects[1].ID);
      Assert.AreEqual (DomainObjectIDs.PersonForUnidirectionalRelationTest, assignedObjects[2].ID);
      Assert.AreEqual (DomainObjectIDs.Customer, assignedObjects[3].ID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = 
        "The property 'Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain.HistoryEntry.Owner' of the loaded DataContainer "
        + "'TI_HistoryEntry|2c7fb7b3-eb16-43f9-bdde-b8b3f23a93d2|System.Guid' refers to ClassID 'TI_OrganizationalUnit', "
        + "but the actual ClassID is 'TI_Person'.")]
    public void SameIDInDifferentConcreteTables ()
    {
      Person person = Person.GetObject (new ObjectID (typeof (Person), new Guid ("{B969AFCB-2CDA-45ff-8490-EB52A86D5464}")));
      DomainObjectCollection historyEntries = person.HistoryEntries;
    }

    [Test]
    public void RelationsFromConcreteSingle ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer);
      Assert.AreEqual ("UnitTests", customer.CreatedBy);
      Assert.AreEqual ("Zaphod", customer.FirstName);
      Assert.AreEqual (CustomerType.Premium, customer.CustomerType);

      Region region = customer.Region;
      Assert.IsNotNull (region);
      Assert.AreEqual (DomainObjectIDs.Region, region.ID);

      DomainObjectCollection orders = customer.Orders;
      Assert.AreEqual (1, orders.Count);
      Assert.AreEqual (DomainObjectIDs.Order, orders[0].ID);

      DomainObjectCollection historyEntries = customer.HistoryEntries;
      Assert.AreEqual (2, historyEntries.Count);
      Assert.AreEqual (DomainObjectIDs.HistoryEntry2, historyEntries[0].ID);
      Assert.AreEqual (DomainObjectIDs.HistoryEntry1, historyEntries[1].ID);

      Client client = customer.Client;
      Assert.AreEqual (DomainObjectIDs.Client, client.ID);

      Assert.IsEmpty (customer.AbstractClassesWithoutDerivations);
    }

    [Test]
    public void RelationsFromConcrete ()
    {
      Person person = Person.GetObject (DomainObjectIDs.Person);
      Assert.AreEqual (DomainObjectIDs.Client, person.Client.ID);
      Assert.AreEqual (1, person.HistoryEntries.Count);
    }


    [Test]
    public void OneToManyRelationToConcreteSingle ()
    {
      Region region = Region.GetObject (DomainObjectIDs.Region);
      Assert.AreEqual (1, region.Customers.Count);
      Assert.AreEqual (DomainObjectIDs.Customer, region.Customers[0].ID);
    }

    [Test]
    public void ManyToOneRelationToConcreteSingle ()
    {
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order);
        Assert.AreEqual (DomainObjectIDs.Customer, order.Customer.ID);
      }
    }

    [Test]
    public void ManyToOneRelationToAbstractClass ()
    {
      HistoryEntry historyEntry = HistoryEntry.GetObject (DomainObjectIDs.HistoryEntry1);
      Assert.AreEqual (DomainObjectIDs.Customer, historyEntry.Owner.ID);
    }

    [Test]
    public void UpdateConcreteSingle ()
    {
      Region expectedNewRegion = Region.NewObject ();
      expectedNewRegion.Name = "Wachau";

      Customer expectedCustomer = Customer.GetObject (DomainObjectIDs.Customer);
      expectedCustomer.LastName = "NewLastName";
      expectedCustomer.Region = expectedNewRegion;

      ClientTransactionScope.CurrentTransaction.Commit ();
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        Customer actualCustomer = Customer.GetObject (expectedCustomer.ID);
        Assert.AreEqual ("NewLastName", actualCustomer.LastName);
        Assert.AreEqual (expectedNewRegion.ID, actualCustomer.Region.ID);
      }
    }

    [Test]
    public void InsertConcreteSingle ()
    {
      Customer expectedCustomer = Customer.NewObject ();
      expectedCustomer.FirstName = "Franz";
      expectedCustomer.LastName = "Kameramann";
      expectedCustomer.DateOfBirth = new DateTime (1950, 1, 3);
      expectedCustomer.CustomerType = CustomerType.Premium;
      expectedCustomer.CustomerSince = DateTime.Now;

      Address expectedAddress = Address.NewObject();
      expectedAddress.Street = "Linzer Straße 1";
      expectedAddress.Zip = "3100";
      expectedAddress.City = "St. Pölten";
      expectedAddress.Country = "Österreich";
      expectedAddress.Person = expectedCustomer;

      ClientTransactionScope.CurrentTransaction.Commit ();
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        Customer actualCustomer = Customer.GetObject (expectedCustomer.ID);
        Assert.IsNotNull (actualCustomer);
        Assert.AreEqual (expectedAddress.ID, actualCustomer.Address.ID);
      }
    }

    [Test]
    public void DeleteConcreteSingle ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer);
      
      foreach (HistoryEntry historyEntry in customer.HistoryEntries.Clone ())
        historyEntry.Delete ();

      customer.Delete ();
      

      ClientTransactionScope.CurrentTransaction.Commit ();
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        try
        {
          Customer.GetObject (DomainObjectIDs.Customer);
          Assert.Fail ("ObjectNotFoundException was expected.");
        }
        catch (ObjectNotFoundException)
        {
        }
      }
    }
  }
}
