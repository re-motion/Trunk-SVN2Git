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
  [DebuggerDisplay ("{GetType().Name}: {PropertyName}, Cardinality: {Cardinality}")]
  public class RelationEndPointDefinition : SerializableMappingObject, IRelationEndPointDefinition
  {
    // types

    // static members and constants

    // serialized member fields
    // Note: RelationEndPointDefinitions can only be serialized if they are part of the current mapping configuration. Only the fields listed below
    // will be serialized; these are used to retrieve the "real" object at deserialization time.

    private readonly string _serializedClassDefinitionID;
    private readonly string _serializedPropertyName;

    // nonserialized member fields

    [NonSerialized]
    private RelationDefinition _relationDefinition;

    [NonSerialized]
    private readonly ClassDefinition _classDefinition;

    [NonSerialized]
    private readonly PropertyDefinition _propertyDefinition;

    [NonSerialized]
    private readonly bool _isMandatory;

    // construction and disposing

    public RelationEndPointDefinition (ClassDefinition classDefinition, string propertyName, bool isMandatory)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      PropertyDefinition propertyDefinition = classDefinition[propertyName];
      if (propertyDefinition == null)
        throw CreateMappingException ("Relation definition error for end point: Class '{0}' has no property '{1}'.", classDefinition.ID, propertyName);

      if (!propertyDefinition.IsObjectID)
      {
        throw CreateMappingException (
            "Relation definition error: Property '{0}' of class '{1}' is of type '{2}', but non-virtual properties must be of type '{3}'.",
            propertyDefinition.PropertyName,
            classDefinition.ID,
            propertyDefinition.PropertyType,
            typeof (ObjectID));
      }

      _classDefinition = classDefinition;
      _serializedClassDefinitionID = _classDefinition.ID;
      _isMandatory = isMandatory;
      _propertyDefinition = propertyDefinition;
      _serializedPropertyName = _propertyDefinition.PropertyName;
    }

    // methods and properties

    #region INullObject Members

    public bool IsNull
    {
      get { return false; }
    }

    #endregion

    #region IRelationEndPointDefinition Members

    public bool CorrespondsTo (string classID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      return (_classDefinition.ID == classID && PropertyName == propertyName);
    }

    public void SetRelationDefinition (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      _relationDefinition = relationDefinition;
    }

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
      get { return _propertyDefinition.PropertyName; }
    }

    public bool IsMandatory
    {
      get { return _isMandatory; }
    }

    public CardinalityType Cardinality
    {
      get { return CardinalityType.One; }
    }

    public Type PropertyType
    {
      get { return _propertyDefinition.PropertyType; }
    }

    public bool IsPropertyTypeResolved
    {
      get { return PropertyType != null; }
    }

    public string PropertyTypeName
    {
      get
      {
        if (IsPropertyTypeResolved)
          return typeof (ObjectID).AssemblyQualifiedName;
        else
          return TypeUtility.GetPartialAssemblyQualifiedName (typeof (ObjectID));
      }
    }

    public bool IsVirtual
    {
      get { return false; }
    }

    #endregion

    public PropertyDefinition PropertyDefinition
    {
      get { return _propertyDefinition; }
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }

    #region Serialization

    public override object GetRealObject (StreamingContext context)
    {
      // Note: A EndPointDefinition knows its ClassDefinition and a ClassDefinition implicitly knows 
      // its RelationEndPointDefinitions via its RelationDefinitions. For bi-directional relationships 
      // with two classes implementing IObjectReference.GetRealObject the order of calling this method is unpredictable.
      // Therefore the members _classDefinition and _propertyDefinition cannot be used here, because they could point to the wrong instances. 
      return MappingConfiguration.Current.ClassDefinitions.GetMandatory (_serializedClassDefinitionID).GetMandatoryRelationEndPointDefinition (_serializedPropertyName);
    }

    protected override bool IsPartOfMapping
    {
      get { return MappingConfiguration.Current.Contains (this); }
    }

    protected override string IDForExceptions
    {
      get { return PropertyName; }
    }

    #endregion
  }
}
