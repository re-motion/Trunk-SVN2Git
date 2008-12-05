// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.SecurityManager.Domain;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class RevisionTest : DomainTest
  {
    [Test]
    public void GetRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.AreEqual (0, Revision.GetRevision ());
      }
    }

    [Test]
    public void IncrementRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Revision.IncrementRevision ();
      }

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.AreEqual (1, Revision.GetRevision ());
      }
    }
  }
}
