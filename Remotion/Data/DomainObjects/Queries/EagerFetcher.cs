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
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Logging;
using Remotion.Utilities;

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

    public void PerformEagerFetching (
        DomainObject[] originalObjects, 
        IRelationEndPointDefinition relationEndPointDefinition, 
        IQuery fetchQuery, 
        IObjectLoader fetchQueryResultLoader,
        IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("originalObjects", originalObjects);
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("fetchQuery", fetchQuery);
      ArgumentUtility.CheckNotNull ("fetchQueryResultLoader", fetchQueryResultLoader);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      s_log.DebugFormat (
          "Eager fetching objects for {0} via query {1} ('{2}').",
          relationEndPointDefinition.PropertyName,
          fetchQuery.ID,
          fetchQuery.Statement);

      CheckOriginalObjects (relationEndPointDefinition, originalObjects);

      // Executing the query will automatically register all virtual end-points. Collection end-points will be explicitly marked complete below
      var fetchedObjects = fetchQueryResultLoader.LoadCollectionQueryResult<DomainObject> (fetchQuery, dataManager);
      s_log.DebugFormat (
          "The eager fetch query yielded {0} related objects for {1} original objects.",
          fetchedObjects.Length,
          originalObjects.Length);

      CheckFetchedObjects (relationEndPointDefinition, fetchedObjects);

      if (relationEndPointDefinition.Cardinality == CardinalityType.Many)
        MarkCollectionEndPointsComplete (relationEndPointDefinition, dataManager, originalObjects, fetchedObjects);
    }

    private void MarkCollectionEndPointsComplete (
        IRelationEndPointDefinition relationEndPointDefinition,
        IDataManager dataManager,
        IEnumerable<DomainObject> originalObjects,
        IEnumerable<DomainObject> fetchedObjects)
    {
      var relatedObjectsByOriginalObject = GroupFetchedObjectsOneMany (fetchedObjects, relationEndPointDefinition, dataManager);
      foreach (var originalObject in originalObjects)
      {
        if (originalObject != null)
        {
          var relationEndPointID = RelationEndPointID.Create (originalObject.ID, relationEndPointDefinition);
          var relatedObjects = relatedObjectsByOriginalObject[originalObject.ID];
          try
          {
            dataManager.MarkCollectionEndPointComplete (relationEndPointID, relatedObjects.ToArray ());
          }
          catch (InvalidOperationException ex)
          {
            s_log.WarnFormat (
                ex, 
                "Eager fetch result for relation end-point '{0}' is discarded; the end-point has already been loaded.", 
                relationEndPointID);
          }
        }
      }
    }

    private MultiDictionary<ObjectID, DomainObject> GroupFetchedObjectsOneMany (
        IEnumerable<DomainObject> relatedObjects, 
        IRelationEndPointDefinition relationEndPointDefinition,
        IDataManager dataManager)
    {
      var result = new MultiDictionary<ObjectID, DomainObject> ();
      var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition ();

      foreach (var relatedObject in relatedObjects)
      {
        if (relatedObject != null)
        {
          // The DataContainer for relatedObject has been registered when the IObjectLoader executed the query
          var dataContainer = dataManager.GetDataContainerWithoutLoading (relatedObject.ID);
          Assertion.IsNotNull (dataContainer);

          var propertyValue = dataContainer.PropertyValues[oppositeEndPointDefinition.PropertyName];
          var originatingObjectID = (ObjectID) propertyValue.GetValueWithoutEvents (ValueAccess.Current);
          if (originatingObjectID != null)
            result.Add (originatingObjectID, relatedObject);
        }
      }
      return result;
    }

    private void CheckOriginalObjects (IRelationEndPointDefinition relationEndPointDefinition, IEnumerable<DomainObject> originalObjects)
    {
      foreach (var originalObject in originalObjects)
      {
        if (originalObject != null)
          CheckClassDefinitionOfOriginalObject (relationEndPointDefinition, originalObject);
      }
    }

    private void CheckFetchedObjects (
        IRelationEndPointDefinition relationEndPointDefinition, 
        IEnumerable<DomainObject> fetchedObjects)
    {
      var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition ();
      var loggedNullRelatedObject = false;

      foreach (var fetchedObject in fetchedObjects)
      {
        if (fetchedObject != null)
        {
          CheckClassDefinitionOfRelatedObject (relationEndPointDefinition, fetchedObject, oppositeEndPointDefinition);
        }
        else if (!loggedNullRelatedObject)
        {
          LogNullRelatedObject (relationEndPointDefinition);
          loggedNullRelatedObject = true;
        }
      }
    }

    private void LogNullRelatedObject (IRelationEndPointDefinition relationEndPointDefinition)
    {
      s_log.WarnFormat (
          "The eager fetch query for relation end point '{0}' returned at least one null object.",
          relationEndPointDefinition.PropertyName);
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
