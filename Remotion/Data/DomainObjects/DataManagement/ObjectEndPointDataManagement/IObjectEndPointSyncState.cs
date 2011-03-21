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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement
{
  /// <summary>
  /// Represents the synchronization state of an <see cref="IObjectEndPoint"/> with the opposite <see cref="IRelationEndPoint"/>, and implements 
  /// accessor methods for that end-point.
  /// </summary>
  public interface IObjectEndPointSyncState : IFlattenedSerializable
  {
    bool IsSynchronized (IObjectEndPoint endPoint);
    void Synchronize (IRealObjectEndPoint endPoint, IRelationEndPoint oppositeEndPoint);

    IDataManagementCommand CreateDeleteCommand (IObjectEndPoint endPoint, Action<ObjectID> oppositeObjectIDSetter);
    IDataManagementCommand CreateSetCommand (IObjectEndPoint endPoint, DomainObject newRelatedObject, Action<ObjectID> oppositeObjectIDSetter);
  }
}