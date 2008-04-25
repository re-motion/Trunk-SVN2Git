using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides a mechanism for retrieving all the <see cref="DomainObject"/> instances directly or indirectly referenced by a root object via
  /// <see cref="PropertyKind.RelatedObject"/> and <see cref="PropertyKind.RelatedObjectCollection"/> properties. A
  /// <see cref="IGraphTraversalStrategy"/> can be given to decide which objects to include and which links to follow when traversing the
  /// object graph.
  /// </summary>
  public class DomainObjectGraphTraverser
  {
    private readonly IGraphTraversalStrategy _strategy;
    private readonly DomainObject _rootObject;

    internal DomainObjectGraphTraverser (DomainObject rootObject, IGraphTraversalStrategy strategy)
    {
      ArgumentUtility.CheckNotNull ("rootObject", rootObject);
      ArgumentUtility.CheckNotNull ("strategy", strategy);

      _rootObject = rootObject;
      _strategy = strategy;
    }

    /// <summary>
    /// Gets the flattened related object graph for the root <see cref="DomainObject"/> associated with this traverser.
    /// </summary>
    /// <returns>A <see cref="Set{T}"/> of <see cref="DomainObject"/> instances containing the root object and all objects directly or indirectly
    /// referenced by it.</returns>
    // Note: Implemented nonrecursively in order to support very large graphs.
    public Set<DomainObject> GetFlattenedRelatedObjectGraph ()
    {
      Set<DomainObject> visited = new Set<DomainObject> ();
      Set<DomainObject> resultSet = new Set<DomainObject> ();
      Set<Tuple<DomainObject, int>> objectsToBeProcessed = new Set<Tuple<DomainObject, int>> (Tuple.NewTuple (_rootObject, 0));

      while (objectsToBeProcessed.Count > 0)
      {
        Tuple<DomainObject, int> current = objectsToBeProcessed.GetAny ();
        objectsToBeProcessed.Remove (current);
        if (!visited.Contains (current.A))
        {
          visited.Add (current.A);
          if (_strategy.ShouldProcessObject (current.A))
            resultSet.Add (current.A);
          objectsToBeProcessed.AddRange (GetNextTraversedObjects (current.A, current.B, _strategy));
        }
      }

      return resultSet;
    }

    protected virtual IEnumerable<Tuple<DomainObject, int>> GetNextTraversedObjects (DomainObject current, int currentDepth, IGraphTraversalStrategy strategy)
    {
      foreach (PropertyAccessor property in current.Properties)
      {
        switch (property.Kind)
        {
          case PropertyKind.RelatedObject:
            if (strategy.ShouldFollowLink (_rootObject, current, currentDepth, property))
            {
              DomainObject relatedObject = (DomainObject) property.GetValueWithoutTypeCheck ();
              if (relatedObject != null)
                yield return Tuple.NewTuple (relatedObject, currentDepth + 1);
            }
            break;
          case PropertyKind.RelatedObjectCollection:
            if (strategy.ShouldFollowLink (_rootObject, current, currentDepth, property))
            {
              foreach (DomainObject relatedObject in (DomainObjectCollection) property.GetValueWithoutTypeCheck ())
              {
                if (relatedObject != null)
                  yield return Tuple.NewTuple (relatedObject, currentDepth + 1);
              }
            }
            break;
        }
      }
    }
  }
}