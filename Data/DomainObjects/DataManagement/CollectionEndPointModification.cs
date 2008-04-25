using System;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class CollectionEndPointModification : RelationEndPointModification
  {
    public readonly CollectionEndPointChangeAgent ChangeAgent;

    private readonly CollectionEndPoint _affectedEndPoint;

    public CollectionEndPointModification (CollectionEndPoint affectedEndPoint, CollectionEndPointChangeAgent changeAgent)
      : base (affectedEndPoint, changeAgent.OldEndPoint, changeAgent.NewEndPoint)
    {
      _affectedEndPoint = affectedEndPoint;
      ChangeAgent = changeAgent;
    }

    public override void Begin ()
    {
      ChangeAgent.BeginRelationChange();
      base.Begin();
    }

    public override void Perform ()
    {
      _affectedEndPoint.PerformRelationChange (this);
    }

    public override void End ()
    {
      ChangeAgent.EndRelationChange();
      base.End();
    }
  }
}