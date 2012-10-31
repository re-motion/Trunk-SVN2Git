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

using System;
using System.Reflection;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.AssemblyLevelMixinDependencies
{
  [AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true)]
  public class AdditionalMixinDependencyAttribute : Attribute, IMixinConfigurationAttribute<Assembly>
  {
    private readonly Type _targetType;
    private readonly Type _dependentMixin;
    private readonly Type _dependency;

    public AdditionalMixinDependencyAttribute (Type targetType, Type dependentMixin, Type dependency)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("dependentMixin", dependentMixin);
      ArgumentUtility.CheckNotNull ("dependency", dependency);

      _targetType = targetType;
      _dependentMixin = dependentMixin;
      _dependency = dependency;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public Type DependentMixin
    {
      get { return _dependentMixin; }
    }

    public Type Dependency
    {
      get { return _dependency; }
    }

    public bool IgnoresDuplicates
    {
      get { return false; }
    }

    public void Apply (MixinConfigurationBuilder configurationBuilder, Assembly attributeTarget)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      ArgumentUtility.CheckNotNull ("attributeTarget", attributeTarget);

      configurationBuilder.ForClass (TargetType).WithMixinDependency (DependentMixin, Dependency);
    }
  }
}