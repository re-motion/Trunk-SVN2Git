// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Data;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  public class TestableRdbmsProvider : RdbmsProvider
  {
    public TestableRdbmsProvider (
        RdbmsProviderDefinition definition,
        IStorageNameProvider storageNameProvider,
        ISqlDialect dialect,
        IPersistenceExtension persistenceExtension,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> commandFactory,
        Func<IDbConnection> connectionFactory)
      : base (definition, storageNameProvider, dialect, persistenceExtension, commandFactory, connectionFactory)
    {

    }
    
  }
}