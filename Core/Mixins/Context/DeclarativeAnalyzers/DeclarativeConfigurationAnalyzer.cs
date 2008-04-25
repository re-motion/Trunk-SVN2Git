using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Mixins.Context.DeclarativeAnalyzers
{
  public class DeclarativeConfigurationAnalyzer
  {
    private readonly ExtendsAnalyzer _extendsAnalyzer;
    private readonly UsesAnalyzer _usesAnalyzer;
    private readonly CompleteInterfaceAnalyzer _completeInterfaceAnalyzer;
    private readonly MixAnalyzer _mixAnalyzer;
    private readonly IgnoresAnalyzer _ignoresAnalyzer;

    public DeclarativeConfigurationAnalyzer (ExtendsAnalyzer extendsAnalyzer, UsesAnalyzer usesAnalyzer, CompleteInterfaceAnalyzer interfaceAnalyzer, 
        MixAnalyzer mixAnalyzer, IgnoresAnalyzer ignoresAnalyzer)
    {
      ArgumentUtility.CheckNotNull ("extendsAnalyzer", extendsAnalyzer);
      ArgumentUtility.CheckNotNull ("usesAnalyzer", usesAnalyzer);
      ArgumentUtility.CheckNotNull ("interfaceAnalyzer", interfaceAnalyzer);
      ArgumentUtility.CheckNotNull ("mixAnalyzer", mixAnalyzer);
      ArgumentUtility.CheckNotNull ("ignoresAnalyzer", ignoresAnalyzer);

      _extendsAnalyzer = extendsAnalyzer;
      _usesAnalyzer = usesAnalyzer;
      _completeInterfaceAnalyzer = interfaceAnalyzer;
      _mixAnalyzer = mixAnalyzer;
      _ignoresAnalyzer = ignoresAnalyzer;
    }
    
    public void Analyze (IEnumerable<Type> types)
    {
      Set<Assembly> assemblies = new Set<Assembly>();

      foreach (Type type in types)
      {
        _extendsAnalyzer.Analyze (type);
        _usesAnalyzer.Analyze (type);
        _completeInterfaceAnalyzer.Analyze (type);
        _ignoresAnalyzer.Analyze (type);

        if (!assemblies.Contains (type.Assembly))
        {
          assemblies.Add (type.Assembly);
          _mixAnalyzer.Analyze (type.Assembly);
        }
      }
    }
  }
}