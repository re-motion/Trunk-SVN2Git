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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class ObjectReaderFactoryTest : SqlProviderBaseTest
  {
    private IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProviderStub;
    private ObjectReaderFactory _factory;
    private IEntityDefinition _entityDefinitionStub;
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private ColumnDefinition _idColumn;
    private ColumnDefinition _classIdColumn;
    private ColumnDefinition _timestampColumn;
    private InfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _rdbmsPersistenceModelProviderStub = MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>();
      _entityDefinitionStub = MockRepository.GenerateStub<IEntityDefinition>();
      _idColumn = ColumnDefinitionObjectMother.IDColumn;
      _classIdColumn = ColumnDefinitionObjectMother.ClassIDColumn;
      _timestampColumn = ColumnDefinitionObjectMother.TimestampColumn;
      _entityDefinitionStub.Stub (stub => stub.IDColumn).Return (_idColumn);
      _entityDefinitionStub.Stub (stub => stub.ClassIDColumn).Return (_classIdColumn);
      _entityDefinitionStub.Stub (stub => stub.TimestampColumn).Return (_timestampColumn);

      _column1 = ColumnDefinitionObjectMother.CreateColumn ("Column1");
      _column2 = ColumnDefinitionObjectMother.CreateColumn ("Column2");

      _infrastructureStoragePropertyDefinitionProvider = new InfrastructureStoragePropertyDefinitionProvider (
          StorageTypeInformationProvider, StorageNameProvider);
      _factory = new ObjectReaderFactory (
          _rdbmsPersistenceModelProviderStub,
          _infrastructureStoragePropertyDefinitionProvider);
    }

    [Test]
    public void CreateDataContainerReader_OverloadWithNoParameters ()
    {
      var result = _factory.CreateDataContainerReader();

      Assert.That (result, Is.TypeOf (typeof (DataContainerReader)));
      var dataContainerReader = (DataContainerReader) result;
      Assert.That (dataContainerReader.OrdinalProvider, Is.TypeOf (typeof (NameBasedColumnOrdinalProvider)));
      Assert.That (
          ((SimpleStoragePropertyDefinition) dataContainerReader.TimestampProperty).ColumnDefinition,
          Is.SameAs (_infrastructureStoragePropertyDefinitionProvider.GetTimestampColumnDefinition()));
      Assert.That (
          ObjectIDStoragePropertyDefinitionTestHelper.GetIDColumnDefinition (((ObjectIDStoragePropertyDefinition) dataContainerReader.IDProperty)),
           Is.SameAs (_infrastructureStoragePropertyDefinitionProvider.GetIDColumnDefinition ()));
      Assert.That (
          ObjectIDStoragePropertyDefinitionTestHelper.GetClassIDColumnDefinition (((ObjectIDStoragePropertyDefinition) dataContainerReader.IDProperty)),
           Is.SameAs (_infrastructureStoragePropertyDefinitionProvider.GetClassIDColumnDefinition ()));
    }

    [Test]
    public void CreateDataContainerReader ()
    {
      var result = _factory.CreateDataContainerReader (_entityDefinitionStub, new[] { _column1, _column2 });

      Assert.That (result, Is.TypeOf (typeof (DataContainerReader)));
      var dataContainerReader = (DataContainerReader) result;
      CheckOrdinalProvider (dataContainerReader.OrdinalProvider);
      CheckObjectIDStoragePropertyDefinition (dataContainerReader.IDProperty);
      Assert.That (((SimpleStoragePropertyDefinition) dataContainerReader.TimestampProperty).ColumnDefinition, Is.SameAs (_timestampColumn));
    }

    [Test]
    public void CreateObjectIDReader ()
    {
      var result = _factory.CreateObjectIDReader (_entityDefinitionStub, new[] { _column1, _column2 });

      Assert.That (result, Is.TypeOf (typeof (ObjectIDReader)));
      var objectIDReader = (ObjectIDReader) result;
      CheckOrdinalProvider (objectIDReader.ColumnOrdinalProvider);
      CheckObjectIDStoragePropertyDefinition (objectIDReader.IDProperty);
    }

    [Test]
    public void CreateTimestampReader ()
    {
      var result = _factory.CreateTimestampReader (_entityDefinitionStub, new[] { _column1, _column2 });

      Assert.That (result, Is.TypeOf (typeof (TimestampReader)));
      var timestampReader = (TimestampReader) result;
      CheckOrdinalProvider (timestampReader.ColumnOrdinalProvider);
      CheckObjectIDStoragePropertyDefinition (timestampReader.IDProperty);
      Assert.That (((SimpleStoragePropertyDefinition) timestampReader.TimestampProperty).ColumnDefinition, Is.SameAs (_timestampColumn));
    }

    private void CheckOrdinalProvider (IColumnOrdinalProvider ordinalProvider)
    {
      Assert.That (ordinalProvider, Is.TypeOf (typeof (DictionaryBasedColumnOrdinalProvider)));
      Assert.That (((DictionaryBasedColumnOrdinalProvider) ordinalProvider).Ordinals.Count, Is.EqualTo (2));
      Assert.That (((DictionaryBasedColumnOrdinalProvider) ordinalProvider).Ordinals[_column1], Is.EqualTo (0));
      Assert.That (((DictionaryBasedColumnOrdinalProvider) ordinalProvider).Ordinals[_column2], Is.EqualTo (1));
    }

    private void CheckObjectIDStoragePropertyDefinition (IRdbmsStoragePropertyDefinition storagePropertyDefinition)
    {
      Assert.That (storagePropertyDefinition, Is.TypeOf (typeof (ObjectIDStoragePropertyDefinition)));
      Assert.That (
          ((ObjectIDStoragePropertyDefinition) storagePropertyDefinition).ValueProperty, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
      Assert.That (
          ObjectIDStoragePropertyDefinitionTestHelper.GetIDColumnDefinition (((ObjectIDStoragePropertyDefinition) storagePropertyDefinition)),
          Is.SameAs (_idColumn));
      Assert.That (
          ObjectIDStoragePropertyDefinitionTestHelper.GetClassIDColumnDefinition (((ObjectIDStoragePropertyDefinition) storagePropertyDefinition)),
          Is.SameAs (_classIdColumn));
    }
  }
}