using System;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// The <see cref="IMetadataFactory"/> interface provides factory methods for creating the reflection objects used to create the 
  /// <see cref="BindableObjectClass"/> and it's associated attributes.
  /// </summary>
  public interface IMetadataFactory
  {
    IClassReflector CreateClassReflector (Type targetType, BindableObjectProvider businessObjectProvider);
    IPropertyFinder CreatePropertyFinder (Type concreteType);
    PropertyReflector CreatePropertyReflector (Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider);
  }
}