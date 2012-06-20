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
using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Decorates a <see cref="IDataManagementCommand"/>, calling the <see cref="IVirtualEndPointStateUpdateListener.VirtualEndPointStateUpdated"/> notification method 
  /// immediately after the decorated <see cref="IDataManagementCommand"/>'s <see cref="IDataManagementCommand.Perform"/> method is called. This is
  /// used by <see cref="StateUpdateRaisingCollectionEndPointDecorator"/> to ensure <see cref="IVirtualEndPointStateUpdateListener"/> implementations
  /// are informed when a command changes the state of a <see cref="ICollectionEndPoint"/>.
  /// </summary>
  public class VirtualEndPointStateUpdatedRaisingCommandDecorator : IDataManagementCommand
  {
    private readonly RelationEndPointID _modifiedEndPointID;
    private readonly IDataManagementCommand _decoratedCommand;
    private readonly IVirtualEndPointStateUpdateListener _listener;
    private readonly Func<bool?> _changeStateProvider;

    public VirtualEndPointStateUpdatedRaisingCommandDecorator (
        IDataManagementCommand decoratedCommand,
        RelationEndPointID modifiedEndPointID,
        IVirtualEndPointStateUpdateListener listener,
        Func<bool?> changeStateProvider)
    {
      ArgumentUtility.CheckNotNull ("decoratedCommand", decoratedCommand);
      ArgumentUtility.CheckNotNull ("modifiedEndPointID", modifiedEndPointID);
      ArgumentUtility.CheckNotNull ("listener", listener);
      ArgumentUtility.CheckNotNull ("changeStateProvider", changeStateProvider);

      _decoratedCommand = decoratedCommand;
      _modifiedEndPointID = modifiedEndPointID;
      _listener = listener;
      _changeStateProvider = changeStateProvider;
    }

    public IDataManagementCommand DecoratedCommand
    {
      get { return _decoratedCommand; }
    }

    public RelationEndPointID ModifiedEndPointID
    {
      get { return _modifiedEndPointID; }
    }

    public IVirtualEndPointStateUpdateListener Listener
    {
      get { return _listener; }
    }

    public Func<bool?> ChangeStateProvider
    {
      get { return _changeStateProvider; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return _decoratedCommand.GetAllExceptions();
    }

    public void Begin ()
    {
      _decoratedCommand.Begin();
    }

    public void Perform ()
    {
      try
      {
        _decoratedCommand.Perform();
      }
      finally
      {
        _listener.VirtualEndPointStateUpdated (_modifiedEndPointID, _changeStateProvider ());
      }
    }

    public void End ()
    {
      _decoratedCommand.End();
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      var expandedCommand = _decoratedCommand.ExpandToAllRelatedObjects();
      return new ExpandedCommand (new VirtualEndPointStateUpdatedRaisingCommandDecorator (expandedCommand, _modifiedEndPointID, _listener, _changeStateProvider));
    }
  }
}