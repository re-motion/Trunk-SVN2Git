using System;
using Remotion.Mixins.Context.FluentBuilders;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class UsesAnalyzer : RelationAnalyzerBase
  {
    public UsesAnalyzer (MixinConfigurationBuilder configurationBuilder) : base (configurationBuilder)
    {
    }

    public virtual void Analyze (Type targetType)
    {
      foreach (UsesAttribute usesAttribute in targetType.GetCustomAttributes (typeof (UsesAttribute), false))
        AnalyzeUsesAttribute (targetType, usesAttribute);
    }

    public virtual void AnalyzeUsesAttribute (Type targetType, UsesAttribute usesAttribute)
    {
      AddMixinAndAdjustException (targetType, usesAttribute.MixinType, usesAttribute.AdditionalDependencies, usesAttribute.SuppressedMixins);
    }
  }
}