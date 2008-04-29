using System;

namespace Remotion.ObjectBinding
{
  /// <summary> 
  //TODO: doc
  public interface IBusinessObjectStringFormatterService : IBusinessObjectService
  {
    string GetPropertyString (IBusinessObject businessObject, IBusinessObjectProperty property, string format);
  }
}