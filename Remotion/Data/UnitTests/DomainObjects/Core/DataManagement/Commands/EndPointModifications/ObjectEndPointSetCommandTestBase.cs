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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  public abstract class ObjectEndPointSetCommandTestBase : ClientTransactionBaseTest
  {
    private ObjectEndPoint _endPoint;
    private ObjectEndPointSetCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (GetRelationEndPointID (), OldRelatedObject.ID);
      _command = CreateCommand (_endPoint, NewRelatedObject);
    }

    protected abstract DomainObject OldRelatedObject { get; }
    protected abstract DomainObject NewRelatedObject { get; }

    protected abstract RelationEndPointID GetRelationEndPointID ();

    protected abstract ObjectEndPointSetCommand CreateCommand (IObjectEndPoint endPoint, DomainObject newRelatedObject);
    protected abstract ObjectEndPointSetCommand CreateCommandMock (MockRepository repository, ObjectEndPoint endPoint, DomainObject newRelatedObject);

    public ObjectEndPointSetCommand Command
    {
      get { return _command; }
    }

    public ObjectEndPoint EndPoint
    {
      get { return _endPoint; }
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPoint, _command.ModifiedEndPoint);
      Assert.AreSame (OldRelatedObject, _command.OldRelatedObject);
      Assert.AreSame (NewRelatedObject, _command.NewRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullObjectEndPoint (ClientTransactionMock, GetRelationEndPointID().Definition);
      CreateCommand (endPoint, NewRelatedObject);
    }

    [Test]
    public virtual void Begin ()
    {
      DomainObject domainObject = EndPoint.GetDomainObject ();
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      _command.Begin();

      Assert.IsTrue (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void Perform_InvokesPerformRelationChange ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (OldRelatedObject.ID));
      _command.Perform();
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (NewRelatedObject.ID));
    }

    [Test]
    public void Perform_TouchesEndPoint ()
    {
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (GetRelationEndPointID (), OldRelatedObject.ID);
      Assert.That (endPoint.HasBeenTouched, Is.False);

      var command = CreateCommand (endPoint, NewRelatedObject);

      command.Perform();

      Assert.That (endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public virtual void End ()
    {
      DomainObject domainObject = EndPoint.GetDomainObject ();
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      _command.End();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public virtual void NotifyClientTransactionOfBegin ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _command.NotifyClientTransactionOfBegin();

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

      _command.NotifyClientTransactionOfEnd ();

      listenerMock.AssertWasCalled (mock => mock.RelationChanged (_endPoint.GetDomainObject (), _endPoint.PropertyName));
    }

    protected IList<RelationEndPointModificationCommand> GetAllCommands (IDataManagementCommand bidirectionalModification)
    {
      return ((CompositeCommand) bidirectionalModification).GetCommands ().Cast<RelationEndPointModificationCommand> ().ToList ();
    }
  }
}
