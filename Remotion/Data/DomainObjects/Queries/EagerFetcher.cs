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
using System.Collections.Generic;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using Remotion.Logging;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Used by the <see cref="ObjectLoader"/> in order to perform eager fetching of collection queries.
  /// See <see cref="IQuery.EagerFetchQueries"/> for more information on eager fetching.
  /// </summary>
  [Serializable]
  public class EagerFetcher : IEagerFetcher
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (EagerFetcher));

    private readonly DataManager _dataManager;

    public EagerFetcher (DataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      _dataManager = dataManager;
    }

    public void CorrelateAndRegisterFetchResults (
        IEnumerable<DomainObject> originalObjects, 
        IEnumerable<DomainObject> fetchedObjects, 
        IRelationEndPointDefinition relationEndPointDefinition)
    {
      if (relationEndPointDefinition.Cardinality == CardinalityType.Many)
      {
        var collatedResult = CollateRelatedObjects (relationEndPointDefinition, fetchedObjects);
        foreach (var originalObject in originalObjects)
        {
          if (originalObject != null)
          {
            CheckClassDefinitionOfOriginalObject (relationEndPointDefinition, originalObject);

            var relationEndPointID = new RelationEndPointID (originalObject.ID, relationEndPointDefinition);
            RegisterRelationResult (relationEndPointID, collatedResult[originalObject].Distinct ());
          }
        }
      }
    }

    private MultiDictionary<DomainObject, DomainObject> CollateRelatedObjects (
        IRelationEndPointDefinition relationEndPointDefinition, 
        IEnumerable<DomainObject> relatedObjects)
    {
      var result = new MultiDictionary<DomainObject, DomainObject>();
      var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition ();
      var loggedNullRelatedObject = false;

      foreach (var relatedObject in relatedObjects)
      {
        if (relatedObject != null)
        {
          CheckClassDefinitionOfRelatedObject (relationEndPointDefinition, relatedObject, oppositeEndPointDefinition);

          var relationEndPointID = new RelationEndPointID (relatedObject.ID, oppositeEndPointDefinition);
          var originatingObject = _dataManager.RelationEndPointMap.GetRelatedObject (relationEndPointID, true);
          if (originatingObject != null)
            result.Add (originatingObject, relatedObject);
          else
            LogRelatedObjectPointingBackToNull (relatedObject);
        }
        else if (!loggedNullRelatedObject)
        {
          LogNullRelatedObject (relationEndPointDefinition);
          loggedNullRelatedObject = true;
        }
      }
      return result;
    }

    private void RegisterRelationResult (RelationEndPointID relationEndPointID, IEnumerable<DomainObject> relatedObjects)
    {
      if (_dataManager.RelationEndPointMap[relationEndPointID] == null)
      {
        _dataManager.RelationEndPointMap.RegisterCollectionEndPoint (relationEndPointID, relatedObjects);
      }
      else
      {
        LogRelatedObjectsAlreadyRegistered(relationEndPointID, relatedObjects);
      }
    }

    private void LogRelatedObjectsAlreadyRegistered (RelationEndPointID relationEndPointID, IEnumerable<DomainObject> relatedObjects)
    {
      s_log.WarnFormat (
          "The eager fetch query for relation end point '{0}' returned {1} related objects for object '{2}', but that object already "
          + "has {3} related objects.",
          relationEndPointID.Definition.PropertyName,
          relatedObjects.Count(),
          relationEndPointID.ObjectID,
          ((ICollectionEndPoint) _dataManager.RelationEndPointMap[relationEndPointID]).OppositeDomainObjects.Count);
    }

    private void LogNullRelatedObject (IRelationEndPointDefinition relationEndPointDefinition)
    {
      s_log.WarnFormat (
          "The eager fetch query for relation end point '{0}' returned at least one null object.",
          relationEndPointDefinition.PropertyName);
    }

    private void LogRelatedObjectPointingBackToNull (DomainObject relatedObject)
    {
      s_log.DebugFormat (
          "The eager fetch query returned related object '{0}', which has a null original object.",
          relatedObject.ID);
    }

    private void CheckClassDefinitionOfOriginalObject (IRelationEndPointDefinition relationEndPointDefinition, DomainObject originalObject)
    {
      if (!relationEndPointDefinition.ClassDefinition.IsSameOrBaseClassOf (originalObject.ID.ClassDefinition))
      {
        var message = string.Format (
            "Eager fetching cannot be performed for query result object '{0}' and relation end point '{1}'. The end point belongs to an object of "
            + "class '{2}' but the query result has class '{3}'.",
            originalObject.ID,
            relationEndPointDefinition.PropertyName,
            relationEndPointDefinition.ClassDefinition.ID,
            originalObject.ID.ClassDefinition.ID);

        throw new UnexpectedQueryResultException (message);
      }
    }

    private void CheckClassDefinitionOfRelatedObject (
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject relatedObject,
        IRelationEndPointDefinition oppositeEndPointDefinition)
    {
      if (!oppositeEndPointDefinition.ClassDefinition.IsSameOrBaseClassOf (relatedObject.ID.ClassDefinition))
      {
        var message = string.Format (
            "An eager fetch query returned an object of an unexpected type. For relation end point '{0}', "
            + "an object of class '{1}' was expected, but an object of class '{2}' was returned.",
            relationEndPointDefinition.PropertyName,
            oppositeEndPointDefinition.ClassDefinition.ID,
            relatedObject.ID.ClassDefinition.ID);

        throw new UnexpectedQueryResultException (message);
      }
    }
  }
}
