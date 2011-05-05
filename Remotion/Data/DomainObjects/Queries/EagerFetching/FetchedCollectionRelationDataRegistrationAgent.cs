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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Implements <see cref="IRelationEndPointRegistrationAgent"/> for collection-valued relation end-points.
  /// </summary>
  [Serializable]
  public class FetchedCollectionRelationDataRegistrationAgent : FetchedRelationDataRegistrationAgentBase
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (FetchedCollectionRelationDataRegistrationAgent));

    public override void GroupAndRegisterRelatedObjects (
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject[] originatingObjects,
        DomainObject[] relatedObjects,
        IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("originatingObjects", originatingObjects);
      ArgumentUtility.CheckNotNull ("relatedObjects", relatedObjects);

      if (relationEndPointDefinition.Cardinality != CardinalityType.Many || relationEndPointDefinition.IsAnonymous)
        throw new ArgumentException ("Only collection-valued relations can be handled by this registration agent.", "relationEndPointDefinition");

      CheckOriginatingObjects (relationEndPointDefinition, originatingObjects);

      var groupRelatedObjects = GroupRelatedObjectsByForeignKey (relatedObjects, relationEndPointDefinition, dataManager);
      MarkCollectionEndPointsComplete (relationEndPointDefinition, dataManager, originatingObjects, groupRelatedObjects);
    }

    private ILookup<ObjectID, DomainObject> GroupRelatedObjectsByForeignKey (
        IEnumerable<DomainObject> relatedObjects, 
        IRelationEndPointDefinition relationEndPointDefinition,
        IDataManager dataManager)
    {
      var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition ();

      // The DataContainer for relatedObject has been registered when the IObjectLoader executed the query, so we can use it to correlate the related
      // objects with the originating objects.
      // Bug: DataContainers from the ClientTransaction might mix with the query result, see  https://www.re-motion.org/jira/browse/RM-3995.
      var relatedObjectsWithForeignKey =
          from relatedObject in relatedObjects
          where relatedObject != null
          let dataContainer =
              CheckRelatedObjectAndGetDataContainer (relatedObject, relationEndPointDefinition, oppositeEndPointDefinition, dataManager)
          let propertyValue = dataContainer.PropertyValues[oppositeEndPointDefinition.PropertyName]
          let originatingObjectID = (ObjectID) propertyValue.GetValueWithoutEvents (ValueAccess.Current)
          select new { originatingObjectID, relatedObject };
      
      return relatedObjectsWithForeignKey.ToLookup (k => k.originatingObjectID, k => k.relatedObject);
    }

    private DataContainer CheckRelatedObjectAndGetDataContainer (
        DomainObject relatedObject,
        IRelationEndPointDefinition relationEndPointDefinition, 
        IRelationEndPointDefinition oppositeEndPointDefinition, 
        IDataManager dataManager)
    {
      CheckClassDefinitionOfRelatedObject (relationEndPointDefinition, relatedObject, oppositeEndPointDefinition);
      return dataManager.GetDataContainerWithoutLoading (relatedObject.ID);
    }

    private void MarkCollectionEndPointsComplete (
       IRelationEndPointDefinition relationEndPointDefinition,
       IDataManager dataManager,
       IEnumerable<DomainObject> originatingObjects,
       ILookup<ObjectID, DomainObject> groupedRelatedObjects)
    {
      var relatedObjectsByOriginalObject = groupedRelatedObjects;
      foreach (var originalObject in originatingObjects)
      {
        if (originalObject != null)
        {
          var relationEndPointID = RelationEndPointID.Create (originalObject.ID, relationEndPointDefinition);
          var relatedObjects = relatedObjectsByOriginalObject[originalObject.ID];
          if (!dataManager.TrySetCollectionEndPointData (relationEndPointID, relatedObjects.ToArray ()))
            s_log.DebugFormat ("Relation data for relation end-point '{0}' is discarded; the end-point has already been loaded.", relationEndPointID);
        }
      }
    }
  }
}