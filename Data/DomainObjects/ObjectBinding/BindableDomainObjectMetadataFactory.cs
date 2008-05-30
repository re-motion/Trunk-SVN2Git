using System;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// The <see cref="BindableDomainObjectMetadataFactory"/> implements the <see cref="IMetadataFactory"/> interface for domain objects.
  /// </summary>
  public class BindableDomainObjectMetadataFactory : IMetadataFactory
  {
    public static BindableDomainObjectMetadataFactory Create ()
    {
      return ObjectFactory.Create<BindableDomainObjectMetadataFactory> (true).With();
    }

    protected BindableDomainObjectMetadataFactory ()
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
      return new BindableDomainObjectPropertyFinder (concreteType);
    }

    public virtual PropertyReflector CreatePropertyReflector (Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return BindableDomainObjectPropertyReflector.Create (concreteType, propertyInfo, businessObjectProvider);
    }
  }
}