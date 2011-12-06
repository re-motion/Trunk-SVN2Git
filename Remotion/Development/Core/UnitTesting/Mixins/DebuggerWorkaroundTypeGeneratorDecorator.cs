using System;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Mixins
{
  /// <summary>
  /// Decorates <see cref="ITypeGenerator"/> in order to notify <see cref="DebuggerWorkaroundModuleManagerDecorator"/> when the type is actually 
  /// generated.
  /// </summary>
  public class DebuggerWorkaroundTypeGeneratorDecorator : ITypeGenerator
  {
    private readonly ITypeGenerator _innerTypeGenerator;
    private readonly DebuggerWorkaroundModuleManagerDecorator _moduleManager;

    public DebuggerWorkaroundTypeGeneratorDecorator (ITypeGenerator innerTypeGenerator, DebuggerWorkaroundModuleManagerDecorator moduleManager)
    {
      ArgumentUtility.CheckNotNull ("innerTypeGenerator", innerTypeGenerator);
      ArgumentUtility.CheckNotNull ("moduleManager", moduleManager);

      _innerTypeGenerator = innerTypeGenerator;
      _moduleManager = moduleManager;
    }

    public Type GetBuiltType ()
    {
      using (StopwatchScope.CreateScope ((ctx, scope) => _moduleManager.TypeGenerated (scope.ElapsedTotal)))
      {
        return _innerTypeGenerator.GetBuiltType ();
      }
    }

    public MethodInfo GetPublicMethodWrapper (MethodDefinition methodToBeWrapped)
    {
      return _innerTypeGenerator.GetPublicMethodWrapper (methodToBeWrapped);
    }

    public TypeBuilder TypeBuilder
    {
      get { return _innerTypeGenerator.TypeBuilder; }
    }

    public bool IsAssemblySigned
    {
      get { return _innerTypeGenerator.IsAssemblySigned; }
    }
  }
}