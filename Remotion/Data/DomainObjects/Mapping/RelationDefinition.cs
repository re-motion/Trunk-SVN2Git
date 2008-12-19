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
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  // Note: No properties and methods of this class are inheritance-aware!
  [Serializable]
  [DebuggerDisplay (
      "{GetType().Name}: {_id}/{_endPointDefinitions[0].PropertyName} ({_endPointDefinitions[0].Cardinality})-{_endPointDefinitions[1].PropertyName} ({_endPointDefinitions[1].Cardinality})"
      )]
  public class RelationDefinition : SerializableMappingObject
  {
    // types

    // static members and constants

    // serialized member fields
    // Note: RelationEndPointDefinitions can only be serialized if they are part of the current mapping configuration. Only the fields listed below
    // will be serialized; these are used to retrieve the "real" object at deserialization time.

    private readonly string _id;

    // nonserialized member fields

    private readonly IRelationEndPointDefinition[] _endPointDefinitions = new IRelationEndPointDefinition[2];

    // construction and disposing

    public RelationDefinition (
        string id,
        IRelationEndPointDefinition endPointDefinition1,
        IRelationEndPointDefinition endPointDefinition2)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("endPointDefinition1", endPointDefinition1);
      ArgumentUtility.CheckNotNull ("endPointDefinition2", endPointDefinition2);

      CheckEndPointDefinitions (id, endPointDefinition1, endPointDefinition2);

      _id = id; // Note: Set ID before passing this instance to other end point definitions.

      try
      {
        endPointDefinition1.SetRelationDefinition (this);
        endPointDefinition2.SetRelationDefinition (this);
      }
      catch (Exception)
      {
        endPointDefinition1.SetRelationDefinition (null);
        endPointDefinition2.SetRelationDefinition (null);
        throw;
      }

      _endPointDefinitions[0] = endPointDefinition1;
      _endPointDefinitions[1] = endPointDefinition2;
    }

    private void CheckEndPointDefinitions (
        string id,
        IRelationEndPointDefinition endPointDefinition1,
        IRelationEndPointDefinition endPointDefinition2)
    {
      if (endPointDefinition1.IsNull && endPointDefinition2.IsNull)
        throw CreateMappingException ("Relation '{0}' cannot have two null end points.", id);

      if (endPointDefinition1.IsVirtual && endPointDefinition2.IsVirtual)
        throw CreateMappingException ("Relation '{0}' cannot have two virtual end points.", id);

      if (!endPointDefinition1.IsVirtual && !endPointDefinition2.IsVirtual)
        throw CreateMappingException ("Relation '{0}' cannot have two non-virtual end points.", id);
    }

    // methods and properties

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

    public IRelationEndPointDefinition[] EndPointDefinitions
    {
      get { return _endPointDefinitions; }
    }

    public IRelationEndPointDefinition GetEndPointDefinition (string classID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      if (_endPointDefinitions[0].CorrespondsTo (classID, propertyName))
        return _endPointDefinitions[0];

      if (_endPointDefinitions[1].CorrespondsTo (classID, propertyName))
        return _endPointDefinitions[1];

      return null;
    }

    public IRelationEndPointDefinition GetOppositeEndPointDefinition (IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);
      return GetOppositeEndPointDefinition (endPointDefinition.ClassDefinition.ID, endPointDefinition.PropertyName);
    }

    public IRelationEndPointDefinition GetOppositeEndPointDefinition (string classID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      if (_endPointDefinitions[0].CorrespondsTo (classID, propertyName))
        return _endPointDefinitions[1];

      if (_endPointDefinitions[1].CorrespondsTo (classID, propertyName))
        return _endPointDefinitions[0];

      return null;
    }

    public ClassDefinition GetOppositeClassDefinition (IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);
      return GetOppositeClassDefinition (endPointDefinition.ClassDefinition.ID, endPointDefinition.PropertyName);
    }

    public ClassDefinition GetOppositeClassDefinition (string classID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      IRelationEndPointDefinition oppositeEndPointDefinition = GetOppositeEndPointDefinition (classID, propertyName);
      if (oppositeEndPointDefinition == null)
        return null;

      return oppositeEndPointDefinition.ClassDefinition;
    }

    public bool IsEndPoint (IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);
      return IsEndPoint (endPointDefinition.ClassDefinition.ID, endPointDefinition.PropertyName);
    }

    public bool IsEndPoint (string classID, string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      foreach (IRelationEndPointDefinition endPointDefinition in _endPointDefinitions)
      {
        if (endPointDefinition.CorrespondsTo (classID, propertyName))
          return true;
      }

      return false;
    }

    public bool Contains (IRelationEndPointDefinition endPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("endPointDefinition", endPointDefinition);

      if (object.ReferenceEquals (endPointDefinition, _endPointDefinitions[0]))
        return true;

      return object.ReferenceEquals (endPointDefinition, _endPointDefinitions[1]);
    }

    public override string ToString ()
    {
      return GetType().FullName + ": " + _id;
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }

    #region Serialization

    public override object GetRealObject (StreamingContext context)
    {
      return MappingConfiguration.Current.RelationDefinitions.GetMandatory (_id);
    }

    protected override bool IsPartOfMapping
    {
      get { return MappingConfiguration.Current.Contains (this); }
    }

    protected override string IDForExceptions
    {
      get { return ID; }
    }

    #endregion
  }
}
