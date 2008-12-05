// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
  public abstract class RelationEndPoint : IEndPoint, IFlattenedSerializable
  {
    // types

    // static members and constants

    public static RelationEndPoint CreateNullRelationEndPoint (IRelationEndPointDefinition definition)
    {
      if (definition.Cardinality == CardinalityType.One)
        return new NullObjectEndPoint (definition);
      else
        return new NullCollectionEndPoint (definition);
    }

    // member fields

    private ClientTransaction _clientTransaction;
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

    public abstract RelationEndPoint Clone ();
    protected internal abstract void AssumeSameState (RelationEndPoint source);
    protected internal abstract void TakeOverCommittedData (RelationEndPoint source);
    protected internal abstract void RegisterWithMap (RelationEndPointMap map);
    public abstract bool HasChanged { get; }
    public abstract bool HasBeenTouched { get; }
    protected internal abstract void Touch ();
    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract void CheckMandatory ();
    public abstract void PerformDelete ();
    public abstract RelationEndPointModification CreateModification (IEndPoint oldEndPoint, IEndPoint newEndPoint);

    // methods and properties

    public RelationEndPointModification CreateModification (IEndPoint oldEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
      return CreateModification (oldEndPoint, RelationEndPoint.CreateNullRelationEndPoint (oldEndPoint.Definition));
    }

    public virtual void NotifyClientTransactionOfBeginRelationChange (IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
      ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

      _clientTransaction.TransactionEventSink.RelationChanging (
          GetDomainObject(),
          _definition.PropertyName,
          oldEndPoint.GetDomainObject(),
          newEndPoint.GetDomainObject());
    }

    public virtual void NotifyClientTransactionOfEndRelationChange ()
    {
      _clientTransaction.TransactionEventSink.RelationChanged (GetDomainObject(), _definition.PropertyName);
    }

    public virtual DomainObject GetDomainObject ()
    {
      return _clientTransaction.GetObject (ObjectID, true);
    }

    public virtual DataContainer GetDataContainer ()
    {
      DomainObject domainObject = GetDomainObject();
      return _clientTransaction.GetDataContainer(domainObject);
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
      get { return _definition.ClassDefinition.GetMandatoryOppositeEndPointDefinition (PropertyName); }
    }

    public RelationDefinition RelationDefinition
    {
      get { return _definition.ClassDefinition.GetMandatoryRelationDefinition (PropertyName); }
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

    internal void SetClientTransaction (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      _clientTransaction = clientTransaction;
    }

    protected MandatoryRelationNotSetException CreateMandatoryRelationNotSetException (
        DomainObject domainObject,
        string propertyName,
        string formatString,
        params object[] args)
    {
      return new MandatoryRelationNotSetException (domainObject, propertyName, string.Format (formatString, args));
    }

    #region INullObject Members

    public virtual bool IsNull
    {
      get { return false; }
    }

    #endregion

    #region Serialization

    protected RelationEndPoint (FlattenedDeserializationInfo info)
    {
      _clientTransaction = info.GetValueForHandle<ClientTransaction>();
      string classDefinitionID = info.GetValueForHandle<string>();
      string propertyName = info.GetValueForHandle<string>();
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
