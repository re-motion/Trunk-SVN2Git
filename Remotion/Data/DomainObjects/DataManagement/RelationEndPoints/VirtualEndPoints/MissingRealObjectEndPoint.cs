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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints
{
  /// <summary>
  /// Implements <see cref="IRealObjectEndPoint"/> for a <see cref="DomainObject"/> stored in an <see cref="IVirtualEndPoint"/> without associated
  /// opposite <see cref="IRealObjectEndPoint"/> (either because the other side hasn't been loaded yet or because it's out-of-sync). 
  /// This acts as a placeholder for that missing <see cref="IRealObjectEndPoint"/>.
  /// </summary>
  public class MissingRealObjectEndPoint : IRealObjectEndPoint
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly RelationEndPointID _endPointID;

    public MissingRealObjectEndPoint (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (endPointID.Definition.IsVirtual)
        throw new ArgumentException ("The end-point ID must be non-virtual.", "endPointID");

      _clientTransaction = clientTransaction;
      _endPointID = endPointID;
    }

    public bool IsNull
    {
      get { return true; }
    }

    public RelationEndPointID ID
    {
      get { return _endPointID; }
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public ObjectID ObjectID
    {
      get { return ID.ObjectID; }
    }

    public IRelationEndPointDefinition Definition
    {
      get { return ID.Definition; }
    }

    public RelationDefinition RelationDefinition
    {
      get { return ID.Definition.RelationDefinition; }
    }

    public bool HasChanged
    {
      get { return false; }
    }

    public bool HasBeenTouched
    {
      get { return false; }
    }

    public DomainObject GetDomainObject ()
    {
      return _clientTransaction.GetObject (ObjectID, true);
    }

    public DomainObject GetDomainObjectReference ()
    {
      return _clientTransaction.GetObjectReference (ObjectID);
    }

    public ObjectID OppositeObjectID
    {
      get { throw new NotSupportedException ("Cannot get OppositeObjectID from a MissingRealObjectEndPoint."); }
    }

    public ObjectID OriginalOppositeObjectID
    {
      get { throw new NotSupportedException ("Cannot get OppositeObjectID from a MissingRealObjectEndPoint."); }
    }

    public DomainObject GetOppositeObject (bool includeDeleted)
    {
      throw new NotSupportedException ("Cannot get opposite object from a MissingRealObjectEndPoint.");
    }

    public DomainObject GetOriginalOppositeObject ()
    {
      throw new NotSupportedException ("Cannot get original opposite object from a MissingRealObjectEndPoint.");
    }

    public bool IsDataComplete
    {
      get { throw new NotSupportedException ("Cannot ask a MissingRealObjectEndPoint whether its data is complete."); }
    }

    public void EnsureDataComplete ()
    {
      throw new NotSupportedException ("Cannot complete the data for a MissingRealObjectEndPoint.");
    }

    public bool IsSynchronized
    {
      get { throw new NotSupportedException ("Cannot ask a MissingRealObjectEndPoint whether it is synchronized."); }
    }

    public void Synchronize ()
    {
      throw new NotSupportedException ("Cannot synchronize a MissingRealObjectEndPoint.");
    }

    public void Touch ()
    {
      throw new NotSupportedException ("Cannot touch a MissingRealObjectEndPoint.");
    }

    public void Commit ()
    {
      throw new NotSupportedException ("Cannot commit a MissingRealObjectEndPoint.");
    }

    public void Rollback ()
    {
      throw new NotSupportedException ("Cannot rollback a MissingRealObjectEndPoint.");
    }

    public void CheckMandatory ()
    {
      throw new NotSupportedException ("Cannot call CheckMandatory on a MissingRealObjectEndPoint.");
    }

    public void SetDataFromSubTransaction (IRelationEndPoint source)
    {
      throw new NotSupportedException ("Cannot call SetDataFromSubTransaction on a MissingRealObjectEndPoint.");
    }

    public IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      throw new NotSupportedException ("Cannot create commands for a MissingRealObjectEndPoint.");
    }

    public IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      throw new NotSupportedException ("Cannot create commands for a MissingRealObjectEndPoint.");
    }

    public IDataManagementCommand CreateDeleteCommand ()
    {
      throw new NotSupportedException ("Cannot create commands for a MissingRealObjectEndPoint.");
    }

    public IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
      throw new NotSupportedException ("Cannot call GetOppositeRelationEndPointIDs on a MissingRealObjectEndPoint.");
    }

    public RelationEndPointID GetOppositeRelationEndPointID ()
    {
      throw new NotSupportedException ("Cannot call GetOppositeRelationEndPointID on a MissingRealObjectEndPoint.");
    }

    public void MarkSynchronized ()
    {
      throw new NotSupportedException ("Cannot call MarkSynchronized on a MissingRealObjectEndPoint.");
    }

    public void MarkUnsynchronized ()
    {
      throw new NotSupportedException ("Cannot call MarkUnsynchronized on a MissingRealObjectEndPoint.");
    }

    public void ResetSyncState ()
    {
      throw new NotSupportedException ("Cannot call ResetSyncState on a MissingRealObjectEndPoint.");
    }

    #region

    protected MissingRealObjectEndPoint (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _clientTransaction = info.GetValueForHandle<ClientTransaction> ();
      _endPointID = info.GetValue<RelationEndPointID> ();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_clientTransaction);
      info.AddValue (_endPointID);
    }

    #endregion

  }
}