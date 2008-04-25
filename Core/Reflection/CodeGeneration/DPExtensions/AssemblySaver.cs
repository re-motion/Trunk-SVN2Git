using System;
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public static class AssemblySaver
  {
    public static string[] SaveAssemblies (ModuleScope scope)
    {
      List<string> paths = new List<string> ();

      if (scope.StrongNamedModule != null)
      {
        scope.SaveAssembly (true);
        paths.Add (scope.StrongNamedModule.FullyQualifiedName);
      }

      if (scope.WeakNamedModule != null)
      {
        scope.SaveAssembly (false);
        paths.Add (scope.WeakNamedModule.FullyQualifiedName);
      }
      return paths.ToArray ();
    }
  }
}