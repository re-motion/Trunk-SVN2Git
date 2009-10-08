// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Represents the non-foreign-key side of a unidirectional relationship.
  /// </summary>
  [Serializable]
  [DebuggerDisplay ("{GetType().Name}: {PropertyName}, Cardinality: {Cardinality}")]
  public class VirtualRelationEndPointDefinition : SerializableMappingObject, IRelationEndPointDefinition
  {
    // types

    // static members and constants

    // serialized member fields
    // Note: RelationEndPointDefinitions can only be serialized if they are part of the current mapping configuration. Only the fields listed below
    // will be serialized; these are used to retrieve the "real" object at deserialization time.

    private readonly string _propertyName;
    private readonly string _serializedClassDefinitionID;

    // nonserialized member fields

    [NonSerialized]
    private RelationDefinition _relationDefinition;

    [NonSerialized]
    private readonly ClassDefinition _classDefinition;

    [NonSerialized]
    private readonly bool _isMandatory;

    [NonSerialized]
    private readonly CardinalityType _cardinality;

    [NonSerialized]
    private readonly Type _propertyType;

    [NonSerialized]
    private readonly string _propertyTypeName;

    [NonSerialized]
    private readonly string _sortExpression;

    // construction and disposing

    public VirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        Type propertyType)
        : this (classDefinition, propertyName, isMandatory, cardinality, propertyType, null)
    {
    }

    public VirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        Type propertyType,
        string sortExpression)
        : this (
            classDefinition, propertyName, isMandatory, cardinality, ArgumentUtility.CheckNotNull ("propertyType", propertyType), null, sortExpression
            )
    {
    }

    public VirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        string propertyTypeName,
        string sortExpression)
        : this (
            classDefinition,
            propertyName,
            isMandatory,
            cardinality,
            null,
            ArgumentUtility.CheckNotNullOrEmpty ("propertyTypeName", propertyTypeName),
            sortExpression)
    {
    }

    private VirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        Type propertyType,
        string propertyTypeName,
        string sortExpression)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckValidEnumValue ("cardinality", cardinality);

      if (classDefinition.IsClassTypeResolved && propertyTypeName != null)
        propertyType = ContextAwareTypeDiscoveryUtility.GetType (propertyTypeName, true);

      if (propertyType != null)
      {
        CheckPropertyType (classDefinition, propertyName, cardinality, propertyType);
        propertyTypeName = propertyType.AssemblyQualifiedName;
      }

      CheckSortExpression (classDefinition, propertyName, cardinality, sortExpression);

      _classDefinition = classDefinition;
      _serializedClassDefinitionID = _classDefinition.ID;
      _cardinality = cardinality;
      _isMandatory = isMandatory;
      _propertyName = propertyName;
      _propertyType = propertyType;
      _propertyTypeName = propertyTypeName;
      _sortExpression = sortExpression;
    }

    private void CheckPropertyType (
        ClassDefinition classDefinition,
        string propertyName,
        CardinalityType cardinality,
        Type propertyType)
    {
      if (propertyType != typeof (DomainObjectCollection)
          && !propertyType.IsSubclassOf (typeof (DomainObjectCollection))
              && !propertyType.IsSubclassOf (typeof (DomainObject)))
      {
        throw CreateMappingException (
            "Relation definition error: Virtual property '{0}' of class '{1}' is of type"
                + "'{2}', but must be derived from 'Remotion.Data.DomainObjects.DomainObject' or "
                    + " 'Remotion.Data.DomainObjects.DomainObjectCollection' or must be"
                        + " 'Remotion.Data.DomainObjects.DomainObjectCollection'.",
            propertyName,
            classDefinition.ID,
            propertyType);
      }

      if (cardinality == CardinalityType.One && !propertyType.IsSubclassOf (typeof (DomainObject)))
      {
        throw CreateMappingException (
            "The property type of a virtual end point of a one-to-one relation"
                + " must be derived from 'Remotion.Data.DomainObjects.DomainObject'.");
      }

      if (cardinality == CardinalityType.Many
          && propertyType != typeof (DomainObjectCollection)
              && !propertyType.IsSubclassOf (typeof (DomainObjectCollection)))
      {
        throw CreateMappingException (
            "The property type of a virtual end point of a one-to-many relation"
                + " must be or be derived from 'Remotion.Data.DomainObjects.DomainObjectCollection'.");
      }
    }

    private void CheckSortExpression (
        ClassDefinition classDefinition,
        string propertyName,
        CardinalityType cardinality,
        string sortExpression)
    {
      if (cardinality == CardinalityType.One && sortExpression != null)
      {
        throw CreateMappingException (
            "Property '{0}' of class '{1}' must not specify a SortExpression, because cardinality is equal to 'one'.",
            propertyName,
            classDefinition.ID);
      }
    }

    // methods and properties

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

    public bool IsMandatory
    {
      get { return _isMandatory; }
    }

    public CardinalityType Cardinality
    {
      get { return _cardinality; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public bool IsPropertyTypeResolved
    {
      get { return _propertyType != null; }
    }

    public string PropertyTypeName
    {
      get { return _propertyTypeName; }
    }

    public bool IsVirtual
    {
      get { return true; }
    }

    public bool IsAnonymous
    {
      get { return false; }
    }

    public string SortExpression
    {
      get { return _sortExpression; }
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
      // Therefore the members _classDefinition and _relationDefinition cannot be used here, because they could point to the wrong instance. 
      return
          MappingConfiguration.Current.ClassDefinitions.GetMandatory (_serializedClassDefinitionID).GetMandatoryRelationEndPointDefinition (
              _propertyName);
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
