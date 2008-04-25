using System;
using System.Reflection;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.ObjectBinding.BindableObject
{
  public interface IMetadataFactory
  {
    IPropertyFinder CreatePropertyFinder (Type concreteType);
    PropertyReflector CreatePropertyReflector (Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider);
  }
}