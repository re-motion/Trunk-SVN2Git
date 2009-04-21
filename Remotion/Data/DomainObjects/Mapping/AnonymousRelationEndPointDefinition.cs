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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Represents the non-existing side of a unidirectional relationship.
  /// </summary>
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

    public bool IsAnonymous
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

    #region Serialization

    public override object GetRealObject (StreamingContext context)
    {
      RelationDefinition relationDefinition = MappingConfiguration.Current.RelationDefinitions.GetMandatory (_serializedRelationID);
      if (relationDefinition.EndPointDefinitions[0].IsAnonymous)
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
