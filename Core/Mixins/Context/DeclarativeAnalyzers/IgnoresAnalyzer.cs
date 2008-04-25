using System;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class IgnoresAnalyzer
  {
    private readonly MixinConfigurationBuilder _configurationBuilder;

    public IgnoresAnalyzer (MixinConfigurationBuilder configurationBuilder)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      _configurationBuilder = configurationBuilder;
    }

    public virtual void Analyze (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      foreach (IgnoresClassAttribute attribute in type.GetCustomAttributes (typeof (IgnoresClassAttribute), false))
        AnalyzeIgnoresClassAttribute (type, attribute);
      foreach (IgnoresMixinAttribute attribute in type.GetCustomAttributes (typeof (IgnoresMixinAttribute), false))
        AnalyzeIgnoresMixinAttribute (type, attribute);
    }

    public virtual void AnalyzeIgnoresClassAttribute (Type mixinType, IgnoresClassAttribute attribute)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      _configurationBuilder.ForClass (attribute.ClassToIgnore).SuppressMixin (mixinType);
    }

    public virtual void AnalyzeIgnoresMixinAttribute (Type targetClassType, IgnoresMixinAttribute attribute)
    {
      ArgumentUtility.CheckNotNull ("targetClassType", targetClassType);
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      _configurationBuilder.ForClass (targetClassType).SuppressMixin (attribute.MixinToIgnore);
    }
  }
}