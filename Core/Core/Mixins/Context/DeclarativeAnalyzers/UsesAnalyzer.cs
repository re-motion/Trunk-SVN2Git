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
  public class UsesAnalyzer : RelationAnalyzerBase
  {
    public UsesAnalyzer (MixinConfigurationBuilder configurationBuilder) : base (configurationBuilder)
    {
    }

    public virtual void Analyze (Type targetType)
    {
      foreach (UsesAttribute usesAttribute in targetType.GetCustomAttributes (typeof (UsesAttribute), false))
        AnalyzeUsesAttribute (targetType, usesAttribute);
    }

    public virtual void AnalyzeUsesAttribute (Type targetType, UsesAttribute usesAttribute)
    {
      AddMixinAndAdjustException (MixinKind.Used, targetType, usesAttribute.MixinType, usesAttribute.IntroducedMemberVisibility, usesAttribute.AdditionalDependencies, usesAttribute.SuppressedMixins);
    }
  }
}
