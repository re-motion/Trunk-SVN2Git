using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement;
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
    public abstract void GroupAndRegisterRelatedObjects (IRelationEndPointDefinition relationEndPointDefinition, DomainObject[] originatingObjects, DomainObject[] relatedObjects, IDataManager dataManager);

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
  }
}