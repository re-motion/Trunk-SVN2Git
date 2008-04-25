using System;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class ObjectEndPointModification : RelationEndPointModification
  {
    private readonly ObjectEndPoint _affectedEndPoint;

    public ObjectEndPointModification (ObjectEndPoint affectedEndPoint, IEndPoint oldEndPoint, IEndPoint newEndPoint)
        : base (affectedEndPoint, oldEndPoint, newEndPoint)
    {
      _affectedEndPoint = affectedEndPoint;
    }

    public override void Perform ()
    {
      _affectedEndPoint.PerformRelationChange (this);
    }
  }
}