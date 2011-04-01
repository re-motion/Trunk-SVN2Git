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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RealObjectEndPoints
{
  /// <summary>
  /// Represents the state of an <see cref="IObjectEndPoint"/> that is synchronized with the opposite <see cref="IRelationEndPoint"/>.
  /// </summary>
  public class SynchronizedRealObjectEndPointSyncState : IRealObjectEndPointSyncState
  {
    private readonly IRelationEndPointProvider _endPointProvider;

    public SynchronizedRealObjectEndPointSyncState(IRelationEndPointProvider endPointProvider)
    {
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      _endPointProvider = endPointProvider;
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public bool IsSynchronized (IRealObjectEndPoint endPoint)
    {
      return true;
    }

    public void Synchronize (IRealObjectEndPoint endPoint, IVirtualEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      // nothing to do here - the end-point is already syncrhonized
    }

    public IDataManagementCommand CreateDeleteCommand (IRealObjectEndPoint endPoint, Action<ObjectID> oppositeObjectIDSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeObjectIDSetter", oppositeObjectIDSetter);

      var oppositeEndPointDefinition = endPoint.Definition.GetOppositeEndPointDefinition ();

      var objectEndPointDeleteCommand = new ObjectEndPointDeleteCommand (endPoint, oppositeObjectIDSetter);

      if (!oppositeEndPointDefinition.IsAnonymous && oppositeEndPointDefinition.IsVirtual)
      {
        var oldRelatedEndPoint = _endPointProvider.GetOppositeVirtualEndPointWithLazyLoad (endPoint, endPoint.OppositeObjectID);
        var newRelatedEndPoint = _endPointProvider.GetOppositeVirtualEndPointWithLazyLoad (endPoint, null);
        return new RealObjectEndPointRegistrationCommandDecorator (objectEndPointDeleteCommand, endPoint, oldRelatedEndPoint, newRelatedEndPoint);
      }
      else
      {
        return objectEndPointDeleteCommand;
      }
    }

    public IDataManagementCommand CreateSetCommand (IRealObjectEndPoint endPoint, DomainObject newRelatedObject, Action<ObjectID> oppositeObjectIDSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeObjectIDSetter", oppositeObjectIDSetter);

      var oppositeEndPointDefinition = endPoint.Definition.GetOppositeEndPointDefinition ();

      var newRelatedObjectID = newRelatedObject != null ? newRelatedObject.ID : null;
      if (endPoint.OppositeObjectID == newRelatedObjectID)
        return new ObjectEndPointSetSameCommand (endPoint, oppositeObjectIDSetter);
      else if (oppositeEndPointDefinition.IsAnonymous)
        return new ObjectEndPointSetUnidirectionalCommand (endPoint, newRelatedObject, oppositeObjectIDSetter);
      else
      {
        var setCommand = oppositeEndPointDefinition.Cardinality == CardinalityType.One
                             ? (IDataManagementCommand) new ObjectEndPointSetOneOneCommand (endPoint, newRelatedObject, oppositeObjectIDSetter)
                             : new ObjectEndPointSetOneManyCommand (endPoint, newRelatedObject, oppositeObjectIDSetter, _endPointProvider);
        var oldRelatedEndPoint = _endPointProvider.GetOppositeVirtualEndPointWithLazyLoad (endPoint, endPoint.OppositeObjectID);
        var newRelatedEndPoint = _endPointProvider.GetOppositeVirtualEndPointWithLazyLoad (endPoint, newRelatedObjectID);
        return new RealObjectEndPointRegistrationCommandDecorator (setCommand, endPoint, oldRelatedEndPoint, newRelatedEndPoint);
      }
    }

    #region Serialization

    public SynchronizedRealObjectEndPointSyncState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider>();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (_endPointProvider);
    }

    #endregion
  }
}