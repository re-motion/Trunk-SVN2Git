using System;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class CompleteInterfaceAnalyzer
  {
    private readonly MixinConfigurationBuilder _configurationBuilder;

    public CompleteInterfaceAnalyzer (MixinConfigurationBuilder configurationBuilder)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      _configurationBuilder = configurationBuilder;
    }

    public virtual void Analyze (Type interfaceType)
    {
      foreach (CompleteInterfaceAttribute interfaceAttribute in interfaceType.GetCustomAttributes (typeof (CompleteInterfaceAttribute), false))
        AnalyzeCompleteInterfaceAttribute (interfaceType, interfaceAttribute);
    }

    public virtual void AnalyzeCompleteInterfaceAttribute (Type interfaceType, CompleteInterfaceAttribute completeInterfaceAttribute)
    {
      _configurationBuilder.ForClass (completeInterfaceAttribute.TargetType).AddCompleteInterface (interfaceType);
    }
  }
}