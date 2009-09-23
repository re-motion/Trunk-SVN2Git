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
using Remotion.Utilities;
using System.Collections.Generic;

namespace Remotion.Mixins.Context
{
  public class InheritedClassContextRetrievalAlgorithm
  {
    /// <summary>
    /// Gets the types this <paramref name="targetType"/> inherits mixins from. A target type inherits mixins from its generic type definition,
    /// its base class, and its interfaces.
    /// </summary>
    /// <param name="targetType">The type whose "base" types should be retrieved.</param>
    /// <returns>The types from which the given <paramref name="targetType"/> inherits its mixins.</returns>
    public static IEnumerable<Type> GetTypesToInheritFrom (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      if (targetType.IsGenericType && !targetType.IsGenericTypeDefinition)
        yield return targetType.GetGenericTypeDefinition ();
      if (targetType.BaseType != null)
        yield return targetType.BaseType;
      foreach (Type interfaceType in targetType.GetInterfaces ())
        yield return interfaceType;
    }

    private readonly Func<Type, ClassContext> _exactGetter;
    private readonly Func<Type, ClassContext> _inheritanceAwareGetter;

    public InheritedClassContextRetrievalAlgorithm (Func<Type, ClassContext> exactGetter, Func<Type, ClassContext> inheritanceAwareGetter)
    {
      ArgumentUtility.CheckNotNull ("exactGetter", exactGetter);
      ArgumentUtility.CheckNotNull ("inheritanceAwareGetter", inheritanceAwareGetter);

      _exactGetter = exactGetter;
      _inheritanceAwareGetter = inheritanceAwareGetter;
    }

    public ClassContext GetWithInheritance (Type type)
    {
      ClassContext exactValue = _exactGetter (type);
      if (exactValue != null)
        return exactValue;

      var combiner = new ClassContextCombiner();
      foreach (var typeToInheritFrom in GetTypesToInheritFrom (type))
        combiner.AddIfNotNull (_inheritanceAwareGetter (typeToInheritFrom));

      return combiner.GetCombinedContexts (type);
    }
  }
}
