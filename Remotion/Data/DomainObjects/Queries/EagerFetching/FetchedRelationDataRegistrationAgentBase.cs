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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Provides common functionality for implementations of <see cref="IFetchedRelationDataRegistrationAgent"/>.
  /// </summary>
  [Serializable]
  public abstract class FetchedRelationDataRegistrationAgentBase : IFetchedRelationDataRegistrationAgent
  {
    public abstract void GroupAndRegisterRelatedObjects (IRelationEndPointDefinition relationEndPointDefinition, DomainObject[] originatingObjects, DomainObject[] relatedObjects, ILoadedDataContainerProvider loadedDataContainerProvider, IRelationEndPointProvider relationEndPointProvider);

    protected void CheckOriginatingObjects (IRelationEndPointDefinition relationEndPointDefinition, IEnumerable<DomainObject> originatingObjects)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("originatingObjects", originatingObjects);

      foreach (var originatingObject in originatingObjects)
      {
        if (originatingObject != null)
          CheckClassDefinitionOfOriginatingObject (relationEndPointDefinition, originatingObject);
      }
    }

    protected void CheckRelatedObjects (
        IRelationEndPointDefinition relationEndPointDefinition,
        IEnumerable<DomainObject> relatedObjects)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("relatedObjects", relatedObjects);

      var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition ();

      foreach (var fetchedObject in relatedObjects)
      {
        if (fetchedObject != null)
          CheckClassDefinitionOfRelatedObject (relationEndPointDefinition, fetchedObject, oppositeEndPointDefinition);
      }
    }

    protected void CheckClassDefinitionOfOriginatingObject (IRelationEndPointDefinition relationEndPointDefinition, DomainObject originatingObject)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("originatingObject", originatingObject);

      if (!relationEndPointDefinition.ClassDefinition.IsSameOrBaseClassOf (originatingObject.ID.ClassDefinition))
      {
        var message = string.Format (
            "Cannot register relation end-point '{0}' for domain object '{1}'. The end-point belongs to an object of "
            + "class '{2}' but the domain object has class '{3}'.",
            relationEndPointDefinition.PropertyName,
            originatingObject.ID,
            relationEndPointDefinition.ClassDefinition.ID,
            originatingObject.ID.ClassDefinition.ID);

        throw new InvalidOperationException (message);
      }
    }

    protected void CheckClassDefinitionOfRelatedObject (
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject relatedObject,
        IRelationEndPointDefinition oppositeEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("relatedObject", relatedObject);
      ArgumentUtility.CheckNotNull ("oppositeEndPointDefinition", oppositeEndPointDefinition);

      if (!oppositeEndPointDefinition.ClassDefinition.IsSameOrBaseClassOf (relatedObject.ID.ClassDefinition))
      {
        var message = string.Format (
            "Cannot associate object '{0}' with the relation end-point '{1}'. An object of type '{2}' was expected.",
            relatedObject.ID,
            relationEndPointDefinition.PropertyName,
            oppositeEndPointDefinition.ClassDefinition.ClassType);

        throw new InvalidOperationException (message);
      }
    }

    protected IEnumerable<Tuple<ObjectID, DomainObject>> GetForeignKeysForVirtualEndPointDefinition (
        IEnumerable<DomainObject> domainObjects, 
        VirtualRelationEndPointDefinition virtualEndPointDefinition, 
        ILoadedDataContainerProvider loadedDataContainerProvider)
    {
      var oppositeEndPointDefinition = (RelationEndPointDefinition) virtualEndPointDefinition.GetOppositeEndPointDefinition ();

      // The DataContainer for relatedObject has been registered when the IObjectLoader executed the query, so we can use it to correlate the related
      // objects with the originating objects.
      // Bug: DataContainers from the ClientTransaction might mix with the query result, see  https://www.re-motion.org/jira/browse/RM-3995.
      return from relatedObject in domainObjects
             where relatedObject != null
             let dataContainer =
                 CheckRelatedObjectAndGetDataContainer (relatedObject, virtualEndPointDefinition, oppositeEndPointDefinition, loadedDataContainerProvider)
             let propertyValue = dataContainer.PropertyValues[oppositeEndPointDefinition.PropertyDefinition.PropertyName]
             let originatingObjectID = (ObjectID) propertyValue.GetValueWithoutEvents (ValueAccess.Current)
             select Tuple.Create (originatingObjectID, relatedObject);
    }

    private DataContainer CheckRelatedObjectAndGetDataContainer (
        DomainObject relatedObject,
        IRelationEndPointDefinition relationEndPointDefinition, 
        IRelationEndPointDefinition oppositeEndPointDefinition,
        ILoadedDataContainerProvider loadedDataContainerProvider)
    {
      CheckClassDefinitionOfRelatedObject (relationEndPointDefinition, relatedObject, oppositeEndPointDefinition);
      return loadedDataContainerProvider.GetDataContainerWithoutLoading (relatedObject.ID);
    }
  }
}