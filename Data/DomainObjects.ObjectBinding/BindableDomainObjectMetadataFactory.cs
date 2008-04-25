using System;
using System.Reflection;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
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