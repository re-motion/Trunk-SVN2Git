// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Data;
using System.Data.SqlClient;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Implementation;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Persistence
{
  [ConcreteImplementation (typeof (SecurityManagerSqlStorageObjectFactory))]
  public class SecurityManagerSqlStorageObjectFactory : SqlStorageObjectFactory
  {
    protected override StorageProvider CreateStorageProvider (
        IPersistenceListener persistenceListener,
        RdbmsProviderDefinition rdbmsProviderDefinition,
        IStorageNameProvider storageNameProvider,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> commandFactory,
        IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);

      return
          ObjectFactory.Create<SecurityManagerRdbmsProvider> (
              ParamList.Create (
                  rdbmsProviderDefinition,
                  storageNameProvider,
                  persistenceListener,
                  commandFactory,
                  storageTypeInformationProvider,
                  (Func<IDbConnection>) (() => new SqlConnection())));
    }
  }
}