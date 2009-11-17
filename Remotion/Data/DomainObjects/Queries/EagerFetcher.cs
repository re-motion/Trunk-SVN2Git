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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using Remotion.Logging;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Used by the <see cref="IQueryManager"/> implementations in order to perform eager fetching of collection queries.
  /// See <see cref="IQuery.EagerFetchQueries"/> for more information on eager fetching.
  /// </summary>
  public class EagerFetcher
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (EagerFetcher));

    private readonly IQueryManager _queryManager;
    private readonly DomainObject[] _originalResults;
    private readonly RelationEndPointMap _relationEndPointMap;

    public EagerFetcher (IQueryManager queryManager, DomainObject[] originalResults)
    {
      ArgumentUtility.CheckNotNull ("queryManager", queryManager);
      ArgumentUtility.CheckNotNull ("originalResults", originalResults);

      _queryManager = queryManager;
      _originalResults = originalResults;
      _relationEndPointMap = _queryManager.ClientTransaction.DataManager.RelationEndPointMap;
    }

    public void PerformEagerFetching (IRelationEndPointDefinition relationEndPointDefinition, IQuery fetchQuery)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("fetchQuery", fetchQuery);

      s_log.DebugFormat (
          "Eager fetching objects for {0} via query {1} ('{2}').", relationEndPointDefinition.PropertyName, fetchQuery.ID, fetchQuery.Statement);

      var fetchedResult = _queryManager.GetCollection (fetchQuery);
      s_log.DebugFormat ("The eager fetch query yielded {0} related objects for {1} original objects.", fetchedResult.Count, _originalResults.Length);

      if (relationEndPointDefinition.Cardinality == CardinalityType.Many)
      {
        var collatedResult = CollateRelatedObjects (fetchQuery, relationEndPointDefinition, fetchedResult.AsEnumerable ());
        foreach (var originalObject in _originalResults)
        {
          if (originalObject != null)
          {
            CheckClassDefinitionOfOriginalObject (relationEndPointDefinition, originalObject);

            var relationEndPointID = new RelationEndPointID (originalObject.ID, relationEndPointDefinition);
            RegisterRelationResult (fetchQuery, relationEndPointID, collatedResult[originalObject].Distinct ());
          }
        }
      }
    }

    private MultiDictionary<DomainObject, DomainObject> CollateRelatedObjects (
        IQuery fetchQuery, IRelationEndPointDefinition relationEndPointDefinition, IEnumerable<DomainObject> relatedObjects)
    {
      var result = new MultiDictionary<DomainObject, DomainObject>();
      var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition ();
      var loggedNullRelatedObject = false;

      foreach (var relatedObject in relatedObjects)
      {
        if (relatedObject != null)
        {
          CheckClassDefinitionOfRelatedObject (fetchQuery, relationEndPointDefinition, relatedObject, oppositeEndPointDefinition);

          var originatingObject = _relationEndPointMap.GetRelatedObject (new RelationEndPointID (relatedObject.ID, oppositeEndPointDefinition), true);
          if (originatingObject != null)
            result.Add (originatingObject, relatedObject);
          else
            LogRelatedObjectPointingBackToNull (fetchQuery, relatedObject);
        }
        else if (!loggedNullRelatedObject)
        {
          LogNullRelatedObject (fetchQuery, relationEndPointDefinition);
          loggedNullRelatedObject = true;
        }
      }
      return result;
    }

    private void RegisterRelationResult (IQuery fetchQuery, RelationEndPointID relationEndPointID, IEnumerable<DomainObject> relatedObjects)
    {
      if (_relationEndPointMap[relationEndPointID] == null)
      {
        _relationEndPointMap.RegisterCollectionEndPoint (relationEndPointID, relatedObjects);
      }
      else
      {
        LogRelatedObjectsAlreadyRegistered(fetchQuery, relationEndPointID, relatedObjects);
      }
    }

    private void LogRelatedObjectsAlreadyRegistered (IQuery fetchQuery, RelationEndPointID relationEndPointID, IEnumerable<DomainObject> relatedObjects)
    {
      s_log.WarnFormat (
          "The eager fetch query '{0}' ('{1}') for relation end point '{2}' returned {3} related objects for object '{4}', but that object already "
          + "has {5} related objects.",
          fetchQuery.ID,
          fetchQuery.Statement,
          relationEndPointID.PropertyName,
          relatedObjects.Count(),
          relationEndPointID.ObjectID,
          ((CollectionEndPoint) _relationEndPointMap[relationEndPointID]).OppositeDomainObjects.Count);
    }

    private void LogNullRelatedObject (IQuery fetchQuery, IRelationEndPointDefinition relationEndPointDefinition)
    {
      s_log.WarnFormat (
          "The eager fetch query '{0}' ('{1}') for relation end point '{2}' returned at least one null object.",
          fetchQuery.ID,
          fetchQuery.Statement,
          relationEndPointDefinition.PropertyName);
    }

    private void LogRelatedObjectPointingBackToNull (IQuery fetchQuery, DomainObject relatedObject)
    {
      s_log.DebugFormat (
          "The eager fetch query '{0}' ('{1}') returned related object '{2}', which has a null original object.",
          fetchQuery.ID,
          fetchQuery.Statement,
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
        IQuery fetchQuery,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject relatedObject,
        IRelationEndPointDefinition oppositeEndPointDefinition)
    {
      if (!oppositeEndPointDefinition.ClassDefinition.IsSameOrBaseClassOf (relatedObject.ID.ClassDefinition))
      {
        var message = string.Format (
            "The eager fetch query '{0}' ('{1}') returned an object of an unexpected type. For relation end point '{2}', "
            + "an object of class '{3}' was expected, but an object of class '{4}' was returned.",
            fetchQuery.ID,
            fetchQuery.Statement,
            relationEndPointDefinition.PropertyName,
            oppositeEndPointDefinition.ClassDefinition.ID,
            relatedObject.ID.ClassDefinition.ID);

        throw new UnexpectedQueryResultException (message);
      }
    }
  }
}
