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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Represents the non-foreign-key side of a unidirectional relationship.
  /// </summary>
  [Serializable]
  [DebuggerDisplay ("{GetType().Name}: {PropertyName}, Cardinality: {Cardinality}")]
  public abstract class VirtualRelationEndPointDefinition : SerializableMappingObject, IRelationEndPointDefinition
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
    private readonly string _sortExpressionText;

    [NonSerialized]
    private readonly DoubleCheckedLockingContainer<SortExpressionDefinition> _sortExpression;

    // construction and disposing

    protected VirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        Type propertyType)
        : this (classDefinition, propertyName, isMandatory, cardinality, propertyType, null)
    {
    }

    protected VirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        Type propertyType,
        string sortExpressionText)
        : this (
            classDefinition, propertyName, isMandatory, cardinality, ArgumentUtility.CheckNotNull ("propertyType", propertyType), null, sortExpressionText
            )
    {
    }

    protected VirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        string propertyTypeName,
        string sortExpressionText)
        : this (
            classDefinition,
            propertyName,
            isMandatory,
            cardinality,
            null,
            ArgumentUtility.CheckNotNullOrEmpty ("propertyTypeName", propertyTypeName),
            sortExpressionText)
    {
    }

    private VirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        Type propertyType,
        string propertyTypeName,
        string sortExpressionText)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckValidEnumValue ("cardinality", cardinality);

      if (classDefinition.IsClassTypeResolved && propertyTypeName != null)
        propertyType = ContextAwareTypeDiscoveryUtility.GetType (propertyTypeName, true);

      if (propertyType != null)
        propertyTypeName = propertyType.AssemblyQualifiedName;
      
      _classDefinition = classDefinition;
      _serializedClassDefinitionID = _classDefinition.ID;
      _cardinality = cardinality;
      _isMandatory = isMandatory;
      _propertyName = propertyName;
      _propertyType = propertyType;
      _propertyTypeName = propertyTypeName;
      _sortExpressionText = sortExpressionText;
      _sortExpression = new DoubleCheckedLockingContainer<SortExpressionDefinition> (() => ParseSortExpression (_sortExpressionText));
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

    public abstract PropertyInfo PropertyInfo { get; }

    public abstract bool IsPropertyInfoResolved { get; }


    public bool IsVirtual
    {
      get { return true; }
    }

    public bool IsAnonymous
    {
      get { return false; }
    }

    public string SortExpressionText
    {
      get { return _sortExpressionText; }
    }

    public SortExpressionDefinition GetSortExpression ()
    {
      return _sortExpression.Value;
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

    private SortExpressionDefinition ParseSortExpression (string sortExpressionText)
    {
      if (sortExpressionText == null)
        return null;

      try
      {
        var parser = new SortExpressionParser (this.GetOppositeClassDefinition ());
        return parser.Parse (sortExpressionText);
      }
      catch (MappingException ex)
      {
        var message = string.Format ("{0}\r\n\r\nDeclaring type: '{1}'\r\nProperty: '{2}'", ex.Message, ClassDefinition.ClassType, PropertyInfo.Name);
        throw new MappingException (message, ex);
      }
    }
  }
}
