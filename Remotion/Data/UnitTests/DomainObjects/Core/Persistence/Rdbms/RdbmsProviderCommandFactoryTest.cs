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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.DataReaders;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class RdbmsProviderCommandFactoryTest : StandardMappingTest
  {
    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStub;
    private IDataContainerReader _dataContainerReaderStub;
    private IObjectIDReader _objectIDReaderStub;

    private RdbmsProviderCommandFactory _factory;

    private TableDefinition _entityDefinition;
    private IDbCommandBuilder _dbCommandBuilderStub;
    private ObjectID _objectID;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilderFactoryStub = MockRepository.GenerateStub<IDbCommandBuilderFactory>();
      _dataContainerReaderStub = MockRepository.GenerateStub<IDataContainerReader> ();
      _objectIDReaderStub = MockRepository.GenerateStub<IObjectIDReader>();

      _factory = new RdbmsProviderCommandFactory (_dbCommandBuilderFactoryStub, _dataContainerReaderStub, _objectIDReaderStub);

      _entityDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));
      _dbCommandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder> ();
      _objectID = CreateObjectID (_entityDefinition);
    }

    [Test]
    public void CreateForSingleIDLookup ()
    {
      _dbCommandBuilderFactoryStub
          .Stub (stub => stub.CreateForSingleIDLookupFromTable (_entityDefinition, AllSelectedColumnsSpecification.Instance, _objectID))
          .Return (_dbCommandBuilderStub);

      var result = _factory.CreateForSingleIDLookup (_objectID);

      Assert.That (result, Is.TypeOf (typeof (SingleDataContainerLoadCommand)));
    }

    [Test]
    public void CreateForMultiIDLookup ()
    {
      _dbCommandBuilderFactoryStub
          .Stub (stub => stub.CreateForMultiIDLookupFromTable (_entityDefinition, AllSelectedColumnsSpecification.Instance, new[] { _objectID }))
          .Return (_dbCommandBuilderStub);

      var result = _factory.CreateForMultiIDLookup (new[] { _objectID });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSortCommand)));
    }

    [Test]
    public void CreateForRelationLookup ()
    {
      var classDefinition = CreateClassDefinition (_entityDefinition);
      var idPropertyDefinition = CreateForeignLeyEndPointDefinition (classDefinition);
      var relationEndPointDefinition = new RelationEndPointDefinition (idPropertyDefinition, false);

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _objectID, null);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
    }

    [Test]
    public void CreateForQuery ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery> ();
      var commandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder>();

      _dbCommandBuilderFactoryStub.Stub (stub => stub.CreateForQuery (queryStub)).Return (commandBuilderStub);

      var result = _factory.CreateForDataContainerQuery (queryStub);

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerLoadCommand)));
      Assert.That (((MultiDataContainerLoadCommand) result).DbCommandBuilders, Is.EqualTo(new[]{commandBuilderStub}));
      Assert.That (((MultiDataContainerLoadCommand) result).DataContainerReader, Is.SameAs(_dataContainerReaderStub));
      Assert.That (((MultiDataContainerLoadCommand) result).AllowNulls, Is.True);
    }

    private ObjectID CreateObjectID (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetStorageEntity (entityDefinition);

      return new ObjectID (classDefinition, Guid.NewGuid());
    }

    private ClassDefinition CreateClassDefinition (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetStorageEntity (entityDefinition);
      return classDefinition;
    }

    private PropertyDefinition CreateForeignLeyEndPointDefinition (ClassDefinition classDefinition)
    {
      return PropertyDefinitionFactory.Create (
          classDefinition,
          StorageClass.Persistent,
          typeof (Order).GetProperty ("OrderTicket"),
          new IDColumnDefinition (ColumnDefinitionObjectMother.ObjectIDColumn, ColumnDefinitionObjectMother.ClassIDColumn));
    }
  }
}