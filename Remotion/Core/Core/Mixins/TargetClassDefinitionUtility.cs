// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Provides common functionality for retrieving <see cref="TargetClassDefinition"/> metadata objects.
  /// </summary>
  public class TargetClassDefinitionUtility
  {
    /// <summary>
    /// Returns a <see cref="ClassContext"/> for the a given target type.
    /// </summary>
    /// <param name="targetOrConcreteType">Base type for which a context should be returned or a concrete mixed type.</param>
    /// <param name="mixinConfiguration">The <see cref="MixinConfiguration"/> to use.</param>
    /// <param name="generationPolicy">Defines whether to return <see langword="null"/> or generate an empty default configuration if no mixin
    /// configuration is available for the given <paramref name="targetOrConcreteType"/>.</param>
    /// <returns>A <see cref="ClassContext"/> for the a given target type, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// Use this to extract a class context for a given target type from an <see cref="MixinConfiguration"/> as it would be used to create the
    /// <see cref="TargetClassDefinition"/> object for the target type. Besides looking up the target type in the given mixin configuration, this
    /// includes generating a default context if <see cref="GenerationPolicy.ForceGeneration"/> is specified and the specialization of generic
    /// arguments in the class context, if any.
    /// </para>
    /// <para>
    /// Use the <paramref name="generationPolicy"/> parameter to configure whether this method should return an empty but valid
    /// <see cref="ClassContext"/> for types that do not have a mixin configuration in the <paramref name="mixinConfiguration"/>.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, the <see cref="ClassContext"/> used for its generation is returned unless
    /// <see cref="GenerationPolicy.ForceGeneration"/> is specified.
    /// </para>
    /// </remarks>
    public static ClassContext GetContext (Type targetOrConcreteType, MixinConfiguration mixinConfiguration, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      ClassContext context;
      if (generationPolicy != GenerationPolicy.ForceGeneration && MixinTypeUtility.IsGeneratedConcreteMixedType (targetOrConcreteType))
        context = MixinReflector.GetClassContextFromConcreteType (targetOrConcreteType);
      else
        context = mixinConfiguration.ClassContexts.GetWithInheritance (targetOrConcreteType);

      if (context == null && generationPolicy == GenerationPolicy.ForceGeneration)
        context = new ClassContext (targetOrConcreteType);

      if (context != null && targetOrConcreteType.IsGenericType && context.Type.IsGenericTypeDefinition)
        context = context.SpecializeWithTypeArguments (targetOrConcreteType.GetGenericArguments ());

      return context;
    }
  }
}
