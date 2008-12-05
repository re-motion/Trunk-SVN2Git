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
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  public class InheritedClassContextRetrievalAlgorithm
  {
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

      ClassContextCollector collector = new ClassContextCollector();
      if (type.IsGenericType && !type.IsGenericTypeDefinition)
        collector.Add (_inheritanceAwareGetter (type.GetGenericTypeDefinition ()));
      if (type.BaseType != null)
        collector.Add (_inheritanceAwareGetter (type.BaseType));
      foreach (Type interfaceType in type.GetInterfaces ())
        collector.Add (_inheritanceAwareGetter (interfaceType));

      return collector.GetCombinedContexts (type);
    }
  }
}
