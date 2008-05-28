using System;
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
    public static readonly BindableDomainObjectMetadataFactory Instance = new BindableDomainObjectMetadataFactory ();

    protected BindableDomainObjectMetadataFactory ()
    {
    }

    public IPropertyFinder CreatePropertyFinder (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      return new BindableDomainObjectPropertyFinder (concreteType);
    }

    public PropertyReflector CreatePropertyReflector (Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return new BindableDomainObjectPropertyReflector (concreteType, propertyInfo, businessObjectProvider);
    }
  }
}