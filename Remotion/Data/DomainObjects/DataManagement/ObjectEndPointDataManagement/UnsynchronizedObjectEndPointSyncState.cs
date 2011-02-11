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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement
{
  public class UnsynchronizedObjectEndPointSyncState : IObjectEndPointSyncState
  {
    public UnsynchronizedObjectEndPointSyncState ()
    {
    }

    public IDataManagementCommand CreateDeleteCommand (IObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      throw CreateInvalidOperationException(endPoint);
    }

    public IDataManagementCommand CreateSetCommand (IObjectEndPoint endPoint, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      throw CreateInvalidOperationException (endPoint);
    }

    private InvalidOperationException CreateInvalidOperationException (IObjectEndPoint endPoint)
    {
      return new InvalidOperationException (
          string.Format (
              "The relation property '{0}' of object '{1}' cannot be changed because it is out of sync with the opposite property '{2}'. "
              + "To make this change, synchronize the two properties by calling the 'ClientTransactionSyncService.SynchronizeRelation' method.",
              endPoint.Definition.PropertyName,
              endPoint.ObjectID,
              endPoint.Definition.GetOppositeEndPointDefinition ().PropertyName));
    }

    #region Serialization

    public UnsynchronizedObjectEndPointSyncState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
    }

    #endregion Serialization
  }
}