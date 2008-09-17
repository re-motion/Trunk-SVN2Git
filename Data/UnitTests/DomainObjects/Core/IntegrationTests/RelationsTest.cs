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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

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

      IndustrialSector industrialSector = IndustrialSector.NewObject ();
      Company oldCompany = Company.NewObject ();
      oldCompany.Ceo = Ceo.NewObject ();
      industrialSector.Companies.Add (oldCompany);
      ClientTransactionMock.Commit ();
      Company company;

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (industrialSector);
        company = Company.NewObject ();
        company.Ceo = Ceo.NewObject ();
        industrialSector.Companies.Add (company);
        ClientTransaction.Current.Commit ();
      }

      ClientTransaction.Current.EnlistDomainObject (company);
      Console.WriteLine ("{0}.IndustrialSector = {1}", company, company.IndustrialSector);
      Console.WriteLine ("{0}.Companies.Count = {1}", industrialSector, industrialSector.Companies.Count);
    }
  }
}
