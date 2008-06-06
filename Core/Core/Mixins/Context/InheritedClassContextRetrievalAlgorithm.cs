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
