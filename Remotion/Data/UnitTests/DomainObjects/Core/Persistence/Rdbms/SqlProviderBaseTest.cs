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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  public class SqlProviderBaseTest : ClientTransactionBaseTest
  {
    private SqlProvider _provider;
    private ReflectionBasedStorageNameProvider _storageNameProvider;
    private ValueConverter _valueConverter;
    private RdbmsProviderCommandFactory _commandFactory;

    public override void SetUp ()
    {
      base.SetUp();

      var storageTypeCalculator = new SqlStorageTypeCalculator();
      _storageNameProvider = new ReflectionBasedStorageNameProvider();
      _valueConverter = new ValueConverter (TestDomainStorageProviderDefinition, _storageNameProvider, TypeConversionProvider.Current);
      _commandFactory = new RdbmsProviderCommandFactory (
          new SqlDbCommandBuilderFactory (
              SqlDialect.Instance,
              _valueConverter),
          new RdbmsPersistenceModelProvider(),
          new InfrastructureStoragePropertyDefinitionProvider (storageTypeCalculator, _storageNameProvider));

      _provider = new SqlProvider (TestDomainStorageProviderDefinition, _storageNameProvider, NullPersistenceListener.Instance, _commandFactory);
    }

    public override void TearDown ()
    {
      _provider.Dispose();
      base.TearDown();
    }

    protected SqlProvider Provider
    {
      get { return _provider; }
    }

    protected ReflectionBasedStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    public ValueConverter ValueConverter
    {
      get { return _valueConverter; }
    }

    public RdbmsProviderCommandFactory CommandFactory
    {
      get { return _commandFactory; }
    }
  }
}