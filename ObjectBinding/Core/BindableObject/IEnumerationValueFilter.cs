using System;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  public interface IEnumerationValueFilter
  {
    bool IsEnabled (IEnumerationValueInfo value, IBusinessObject businessObject, IBusinessObjectEnumerationProperty property);
  }
}