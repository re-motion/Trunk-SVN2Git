using System;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class NullEndPointModification : RelationEndPointModification
  {
    public NullEndPointModification (RelationEndPoint affectedEndPoint, IEndPoint oldEndPoint, IEndPoint newEndPoint)
        : base (affectedEndPoint, oldEndPoint, newEndPoint)
    {
    }

    public override void Begin ()
    {
      // do nothing
    }

    public override void Perform ()
    {
      // do nothing
    }

    public override void End ()
    {
      // do nothing
    }

    public override void NotifyClientTransactionOfBegin ()
    {
      // do nothing
    }

    public override void NotifyClientTransactionOfEnd ()
    {
      // do nothing
    }
  }
}