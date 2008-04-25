using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
public class NullObjectEndPoint : ObjectEndPoint
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public NullObjectEndPoint (IRelationEndPointDefinition definition) : base (definition)
  {
  }

  // methods and properties

  public override RelationEndPointModification CreateModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
  {
    return new NullEndPointModification (this, oldEndPoint, newEndPoint);
  }

  public override void NotifyClientTransactionOfBeginRelationChange (IEndPoint oldEndPoint, IEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);
  }

  public override void NotifyClientTransactionOfEndRelationChange ()
  {
  }

  public override void PerformDelete ()
  {
    throw new InvalidOperationException ("PerformDelete cannot be called on a NullObjectEndPoint.");    
  }

  public override void CheckMandatory ()
  {
    throw new InvalidOperationException ("CheckMandatory cannot be called on a NullObjectEndPoint.");    
  }

  public override void Commit ()
  {
    throw new InvalidOperationException ("Commit cannot be called on a NullObjectEndPoint.");    
  }

  public override void Rollback ()
  {
    throw new InvalidOperationException ("Rollback cannot be called on a NullObjectEndPoint.");
  }

  public override bool HasChanged
  {
    get { return false; }
  }

  public override DataContainer GetDataContainer ()
  {
    return null;
  }

  public override DomainObject GetDomainObject ()
  {
    return null;
  }

  public override ObjectID ObjectID
  {
    get { return null; }
  }

  public override RelationEndPointID ID
  {
    get { return null; }
  }

  public override bool IsNull
  {
    get { return true; }
  }

  protected override void SerializeIntoFlatStructure (Remotion.Data.DomainObjects.Infrastructure.FlattenedSerializationInfo info)
  {
    throw new InvalidOperationException ("Rollback cannot be called on a NullCollectionEndPoint.");
  }
}
}
