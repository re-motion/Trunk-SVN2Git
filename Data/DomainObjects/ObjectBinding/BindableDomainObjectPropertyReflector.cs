using System;
using Remotion.Data.DomainObjects.Design;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// Use the <see cref="BindableDomainObjectPropertyReflector"/> to create <see cref="IBusinessObjectProperty"/> implementations for the 
  /// bindable domain object extension of the business object interfaces.
  /// </summary>
  public class BindableDomainObjectPropertyReflector : PropertyReflector
  {
    public static BindableDomainObjectPropertyReflector Create (Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      return ObjectFactory.Create <BindableDomainObjectPropertyReflector>().With (concreteType, propertyInfo, businessObjectProvider);
    }

    private PropertyDefinition _propertyDefinition;
    private IRelationEndPointDefinition _relationEndPointDefinition;

    protected BindableDomainObjectPropertyReflector (Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider)
        : base (propertyInfo, businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      InitializeMappingDefinitions (concreteType, propertyInfo);
    }

    private void InitializeMappingDefinitions (Type concreteType, IPropertyInformation propertyInfo)
    {
      Type targetType = Mixins.TypeUtility.GetUnderlyingTargetType (concreteType);

      if (DesignerUtility.IsDesignMode)
      {
        DomainObjectsDesignModeHelper domainObjectsDesignModeHelper = new DomainObjectsDesignModeHelper (DesignerUtility.DesignModeHelper);
        domainObjectsDesignModeHelper.InitializeConfiguration();
      }

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[targetType];
      if (classDefinition != null)
      {
        string propertyName = ReflectionUtility.GetPropertyName (propertyInfo.GetOriginalDeclaringType(), propertyInfo.Name);
        _propertyDefinition = classDefinition.GetPropertyDefinition (propertyName);
        _relationEndPointDefinition = classDefinition.GetRelationEndPointDefinition (propertyName);
      }
    }

    protected override bool GetIsRequired ()
    {
      if (_relationEndPointDefinition != null)
        return _relationEndPointDefinition.IsMandatory;
      else if (_propertyDefinition != null)
        return !_propertyDefinition.IsNullable;
      else
        return base.GetIsRequired();
    }

    protected override int? GetMaxLength ()
    {
      if (_propertyDefinition != null)
        return _propertyDefinition.MaxLength;
      else
        return base.GetMaxLength();
    }
  }
}