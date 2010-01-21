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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  public abstract class ObjectEndPointSetModificationBaseTest : ClientTransactionBaseTest
  {
    private ObjectEndPoint _endPoint;
    private ObjectEndPointSetModificationBase _modification;

    public override void SetUp ()
    {
      base.SetUp();

      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (GetRelationEndPointID (), OldRelatedObject.ID);
      _modification = CreateModification (_endPoint, NewRelatedObject);
    }

    protected abstract DomainObject OldRelatedObject { get; }
    protected abstract DomainObject NewRelatedObject { get; }

    protected abstract RelationEndPointID GetRelationEndPointID ();

    protected abstract ObjectEndPointSetModificationBase CreateModification (IObjectEndPoint endPoint, DomainObject newRelatedObject);
    protected abstract ObjectEndPointSetModificationBase CreateModificationMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject);

    public ObjectEndPointSetModificationBase Modification
    {
      get { return _modification; }
    }

    public ObjectEndPoint EndPoint
    {
      get { return _endPoint; }
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPoint, _modification.ModifiedEndPoint);
      Assert.AreSame (OldRelatedObject, _modification.OldRelatedObject);
      Assert.AreSame (NewRelatedObject, _modification.NewRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModification is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullObjectEndPoint (ClientTransactionMock, GetRelationEndPointID().Definition);
      CreateModification (endPoint, NewRelatedObject);
    }

    [Test]
    public virtual void Begin ()
    {
      DomainObject domainObject = EndPoint.GetDomainObject ();
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      _modification.Begin();

      Assert.IsTrue (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void Perform_InvokesPerformRelationChange ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (OldRelatedObject.ID));
      _modification.Perform();
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (NewRelatedObject.ID));
    }

    [Test]
    public void Perform_TouchesEndPoint ()
    {
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (GetRelationEndPointID (), OldRelatedObject.ID);
      Assert.That (endPoint.HasBeenTouched, Is.False);

      var modification = CreateModification (endPoint, NewRelatedObject);

      modification.Perform();

      Assert.That (endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public virtual void End ()
    {
      DomainObject domainObject = EndPoint.GetDomainObject ();
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      _modification.End();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public virtual void NotifyClientTransactionOfBegin ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _modification.NotifyClientTransactionOfBegin();

      listenerMock.AssertWasCalled(mock => mock.RelationChanging (
          _endPoint.GetDomainObject (), 
          _endPoint.PropertyName, 
          OldRelatedObject, 
          NewRelatedObject));
    }

    [Test]
    public virtual void NotifyClientTransactionOfEnd ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _modification.NotifyClientTransactionOfEnd ();

      listenerMock.AssertWasCalled (mock => mock.RelationChanged (_endPoint.GetDomainObject (), _endPoint.PropertyName));
    }

    protected IList<RelationEndPointModification> GetModificationSteps (IDataManagementCommand bidirectionalModification)
    {
      return ((CompositeDataManagementCommand) bidirectionalModification).GetCommands ().Cast<RelationEndPointModification> ().ToList ();
    }
  }
}
