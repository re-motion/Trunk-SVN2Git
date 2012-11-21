// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  // Note: No properties and methods of this class are inheritance-aware!
  [DebuggerDisplay (
      "{GetType().Name}: {_id}/{_endPointDefinitions[0].PropertyName} ({_endPointDefinitions[0].Cardinality})-{_endPointDefinitions[1].PropertyName} ({_endPointDefinitions[1].Cardinality})"
      )]
  public class RelationDefinition
  {
    private readonly string _id;
    private readonly IRelationEndPointDefinition[] _endPointDefinitions = new IRelationEndPointDefinition[2];

    public RelationDefinition (
        string id,
        IRelationEndPointDefinition endPointDefinition1,
        IRelationEndPointDefinition endPointDefinition2)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("endPointDefinition1", endPointDefinition1);
      ArgumentUtility.CheckNotNull ("endPointDefinition2", endPointDefinition2);

      _id = id;

      _endPointDefinitions[0] = endPointDefinition1;
      _endPointDefinitions[1] = endPointDefinition2;
    }

    public IRelationEndPointDefinition GetMandatoryOppositeRelationEndPointDefinition (IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);

      IRelationEndPointDefinition oppositeEndPointDefinition = GetOppositeEndPointDefinition (endPointDefinition);
      if (oppositeEndPointDefinition == null)
      {
        throw CreateMappingException (
            "Relation '{0}' has no association with class '{1}' and property '{2}'.",
            ID,
            endPointDefinition.ClassDefinition.ID,
            endPointDefinition.PropertyName);
      }

      return oppositeEndPointDefinition;
    }

    public string ID
    {
      get { return _id; }
    }

    public RelationKindType RelationKind
    {
      get 
      {
        foreach (IRelationEndPointDefinition endPointDefinition in _endPointDefinitions)
        {
          if (endPointDefinition is AnonymousRelationEndPointDefinition)
            return RelationKindType.Unidirectional;
          else if (endPointDefinition.Cardinality == CardinalityType.Many)
            return RelationKindType.OneToMany;
        }
        return RelationKindType.OneToOne; 
      }
    }

    public ReadOnlyCollection<IRelationEndPointDefinition> EndPointDefinitions
    {
      get { return new ReadOnlyCollection<IRelationEndPointDefinition> (_endPointDefinitions); }
    }

    public IRelationEndPointDefinition GetEndPointDefinition (string classID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      if (_endPointDefinitions[0].ClassDefinition.ID == classID && _endPointDefinitions[0].PropertyName == propertyName)
        return _endPointDefinitions[0];

      if (_endPointDefinitions[1].ClassDefinition.ID == classID && _endPointDefinitions[1].PropertyName == propertyName)
        return _endPointDefinitions[1];

      return null;
    }

    public IRelationEndPointDefinition GetOppositeEndPointDefinition (IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);

      if (endPointDefinition == _endPointDefinitions[0])
        return _endPointDefinitions[1];
      else if (endPointDefinition == _endPointDefinitions[1])
        return _endPointDefinitions[0];
      else
        return null;
    }

    public IRelationEndPointDefinition GetOppositeEndPointDefinition (string classID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      var matchingEndPointDefinition = GetEndPointDefinition (classID, propertyName);
      if (matchingEndPointDefinition == null)
        return null;

      return GetOppositeEndPointDefinition (matchingEndPointDefinition);
    }

    [Obsolete (
    "This method is obsolete because it can lead to inefficient code. Use 'endPointDefinition.GetOppositeEndPointDefinition().ClassDefinition' "
    + "instead. (1.13.176)")]
    public ClassDefinition GetOppositeClassDefinition (IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);
      return GetOppositeClassDefinition (endPointDefinition.ClassDefinition.ID, endPointDefinition.PropertyName);
    }

    [Obsolete (
        "This method is obsolete because it can lead to inefficient code. Use "
        + "'GetEndPointDefinition (classID, propertyName).GetOppositeEndPointDefinition().ClassDefinition' instead. If you already have an "
        + "IRelationEndPointDefinition, just use 'endPointDefinition.GetOppositeEndPointDefinition().ClassDefinition'. (1.13.176)")]
    public ClassDefinition GetOppositeClassDefinition (string classID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      IRelationEndPointDefinition oppositeEndPointDefinition = GetOppositeEndPointDefinition (classID, propertyName);
      if (oppositeEndPointDefinition == null)
        return null;

      return oppositeEndPointDefinition.ClassDefinition;
    }

    public bool IsEndPoint (string classID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      return GetEndPointDefinition (classID, propertyName) != null;
    }

    public bool Contains (IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);

      return endPointDefinition == _endPointDefinitions[0] || endPointDefinition == _endPointDefinitions[1];
    }

    public override string ToString ()
    {
      return GetType().FullName + ": " + _id;
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (String.Format (message, args));
    }
  }
}
