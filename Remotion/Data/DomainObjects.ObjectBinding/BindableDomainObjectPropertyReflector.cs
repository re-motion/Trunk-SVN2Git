// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Reflection;
using Remotion.TypePipe;
using Remotion.Utilities;
using ParamList = Remotion.TypePipe.ParamList;
using PropertyReflector = Remotion.ObjectBinding.BindableObject.PropertyReflector;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// Use the <see cref="BindableDomainObjectPropertyReflector"/> to create <see cref="IBusinessObjectProperty"/> implementations for the 
  /// bindable domain object extension of the business object interfaces.
  /// </summary>
  public class BindableDomainObjectPropertyReflector : PropertyReflector
  {
    public static BindableDomainObjectPropertyReflector Create (IPropertyInformation propertyInfo,
        BindableObjectProvider businessObjectProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IDefaultValueStrategy defaultValueStrategy)
    {
      return ObjectFactory.Create<BindableDomainObjectPropertyReflector> (
          true,
          ParamList.Create (propertyInfo, businessObjectProvider, domainModelConstraintProvider, defaultValueStrategy));
    }

    private readonly IPropertyInformation _propertyInfo;
    private readonly IDefaultValueStrategy _defaultValueStrategy;
    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;

    protected BindableDomainObjectPropertyReflector (IPropertyInformation propertyInfo,
        BindableObjectProvider businessObjectProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IDefaultValueStrategy defaultValueStrategy)
        : base (propertyInfo, businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);
      ArgumentUtility.CheckNotNull ("domainModelConstraintProvider", domainModelConstraintProvider);
      ArgumentUtility.CheckNotNull ("defaultValueStrategy", defaultValueStrategy);

      _propertyInfo = propertyInfo;
      _domainModelConstraintProvider = domainModelConstraintProvider;
      _defaultValueStrategy = defaultValueStrategy;
    }

    protected override bool GetIsRequired ()
    {
      if (base.GetIsRequired())
        return true;
      return !_domainModelConstraintProvider.IsNullable (_propertyInfo);
    }

    protected override int? GetMaxLength ()
    {
      return base.GetMaxLength() ?? _domainModelConstraintProvider.GetMaxLength (_propertyInfo);
    }

    protected override IDefaultValueStrategy GetDefaultValueStrategy ()
    {
      return _defaultValueStrategy;
    }
  }
}