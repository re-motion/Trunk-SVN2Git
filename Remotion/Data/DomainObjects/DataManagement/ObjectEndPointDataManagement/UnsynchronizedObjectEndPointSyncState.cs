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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement
{
  [Serializable]
  public class UnsynchronizedObjectEndPointSyncState : IObjectEndPointSyncState
  {
    [NonSerialized]
    private readonly IObjectEndPoint _endPoint;

    public UnsynchronizedObjectEndPointSyncState (IObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      _endPoint = endPoint;
    }

    public IObjectEndPoint EndPoint
    {
      get { return _endPoint; }
    }

    public IDataManagementCommand CreateDeleteCommand ()
    {
      throw CreateInvalidOperationException();
    }

    public IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      throw CreateInvalidOperationException ();
    }

    private InvalidOperationException CreateInvalidOperationException ()
    {
      return new InvalidOperationException (
          string.Format (
              "The relation property '{0}' of object '{1}' cannot be changed because it is out of sync with the opposite property '{2}'. "
              + "To make this change, synchronize the two properties by calling the 'ClientTransactionSyncService.SynchronizeRelation' method.",
              _endPoint.Definition.PropertyName,
              _endPoint.ObjectID,
              _endPoint.Definition.GetOppositeEndPointDefinition ().PropertyName));
    }
  }
}