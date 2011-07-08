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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Defines an interface for classes instantiating <see cref="IStorageProviderCommand{T} "/> instances for the basic storage provider operations.
  /// <see cref="StorageProvider"/> uses this factory interface when the respective provider methods are called.
  /// </summary>
  public interface IStorageProviderCommandFactory<in TExecutionContext>
  {
    IStorageProviderCommand<DataContainer, TExecutionContext> CreateForSingleIDLookup (ObjectID objectID);
    // TODO 4113: Refactor to return IEnumerable<DataContainerLookupResult>
    IStorageProviderCommand<IEnumerable<DataContainer>, TExecutionContext> CreateForMultiIDLookup (IEnumerable<ObjectID> objectIDs);

    IStorageProviderCommand<IEnumerable<DataContainer>, TExecutionContext> CreateForRelationLookup (
        RelationEndPointDefinition foreignKeyEndPoint, ObjectID foreignKeyValue, SortExpressionDefinition sortExpressionDefinition);

    IStorageProviderCommand<IEnumerable<DataContainer>, TExecutionContext> CreateForDataContainerQuery (IQuery query);

    IStorageProviderCommand<TExecutionContext> CreateForSave (DataContainer[] dataContainers);
  }
}