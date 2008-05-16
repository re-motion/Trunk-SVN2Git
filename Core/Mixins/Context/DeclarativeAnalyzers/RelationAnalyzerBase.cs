using System;
using System.Collections.Generic;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public abstract class RelationAnalyzerBase
  {
    private readonly MixinConfigurationBuilder _configurationBuilder;

    public RelationAnalyzerBase (MixinConfigurationBuilder configurationBuilder)
    {
      ArgumentUtility.CheckNotNull ("configurationBuilder", configurationBuilder);
      _configurationBuilder = configurationBuilder;
    }

    protected void AddMixinAndAdjustException (MixinKind mixinKind, Type targetType, Type mixinType, IEnumerable<Type> additionalDependencies, IEnumerable<Type> suppressedMixins)
    {
      try
      {
        _configurationBuilder.AddMixinToClass (mixinKind, targetType, mixinType, additionalDependencies, suppressedMixins);
      }
      catch (Exception ex)
      {
        throw new ConfigurationException (ex.Message, ex);
      }
    }
  }
}