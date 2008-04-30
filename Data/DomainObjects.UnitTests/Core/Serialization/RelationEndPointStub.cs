using System;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Serialization
{
  [Serializable]
  public class RelationEndPointStub : RelationEndPoint
  {
    public RelationEndPointStub (ClientTransaction clientTransaction, RelationEndPointID id)
        : base (clientTransaction, id)
    {
    }

    public RelationEndPointStub (FlattenedDeserializationInfo info)
      : base (info)
    {
    }


    public override RelationEndPoint Clone ()
    {
      throw new NotImplementedException();
    }

    protected override void AssumeSameState (RelationEndPoint source)
    {
      throw new NotImplementedException();
    }

    protected override void TakeOverCommittedData (RelationEndPoint source)
    {
      throw new NotImplementedException();
    }

    protected override void RegisterWithMap (RelationEndPointMap map)
    {
      throw new NotImplementedException();
    }

    public override bool HasChanged
    {
      get { throw new NotImplementedException(); }
    }

    public override bool HasBeenTouched
    {
      get { throw new NotImplementedException(); }
    }

    protected override void Touch ()
    {
      throw new NotImplementedException();
    }

    public override void Commit ()
    {
      throw new NotImplementedException();
    }

    public override void Rollback ()
    {
      throw new NotImplementedException();
    }

    public override void CheckMandatory ()
    {
      throw new NotImplementedException();
    }

    public override void PerformDelete ()
    {
      throw new NotImplementedException();
    }

    public override RelationEndPointModification CreateModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      throw new NotImplementedException();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
    }
  }
}