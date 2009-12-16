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
  public sealed class RelationEndPointID : IFlattenedSerializable
  {
    public static RelationEndPointID[] GetAllRelationEndPointIDs (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      IRelationEndPointDefinition[] endPointDefinitions = dataContainer.ClassDefinition.GetRelationEndPointDefinitions ();

      var relationEndPointIDs = new RelationEndPointID[endPointDefinitions.Length];

      for (int i = 0; i < endPointDefinitions.Length; i++)
        relationEndPointIDs[i] = new RelationEndPointID (dataContainer.ID, endPointDefinitions[i].PropertyName);

      return relationEndPointIDs;
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

    public RelationEndPointID (ObjectID objectID, IRelationEndPointDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      _objectID = objectID;
      _definition = definition;
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

    #region Serialization

// ReSharper disable UnusedMember.Local
    private RelationEndPointID (FlattenedDeserializationInfo info)
    {
      var classDefinitionID = info.GetValueForHandle<string>();
      var propertyName = info.GetValueForHandle<string>();
      _definition =
          MappingConfiguration.Current.ClassDefinitions.GetMandatory (classDefinitionID).GetMandatoryRelationEndPointDefinition (propertyName);
      _objectID = info.GetValueForHandle<ObjectID>();
    }
    // ReSharper restore UnusedMember.Local

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_definition.ClassDefinition.ID);
      info.AddHandle (_definition.PropertyName);
      info.AddHandle (_objectID);
    }

    #endregion


    public override int GetHashCode ()
    {
      var objectIDHashCode = _objectID != null ? _objectID.GetHashCode() : 0;
      return objectIDHashCode ^ Definition.PropertyName.GetHashCode();
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
      return string.Format ("{0}/{1}", _objectID, Definition.PropertyName);
    }
  }
}