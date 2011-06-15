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
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  public class IndirectDataContainerLookupCommand : IStorageProviderCommand<IEnumerable<DataContainer>>
  {
    private readonly IStorageProviderCommand<IEnumerable<ObjectID>> _objectIDLoadCommand;
    private readonly IStorageProviderCommandFactory _storageProviderCommandFactory;

    public IndirectDataContainerLookupCommand (
        IStorageProviderCommand<IEnumerable<ObjectID>> objectIDLoadCommand,
        IStorageProviderCommandFactory storageProviderCommandFactory)
    {
      ArgumentUtility.CheckNotNull ("objectIDLoadCommand", objectIDLoadCommand);
      ArgumentUtility.CheckNotNull ("storageProviderCommandFactory", storageProviderCommandFactory);

      _objectIDLoadCommand = objectIDLoadCommand;
      _storageProviderCommandFactory = storageProviderCommandFactory;
    }

    public IStorageProviderCommand<IEnumerable<ObjectID>> ObjectIDLoadCommand
    {
      get { return _objectIDLoadCommand; }
    }

    public IStorageProviderCommandFactory StorageProviderCommandFactory
    {
      get { return _storageProviderCommandFactory; }
    }

    public IEnumerable<DataContainer> Execute ()
    {
      var objectIds = _objectIDLoadCommand.Execute();
      return _storageProviderCommandFactory.CreateForMultiIDLookup (objectIds.ToArray()).Execute();
    }
  }
}