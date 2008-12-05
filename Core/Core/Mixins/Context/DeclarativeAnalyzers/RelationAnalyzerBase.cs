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
