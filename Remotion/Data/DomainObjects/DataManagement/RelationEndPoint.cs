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

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Provides an abstract base implementation of non-transient relation end points that can be stored in the <see cref="RelationEndPointMap"/>.
  /// </summary>
  public abstract class RelationEndPoint : IEndPoint, IFlattenedSerializable
  {
    public static IEndPoint CreateNullRelationEndPoint (ClientTransaction clientTransaction, IRelationEndPointDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("definition", definition);

      if (definition.Cardinality == CardinalityType.One)
        return new NullObjectEndPoint (clientTransaction, definition);
      else
        return new NullCollectionEndPoint (clientTransaction, definition);
    }

    private readonly ClientTransaction _clientTransaction;
    private readonly RelationEndPointID _id;

    protected RelationEndPoint (ClientTransaction clientTransaction, RelationEndPointID id)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("id", id);

      _clientTransaction = clientTransaction;
      _id = id;
    }

    public abstract bool IsDataAvailable { get; }
    public abstract bool HasChanged { get; }
    public abstract bool HasBeenTouched { get; }

    public abstract void EnsureDataAvailable ();
    public abstract void Touch ();

    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract void SetValueFrom (RelationEndPoint source);

    public abstract void CheckMandatory ();
    public abstract void PerformDelete ();
    public abstract IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject);

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public RelationEndPointID ID
    {
      get { return _id; }
    }

    public ObjectID ObjectID
    {
      get { return _id.ObjectID; }
    }

    public IRelationEndPointDefinition Definition
    {
      get { return ID.Definition; }
    }

    public string PropertyName
    {
      get { return Definition.PropertyName; }
    }

    public RelationDefinition RelationDefinition
    {
      get { return Definition.RelationDefinition; }
    }

    public bool IsVirtual
    {
      get { return Definition.IsVirtual; }
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    public virtual void NotifyClientTransactionOfBeginRelationChange (DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      _clientTransaction.TransactionEventSink.RelationChanging (this.GetDomainObject(), PropertyName, oldRelatedObject, newRelatedObject);
    }

    public virtual void NotifyClientTransactionOfEndRelationChange ()
    {
      _clientTransaction.TransactionEventSink.RelationChanged (this.GetDomainObject(), PropertyName);
    }

    protected MandatoryRelationNotSetException CreateMandatoryRelationNotSetException (
        DomainObject domainObject,
        string propertyName,
        string formatString,
        params object[] args)
    {
      return new MandatoryRelationNotSetException (domainObject, propertyName, string.Format (formatString, args));
    }

    #region Serialization

    protected RelationEndPoint (FlattenedDeserializationInfo info)
    {
      _clientTransaction = info.GetValueForHandle<ClientTransaction>();
      _id = info.GetValue<RelationEndPointID>();
    }

    protected abstract void SerializeIntoFlatStructure (FlattenedSerializationInfo info);

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_clientTransaction);
      info.AddValue (_id);

      SerializeIntoFlatStructure (info);
    }

    #endregion
  }
}