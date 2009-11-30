
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class RelationEndPointTouchModificationTest : ClientTransactionBaseTest
  {
    private RelationEndPoint _endPointMock;
    private DomainObject _domainObjectOfEndPoint;
    private RelationEndPointTouchModification _modification;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObjectOfEndPoint = Customer.GetObject (DomainObjectIDs.Customer1);
      _endPointMock = MockRepository.GenerateMock<RelationEndPoint> (
          ClientTransactionMock, 
          _domainObjectOfEndPoint.ID, 
          typeof (Customer).FullName + ".Orders");

      _modification = new RelationEndPointTouchModification (_endPointMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_modification.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (_modification.OldRelatedObject, Is.Null);
      Assert.That (_modification.NewRelatedObject, Is.Null);
    }

    [Test]
    public void Begin ()
    {
      var eventReceiver = new DomainObjectEventReceiver (_domainObjectOfEndPoint);

      _endPointMock.Replay ();
      _modification.Begin ();
      _endPointMock.VerifyAllExpectations ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void End ()
    {
      var eventReceiver = new DomainObjectEventReceiver (_domainObjectOfEndPoint);

      _endPointMock.Replay ();
      _modification.End ();
      _endPointMock.VerifyAllExpectations ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void Perform ()
    {
      _endPointMock.Expect (mock => mock.Touch ());
      _endPointMock.Replay ();

      _modification.Perform ();

      _endPointMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void CreateBidirectionalModification ()
    {
      _modification.CreateRelationModification ();
    }
  }
}
