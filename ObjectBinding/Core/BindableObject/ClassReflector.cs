using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Collections;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  public class ClassReflector
  {
    private readonly Type _targetType;
    private readonly Type _concreteType;
    private readonly BindableObjectProvider _businessObjectProvider;
    private readonly IMetadataFactory _metadataFactory;

    public ClassReflector (Type targetType, BindableObjectProvider businessObjectProvider, IMetadataFactory metadataFactory)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);
      ArgumentUtility.CheckNotNull ("metadataFactory", metadataFactory);

      _targetType = targetType;
      _concreteType = Mixins.TypeUtility.GetConcreteMixedType (_targetType);
      _businessObjectProvider = businessObjectProvider;
      _metadataFactory = metadataFactory;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public Type ConcreteType
    {
      get { return _concreteType; }
    }

    public BindableObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    }

    public BindableObjectClass GetMetadata ()
    {
      BindableObjectClass bindableObjectClass;
      if (typeof (IBusinessObjectWithIdentity).IsAssignableFrom (ConcreteType))
        bindableObjectClass = new BindableObjectClassWithIdentity (_concreteType, _businessObjectProvider);
      else
        bindableObjectClass = new BindableObjectClass (_concreteType, _businessObjectProvider);

      bindableObjectClass.SetProperties (GetProperties());

      return bindableObjectClass;
    }

    private IEnumerable<PropertyBase> GetProperties ()
    {
      IPropertyFinder propertyFinder = _metadataFactory.CreatePropertyFinder (_concreteType);

      Dictionary<string, PropertyBase> propertiesByName = new Dictionary<string, PropertyBase> ();
      foreach (IPropertyInformation propertyInfo in propertyFinder.GetPropertyInfos ())
      {
        PropertyReflector propertyReflector = _metadataFactory.CreatePropertyReflector (_concreteType, propertyInfo, _businessObjectProvider);
        PropertyBase property = propertyReflector.GetMetadata ();
        if (propertiesByName.ContainsKey (property.Identifier))
        {
          string message = string.Format ("Type '{0}' has two properties called '{1}', this is currently not supported.", TargetType.FullName,
              property.Identifier);
          throw new NotSupportedException (message);
        }
        else
          propertiesByName.Add (property.Identifier, property);
      }

      return propertiesByName.Values;
    }
  }
}