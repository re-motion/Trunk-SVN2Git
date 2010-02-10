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
using System.Collections.ObjectModel;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class ObjectLoaderTest : StandardMappingTest
  {
    private MockRepository _mockRepository;

    private IDataSource _dataSourceMock;
    private IClientTransactionListener _eventSinkMock;
    private ClientTransactionMock _clientTransaction;

    private ObjectLoader _objectLoader;

    private DataContainer _dataContainer1;
    private DataContainer _dataContainer2;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();
      
      _dataSourceMock = _mockRepository.StrictMock<IDataSource> ();
      _eventSinkMock = _mockRepository.DynamicMock<IClientTransactionListener> ();
      _clientTransaction = new ClientTransactionMock();

      _objectLoader = new ObjectLoader (_clientTransaction, _dataSourceMock, _eventSinkMock);

      _dataContainer1 = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      _dataContainer2 = DataContainer.CreateForExisting (DomainObjectIDs.Order2, null, pd => pd.DefaultValue);
    }

    [Test]
    public void LoadObject ()
    {
      _dataSourceMock.Stub (mock => mock.LoadDataContainer (DomainObjectIDs.Order1)).Return (_dataContainer1);

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadObject (DomainObjectIDs.Order1);

      CheckLoadedObject (result, _dataContainer1);
    }

    [Test]
    public void LoadObject_Events ()
    {
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _dataSourceMock.Expect (mock => mock.LoadDataContainer (DomainObjectIDs.Order1)).Return (_dataContainer1);
        _eventSinkMock
            .Expect (mock => mock.ObjectsLoading (Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1 })))
            .WhenCalled (mi => Assert.That (_clientTransaction.DataManager.DataContainerMap[_dataContainer1.ID], Is.Null));

        _eventSinkMock
            .Expect (mock => mock.ObjectsLoaded (
                Arg<ReadOnlyCollection<DomainObject>>.Matches (list => list.Count == 1 && list[0].ID == _dataContainer1.ID)))
            .WhenCalled (mi =>
            {
              Assert.That (_clientTransaction.DataManager.DataContainerMap[_dataContainer1.ID], Is.SameAs (_dataContainer1));
              Assert.That (((Order) ((ReadOnlyCollection<DomainObject>) mi.Arguments[0])[0]).OnLoadedCalled, Is.True);
              Assert.That (transactionEventReceiver.LoadedDomainObjects, Is.Empty);
            });
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObject (DomainObjectIDs.Order1);

      _mockRepository.VerifyAll();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result }));
    }

    [Test]
    public void LoadObjects ()
    {
      _dataSourceMock
          .Stub (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _dataContainer1, _dataContainer2 }, true));

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true);

      Assert.That (result.Length, Is.EqualTo (2));

      CheckLoadedObject (result[0], _dataContainer1);
      CheckLoadedObject (result[1], _dataContainer2);
    }

    [Test]
    public void LoadObjects_WithUnknownObjects ()
    {
      _dataSourceMock
          .Stub (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _dataContainer1, _dataContainer2 }, true));

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order2 }, true);

      Assert.That (result.Length, Is.EqualTo (3));

      CheckLoadedObject (result[0], _dataContainer1);
      Assert.That (result[1], Is.Null);
      CheckLoadedObject (result[2], _dataContainer2);
    }

    [Test]
    public void LoadObjects_Ordering ()
    {
      _dataSourceMock
          .Stub (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _dataContainer2, _dataContainer1 }, true));

      _dataSourceMock.Replay ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true);

      Assert.That (result.Length, Is.EqualTo (2));

      CheckLoadedObject (result[0], _dataContainer1);
      CheckLoadedObject (result[1], _dataContainer2);
    }

    [Test]
    public void LoadObjects_Events ()
    {
      var transactionEventReceiver = new ClientTransactionEventReceiver (_clientTransaction);

      using (_mockRepository.Ordered ())
      {
        _dataSourceMock
            .Expect (mock => mock.LoadDataContainers (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true))
          .Return (new DataContainerCollection (new[] { _dataContainer1, _dataContainer2 }, true));

        _eventSinkMock
            .Expect (mock => mock.ObjectsLoading (Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 })))
            .WhenCalled (mi =>
            {
              Assert.That (_clientTransaction.DataManager.DataContainerMap[_dataContainer1.ID], Is.Null);
              Assert.That (_clientTransaction.DataManager.DataContainerMap[_dataContainer2.ID], Is.Null);
            });

        _eventSinkMock
            .Expect (mock => mock.ObjectsLoaded (
                Arg<ReadOnlyCollection<DomainObject>>.Matches (list => 
                    list.Count == 2 
                    && list[0].ID == _dataContainer1.ID 
                    && list[1].ID == _dataContainer2.ID)))
            .WhenCalled (mi =>
            {
              Assert.That (_clientTransaction.DataManager.DataContainerMap[_dataContainer1.ID], Is.SameAs (_dataContainer1));
              Assert.That (_clientTransaction.DataManager.DataContainerMap[_dataContainer2.ID], Is.SameAs (_dataContainer2));
              
              var list = ((ReadOnlyCollection<DomainObject>) mi.Arguments[0]);
              Assert.That (((Order) list[0]).OnLoadedCalled, Is.True);
              Assert.That (((Order) list[1]).OnLoadedCalled, Is.True);
              
              Assert.That (transactionEventReceiver.LoadedDomainObjects, Is.Empty);
            });
      }

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 }, true);

      _mockRepository.VerifyAll ();
      Assert.That (transactionEventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
      Assert.That (transactionEventReceiver.LoadedDomainObjects[0], Is.EqualTo (new[] { result[0], result[1] }));
    }

    private void CheckLoadedObject (DomainObject loadedObject, DataContainer dataContainer)
    {
      Assert.That (loadedObject, Is.InstanceOfType (dataContainer.DomainObjectType));
      Assert.That (loadedObject.ID, Is.EqualTo (dataContainer.ID));
      Assert.That (_clientTransaction.IsEnlisted (loadedObject), Is.True);
      Assert.That (dataContainer.ClientTransaction, Is.SameAs (_clientTransaction));
      Assert.That (dataContainer.DomainObject, Is.SameAs (loadedObject));
    }

  }
}