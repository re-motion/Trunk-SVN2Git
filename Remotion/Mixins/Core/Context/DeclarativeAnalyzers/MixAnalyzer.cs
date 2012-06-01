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
using Remotion.Collections;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class MixAnalyzer : RelationAnalyzerBase
  {
    private readonly Set<MixAttribute> _handledBindings = new Set<MixAttribute>();

    public MixAnalyzer (MixinConfigurationBuilder configurationBuilder) : base (configurationBuilder)
    {
    }

    public virtual void Analyze (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      foreach (MixAttribute attribute in assembly.GetCustomAttributes (typeof (MixAttribute), false))
      {
        if (!_handledBindings.Contains (attribute))
        {
          AnalyzeMixAttribute (attribute, assembly);
          _handledBindings.Add (attribute);
        }
      }
    }

    public virtual void AnalyzeMixAttribute (MixAttribute mixAttribute, Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("mixAttribute", mixAttribute);
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      AddMixinAndAdjustException (
          mixAttribute.MixinKind,
          mixAttribute.TargetType,
          mixAttribute.MixinType,
          mixAttribute.IntroducedMemberVisibility,
          mixAttribute.AdditionalDependencies,
          mixAttribute.SuppressedMixins,
          MixinContextOrigin.CreateForCustomAttribute (mixAttribute, assembly));
    }
 }
}
