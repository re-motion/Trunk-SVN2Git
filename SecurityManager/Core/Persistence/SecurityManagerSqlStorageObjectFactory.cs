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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
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
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> commandFactory)
    {
      ArgumentUtility.CheckNotNull ("persistenceListener", persistenceListener);
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("commandFactory", commandFactory);

      return ObjectFactory.Create<SecurityManagerSqlProvider> (ParamList.Create (rdbmsProviderDefinition, storageNameProvider, persistenceListener, commandFactory));
    }
  }
}