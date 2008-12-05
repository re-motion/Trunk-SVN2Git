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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Cloning;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Cloning
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
      _clonerMock = _mockRepository.StrictMock<DomainObjectCloner> ();
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
