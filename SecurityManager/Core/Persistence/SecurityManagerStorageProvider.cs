// This file is part of re-strict (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Data;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.SecurityManager.Persistence
{
  public class SecurityManagerRdbmsProvider : RdbmsProvider
  {
    // constants

    // types

    // static members

    // member fields

    private readonly RevisionStorageProviderExtension _revisionExtension;

    // construction and disposing

    public SecurityManagerRdbmsProvider (
        RdbmsProviderDefinition definition,
        IStorageNameProvider storageNameProvider,
        IPersistenceExtension persistenceExtension,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> commandFactory,
        Func<IDbConnection> connectionFactory)
        : base (
            definition,
            storageNameProvider,
            Data.DomainObjects.Persistence.Rdbms.SqlServer.SqlDialect.Instance,
            persistenceExtension,
            commandFactory,
            connectionFactory)
    {
      _revisionExtension = new RevisionStorageProviderExtension();
    }

    // methods and properties

    public override void Save (IEnumerable<DataContainer> dataContainers)
    {
      _revisionExtension.Saving (Connection.WrappedInstance, Transaction.WrappedInstance, dataContainers);
      base.Save (dataContainers);
    }
  }
}