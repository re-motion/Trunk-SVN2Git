/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  [DebuggerDisplay ("{GetType().Name}: Cardinality: {Cardinality}")]
  public class AnonymousRelationEndPointDefinition : SerializableMappingObject, IRelationEndPointDefinition
  {
    // types

    // static members and constants

    // serialized member fields
    // Note: RelationEndPointDefinitions can only be serialized if they are part of the current mapping configuration. Only the fields listed below
    // will be serialized; these are used to retrieve the "real" object at deserialization time.

    private string _serializedRelationID;

    // nonserialized member fields

    [NonSerialized]
    private RelationDefinition _relationDefinition;

    [NonSerialized]
    private readonly ClassDefinition _classDefinition;

    // construction and disposing

    public AnonymousRelationEndPointDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      _classDefinition = classDefinition;
    }

    // methods and properties

    #region INullObject Members

    public bool IsNull
    {
      get { return true; }
    }

    #endregion

    #region IRelationEndPointDefinition Members

    public RelationDefinition RelationDefinition
    {
      get { return _relationDefinition; }
    }

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public string PropertyName
    {
      get { return null; }
    }

    public Type PropertyType
    {
      get { return null; }
    }

    public bool IsPropertyTypeResolved
    {
      get { return _classDefinition.IsClassTypeResolved; }
    }

    public string PropertyTypeName
    {
      get { return null; }
    }

    public bool IsMandatory
    {
      get { return false; }
    }

    public CardinalityType Cardinality
    {
      get { return CardinalityType.Many; }
    }

    public bool IsVirtual
    {
      get { return true; }
    }

    public bool CorrespondsTo (string classID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      return (_classDefinition.ID == classID && propertyName == null);
    }

    public void SetRelationDefinition (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      _relationDefinition = relationDefinition;
      _serializedRelationID = _relationDefinition.ID;
    }

    #endregion

    #region Serialization

    public override object GetRealObject (StreamingContext context)
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions.GetMandatory (_serializedRelationID);
      if (relationDefinition.EndPointDefinitions[0].IsNull)
        return relationDefinition.EndPointDefinitions[0];
      else
        return relationDefinition.EndPointDefinitions[1];
    }

    protected override bool IsPartOfMapping
    {
      get { return MappingConfiguration.Current.Contains (this); }
    }

    protected override string IDForExceptions
    {
      get { return "<anonymous>"; }
    }

    #endregion
  }
}
