using System;
using System.Collections.Generic;
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