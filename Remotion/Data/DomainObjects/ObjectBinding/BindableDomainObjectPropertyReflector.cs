// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Data.DomainObjects.Design;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;
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
      return ObjectFactory.Create <BindableDomainObjectPropertyReflector> (true, ParamList.Create (concreteType, propertyInfo, businessObjectProvider));
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
      Type targetType = Mixins.MixinTypeUtility.GetUnderlyingTargetType (concreteType);

      if (DesignerUtility.IsDesignMode)
      {
        DomainObjectsDesignModeHelper domainObjectsDesignModeHelper = new DomainObjectsDesignModeHelper (DesignerUtility.DesignModeHelper);
        domainObjectsDesignModeHelper.InitializeConfiguration();
      }

      // TODO: use GetMandatory
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[targetType];
      if (classDefinition != null)
      {
        string propertyName = MappingConfiguration.Current.NameResolver.GetPropertyName (propertyInfo.GetOriginalDeclaringType (), propertyInfo.Name);
        _propertyDefinition = classDefinition.GetPropertyDefinition (propertyName);
        _relationEndPointDefinition = classDefinition.GetRelationEndPointDefinition (propertyName);
      }
    }

    protected override bool GetIsRequired ()
    {
      if (_relationEndPointDefinition != null)
        return _relationEndPointDefinition.IsMandatory;
      else if (_propertyDefinition != null && !_propertyDefinition.PropertyType.IsValueType)
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
