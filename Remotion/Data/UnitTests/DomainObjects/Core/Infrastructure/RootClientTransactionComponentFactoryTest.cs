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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class RootClientTransactionComponentFactoryTest
  {
    private RootClientTransactionComponentFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = new RootClientTransactionComponentFactory ();
    }

    [Test]
    public void CreateInvalidDomainObjectManager ()
    {
      var manager = _factory.CreateInvalidDomainObjectManager ();
      Assert.That (manager, Is.TypeOf (typeof (RootInvalidDomainObjectManager)));
      Assert.That (((RootInvalidDomainObjectManager) manager).InvalidObjectCount, Is.EqualTo (0));
    }

    [Test]
    public void CreateDataManager_CollectionEndPointChangeDetectionStrategy ()
    {
      var invalidDomainObjectManagerStub = MockRepository.GenerateStub<IInvalidDomainObjectManager> ();
      var objectLoaderStub = MockRepository.GenerateStub<IObjectLoader> ();
      var dataManager = _factory.CreateDataManager (new ClientTransactionMock(), invalidDomainObjectManagerStub, objectLoaderStub);

      var dataKeeperFactory = ((RelationEndPointMap) dataManager.RelationEndPointMap).CollectionEndPointDataKeeperFactory;
      Assert.That (
          ((CollectionEndPointDataKeeperFactory) dataKeeperFactory).ChangeDetectionStrategy,
          Is.InstanceOf (typeof (RootCollectionEndPointChangeDetectionStrategy)));
      Assert.That (PrivateInvoke.GetNonPublicField (dataManager, "_invalidDomainObjectManager"), Is.SameAs (invalidDomainObjectManagerStub));
      Assert.That (PrivateInvoke.GetNonPublicField (dataManager, "_objectLoader"), Is.SameAs (objectLoaderStub));
    }

  }
}