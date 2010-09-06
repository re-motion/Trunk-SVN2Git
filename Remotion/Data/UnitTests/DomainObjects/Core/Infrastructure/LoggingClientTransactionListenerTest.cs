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
using System.Collections.ObjectModel;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class LoggingClientTransactionListenerTest
  {
    private MemoryAppender _memoryAppender;
    private ClientTransactionMock _clientTransaction;
    private LoggingClientTransactionListener _listener;
    private Client _domainObject;
    private DataContainer _dataContainer;
    private PropertyValue _propertyValue;

    [SetUp]
    public void SetUp ()
    {
      _memoryAppender = new MemoryAppender();
      BasicConfigurator.Configure (_memoryAppender);

      _clientTransaction = new ClientTransactionMock();

      _listener = new LoggingClientTransactionListener();

      _domainObject = DomainObjectMother.CreateObjectInTransaction<Client> (_clientTransaction);
      _dataContainer = _domainObject.GetInternalDataContainerForTransaction (_clientTransaction);
      _propertyValue = _dataContainer.PropertyValues[0];
    }

    [TearDown]
    public virtual void TearDown ()
    {
      _memoryAppender.Clear();
      LogManager.ResetConfiguration ();
      
      Assert.That (LogManager.GetLogger (typeof (LoggingClientTransactionListener)).IsDebugEnabled, Is.False);
    }

    [Test]
    public void TransactionInitializing ()
    {
      _listener.TransactionInitializing (_clientTransaction);
      var loggingEvents = GetLoggingEvents();

      Assert.That (loggingEvents.Last().RenderedMessage, Is.EqualTo (string.Format ("{0} TransactionInitializing", _clientTransaction.ID)));
    }

    [Test]
    public void TransactionDiscarding ()
    {
      _listener.TransactionDiscarding (_clientTransaction);
      var loggingEvents = GetLoggingEvents();

      Assert.That (loggingEvents.Last().RenderedMessage, Is.EqualTo (string.Format ("{0} TransactionDiscarding", _clientTransaction.ID)));
    }

    [Test]
    public void SubTransactionCreating ()
    {
      _listener.SubTransactionCreating (_clientTransaction);
      var loggingEvents = GetLoggingEvents();

      Assert.That (loggingEvents.Last().RenderedMessage, Is.EqualTo (string.Format ("{0} SubTransactionCreating", _clientTransaction.ID)));
    }

    [Test]
    public void SubTransactionCreated ()
    {
      _listener.SubTransactionCreated (_clientTransaction, _clientTransaction);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (string.Format ("{0} SubTransactionCreated: {1}", _clientTransaction.ID, _clientTransaction.ID)));
    }

    [Test]
    public void NewObjectCreating ()
    {
      ClientTransactionMock.CreateRootTransaction();

      _listener.NewObjectCreating (_clientTransaction, typeof (string), _domainObject);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (string.Format ("{0} NewObjectCreating: {1}", _clientTransaction.ID, typeof (string).FullName)));
    }

    [Test]
    public void ObjectsLoading ()
    {
      _listener.ObjectsLoading (_clientTransaction, new ReadOnlyCollection<ObjectID> (new List<ObjectID>()));
      var loggingEvents = GetLoggingEvents();

      Assert.That (loggingEvents.Last().RenderedMessage, Is.EqualTo (string.Format ("{0} ObjectsLoading: {1}", _clientTransaction.ID, "")));
    }

    [Test]
    public void ObjectsUnloaded ()
    {
      _listener.ObjectsUnloaded (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject }));
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage, Is.EqualTo (string.Format ("{0} ObjectsUnloaded: {1}", _clientTransaction.ID, _domainObject.ID)));
    }

    [Test]
    public void ObjectsLoaded ()
    {
      _listener.ObjectsLoaded (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject }));
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage, Is.EqualTo (string.Format ("{0} ObjectsLoaded: {1}", _clientTransaction.ID, _domainObject.ID)));
    }

    [Test]
    public void ObjectsUnloading ()
    {
      _listener.ObjectsUnloading (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject }));
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage, Is.EqualTo (string.Format ("{0} ObjectsUnloading: {1}", _clientTransaction.ID, _domainObject.ID)));
    }

    [Test]
    public void ObjectDeleting ()
    {
      _listener.ObjectDeleting (_clientTransaction, _domainObject);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage, Is.EqualTo (string.Format ("{0} ObjectDeleting: {1}", _clientTransaction.ID, _domainObject.ID)));
    }

    [Test]
    public void ObjectDeleted ()
    {
      _listener.ObjectDeleted (_clientTransaction, _domainObject);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage, Is.EqualTo (string.Format ("{0} ObjectDeleted: {1}", _clientTransaction.ID, _domainObject.ID)));
    }

    [Test]
    public void PropertyValueReading ()
    {
      _listener.PropertyValueReading (_clientTransaction, _dataContainer, _propertyValue, ValueAccess.Current);

      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} PropertyValueReading: {1} ({2}, {3})", _clientTransaction.ID, _propertyValue.Name, ValueAccess.Current, _dataContainer.ID)));
    }

    [Test]
    public void PropertyValueChanging ()
    {
      _listener.PropertyValueChanging (_clientTransaction, _dataContainer, _propertyValue, 1, 2);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format ("{0} PropertyValueChanging: {1} {2}->{3} ({4})", _clientTransaction.ID, _propertyValue.Name, 1, 2, _dataContainer.ID)));
    }

    [Test]
    public void PropertyValueChanged ()
    {
      _listener.PropertyValueChanged (_clientTransaction, _dataContainer, _propertyValue, 1, 2);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format ("{0} PropertyValueChanged: {1} {2}->{3} ({4})", _clientTransaction.ID, _propertyValue.Name, 1, 2, _dataContainer.ID)));
    }

    [Test]
    public void RelationReading ()
    {
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      relationEndPointDefinition.Stub (n => n.PropertyName).Return ("Name");
      _listener.RelationReading (_clientTransaction, _domainObject, relationEndPointDefinition, ValueAccess.Current);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} RelationReading: {1} ({2}, {3})",
                  _clientTransaction.ID,
                  relationEndPointDefinition.PropertyName,
                  ValueAccess.Current,
                  _domainObject.ID)));
    }

    [Test]
    public void RelationRead ()
    {
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      relationEndPointDefinition.Stub (n => n.PropertyName).Return ("Name");

      _listener.RelationRead (_clientTransaction, _domainObject, relationEndPointDefinition, _domainObject, ValueAccess.Current);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} RelationRead: {1}=={2} ({3}, {4})",
                  _clientTransaction.ID,
                  relationEndPointDefinition.PropertyName,
                  _domainObject.ID,
                  ValueAccess.Current,
                  _domainObject.ID)));
    }

    [Test]
    public void RelationChanging ()
    {
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      relationEndPointDefinition.Stub (n => n.PropertyName).Return ("Name");

      _listener.RelationChanging (_clientTransaction, _domainObject, relationEndPointDefinition, _domainObject, _domainObject);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} RelationChanging: {1}: {2}->{3} /{4}",
                  _clientTransaction.ID,
                  relationEndPointDefinition.PropertyName,
                  _domainObject.ID,
                  _domainObject.ID,
                  _domainObject.ID)));
    }

    [Test]
    public void RelationChanged ()
    {
      var relationEndPointDefinition = MockRepository.GenerateStub<IRelationEndPointDefinition>();
      relationEndPointDefinition.Stub (n => n.PropertyName).Return ("Name");

      _listener.RelationChanged (_clientTransaction, _domainObject, relationEndPointDefinition);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} RelationChanged: {1} ({2})",
                  _clientTransaction.ID,
                  relationEndPointDefinition.PropertyName,
                  _domainObject.ID)));
    }

    [Test]
    public void TransactionCommitting ()
    {
      _listener.TransactionCommitting (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject }));
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} TransactionCommitting: {1}",
                  _clientTransaction.ID,
                  _domainObject.ID)));
    }

    [Test]
    public void TransactionCommitted ()
    {
      _listener.TransactionCommitted (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject }));
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} TransactionCommitted: {1}",
                  _clientTransaction.ID,
                  _domainObject.ID)));
    }

    [Test]
    public void TransactionRollingBack ()
    {
      _listener.TransactionRollingBack (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject }));
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} TransactionRollingBack: {1}",
                  _clientTransaction.ID,
                  _domainObject.ID)));
    }

    [Test]
    public void TransactionRolledBack ()
    {
      _listener.TransactionRolledBack (_clientTransaction, new ReadOnlyCollection<DomainObject> (new List<DomainObject> { _domainObject }));
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} TransactionRolledBack: {1}",
                  _clientTransaction.ID,
                  _domainObject.ID)));
    }

    [Test]
    public void RelationEndPointMapRegistering ()
    {
      RelationEndPointID relationEndPointID;
      RealObjectEndPoint relationEndPoint;
      using (_clientTransaction.EnterNonDiscardingScope())
      {
        relationEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_domainObject.ID, "ParentClient");
        relationEndPoint = RelationEndPointObjectMother.CreateRealObjectEndPoint (relationEndPointID);
      }

      _listener.RelationEndPointMapRegistering (_clientTransaction, relationEndPoint);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} RelationEndPointMapRegistering: {1}",
                  _clientTransaction.ID,
                  relationEndPointID)));
    }

    [Test]
    public void RelationEndPointMapUnregistering ()
    {
      RelationEndPointID relationEndPointID;

      using (_clientTransaction.EnterNonDiscardingScope())
      {
        relationEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_domainObject.ID, "ParentClient");
      }

      _listener.RelationEndPointMapUnregistering (_clientTransaction, relationEndPointID);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} RelationEndPointMapUnregistering: {1}",
                  _clientTransaction.ID,
                  relationEndPointID)));
    }

    [Test]
    public void RelationEndPointUnloading ()
    {
      RelationEndPointID relationEndPointID;
      RealObjectEndPoint relationEndPoint;
      using (_clientTransaction.EnterNonDiscardingScope())
      {
        relationEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_domainObject.ID, "ParentClient");
        relationEndPoint = RelationEndPointObjectMother.CreateRealObjectEndPoint (relationEndPointID);
      }

      _listener.RelationEndPointUnloading (_clientTransaction, relationEndPoint);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} RelationEndPointUnloading: {1}",
                  _clientTransaction.ID,
                  relationEndPointID)));
    }

    [Test]
    public void DataManagerMarkingObjectInvalid ()
    {
      _listener.DataManagerMarkingObjectInvalid (_clientTransaction, _domainObject.ID);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} DataManagerMarkingObjectInvalid: {1}",
                  _clientTransaction.ID,
                  _domainObject.ID)));
    }

    [Test]
    public void DataContainerMapRegistering ()
    {
      _listener.DataContainerMapRegistering (_clientTransaction, _dataContainer);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} DataContainerMapRegistering: {1}",
                  _clientTransaction.ID,
                  _dataContainer.ID)));
    }

    [Test]
    public void DataContainerMapUnregistering ()
    {
      _listener.DataContainerMapUnregistering (_clientTransaction, _dataContainer);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} DataContainerMapUnregistering: {1}",
                  _clientTransaction.ID,
                  _dataContainer.ID)));
    }

    [Test]
    public void DataContainerStateUpdated ()
    {
      var newDataContainerState = new StateType();
      _listener.DataContainerStateUpdated (_clientTransaction, _dataContainer, newDataContainerState);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} DataContainerStateUpdated: {1} {2}",
                  _clientTransaction.ID,
                  _dataContainer.ID,
                  newDataContainerState)));
    }

    [Test]
    public void VirtualRelationEndPointStateUpdated ()
    {
      RelationEndPointID relationEndPointID;

      using (_clientTransaction.EnterNonDiscardingScope())
      {
        relationEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_domainObject.ID, "ParentClient");
      }

      _listener.VirtualRelationEndPointStateUpdated (_clientTransaction, relationEndPointID, false);
      var loggingEvents = GetLoggingEvents();

      Assert.That (
          loggingEvents.Last().RenderedMessage,
          Is.EqualTo (
              string.Format (
                  "{0} VirtualRelationEndPointStateUpdated: {1} {2}",
                  _clientTransaction.ID,
                  relationEndPointID,
                  false)));
    }


    private IEnumerable<LoggingEvent> GetLoggingEvents ()
    {
      return _memoryAppender.GetEvents();
    }
  }
}