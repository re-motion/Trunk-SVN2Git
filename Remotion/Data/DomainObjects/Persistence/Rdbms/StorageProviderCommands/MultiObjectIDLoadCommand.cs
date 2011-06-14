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
using System.Data;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  public class MultiObjectIDLoadCommand : IStorageProviderCommand<IEnumerable<ObjectID>>
  {
    private readonly IEnumerable<IDbCommandBuilder> _dbCommandBuilders;
    private readonly IDbCommandFactory _dbCommandFactory;
    private readonly IDbCommandExecutor _dbCommandExecutor;
    private readonly IObjectIDFactory _objectIDFactory;
    
    public MultiObjectIDLoadCommand (
        IEnumerable<IDbCommandBuilder> dbCommandBuilders,
        IDbCommandFactory dbCommandFactory,
        IDbCommandExecutor dbCommandExecutor,
        IObjectIDFactory objectIDFactory)
    {
      ArgumentUtility.CheckNotNull ("dbCommandBuilders", dbCommandBuilders);
      ArgumentUtility.CheckNotNull ("dbCommandFactory", dbCommandFactory);
      ArgumentUtility.CheckNotNull ("dbCommandExecutor", dbCommandExecutor);
      ArgumentUtility.CheckNotNull ("objectIDFactory", objectIDFactory);

      _dbCommandBuilders = dbCommandBuilders;
      _dbCommandFactory = dbCommandFactory;
      _dbCommandExecutor = dbCommandExecutor;
      _objectIDFactory = objectIDFactory;
    }

    public IEnumerable<IDbCommandBuilder> DbCommandBuilders
    {
      get { return _dbCommandBuilders; }
    }

    public IDbCommandFactory DbCommandFactory
    {
      get { return _dbCommandFactory; }
    }

    public IDbCommandExecutor DbCommandExecutor
    {
      get { return _dbCommandExecutor; }
    }

    public IObjectIDFactory ObjectIDFactory
    {
      get { return _objectIDFactory; }
    }

    public IEnumerable<ObjectID> Execute ()
    {
      return _dbCommandBuilders.SelectMany (LoadObjectIDsFromCommandBuilder);
    }

    private IEnumerable<ObjectID> LoadObjectIDsFromCommandBuilder (IDbCommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);

      using (var command = commandBuilder.Create (_dbCommandFactory))
      {
        using (var reader = _dbCommandExecutor.ExecuteReader (command, CommandBehavior.SingleResult))
        {
          return _objectIDFactory.CreateObjectIDCollection (reader);
        }
      }
    }
  }
}