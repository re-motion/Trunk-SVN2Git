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

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class ExtendsAnalyzer : RelationAnalyzerBase
  {
    public ExtendsAnalyzer (MixinConfigurationBuilder configurationBuilder) : base (configurationBuilder)
    {
    }

    public virtual void Analyze (Type extender)
    {
      foreach (ExtendsAttribute mixinAttribute in extender.GetCustomAttributes (typeof (ExtendsAttribute), false))
        AnalyzeExtendsAttribute (extender, mixinAttribute);
    }

    public virtual void AnalyzeExtendsAttribute (Type extender, ExtendsAttribute mixinAttribute)
    {
      Type mixinType = ApplyMixinTypeArguments (mixinAttribute.TargetType, extender, mixinAttribute.MixinTypeArguments);
      AddMixinAndAdjustException (MixinKind.Extending, mixinAttribute.TargetType, mixinType, mixinAttribute.IntroducedMemberVisibility, mixinAttribute.AdditionalDependencies, mixinAttribute.SuppressedMixins);
    }

    private Type ApplyMixinTypeArguments (Type targetType, Type mixinType, Type[] typeArguments)
    {
      try
      {
        if (typeArguments.Length > 0)
          return mixinType.MakeGenericType (typeArguments);
        else
          return mixinType;
      }
      catch (Exception ex)
      {
        string message = string.Format (
            "The ExtendsAttribute for target class {0} applied to mixin type {1} specified invalid generic type "
                + "arguments.",
            targetType.FullName,
            mixinType.FullName);
        throw new ConfigurationException (message, ex);
      }
    }
  }
}
