using System;
using System.Reflection;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  public interface IBindableObjectGlobalizationService : IBusinessObjectService
  {
    string GetEnumerationValueDisplayName (Enum value);
    string GetBooleanValueDisplayName (bool value);
    string GetPropertyDisplayName (IPropertyInformation info);
  }
}