/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
      AddMixinAndAdjustException (MixinKind.Extending, mixinAttribute.TargetType, mixinType, mixinAttribute.AdditionalDependencies, mixinAttribute.SuppressedMixins);
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
