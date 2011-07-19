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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UberProfIntegration;
using Remotion.ServiceLocation;

namespace Remotion.Data.UnitTests.DomainObjects.UberProfIntegration
{
  [TestFixture]
  public class IntegrationTest : UberProfIntegrationTestBase
  {
    private TracingLinqToSqlAppender _tracingLinqToSqlAppender;

    public override void SetUp ()
    {
      base.SetUp();

      var locator = new DefaultServiceLocator();
      var factory = new LinqToSqlListenerFactory();
      locator.Register (typeof (IClientTransactionListenerFactory), () => factory);
      locator.Register (typeof (IPersistenceListenerFactory), () => factory);
      ServiceLocator.SetLocatorProvider (() => locator);

      _tracingLinqToSqlAppender = new TracingLinqToSqlAppender();
      SetAppender (_tracingLinqToSqlAppender);

    }

    public override void TearDown ()
    {
      base.TearDown();

      ServiceLocator.SetLocatorProvider (null);
    }

    [Test]
    public void LoadSingleObject ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction ();
      LifetimeService.GetObject (clientTransaction, DomainObjectIDs.Order1, false);
      clientTransaction.Discard();

      Assert.That (
          _tracingLinqToSqlAppender.TraceLog,
          Is.StringMatching (
              @"^ConnectionStarted \((?<connectionid>[^,]+)\)" + Environment.NewLine
              + @"StatementExecuted \(\k<connectionid>, (?<statementid>[^,]+), -- Statement \k<statementid>" + Environment.NewLine
              + @"SELECT \[ID\], \[ClassID\], \[Timestamp\], \[OrderNo\], \[DeliveryDate\], \[OfficialID\], \[CustomerID\], \[CustomerIDClassID\] "
              + @"FROM \[Order\] WHERE \[ID\] = \@ID;" + Environment.NewLine
              + @"-- Ignore unbounded result sets: TOP \*" + Environment.NewLine
              + @"-- Parameters:" + Environment.NewLine
              + @"-- \@ID = \[-\[5682f032-2f0b-494b-a31c-c97f02b89c36\]-\] \[-\[Type \(0\)\]-\]" + Environment.NewLine
              + @"\)" + Environment.NewLine
              + @"CommandDurationAndRowCount \(\k<connectionid>, \d+, \<null\>\)" + Environment.NewLine
              + @"StatementRowCount \(\k<connectionid>, \k<statementid>, 1\)" + Environment.NewLine
              + @"ConnectionDisposed \(\k<connectionid>\)" + Environment.NewLine
              + @"$"));
    }
  }
}