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
using System.Reflection;
using Remotion.Collections;
using Remotion.Mixins.Context.FluentBuilders;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class MixAnalyzer : RelationAnalyzerBase
  {
    private readonly Set<MixAttribute> _handledBindings = new Set<MixAttribute>();

    public MixAnalyzer (MixinConfigurationBuilder configurationBuilder) : base (configurationBuilder)
    {
    }

    public virtual void Analyze (ICustomAttributeProvider assembly)
    {
      foreach (MixAttribute attribute in assembly.GetCustomAttributes (typeof (MixAttribute), false))
      {
        if (!_handledBindings.Contains (attribute))
        {
          AnalyzeMixAttribute (attribute);
          _handledBindings.Add (attribute);
        }
      }
    }

    public virtual void AnalyzeMixAttribute (MixAttribute mixAttribute)
    {
      AddMixinAndAdjustException (mixAttribute.MixinKind, mixAttribute.TargetType, mixAttribute.MixinType, mixAttribute.IntroducedMemberVisibility, mixAttribute.AdditionalDependencies, mixAttribute.SuppressedMixins);
    }
  }
}
