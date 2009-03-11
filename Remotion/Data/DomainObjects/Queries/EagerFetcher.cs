// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Used by the <see cref="IQueryManager"/> implementations in order to perform eager fetching of collection queries.
  /// See <see cref="IQuery.EagerFetchQueries"/> for more information on eager fetching.
  /// </summary>
  public class EagerFetcher
  {
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

      if (relationEndPointDefinition.Cardinality != CardinalityType.Many)
        throw new ArgumentException ("Eager fetching is only supported for collection-valued relation properties.", "relationEndPointDefinition");

      var fetchedResult = _queryManager.GetCollection (fetchQuery);

      var collatedResult = CollateRelatedObjects (fetchQuery, relationEndPointDefinition, fetchedResult.AsEnumerable ());
      foreach (var originalObject in _originalResults)
      {
        if (originalObject != null)
        {
          var relationEndPointID = new RelationEndPointID (originalObject.ID, relationEndPointDefinition);
          RegisterRelationResult (relationEndPointID, collatedResult[originalObject].Distinct());
        }
      }
    }

    private MultiDictionary<DomainObject, DomainObject> CollateRelatedObjects (IQuery fetchQuery, IRelationEndPointDefinition relationEndPointDefinition, IEnumerable<DomainObject> relatedObjects)
    {
      var result = new MultiDictionary<DomainObject, DomainObject> ();

      var oppositeEndPointDefinition = relationEndPointDefinition.RelationDefinition.GetOppositeEndPointDefinition (relationEndPointDefinition);
      foreach (var relatedObject in relatedObjects)
      {
        if (relatedObject != null)
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

          var originatingObject = _relationEndPointMap.GetRelatedObject (new RelationEndPointID (relatedObject.ID, oppositeEndPointDefinition), true);
          if (originatingObject != null)
            result.Add (originatingObject, relatedObject);          
        }
      }
      return result;
    }

    private void RegisterRelationResult (RelationEndPointID relationEndPointID, IEnumerable<DomainObject> relatedObjects)
    {
      if (_relationEndPointMap[relationEndPointID] == null)
      {
        var domainObjectCollection = DomainObjectCollection.Create (
            relationEndPointID.Definition.PropertyType,
            relatedObjects,
            relationEndPointID.OppositeEndPointDefinition.ClassDefinition.ClassType);

        _relationEndPointMap.RegisterCollectionEndPoint (relationEndPointID, domainObjectCollection);
      }
    }
  }
}