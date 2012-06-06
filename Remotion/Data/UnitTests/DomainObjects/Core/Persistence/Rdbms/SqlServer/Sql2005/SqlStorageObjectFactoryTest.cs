// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.UnitTests.DomainObjects.Core.Linq;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance;
using Remotion.Development.UnitTesting;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Mixins;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.Sql2005
{
  [TestFixture]
  public class SqlStorageObjectFactoryTest
  {
    private SqlStorageObjectFactory _sqlProviderFactory;
    private RdbmsProviderDefinition _rdbmsProviderDefinition;
    private IPersistenceExtension _persistenceExtensionStub;
    private StorageGroupBasedStorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    [SetUp]
    public void SetUp ()
    {
      _rdbmsProviderDefinition = new RdbmsProviderDefinition ("TestDomain", new SqlStorageObjectFactory(), "ConnectionString");
      _sqlProviderFactory = new SqlStorageObjectFactory();
      _persistenceExtensionStub = MockRepository.GenerateStub<IPersistenceExtension>();
      _storageProviderDefinitionFinder = new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
    }

    [Test]
    public void CreateStorageProvider ()
    {
      var result = _sqlProviderFactory.CreateStorageProvider (_persistenceExtensionStub, _rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (RdbmsProvider)));
      Assert.That (result.PersistenceExtension, Is.SameAs (_persistenceExtensionStub));
      Assert.That (result.StorageProviderDefinition, Is.SameAs (_rdbmsProviderDefinition));

      var commandFactory = (RdbmsProviderCommandFactory) PrivateInvoke.GetNonPublicProperty (result, "StorageProviderCommandFactory");
      Assert.That (commandFactory.DbCommandBuilderFactory, Is.TypeOf (typeof (SqlDbCommandBuilderFactory)));
      Assert.That (commandFactory.RdbmsPersistenceModelProvider, Is.TypeOf (typeof (RdbmsPersistenceModelProvider)));
      Assert.That (commandFactory.ObjectReaderFactory, Is.TypeOf (typeof (ObjectReaderFactory)));
      Assert.That (commandFactory.DataStoragePropertyDefinitionFactory, Is.TypeOf (typeof (DataStoragePropertyDefinitionFactory)));
      var dataStoragePropertyDefinitionFactory = (DataStoragePropertyDefinitionFactory) commandFactory.DataStoragePropertyDefinitionFactory;
      var relationStoragePropertyDefinitionFactory =
          (RelationStoragePropertyDefinitionFactory) dataStoragePropertyDefinitionFactory.RelationStoragePropertyDefinitionFactory;
      Assert.That (relationStoragePropertyDefinitionFactory.ForceClassIDColumnInForeignKeyProperties, Is.False);
    }

    [Test]
    public void CreateStorageProviderWithMixin ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (RdbmsProvider)).Clear ().AddMixins (typeof (SqlProviderTestMixin)).EnterScope ())
      {
        var result = _sqlProviderFactory.CreateStorageProvider (_persistenceExtensionStub, _rdbmsProviderDefinition);

        Assert.That (Mixin.Get<SqlProviderTestMixin> (result), Is.Not.Null);
      }
    }

    [Test]
    public void CreatePersistenceModelLoader ()
    {
      var result = _sqlProviderFactory.CreatePersistenceModelLoader (_rdbmsProviderDefinition, _storageProviderDefinitionFinder);

      Assert.That (result, Is.TypeOf (typeof (RdbmsPersistenceModelLoader)));
      var rdbmsPersistenceModelLoader = (RdbmsPersistenceModelLoader) result;
      
      Assert.That (rdbmsPersistenceModelLoader.DataStoragePropertyDefinitionFactory, Is.TypeOf (typeof (DataStoragePropertyDefinitionFactory)));
      var dataStoragePropertyDefinitionFactory =
          (DataStoragePropertyDefinitionFactory) rdbmsPersistenceModelLoader.DataStoragePropertyDefinitionFactory;
      var relationStoragePropertyDefinitionFactory =
          (RelationStoragePropertyDefinitionFactory) dataStoragePropertyDefinitionFactory.RelationStoragePropertyDefinitionFactory;
      Assert.That (relationStoragePropertyDefinitionFactory.ForceClassIDColumnInForeignKeyProperties, Is.False);
      
      Assert.That (rdbmsPersistenceModelLoader.EntityDefinitionFactory, Is.TypeOf(typeof(RdbmsStorageEntityDefinitionFactory)));
      Assert.That (rdbmsPersistenceModelLoader.RdbmsPersistenceModelProvider, Is.TypeOf (typeof (RdbmsPersistenceModelProvider)));
      Assert.That (rdbmsPersistenceModelLoader.StorageNameProvider, Is.TypeOf (typeof (ReflectionBasedStorageNameProvider)));
    }

    [Test]
    public void CreateStorageNameProvider ()
    {
      var result = _sqlProviderFactory.CreateStorageNameProvider();

      Assert.That (result, Is.TypeOf (typeof (ReflectionBasedStorageNameProvider)));
    }

    [Test]
    public void CreateSchemaFileBuilder ()
    {
      var tableBuilderStub = MockRepository.GenerateStub<TableScriptBuilder> (
          MockRepository.GenerateStub<ITableScriptElementFactory>(), new SqlCommentScriptElementFactory());
      var viewBuilderStub = MockRepository.GenerateStub<ViewScriptBuilder> (
          MockRepository.GenerateStub<IViewScriptElementFactory<TableDefinition>>(),
          MockRepository.GenerateStub<IViewScriptElementFactory<UnionViewDefinition>>(),
          MockRepository.GenerateStub<IViewScriptElementFactory<FilterViewDefinition>>(),
          MockRepository.GenerateStub<IViewScriptElementFactory<EmptyViewDefinition>>(),
          new SqlCommentScriptElementFactory());
      var constraintBuilderStub =
          MockRepository.GenerateStub<ForeignKeyConstraintScriptBuilder> (
              MockRepository.GenerateStub<IForeignKeyConstraintScriptElementFactory>(), new SqlCommentScriptElementFactory());
      var indexScriptElementFactoryStub = MockRepository.GenerateStub<SqlIndexScriptElementFactory> (
          MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlIndexDefinition>>(),
          MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlPrimaryXmlIndexDefinition>>(),
          MockRepository.GenerateStub<ISqlIndexDefinitionScriptElementFactory<SqlSecondaryXmlIndexDefinition>>());
      var indexBuilderStub = MockRepository.GenerateStub<IndexScriptBuilder> (indexScriptElementFactoryStub, new SqlCommentScriptElementFactory());
      var synonymBuilderStub =
          MockRepository.GenerateStub<SynonymScriptBuilder> (
              MockRepository.GenerateStub<ISynonymScriptElementFactory<TableDefinition>>(),
              MockRepository.GenerateStub<ISynonymScriptElementFactory<UnionViewDefinition>>(),
              MockRepository.GenerateStub<ISynonymScriptElementFactory<FilterViewDefinition>>(),
              MockRepository.GenerateStub<ISynonymScriptElementFactory<EmptyViewDefinition>>(),
              new SqlCommentScriptElementFactory());

      var sqlProviderFactory = new TestableSqlStorageObjectFactory (
          tableBuilderStub, viewBuilderStub, constraintBuilderStub, indexBuilderStub, synonymBuilderStub);

      var result = sqlProviderFactory.CreateSchemaScriptBuilder (_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (SqlDatabaseSelectionScriptElementBuilder)));
      Assert.That (((SqlDatabaseSelectionScriptElementBuilder) result).InnerScriptBuilder, Is.TypeOf (typeof (CompositeScriptBuilder)));
      Assert.That (
          ((CompositeScriptBuilder) ((SqlDatabaseSelectionScriptElementBuilder) result).InnerScriptBuilder).RdbmsProviderDefinition,
          Is.SameAs (_rdbmsProviderDefinition));
      Assert.That (
          ((CompositeScriptBuilder) ((SqlDatabaseSelectionScriptElementBuilder) result).InnerScriptBuilder).ScriptBuilders,
          Is.EqualTo (
              new IScriptBuilder[]
              {
                  tableBuilderStub,
                  constraintBuilderStub,
                  viewBuilderStub,
                  indexBuilderStub,
                  synonymBuilderStub
              }));
    }

    [Test]
    public void CreateLinqQueryExecutor ()
    {
      var classDefintion = ClassDefinitionObjectMother.CreateClassDefinition (typeof (TIOrder), null);
      var methodCallTransformerProvider = MockRepository.GenerateStub<IMethodCallTransformerProvider>();

      var result = _sqlProviderFactory.CreateLinqQueryExecutor (
          classDefintion, methodCallTransformerProvider, ResultOperatorHandlerRegistry.CreateDefault());

      Assert.That (result, Is.TypeOf (typeof (DomainObjectQueryExecutor)));
    }

    [Test]
    public void CreateLinqQueryExecutor_DomainObjectQueryGeneratorCanBeMixed ()
    {
      var classDefintion = ClassDefinitionObjectMother.CreateClassDefinition (typeof (TIOrder), null);
      var methodCallTransformerProvider = MockRepository.GenerateStub<IMethodCallTransformerProvider>();

      using (MixinConfiguration.BuildNew ().ForClass<DomainObjectQueryGenerator> ().AddMixin<TestQueryGeneratorMixin> ().EnterScope ())
      {
        var executor = (DomainObjectQueryExecutor) _sqlProviderFactory.CreateLinqQueryExecutor (
            classDefintion, methodCallTransformerProvider, ResultOperatorHandlerRegistry.CreateDefault());
        var generator = executor.QueryGenerator;
        Assert.That (Mixin.Get<TestQueryGeneratorMixin> (generator), Is.Not.Null);
      }
    }

    [Test]
    public void CreateLinqQueryExecutor_WithMixedStages ()
    {
      var classDefintion = ClassDefinitionObjectMother.CreateClassDefinition (typeof (TIOrder), null);
      var methodCallTransformerProvider = MockRepository.GenerateStub<IMethodCallTransformerProvider>();

      using (MixinConfiguration.BuildNew()
          .ForClass<DefaultSqlPreparationStage>().AddMixin<TestSqlPreparationStageMixin>()
          .ForClass<DefaultMappingResolutionStage>().AddMixin<TestMappingResolutionStageMixin>()
          .ForClass<DefaultSqlGenerationStage>().AddMixin<TestSqlGenerationStageMixin>()
          .EnterScope())
      {
        var executor = (DomainObjectQueryExecutor) _sqlProviderFactory.CreateLinqQueryExecutor (
            classDefintion, methodCallTransformerProvider, ResultOperatorHandlerRegistry.CreateDefault());
        var domainObjectQueryGenerator = (DomainObjectQueryGenerator) executor.QueryGenerator;
        var sqlQueryGenerator = (SqlQueryGenerator) domainObjectQueryGenerator.SqlQueryGenerator;

        Assert.That (Mixin.Get<TestSqlPreparationStageMixin> (sqlQueryGenerator.PreparationStage), Is.Not.Null);
        Assert.That (Mixin.Get<TestMappingResolutionStageMixin> (sqlQueryGenerator.ResolutionStage), Is.Not.Null);
        Assert.That (Mixin.Get<TestSqlGenerationStageMixin> (sqlQueryGenerator.GenerationStage), Is.Not.Null);
      }
    }
  }
}