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
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlEntryPriorityComparerTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void Compare_Equals ()
    {
      AccessControlEntry leftAce = AccessControlEntry.NewObject();
      leftAce.Priority = 42;
      AccessControlEntry rightAce = AccessControlEntry.NewObject();
      rightAce.Priority = 42;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.AreEqual (0, comparer.Compare (leftAce, rightAce));
      Assert.AreEqual (0, comparer.Compare (rightAce, leftAce));
    }

    [Test]
    public void Compare_LeftIsLessThanRight ()
    {
      AccessControlEntry leftAce = AccessControlEntry.NewObject();
      leftAce.Priority = 24;
      AccessControlEntry rightAce = AccessControlEntry.NewObject();
      rightAce.Priority = 42;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Less (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_LeftIsGreaterThanRight ()
    {
      AccessControlEntry leftAce = AccessControlEntry.NewObject();
      leftAce.Priority = 42;
      AccessControlEntry rightAce = AccessControlEntry.NewObject();
      rightAce.Priority = 24;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Greater (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_LeftIsLessThanRightAndRightIsCalculated ()
    {
      AccessControlEntry leftAce = AccessControlEntry.NewObject();
      leftAce.Priority = 2;
      AccessControlEntry rightAce = AccessControlEntry.NewObject();
      rightAce.User = UserSelection.Owner;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Less (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_RightIsNull ()
    {
      AccessControlEntry leftAce = AccessControlEntry.NewObject();
      leftAce.Priority = 0;
      AccessControlEntry rightAce = null;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Greater (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_LeftIsNull ()
    {
      AccessControlEntry leftAce = null;
      AccessControlEntry rightAce = AccessControlEntry.NewObject();
      rightAce.Priority = 0;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.Less (comparer.Compare (leftAce, rightAce), 0);
    }

    [Test]
    public void Compare_BothAreNull ()
    {
      AccessControlEntry leftAce = null;
      AccessControlEntry rightAce = null;
      AccessControlEntryPriorityComparer comparer = new AccessControlEntryPriorityComparer ();

      Assert.AreEqual (0, comparer.Compare (leftAce, rightAce));
    }
  }
}
