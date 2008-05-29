using System;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// The <see cref="BindableObjectMetadataFactory"/> implements the <see cref="IMetadataFactory"/> interface for the plain reflection based 
  /// bindable object implementation.
  /// </summary>
  public class BindableObjectMetadataFactory : IMetadataFactory
  {
    public static BindableObjectMetadataFactory Create()
    {
      return ObjectFactory.Create<BindableObjectMetadataFactory> (true).With();
    }

    protected BindableObjectMetadataFactory ()
    {
    }

    public virtual IClassReflector CreateClassReflector (Type targetType, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType); 
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return new ClassReflector (targetType, businessObjectProvider, this);
    }

    public virtual IPropertyFinder CreatePropertyFinder (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);

      return new ReflectionBasedPropertyFinder (concreteType);
    }

    public virtual PropertyReflector CreatePropertyReflector (Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return new PropertyReflector (propertyInfo, businessObjectProvider);
    }
  }
}