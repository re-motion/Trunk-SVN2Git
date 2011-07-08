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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;
using Remotion.Collections;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// Executes a given <see cref="IStorageProviderCommand{T,TExecutionContext}"/> and sorts the resulting <see cref="DataContainer"/> instances
  /// according to a given list of <see cref="ObjectID"/> values.
  /// </summary>
  // TODO 4113: Refactor to return IEnumerable<DataContainerLookupResult>
  public class MultiDataContainerSortCommand : IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>
  {
    private readonly ObjectID[] _objectIDs;
    private readonly IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> _command;

    public MultiDataContainerSortCommand (
        IEnumerable<ObjectID> objectIDs, 
        IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> command)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);
      ArgumentUtility.CheckNotNull ("command", command);

      _objectIDs = objectIDs.ToArray();
      _command = command;
    }

    public ObjectID[] ObjectIDs
    {
      get { return _objectIDs; }
    }

    public IStorageProviderCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext> Command
    {
      get { return _command; }
    }
    
    public IEnumerable<DataContainer> Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      var dataContainersByID = new Dictionary<ObjectID, DataContainer>();
      var dataContainers = _command.Execute (executionContext);
      foreach (var dataContainer in dataContainers.Where(dc=>dc!=null))
        dataContainersByID[dataContainer.ID] = dataContainer;
      
      return _objectIDs.Select (id => id!=null ? dataContainersByID.GetValueOrDefault (id) : null);
    }
  }
}