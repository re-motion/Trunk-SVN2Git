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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class IndirectDataContainerLookupCommandTest : SqlProviderBaseTest
  {
    private IStorageProviderCommand<IEnumerable<ObjectID>, IRdbmsProviderCommandExecutionContext> _objectIDLoadCommandStub;
    private IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _storageProviderFactoryStub;
    private IndirectDataContainerLookupCommand _lookupCommand;
    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private IStorageProviderCommand<DataContainer[], IRdbmsProviderCommandExecutionContext> _fakeStorageProviderCommandStub;
    private DataContainer[] _fakeResult;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp();

      _fakeResult = new DataContainer[0];
      _objectID1 = new ObjectID ("Order", Guid.NewGuid());
      _objectID2 = new ObjectID ("OrderItem", Guid.NewGuid());
      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext> ();
      
      _fakeStorageProviderCommandStub = MockRepository.GenerateStub<IStorageProviderCommand<DataContainer[], IRdbmsProviderCommandExecutionContext>>();
      _fakeStorageProviderCommandStub.Stub (stub => stub.Execute(_commandExecutionContextStub)).Return (_fakeResult);

      _objectIDLoadCommandStub = MockRepository.GenerateStub<IStorageProviderCommand<IEnumerable<ObjectID>, IRdbmsProviderCommandExecutionContext>>();
      _objectIDLoadCommandStub.Stub (stub => stub.Execute(_commandExecutionContextStub)).Return (new[] { _objectID1, _objectID2 });

      _storageProviderFactoryStub = MockRepository.GenerateStub<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>();
      _storageProviderFactoryStub.Stub (stub => stub.CreateForMultiIDLookup (Arg<ObjectID[]>.List.Equal (new[] { _objectID1, _objectID2 }))).Return (
          _fakeStorageProviderCommandStub);

      _lookupCommand = new IndirectDataContainerLookupCommand (_objectIDLoadCommandStub, _storageProviderFactoryStub);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_lookupCommand.ObjectIDLoadCommand, Is.SameAs (_objectIDLoadCommandStub));
      Assert.That (_lookupCommand.StorageProviderCommandFactory, Is.SameAs (_storageProviderFactoryStub));
    }

    [Test]
    public void Execute ()
    {
      var result = _lookupCommand.Execute(_commandExecutionContextStub);

      Assert.That (result, Is.SameAs (_fakeResult));
    }
  }
}