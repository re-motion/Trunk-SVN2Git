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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public abstract class ObjectEndPoint : RelationEndPoint, IObjectEndPoint
  {
    private readonly IRelationEndPointLazyLoader _lazyLoader;
    private readonly IRelationEndPointProvider _endPointProvider;

    protected ObjectEndPoint (
        ClientTransaction clientTransaction, RelationEndPointID id, IRelationEndPointLazyLoader lazyLoader, IRelationEndPointProvider endPointProvider)
        : base (clientTransaction, id)
    {
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);

      _lazyLoader = lazyLoader;
      _endPointProvider = endPointProvider;
    }

    public IRelationEndPointLazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public abstract ObjectID OppositeObjectID { get; }
    public abstract ObjectID OriginalOppositeObjectID { get; }

    protected abstract void SetOppositeObjectIDValueFrom (IObjectEndPoint sourceObjectEndPoint);

    public abstract IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject);

    public DomainObject GetOppositeObject (bool includeDeleted)
    {
      if (OppositeObjectID == null)
        return null;
      else if (includeDeleted && ClientTransaction.IsInvalid (OppositeObjectID))
        return ClientTransaction.GetInvalidObjectReference (OppositeObjectID);
      else
        return ClientTransaction.GetObject (OppositeObjectID, includeDeleted);
    }

    public DomainObject GetOriginalOppositeObject ()
    {
      if (OriginalOppositeObjectID == null)
        return null;

      return ClientTransaction.GetObject (OriginalOppositeObjectID, true);
    }

    public override void CheckMandatory ()
    {
      if (OppositeObjectID == null)
      {
        throw CreateMandatoryRelationNotSetException (
            GetDomainObjectReference(),
            PropertyName,
            "Mandatory relation property '{0}' of domain object '{1}' cannot be null.",
            PropertyName,
            ObjectID);
      }
    }

    public override sealed IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      if (removedRelatedObject.ID != OppositeObjectID)
      {
        string removedID = removedRelatedObject.ID.ToString();
        string currentID = OppositeObjectID != null ? OppositeObjectID.ToString() : "<null>";

        var message = string.Format (
            "Cannot remove object '{0}' from object end point '{1}' - it currently holds object '{2}'.",
            removedID,
            PropertyName,
            currentID);
        throw new InvalidOperationException (message);
      }

      return CreateSetCommand (null);
    }

    public override sealed void SetValueFrom (IRelationEndPoint source)
    {
      var sourceObjectEndPoint = ArgumentUtility.CheckNotNullAndType<ObjectEndPoint> ("source", source);

      if (Definition != sourceObjectEndPoint.Definition)
      {
        var message = string.Format (
            "Cannot set this end point's value from '{0}'; the end points do not have the same end point definition.",
            source.ID);
        throw new ArgumentException (message, "source");
      }

      if (OppositeObjectID != sourceObjectEndPoint.OppositeObjectID)
        SetOppositeObjectIDValueFrom (sourceObjectEndPoint);

      if (sourceObjectEndPoint.HasBeenTouched || HasChanged)
        Touch();
    }
    
    public RelationEndPointID GetOppositeRelationEndPointID ()
    {
      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition();
      if (oppositeEndPointDefinition.IsAnonymous)
        return null;

      var oppositeEndPointID = RelationEndPointID.Create (OppositeObjectID, oppositeEndPointDefinition);
      return oppositeEndPointID;
    }

    public override IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
      var oppositeEndPointID = GetOppositeRelationEndPointID();

      if (oppositeEndPointID == null)
        return Enumerable.Empty<RelationEndPointID>();
      else
        return new[] { oppositeEndPointID };
    }

    #region Serialization

    protected ObjectEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _lazyLoader = info.GetValueForHandle<IRelationEndPointLazyLoader>();
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider>();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_lazyLoader);
      info.AddHandle (_endPointProvider);
    }

    #endregion
  }
}