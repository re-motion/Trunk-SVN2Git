using System;

namespace Remotion.ObjectBinding
{
  // TODO FS: Move to OB.Interfaces
  /// <summary> 
  //TODO: doc
  public interface IBusinessObjectStringFormatterService : IBusinessObjectService
  {
    string GetPropertyString (IBusinessObject businessObject, IBusinessObjectProperty property, string format);
  }
}