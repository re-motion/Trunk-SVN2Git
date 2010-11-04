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
using System.Linq;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Identifies a relation end point on a given object (<see cref="ObjectID"/>) of a given kind (<see cref="Definition"/>).
  /// </summary>
  /// <remarks>
  /// This type implements custom serialization. It serializes the class definition ID, the relation end point name, and the objectID.
  /// </remarks>
  [Serializable]
  public sealed class RelationEndPointID : ISerializable
  {
    public static RelationEndPointID[] GetAllRelationEndPointIDs (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      
      var endPointDefinitions = objectID.ClassDefinition.GetRelationEndPointDefinitions ();
      return endPointDefinitions.Select (endPointDefinition => new RelationEndPointID (objectID, endPointDefinition)).ToArray();
    }

    public static bool operator == (RelationEndPointID endPointID1, RelationEndPointID endPointID2)
    {
      return Equals (endPointID1, endPointID2);
    }

    public static bool operator != (RelationEndPointID endPointID1, RelationEndPointID endPointID2)
    {
      return !Equals (endPointID1, endPointID2);
    }

    public static bool Equals (RelationEndPointID endPointID1, RelationEndPointID endPointID2)
    {
      if (ReferenceEquals (endPointID1, endPointID2))
        return true;
      if (ReferenceEquals (endPointID1, null))
        return false;

      return endPointID1.Equals (endPointID2);
    }

    private static IRelationEndPointDefinition GetRelationEndPointDefinition (ObjectID objectID, string propertyName)
    {
      return objectID.ClassDefinition.GetMandatoryRelationEndPointDefinition (propertyName);
    }

    private readonly IRelationEndPointDefinition _definition;
    private readonly ObjectID _objectID;

    private readonly int _cachedHashCode;

    public RelationEndPointID (ObjectID objectID, IRelationEndPointDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      _objectID = objectID;
      _definition = definition;

      _cachedHashCode = CalculateHashCode();
    }

    public RelationEndPointID (ObjectID objectID, string propertyName)
        : this (
            ArgumentUtility.CheckNotNull ("objectID", objectID),
            GetRelationEndPointDefinition (objectID, ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName)))
    {
    }

    public IRelationEndPointDefinition Definition
    {
      get { return _definition; }
    }

    public ObjectID ObjectID
    {
      get { return _objectID; }
    }

    public override int GetHashCode ()
    {
      return _cachedHashCode;
    }

    public override bool Equals (object obj)
    {
      if (obj == null)
        return false;
      if (GetType() != obj.GetType())
        return false;

      var other = (RelationEndPointID) obj;
      if (!Equals (_objectID, other._objectID))
        return false;
      if (!Equals (_definition, other._definition))
        return false;

      return true;
    }

    public override string ToString ()
    {
      return string.Format ("{0}/{1}", _objectID != null ? _objectID.ToString() : "null", Definition.PropertyName);
    }

    private int CalculateHashCode ()
    {
      var propertyName = Definition.PropertyName;
      return (_objectID != null ? _objectID.GetHashCode () : 0) ^ (propertyName != null ? propertyName.GetHashCode () : 0);
    }

    #region Serialization

    // ReSharper disable UnusedMember.Local
    private RelationEndPointID (SerializationInfo info, StreamingContext context)
    {
      var classDefinitionID = info.GetString ("classDefinitionID");
      var propertyName = info.GetString ("propertyName");
      
      _definition =
          MappingConfiguration.Current.ClassDefinitions.GetMandatory (classDefinitionID).GetMandatoryRelationEndPointDefinition (propertyName);
      _objectID = (ObjectID) info.GetValue ("objectID", typeof (ObjectID));

      _cachedHashCode = CalculateHashCode ();
    }
    // ReSharper restore UnusedMember.Local

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("classDefinitionID", _definition.ClassDefinition.ID);
      info.AddValue ("propertyName", _definition.PropertyName);
      info.AddValue ("objectID", _objectID);
    }

    #endregion
  }
}