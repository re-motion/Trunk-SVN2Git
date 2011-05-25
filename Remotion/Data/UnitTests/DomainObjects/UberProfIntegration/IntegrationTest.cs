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
using Remotion.Data.DomainObjects.UberProfIntegration;
using Remotion.Implementation;
using Remotion.ServiceLocation;

namespace Remotion.Data.UnitTests.DomainObjects.UberProfIntegration
{
  [TestFixture]
  public class IntegrationTest : UberProfIntegrationTestBase
  {
    private TracingLinqToSqlAppender _tracingLinqToSqlAppender;

    public override void SetUp ()
    {
      base.SetUp ();

      var locator = new DefaultServiceLocator();
      locator.Register (typeof (IClientTransactionListenerFactory), typeof (LinqToSqlListenerFactory), LifetimeKind.Singleton);
      ServiceLocator.SetLocatorProvider (() => locator);

      _tracingLinqToSqlAppender = new TracingLinqToSqlAppender ();
      MockableAppender.AppenderMock = _tracingLinqToSqlAppender;
    }

    public override void TearDown ()
    {
      base.TearDown ();

      ServiceLocator.SetLocatorProvider (null);
    }

    [Test]
    [Ignore ("TODO 3838")]
    public void LoadSingleObject ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction ();
      LifetimeService.GetObject (clientTransaction, DomainObjectIDs.Order1, false);

      Console.WriteLine (_tracingLinqToSqlAppender.TraceLog);
      Assert.That (_tracingLinqToSqlAppender.TraceLog, Is.EqualTo ("???"));
    }
  }
}