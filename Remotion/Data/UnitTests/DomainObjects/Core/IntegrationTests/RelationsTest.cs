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
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class RelationsTest : ClientTransactionBaseTest
  {
    [Test]
    public void OneToOneRelationChangeTest ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket orderTicket = order.OrderTicket;

      var orderEventReceiver = new DomainObjectRelationCheckEventReceiver (order);
      var orderTicketEventReceiver = new DomainObjectRelationCheckEventReceiver (orderTicket);

      orderTicket.Order = null;

      Assert.IsTrue (orderEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (orderTicketEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreSame (orderTicket, orderEventReceiver.GetChangingRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
      Assert.AreSame (order, orderTicketEventReceiver.GetChangingRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));

      Assert.IsTrue (orderEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.IsTrue (orderTicketEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreSame (null, orderEventReceiver.GetChangedRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
      Assert.AreSame (null, orderTicketEventReceiver.GetChangedRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));
    }

    [Test]
    [Ignore ("TODO: Should throw with a sensible message - see COMMONS-731")]
    public void LoadSecondRelationHalf_WithChangedRelationSinceFirstHalf_OneOne()
    {
      SetDatabaseModifyable ();

      Employee employee = Employee.NewObject ();
      ClientTransactionMock.Commit ();
      Computer computer;

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (employee);
        computer = Computer.NewObject ();
        computer.Employee = employee;
        ClientTransaction.Current.Commit ();
      }

      ClientTransaction.Current.EnlistDomainObject (computer);
      Console.WriteLine ("{0}.Computer = {1}", employee, employee.Computer);
      Console.WriteLine ("{0}.Employee = {1}", computer, computer.Employee);
    }

    [Test]
    [Ignore ("TODO: Should throw with a sensible message - see COMMONS-731")]
    public void LoadSecondRelationHalf_WithChangedRelationSinceFirstHalf_OneMany ()
    {
      SetDatabaseModifyable ();

      // setup new IndustrialSector object in database
      IndustrialSector industrialSector = CreateNewIndustrialSector();
      ClientTransactionMock.Commit ();
      
      // in parallel transaction, add a Company to the IndustrialSector
      ObjectID newCompanyID = AddNewCompanyInDatabase(industrialSector);
      // load Company into this transaction; in the database, the Company has a foreign key to the IndustrialSector
      Company newCompany = Company.GetObject (newCompanyID); // TODO 731 (re-motion 2.1): This should throw an exception.

      // this prints true/false => the bidirectional relation is inconsistent
      Console.WriteLine ("{0}.IndustrialSector = {1} (the company has a reference to the industrial sector: {2})", 
          newCompany, newCompany.IndustrialSector, newCompany.IndustrialSector == industrialSector);
      Console.WriteLine ("{0}.Companies.Count = {1} (the industrial sector has a reference to the company: {2})", 
          industrialSector, industrialSector.Companies.Count, industrialSector.Companies.ContainsObject (industrialSector));
    }

    [Test]
    [Ignore ("TODO: Should throw with a sensible message - see COMMONS-921")]
    public void LoadSecondRelationHalf_WithChangedRelationSinceFirstHalf_OneMany_ProblemDetectedInAdd ()
    {
      SetDatabaseModifyable ();

      // setup new IndustrialSector object in database
      IndustrialSector industrialSector = CreateNewIndustrialSector ();
      ClientTransactionMock.Commit ();

      // in parallel transaction, add a Company to the IndustrialSector
      ObjectID newCompanyID = AddNewCompanyInDatabase (industrialSector);
      // load Company into this transaction; in the database, the Company has a foreign key to the IndustrialSector
      Company newCompany = Company.GetObject (newCompanyID); // TODO 731 (re-motion 2.1): This should throw an exception.

      industrialSector.Companies.Add (newCompany); // TODO 921: Throw here
      Console.WriteLine ("{0}.IndustrialSector = {1} (the company has a reference to the industrial sector: {2})",
          newCompany, newCompany.IndustrialSector, newCompany.IndustrialSector == industrialSector);
      Console.WriteLine ("{0}.Companies.Count = {1} (the industrial sector has a reference to the company: {2})",
          industrialSector, industrialSector.Companies.Count, industrialSector.Companies.ContainsObject (industrialSector));
    }

    [Test]
    public void Relation_WithMoreThan2100Objects ()
    {
      SetDatabaseModifyable ();
      
      var transaction = ClientTransaction.CreateRootTransaction();
      var orderInOtherTx = DomainObjectMother.GetObjectInTransaction<Order> (transaction, DomainObjectIDs.Order1);
      var orderItemsInOtherTx = transaction.Execute (() => Enumerable.Range (0, 4000).Select (i =>
      {
        var orderItem = OrderItem.NewObject();
        orderInOtherTx.OrderItems.Add (orderItem);
        return orderItem;
      }).ToArray());

      transaction.Commit ();

      var orderInThisTx = Order.GetObject (orderInOtherTx.ID);
      var orderItems = orderInThisTx.OrderItems;

      Assert.That (orderItems.Count, Is.EqualTo (4002));
      Assert.That (orderItems.Contains (DomainObjectIDs.OrderItem1), Is.True);
      Assert.That (orderItems.Contains (DomainObjectIDs.OrderItem2), Is.True);
      foreach (var orderItemInOtherTx in orderItemsInOtherTx)
        Assert.That (orderItems.Contains (orderItemInOtherTx.ID), Is.True);
    }

    [Test]
    public void Relation_WithMoreThan2100Objects_WithTableInheritance ()
    {
      SetDatabaseModifyable ();

      var transaction = ClientTransaction.CreateRootTransaction ();
      var folderInOtherTx = DomainObjectMother.CreateObjectInTransaction<TableInheritance.TestDomain.Folder> (transaction);
      transaction.Execute (() => folderInOtherTx.CreatedAt = new DateTime(2010, 01, 01));
      var filesInOtherTx = transaction.Execute(() => Enumerable.Range (0, 4000).Select (i =>
      {
        var file = TableInheritance.TestDomain.File.NewObject();
        file.CreatedAt = new DateTime (2010, 01, 01);
        folderInOtherTx.FileSystemItems.Add (file);
        return file;
      }).ToArray());

      transaction.Commit ();

      var folderInThisTx = TableInheritance.TestDomain.Folder.GetObject (folderInOtherTx.ID);
      var fileSystemItems = folderInThisTx.FileSystemItems;

      Assert.That (fileSystemItems.Count, Is.EqualTo (4000));
      foreach (var fileInOtherTx in filesInOtherTx)
        Assert.That (fileSystemItems.Contains (fileInOtherTx.ID), Is.True);
    }

    private IndustrialSector CreateNewIndustrialSector ()
    {
      IndustrialSector industrialSector = IndustrialSector.NewObject ();
      Company oldCompany = Company.NewObject ();
      oldCompany.Ceo = Ceo.NewObject ();
      industrialSector.Companies.Add (oldCompany);
      return industrialSector;
    }

    private ObjectID AddNewCompanyInDatabase (IndustrialSector industrialSector)
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var industrialSectorInTx = IndustrialSector.GetObject (industrialSector.ID);
        Company newCompany = Company.NewObject ();
        newCompany.Ceo = Ceo.NewObject ();
        industrialSectorInTx.Companies.Add (newCompany);
        ClientTransaction.Current.Commit ();
        return newCompany.ID;
      }
    }
  }
}
