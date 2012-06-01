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
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class ExtendsAnalyzer : RelationAnalyzerBase
  {
    public ExtendsAnalyzer (MixinConfigurationBuilder configurationBuilder) : base (configurationBuilder)
    {
    }

    public virtual void Analyze (Type extender)
    {
      ArgumentUtility.CheckNotNull ("extender", extender);

      foreach (ExtendsAttribute mixinAttribute in extender.GetCustomAttributes (typeof (ExtendsAttribute), false))
        AnalyzeExtendsAttribute (extender, mixinAttribute);
    }

    public virtual void AnalyzeExtendsAttribute (Type extender, ExtendsAttribute mixinAttribute)
    {
      ArgumentUtility.CheckNotNull ("extender", extender);
      ArgumentUtility.CheckNotNull ("mixinAttribute", mixinAttribute);

      Type mixinType = ApplyMixinTypeArguments (mixinAttribute.TargetType, extender, mixinAttribute.MixinTypeArguments);
      AddMixinAndAdjustException (
          MixinKind.Extending,
          mixinAttribute.TargetType,
          mixinType,
          mixinAttribute.IntroducedMemberVisibility,
          mixinAttribute.AdditionalDependencies,
          mixinAttribute.SuppressedMixins,
          MixinContextOrigin.CreateForCustomAttribute (mixinAttribute, extender));
    }

    private Type ApplyMixinTypeArguments (Type targetType, Type mixinType, Type[] typeArguments)
    {
      if (typeArguments.Length > 0)
      {
        CheckNumberOfTypeArguments(targetType, mixinType, typeArguments);
        CheckMixinIsOpenGeneric(targetType, mixinType);

        try
        {
          return mixinType.MakeGenericType (typeArguments);
        }
        catch (ArgumentException ex)
        {
          string message = string.Format (
              "The ExtendsAttribute for target class '{0}' applied to mixin type '{1}' specified invalid generic type arguments: {2}",
              targetType,
              mixinType,
              ex.Message);
          throw new ConfigurationException (message, ex);
        }
      }
      else
        return mixinType;
    }

    private void CheckMixinIsOpenGeneric (Type targetType, Type mixinType)
    {
      if (!mixinType.IsGenericTypeDefinition)
      {
        string message = string.Format (
            "The ExtendsAttribute for target class '{0}' applied to mixin type '{1}' specified generic type arguments, but the mixin type already has "
            + "type arguments specified.",
            targetType,
            mixinType);
        throw new ConfigurationException (message);
      }
    }

    private void CheckNumberOfTypeArguments (Type targetType, Type mixinType, Type[] typeArguments)
    {
      int expectedTypeArgumentLength = mixinType.IsGenericType ? mixinType.GetGenericArguments ().Length : 0;
      if (typeArguments.Length != expectedTypeArgumentLength)
      {
        string message = string.Format (
            "The ExtendsAttribute for target class {0} applied to mixin type {1} specified {2} generic type argument(s) when {3} argument(s) were expected.",
            targetType.FullName,
            mixinType.FullName,
            typeArguments.Length,
            expectedTypeArgumentLength);
        throw new ConfigurationException (message);
      }
    }
  }
}
