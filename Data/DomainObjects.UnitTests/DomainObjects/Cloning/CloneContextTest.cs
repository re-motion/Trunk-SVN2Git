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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Cloning;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Cloning
{
  [TestFixture]
  public class CloneContextTest : ClientTransactionBaseTest
  {
    private DomainObjectCloner _clonerMock;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp ();
      _mockRepository = new MockRepository();
      _clonerMock = _mockRepository.CreateMock<DomainObjectCloner> ();
    }

    [Test]
    public void Initialization ()
    {
      CloneContext context = new CloneContext(_clonerMock);
      Assert.That (context.CloneHulls.Count, Is.EqualTo (0));
    }

    [Test]
    public void GetCloneFor_CallsClonerForNewObject ()
    {
      CloneContext context = new CloneContext (_clonerMock);
      Order source = Order.GetObject (DomainObjectIDs.Order1);
      Order clone = Order.NewObject ();

      Expect.Call (_clonerMock.CreateCloneHull<DomainObject> (source)).Return (clone);
      _mockRepository.ReplayAll ();
      Assert.That (context.GetCloneFor (source), Is.SameAs (clone));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetCloneFor_DoesntCallClonerTwiceForKnownObject ()
    {
      CloneContext context = new CloneContext (_clonerMock);
      Order source = Order.GetObject (DomainObjectIDs.Order1);
      Order clone = Order.NewObject ();

      Expect.Call (_clonerMock.CreateCloneHull<DomainObject> (source)).Return (clone);
      _mockRepository.ReplayAll ();
      Assert.That (context.GetCloneFor (source), Is.SameAs (clone));
      Assert.That (context.GetCloneFor (source), Is.SameAs (clone));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetCloneFor_AddsToShallowClones()
    {
      CloneContext context = new CloneContext (_clonerMock);
      Order source = Order.GetObject (DomainObjectIDs.Order1);
      Order clone = Order.NewObject ();

      SetupResult.For (_clonerMock.CreateCloneHull<DomainObject> (source)).Return (clone);
      _mockRepository.ReplayAll ();

      context.GetCloneFor (source);
      Assert.That (context.CloneHulls.Contains (new Tuple<DomainObject, DomainObject> (source, clone)));
    }

    [Test]
    public void GetCloneFor_DoesntAddToShallowClonesForKnownObject ()
    {
      CloneContext context = new CloneContext (_clonerMock);
      Order source = Order.GetObject (DomainObjectIDs.Order1);
      Order clone = Order.NewObject ();

      SetupResult.For (_clonerMock.CreateCloneHull<DomainObject> (source)).Return (clone);
      _mockRepository.ReplayAll ();

      context.GetCloneFor (source);
      context.GetCloneFor (source);
      Assert.That (context.CloneHulls.Count, Is.EqualTo(1));
    }
  }
}