using System;

namespace Remotion.ObjectBinding
{
  // TODO FS: Move to OB.Interfaces
  //TODO: doc
  /// <summary> 
  public interface IBusinessObjectClassService : IBusinessObjectService
  {
    IBusinessObjectClass GetBusinessObjectClass (Type type);
  }
}