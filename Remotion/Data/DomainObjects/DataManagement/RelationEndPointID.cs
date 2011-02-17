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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Identifies a relation end point on a given object (<see cref="ObjectID"/>) of a given kind (<see cref="Definition"/>).
  /// </summary>
  [Serializable]
  public sealed class RelationEndPointID : IFlattenedSerializable
  {
    public static RelationEndPointID Create (ObjectID objectID, IRelationEndPointDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      return new RelationEndPointID (objectID, definition);
    }
    
    public static RelationEndPointID Create (ObjectID objectID, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      IRelationEndPointDefinition endPointDefinition;
      try
      {
        endPointDefinition = objectID.ClassDefinition.GetMandatoryRelationEndPointDefinition (propertyIdentifier);
      }
      catch (MappingException ex)
      {
        throw new ArgumentException (ex.Message, "propertyIdentifier", ex);
      }

      return new RelationEndPointID (objectID, endPointDefinition);
    }

    public static RelationEndPointID Create (ObjectID objectID, Type declaringType, string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("shortPropertyName", shortPropertyName);

      PropertyAccessorData data;
      try
      {
        data = objectID.ClassDefinition.PropertyAccessorDataCache.GetMandatoryPropertyAccessorData (declaringType, shortPropertyName);
      }
      catch (MappingException ex)
      {
        throw new ArgumentException (ex.Message, "shortPropertyName", ex);
      }

      if (data.RelationEndPointDefinition == null)
      {
        var message = string.Format ("The property '{0}' is not a relation property.", data.PropertyIdentifier);
        throw new ArgumentException (message, "shortPropertyName");
      }

      return new RelationEndPointID (objectID, data.RelationEndPointDefinition);
    }

    public static RelationEndPointID[] GetAllRelationEndPointIDs (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      
      var endPointDefinitions = objectID.ClassDefinition.GetRelationEndPointDefinitions ();
      return endPointDefinitions.Select (endPointDefinition => Create(objectID, endPointDefinition)).ToArray();
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

    private readonly IRelationEndPointDefinition _definition;
    private readonly ObjectID _objectID;

    [NonSerialized]
    private int _cachedHashCode;

    private RelationEndPointID (ObjectID objectID, IRelationEndPointDefinition definition)
    {
      _objectID = objectID;
      _definition = definition;

      _cachedHashCode = CalculateHashCode();
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
      // Use lazy initialization because of deserialization.

      // Note: The following code is not completely thread-safe - the hash code might be calculated twice on different threads. 
      // However, we can assume that an int assignment is atomic (and the XOR operation is fully performed before the assignment takes place), 
      // so no half-calculated values should become visible.

      // Note: We assume that a hash code value of 0 means that it wasn't initialized. In the very unlikely situation that 
      // the hash code is really 0, it would be recalculated on each call.

      if (_cachedHashCode == 0)
        _cachedHashCode = CalculateHashCode ();

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
    private RelationEndPointID (FlattenedDeserializationInfo info)
    {
      var classDefinitionID = info.GetValueForHandle<string> ();
      var propertyName = info.GetValueForHandle<string> ();

      _definition =
          MappingConfiguration.Current.ClassDefinitions.GetMandatory (classDefinitionID).GetMandatoryRelationEndPointDefinition (propertyName);
      _objectID = info.GetValueForHandle<ObjectID> ();

      _cachedHashCode = CalculateHashCode ();
    }
    // ReSharper restore UnusedMember.Local

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_definition.ClassDefinition.ID);
      info.AddHandle (_definition.PropertyName);
      info.AddHandle (_objectID);
    }

    #endregion
  }
}