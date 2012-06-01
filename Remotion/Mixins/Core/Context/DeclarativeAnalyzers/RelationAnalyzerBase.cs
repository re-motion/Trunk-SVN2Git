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
using System.Collections.Generic;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public abstract class RelationAnalyzerBase
  {
    private readonly MixinConfigurationBuilder _configurationBuilder;

    protected RelationAnalyzerBase (MixinConfigurationBuilder configurationBuilder)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      _configurationBuilder = configurationBuilder;
    }

    protected void AddMixinAndAdjustException (
        MixinKind mixinKind, 
        Type targetType, 
        Type mixinType, 
        MemberVisibility introducedMemberVisibility, 
        IEnumerable<Type> additionalDependencies, 
        IEnumerable<Type> suppressedMixins,
        MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("additionalDependencies", additionalDependencies);
      ArgumentUtility.CheckNotNull ("suppressedMixins", suppressedMixins);
      ArgumentUtility.CheckNotNull ("origin", origin);

      try
      {
        _configurationBuilder.AddMixinToClass (
            mixinKind, targetType, mixinType, introducedMemberVisibility, additionalDependencies, suppressedMixins, origin);
      }
      catch (Exception ex)
      {
        throw new ConfigurationException (ex.Message, ex);
      }
    }
  }
}
