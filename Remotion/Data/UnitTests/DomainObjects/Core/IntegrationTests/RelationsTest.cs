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
using Remotion.Data.Linq.Utilities;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.FunctionalProgramming;

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
      Assert.AreSame (
          orderTicket, orderEventReceiver.GetChangingRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
      Assert.AreSame (
          order, orderTicketEventReceiver.GetChangingRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));

      Assert.IsTrue (orderEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.IsTrue (orderTicketEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreSame (null, orderEventReceiver.GetChangedRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"));
      Assert.AreSame (
          null, orderTicketEventReceiver.GetChangedRelatedDomainObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order"));
    }

    [Test]
    [Ignore ("TODO: Should throw with a sensible message - see COMMONS-731")]
    public void LoadSecondRelationHalf_WithChangedRelationSinceFirstHalf_OneOne ()
    {
      SetDatabaseModifyable();

      Employee employee = Employee.NewObject();
      ClientTransactionMock.Commit();
      Computer computer;

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        ClientTransaction.Current.EnlistDomainObject (employee);
        computer = Computer.NewObject();
        computer.Employee = employee;
        ClientTransaction.Current.Commit();
      }

      ClientTransaction.Current.EnlistDomainObject (computer);
      Console.WriteLine ("{0}.Computer = {1}", employee, employee.Computer);
      Console.WriteLine ("{0}.Employee = {1}", computer, computer.Employee);
    }

    [Test]
    [Ignore ("TODO: Should throw with a sensible message - see COMMONS-731")]
    public void LoadSecondRelationHalf_WithChangedRelationSinceFirstHalf_OneMany ()
    {
      SetDatabaseModifyable();

      // setup new IndustrialSector object in database
      IndustrialSector industrialSector = CreateNewIndustrialSector();
      ClientTransactionMock.Commit();

      // in parallel transaction, add a Company to the IndustrialSector
      ObjectID newCompanyID = AddNewCompanyInDatabase (industrialSector);
      // load Company into this transaction; in the database, the Company has a foreign key to the IndustrialSector
      Company newCompany = Company.GetObject (newCompanyID); // TODO 731 (re-motion 2.1): This should throw an exception.

      // this prints true/false => the bidirectional relation is inconsistent
      Console.WriteLine (
          "{0}.IndustrialSector = {1} (the company has a reference to the industrial sector: {2})",
          newCompany,
          newCompany.IndustrialSector,
          newCompany.IndustrialSector == industrialSector);
      Console.WriteLine (
          "{0}.Companies.Count = {1} (the industrial sector has a reference to the company: {2})",
          industrialSector,
          industrialSector.Companies.Count,
          industrialSector.Companies.ContainsObject (industrialSector));
    }

    [Test]
    [Ignore ("TODO: Should throw with a sensible message - see COMMONS-921")]
    public void LoadSecondRelationHalf_WithChangedRelationSinceFirstHalf_OneMany_ProblemDetectedInAdd ()
    {
      SetDatabaseModifyable();

      // setup new IndustrialSector object in database
      IndustrialSector industrialSector = CreateNewIndustrialSector();
      ClientTransactionMock.Commit();

      // in parallel transaction, add a Company to the IndustrialSector
      ObjectID newCompanyID = AddNewCompanyInDatabase (industrialSector);
      // load Company into this transaction; in the database, the Company has a foreign key to the IndustrialSector
      Company newCompany = Company.GetObject (newCompanyID); // TODO 731 (re-motion 2.1): This should throw an exception.

      industrialSector.Companies.Add (newCompany); // TODO 921: Throw here
      Console.WriteLine (
          "{0}.IndustrialSector = {1} (the company has a reference to the industrial sector: {2})",
          newCompany,
          newCompany.IndustrialSector,
          newCompany.IndustrialSector == industrialSector);
      Console.WriteLine (
          "{0}.Companies.Count = {1} (the industrial sector has a reference to the company: {2})",
          industrialSector,
          industrialSector.Companies.Count,
          industrialSector.Companies.ContainsObject (industrialSector));
    }

    [Test]
    public void Relation_WithMoreThan2100Objects ()
    {
      SetDatabaseModifyable();

      var insertedIDs = Enumerable.Range (0, 4000).Select (x => Guid.NewGuid()).ToArray();
      var insertStatements = insertedIDs.Select (
          id => string.Format (
              "insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) values ('{0}', 'OrderItem', '{1}', 1, 'Test2100')",
              id,
              DomainObjectIDs.Order1.Value));

      var script = SeparatedStringBuilder.Build (Environment.NewLine, insertStatements);
      DatabaseAgent.ExecuteCommand (script);

      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order.OrderItems;

      Assert.That (orderItems.Count, Is.EqualTo (4002));

      var loadedIDs = orderItems.Select (oi => (Guid) oi.ID.Value);
      var expectedIDs = insertedIDs.Concat (new[] { (Guid) DomainObjectIDs.OrderItem1.Value, (Guid) DomainObjectIDs.OrderItem2.Value });
      Assert.That (loadedIDs.SetEquals (expectedIDs), Is.True);
    }

    [Test]
    public void Relation_WithMoreThan2100Objects_WithTableInheritance ()
    {
      SetDatabaseModifyable();

      var domainObjectIDs = new TableInheritance.DomainObjectIDs ();

      var insertedIDs = Enumerable.Range (0, 4000).Select (x => Guid.NewGuid ()).ToArray ();
      var insertStatements = insertedIDs.Select (
          id => string.Format (
              "insert into [TableInheritance_File] (ID, ClassID, [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt]) "
              + "values ('{0}', 'TI_File', 'Test', '{1}', 'TI_Folder', 42, '2006/02/03')",
              id,
              domainObjectIDs.Folder1.Value));

      var script = SeparatedStringBuilder.Build (Environment.NewLine, insertStatements);
      DatabaseAgent.ExecuteCommand (script);

      var folder = TableInheritance.TestDomain.Folder.GetObject (domainObjectIDs.Folder1);
      var fileSystemItems = folder.FileSystemItems;

      Assert.That (fileSystemItems.Count, Is.EqualTo (4001));
      var loadedIDs = fileSystemItems.Select (oi => (Guid) oi.ID.Value);
      var expectedIDs = insertedIDs.Concat (new[] { (Guid) domainObjectIDs.File1.Value });
      Assert.That (loadedIDs.SetEquals (expectedIDs), Is.True);
    }

    private IndustrialSector CreateNewIndustrialSector ()
    {
      IndustrialSector industrialSector = IndustrialSector.NewObject();
      Company oldCompany = Company.NewObject();
      oldCompany.Ceo = Ceo.NewObject();
      industrialSector.Companies.Add (oldCompany);
      return industrialSector;
    }

    private ObjectID AddNewCompanyInDatabase (IndustrialSector industrialSector)
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        var industrialSectorInTx = IndustrialSector.GetObject (industrialSector.ID);
        Company newCompany = Company.NewObject();
        newCompany.Ceo = Ceo.NewObject();
        industrialSectorInTx.Companies.Add (newCompany);
        ClientTransaction.Current.Commit();
        return newCompany.ID;
      }
    }
  }
}