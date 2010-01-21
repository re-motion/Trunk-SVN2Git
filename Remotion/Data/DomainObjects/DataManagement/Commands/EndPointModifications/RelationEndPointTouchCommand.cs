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

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents a command that touches, but does not change the modified end point.
  /// </summary>
  public class RelationEndPointTouchCommand : IDataManagementCommand
  {
    private readonly IEndPoint _endPoint;

    public RelationEndPointTouchCommand (IEndPoint endPoint)
    {
      _endPoint = endPoint;
    }

    public IEndPoint EndPoint
    {
      get { return _endPoint; }
    }

    public void NotifyClientTransactionOfBegin ()
    {
      // do not issue any notifications
    }

    public void Begin ()
    {
      // do not issue any notifications
    }

    public void Perform ()
    {
      _endPoint.Touch ();
    }

    public void End ()
    {
      // do not issue any notifications
    }

    public void NotifyClientTransactionOfEnd ()
    {
      // do not issue any notifications
    }

    public IDataManagementCommand ExtendToAllRelatedObjects ()
    {
      return this;
    }
  }
}