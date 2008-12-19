// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class IgnoresAnalyzer
  {
    private readonly MixinConfigurationBuilder _configurationBuilder;

    public IgnoresAnalyzer (MixinConfigurationBuilder configurationBuilder)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      _configurationBuilder = configurationBuilder;
    }

    public virtual void Analyze (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      foreach (IgnoresClassAttribute attribute in type.GetCustomAttributes (typeof (IgnoresClassAttribute), false))
        AnalyzeIgnoresClassAttribute (type, attribute);
      foreach (IgnoresMixinAttribute attribute in type.GetCustomAttributes (typeof (IgnoresMixinAttribute), false))
        AnalyzeIgnoresMixinAttribute (type, attribute);
    }

    public virtual void AnalyzeIgnoresClassAttribute (Type mixinType, IgnoresClassAttribute attribute)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      _configurationBuilder.ForClass (attribute.ClassToIgnore).SuppressMixin (mixinType);
    }

    public virtual void AnalyzeIgnoresMixinAttribute (Type targetClassType, IgnoresMixinAttribute attribute)
    {
      ArgumentUtility.CheckNotNull ("targetClassType", targetClassType);
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      _configurationBuilder.ForClass (targetClassType).SuppressMixin (attribute.MixinToIgnore);
    }
  }
}
