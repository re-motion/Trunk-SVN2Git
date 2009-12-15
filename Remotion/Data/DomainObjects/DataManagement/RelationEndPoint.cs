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
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public abstract class RelationEndPoint : IEndPoint, IFlattenedSerializable
  {
    // types

    // static members and constants

    public static IEndPoint CreateNullRelationEndPoint (IRelationEndPointDefinition definition)
    {
      if (definition.Cardinality == CardinalityType.One)
        return new NullObjectEndPoint (definition);
      else
        return new NullCollectionEndPoint (definition);
    }

    // member fields

    private readonly ClientTransaction _clientTransaction;
    private readonly IRelationEndPointDefinition _definition;
    private readonly RelationEndPointID _id;

    // construction and disposing

    protected RelationEndPoint (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition definition)
        : this (clientTransaction, domainObject.ID, definition)
    {
    }

    protected RelationEndPoint (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        IRelationEndPointDefinition definition)
        : this (clientTransaction, dataContainer.ID, definition.PropertyName)
    {
    }

    protected RelationEndPoint (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        string propertyName)
        : this (clientTransaction, domainObject.ID, propertyName)
    {
    }

    protected RelationEndPoint (
        ClientTransaction clientTransaction,
        DataContainer dataContainer,
        string propertyName)
        : this (clientTransaction, dataContainer.ID, propertyName)
    {
    }

    protected RelationEndPoint (
        ClientTransaction clientTransaction,
        ObjectID objectID,
        IRelationEndPointDefinition definition)
        : this (clientTransaction, objectID, definition.PropertyName)
    {
    }

    protected RelationEndPoint (
        ClientTransaction clientTransaction,
        ObjectID objectID,
        string propertyName)
        : this (clientTransaction, new RelationEndPointID (objectID, propertyName))
    {
    }

    protected RelationEndPoint (ClientTransaction clientTransaction, RelationEndPointID id)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("id", id);

      _clientTransaction = clientTransaction;
      _id = id;
      _definition = _id.Definition;
    }

    protected RelationEndPoint (IRelationEndPointDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      _definition = definition;
    }

    // abstract methods and properties

    public abstract bool HasChanged { get; }
    public abstract bool HasBeenTouched { get; }

    public abstract void Touch ();
    
    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract void SetValueFrom (RelationEndPoint source);
    
    public abstract void CheckMandatory ();
    public abstract void PerformDelete ();
    public abstract IRelationEndPointModification CreateRemoveModification (DomainObject removedRelatedObject);

    // methods and properties

    public virtual void NotifyClientTransactionOfBeginRelationChange (DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      _clientTransaction.TransactionEventSink.RelationChanging (this.GetDomainObject(), _definition.PropertyName, oldRelatedObject, newRelatedObject);
    }

    public virtual void NotifyClientTransactionOfEndRelationChange ()
    {
      _clientTransaction.TransactionEventSink.RelationChanged (this.GetDomainObject(), _definition.PropertyName);
    }

    public virtual ObjectID ObjectID
    {
      get { return _id.ObjectID; }
    }

    public IRelationEndPointDefinition Definition
    {
      get { return _definition; }
    }

    public string PropertyName
    {
      get { return _definition.PropertyName; }
    }

    public IRelationEndPointDefinition OppositeEndPointDefinition
    {
      get { return _definition.GetOppositeEndPointDefinition(); }
    }

    public RelationDefinition RelationDefinition
    {
      get { return _definition.RelationDefinition; }
    }

    public bool IsVirtual
    {
      get { return _definition.IsVirtual; }
    }

    public virtual RelationEndPointID ID
    {
      get { return _id; }
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    protected MandatoryRelationNotSetException CreateMandatoryRelationNotSetException (
        DomainObject domainObject,
        string propertyName,
        string formatString,
        params object[] args)
    {
      return new MandatoryRelationNotSetException (domainObject, propertyName, string.Format (formatString, args));
    }

    // TODO: Make explicit implementation
    public virtual bool IsNull
    {
      get { return false; }
    }

    #region Serialization

    protected RelationEndPoint (FlattenedDeserializationInfo info)
    {
      _clientTransaction = info.GetValueForHandle<ClientTransaction>();
      
      var classDefinitionID = info.GetValueForHandle<string>();
      var propertyName = info.GetValueForHandle<string>();
      _definition =
          MappingConfiguration.Current.ClassDefinitions.GetMandatory (classDefinitionID).GetMandatoryRelationEndPointDefinition (propertyName);
      
      _id = info.GetValue<RelationEndPointID>();
    }

    protected abstract void SerializeIntoFlatStructure (FlattenedSerializationInfo info);

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_clientTransaction);
      info.AddHandle (_definition.ClassDefinition.ID);
      info.AddHandle (_definition.PropertyName);
      info.AddValue (_id);

      SerializeIntoFlatStructure (info);
    }

    #endregion
  }
}
