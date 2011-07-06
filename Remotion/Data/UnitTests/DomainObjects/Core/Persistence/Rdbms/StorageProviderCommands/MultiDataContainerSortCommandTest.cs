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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class MultiDataContainerSortCommandTest : StandardMappingTest
  {
    private IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> _commandStub;
    private IRdbmsProviderCommandExecutionContext _executionContext;
    private DataContainer _order1Container;
    private DataContainer _order2Container;

    public override void SetUp ()
    {
      base.SetUp();

      _commandStub = MockRepository.GenerateStub<IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>>();
      _executionContext = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();

      _order1Container = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _order2Container = DataContainer.CreateNew (DomainObjectIDs.Order2);
    }

    [Test]
    public void Execute ()
    {
      var command = new MultiDataContainerSortCommand (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1 }, _commandStub);
      _commandStub.Stub (stub => stub.Execute (_executionContext)).Return (new[] { _order1Container, _order2Container });

      var result = command.Execute (_executionContext).ToList ();

      Assert.That (result, Is.EqualTo(new[] { _order1Container, _order2Container, null }));
    }

    [Test]
    public void Execute_DuplicatedObjectID ()
    {
      var command = new MultiDataContainerSortCommand (new[] { DomainObjectIDs.Order1 }, _commandStub);

      var order3Container = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _commandStub.Stub (stub => stub.Execute (_executionContext)).Return (new[] { _order1Container, order3Container });

      var result = command.Execute (_executionContext).ToList ();

      Assert.That (result, Is.EqualTo (new[] { order3Container })); //TODO 4078: correct??
    }

    [Test]
    public void Execute_DuplicatedDataContainer ()
    {
      var command = new MultiDataContainerSortCommand (new[] { DomainObjectIDs.Order1 }, _commandStub);

      _commandStub.Stub (stub => stub.Execute (_executionContext)).Return (new[] { _order1Container, _order1Container });

      var result = command.Execute (_executionContext).ToList ();

      Assert.That (result, Is.EqualTo (new[] { _order1Container }));
    }

    [Test]
    public void Execute_NullDataContainer ()
    {
      var command = new MultiDataContainerSortCommand (new[] { DomainObjectIDs.Order1 }, _commandStub);

      _commandStub.Stub (stub => stub.Execute (_executionContext)).Return (new[] { _order1Container, null });

      var result = command.Execute (_executionContext).ToList ();

      Assert.That (result, Is.EqualTo (new[] { _order1Container }));
    }

    [Test]
    public void Execute_NullObjectID ()
    {
      var command = new MultiDataContainerSortCommand (new[] { DomainObjectIDs.Order1, null }, _commandStub);

      _commandStub.Stub (stub => stub.Execute (_executionContext)).Return (new[] { _order1Container });

      var result = command.Execute (_executionContext).ToList ();

      Assert.That (result, Is.EqualTo (new[] { _order1Container, null }));
    }
  }
}