using System;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  public class DefaultMetadataFactory : IMetadataFactory
  {
    public static readonly DefaultMetadataFactory Instance = new DefaultMetadataFactory ();

    protected DefaultMetadataFactory ()
    {
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