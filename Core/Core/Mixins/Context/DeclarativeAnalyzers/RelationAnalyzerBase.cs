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
using System.Collections.Generic;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public abstract class RelationAnalyzerBase
  {
    private readonly MixinConfigurationBuilder _configurationBuilder;

    public RelationAnalyzerBase (MixinConfigurationBuilder configurationBuilder)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      _configurationBuilder = configurationBuilder;
    }

    protected void AddMixinAndAdjustException (MixinKind mixinKind, Type targetType, Type mixinType, MemberVisibility introducedMemberVisibility, IEnumerable<Type> additionalDependencies, IEnumerable<Type> suppressedMixins)
    {
      try
      {
        _configurationBuilder.AddMixinToClass (mixinKind, targetType, mixinType, introducedMemberVisibility, additionalDependencies, suppressedMixins);
      }
      catch (Exception ex)
      {
        throw new ConfigurationException (ex.Message, ex);
      }
    }
  }
}
