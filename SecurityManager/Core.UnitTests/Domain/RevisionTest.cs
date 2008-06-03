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

      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.AreEqual (0, Revision.GetRevision ());
      }
    }

    [Test]
    public void IncrementRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        Revision.IncrementRevision ();
      }

      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.AreEqual (1, Revision.GetRevision ());
      }
    }
  }
}
