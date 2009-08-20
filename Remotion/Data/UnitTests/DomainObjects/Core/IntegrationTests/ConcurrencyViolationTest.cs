// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class ConcurrencyViolationTest : ClientTransactionBaseTest
  {
    [Test]
    public void ConcurrencyViolationException_WhenSomebodyElseModifiesData ()
    {
      SetDatabaseModifyable();

      var computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.SerialNumber = "100";

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var computerInOtherTransaction = Computer.GetObject (DomainObjectIDs.Computer1);
        computerInOtherTransaction.SerialNumber = "200";
        ClientTransaction.Current.Commit ();
      }

      try
      {
        ClientTransactionMock.Commit ();
        Assert.Fail ("Expected ConcurrencyViolationException");
      }
      catch (ConcurrencyViolationException)
      {
        // succeed
      }
    }

    [Test]
    public void ConcurrencyViolationException_WhenSomebodyElseMarksAsChanged ()
    {
      SetDatabaseModifyable ();

      var computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.MarkAsChanged ();

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var computerInOtherTransaction = Computer.GetObject (DomainObjectIDs.Computer1);
        computerInOtherTransaction.MarkAsChanged ();
        ClientTransaction.Current.Commit ();
      }

      try
      {
        ClientTransactionMock.Commit ();
        Assert.Fail ("Expected ConcurrencyViolationException");
      }
      catch (ConcurrencyViolationException)
      {
        // succeed
      }
    }
  }
}