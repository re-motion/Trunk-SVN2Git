using System;

namespace Remotion.ObjectBinding
{
  //TODO: doc
  /// <summary> 
  public interface IBusinessObjectClassService : IBusinessObjectService
  {
    IBusinessObjectClass GetBusinessObjectClass (Type type);
  }
}