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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Linq.SqlBackend.SqlPreparation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.Sql2005
{
  public class TestableSqlStorageObjectFactory : SqlStorageObjectFactory
  {
    private readonly TableScriptBuilder _tableBuilder;
    private readonly ViewScriptBuilder _viewBuilder;
    private readonly ForeignKeyConstraintScriptBuilder _constraintBuilder;
    private readonly IndexScriptBuilder _indexBuilder;
    private readonly SynonymScriptBuilder _synonymBuilder;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IStorageNameProvider _storageNameProvider;
    private readonly IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;
    private readonly IStorageProviderDefinitionFinder _storageProviderDefinitionFinder;
    private readonly IDataStoragePropertyDefinitionFactory _dataStoragePropertyDefinitionFactory;
    private readonly IValueStoragePropertyDefinitionFactory _valueStoragePropertyDefinitionFactory;
    private readonly IRelationStoragePropertyDefinitionFactory _relationStoragePropertyDefinitionFactory;
    private readonly ISqlQueryGenerator _sqlQueryGenerator;
    private readonly IForeignKeyConstraintDefinitionFactory _foreignKeyConstraintDefinitionFactory;
    private readonly IStoragePropertyDefinitionResolver _storagePropertyDefinitionResolver;

    public TestableSqlStorageObjectFactory (
        TableScriptBuilder tableBuilder,
        ViewScriptBuilder viewBuilder,
        ForeignKeyConstraintScriptBuilder constraintBuilder,
        IndexScriptBuilder indexBuilder,
        SynonymScriptBuilder synonymBuilder)
    {
      _indexBuilder = indexBuilder;
      _constraintBuilder = constraintBuilder;
      _viewBuilder = viewBuilder;
      _tableBuilder = tableBuilder;
      _synonymBuilder = synonymBuilder;
    }

    public TestableSqlStorageObjectFactory (IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider, IStorageTypeInformationProvider storageTypeInformationProvider, IDbCommandBuilderFactory dbCommandBuilderFactory, IStorageNameProvider storageNameProvider, IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider, IStorageProviderDefinitionFinder storageProviderDefinitionFinder, IDataStoragePropertyDefinitionFactory dataStoragePropertyDefinitionFactory, IValueStoragePropertyDefinitionFactory valueStoragePropertyDefinitionFactory, IRelationStoragePropertyDefinitionFactory relationStoragePropertyDefinitionFactory, ISqlQueryGenerator sqlQueryGenerator, IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactoryFactory, IStoragePropertyDefinitionResolver storagePropertyDefinitionResolver)
    {
      _infrastructureStoragePropertyDefinitionProvider = infrastructureStoragePropertyDefinitionProvider;
      _storageNameProvider = storageNameProvider;
      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _storageTypeInformationProvider = storageTypeInformationProvider;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
      _storageProviderDefinitionFinder = storageProviderDefinitionFinder;
      _dataStoragePropertyDefinitionFactory = dataStoragePropertyDefinitionFactory;
      _valueStoragePropertyDefinitionFactory = valueStoragePropertyDefinitionFactory;
      _relationStoragePropertyDefinitionFactory = relationStoragePropertyDefinitionFactory;
      _sqlQueryGenerator = sqlQueryGenerator;
      _foreignKeyConstraintDefinitionFactory = foreignKeyConstraintDefinitionFactoryFactory;
      _storagePropertyDefinitionResolver = storagePropertyDefinitionResolver;
    }

    public override TableScriptBuilder CreateTableBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _tableBuilder ?? base.CreateTableBuilder(storageProviderDefinition);
    }

    public override ViewScriptBuilder CreateViewBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _viewBuilder ?? base.CreateViewBuilder(storageProviderDefinition);
    }

    public override ForeignKeyConstraintScriptBuilder CreateConstraintBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _constraintBuilder ?? base.CreateConstraintBuilder(storageProviderDefinition);
    }

    public override IndexScriptBuilder CreateIndexBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _indexBuilder ?? base.CreateIndexBuilder(storageProviderDefinition);
    }

    public override SynonymScriptBuilder CreateSynonymBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _synonymBuilder ?? base.CreateSynonymBuilder(storageProviderDefinition);
    }

    public override IStorageNameProvider CreateStorageNameProvider (RdbmsProviderDefinition storageProviderDefiniton)
    {
      return _storageNameProvider ?? base.CreateStorageNameProvider(storageProviderDefiniton);
    }

    public override IInfrastructureStoragePropertyDefinitionProvider CreateInfrastructureStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _infrastructureStoragePropertyDefinitionProvider ?? base.CreateInfrastructureStoragePropertyDefinitionFactory(storageProviderDefinition);
    }

    public override IDbCommandBuilderFactory CreateDbCommandBuilderFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _dbCommandBuilderFactory ?? base.CreateDbCommandBuilderFactory(storageProviderDefinition);
    }

    public override IStorageTypeInformationProvider CreateStorageTypeInformationProvider (RdbmsProviderDefinition rdmsStorageProviderDefinition)
    {
      return _storageTypeInformationProvider ?? base.CreateStorageTypeInformationProvider(rdmsStorageProviderDefinition);
    }

    public override IRdbmsPersistenceModelProvider CreateRdbmsPersistenceModelProvider (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _rdbmsPersistenceModelProvider ?? base.CreateRdbmsPersistenceModelProvider(storageProviderDefinition);
    }

    public override IStorageProviderDefinitionFinder CreateStorageProviderDefinitionFinder (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _storageProviderDefinitionFinder ?? base.CreateStorageProviderDefinitionFinder(storageProviderDefinition);
    }

    public override IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition, IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      return _dataStoragePropertyDefinitionFactory
             ?? base.CreateDataStoragePropertyDefinitionFactory (storageProviderDefinition, storageProviderDefinitionFinder);
    }

    public override IValueStoragePropertyDefinitionFactory CreateValueStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _valueStoragePropertyDefinitionFactory ?? base.CreateValueStoragePropertyDefinitionFactory(storageProviderDefinition);
    }

    public override IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (RdbmsProviderDefinition storageProviderDefinition, IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      return _relationStoragePropertyDefinitionFactory ?? base.CreateRelationStoragePropertyDefinitionFactory(storageProviderDefinition, storageProviderDefinitionFinder);
    }

    public override ISqlQueryGenerator CreateSqlQueryGenerator (RdbmsProviderDefinition storageProviderDefinition, IMethodCallTransformerProvider methodCallTransformerProvider, ResultOperatorHandlerRegistry resultOperatorHandlerRegistry)
    {
      return _sqlQueryGenerator ?? base.CreateSqlQueryGenerator (storageProviderDefinition, methodCallTransformerProvider, resultOperatorHandlerRegistry);
    }

    public override IForeignKeyConstraintDefinitionFactory CreateForeignKeyConstraintDefinitionsFactory (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _foreignKeyConstraintDefinitionFactory ?? base.CreateForeignKeyConstraintDefinitionsFactory(storageProviderDefinition);
    }

    public override IStoragePropertyDefinitionResolver CreateStoragePropertyDefinitionResolver (RdbmsProviderDefinition storageProviderDefinition)
    {
      return _storagePropertyDefinitionResolver ?? base.CreateStoragePropertyDefinitionResolver (storageProviderDefinition);
    }
  }
}