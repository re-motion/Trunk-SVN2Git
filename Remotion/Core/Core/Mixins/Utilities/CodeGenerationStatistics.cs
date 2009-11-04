// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Diagnostics;
using System.Reflection.Emit;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;

namespace Remotion.Mixins.Utilities
{
  /// <summary>
  /// Provides statistical information about the resources used by the mixin code generation engine.
  /// </summary>
  public class CodeGenerationStatistics
  {
    public static AssemblyBuilder CurrentUnsignedAssemblyBuilder
    {
      get
      {
        if (ConcreteTypeBuilder.HasCurrent && ConcreteTypeBuilder.Current.Scope.UnsignedModule != null)
          return (AssemblyBuilder) ConcreteTypeBuilder.Current.Scope.UnsignedModule.Assembly;
        else
          return null;
      }
    }

    public static AssemblyBuilder CurrentSignedAssemblyBuilder
    {
      get
      {
        if (ConcreteTypeBuilder.HasCurrent && ConcreteTypeBuilder.Current.Scope.SignedModule != null)
          return (AssemblyBuilder) ConcreteTypeBuilder.Current.Scope.SignedModule.Assembly;
        else
          return null;
      }
    }

    public static Type[] GetTypesInCurrentSignedBuilder () 
    {
      AssemblyBuilder builder = CurrentSignedAssemblyBuilder;
      return builder != null ? builder.GetTypes() : Type.EmptyTypes;
    }

    public static Type[] GetTypesInCurrentUnsignedBuilder ()
    {
      AssemblyBuilder builder = CurrentUnsignedAssemblyBuilder;
      return builder != null ? builder.GetTypes () : Type.EmptyTypes;
    }
    
    public static int CreatedAssemblyCount
    {
      get { return ModuleManager.CreatedAssemblies.Count; }
    }

    public static TimeSpan TimeSpentInCodeGeneration
    {
      get { return CodeGenerationTimer.CodeGenerationTime; }
    }

    public static int GetBuiltTypeCount ()
    {
      int count = 0;
      foreach (var assembly in ModuleManager.CreatedAssemblies)
        count += assembly.GetTypes().Length;
      return count;
    }

    public static MemoryUsageInfo GetCurrentProcessMemoryUsage ()
    {
      return MemoryUsageInfo.GetCurrent ("Current process memory");
    }

    public static string GetStatisticsString ()
    {
      return string.Format (
          "Code generation statistics{0}--------------------------{0}{1}{0}"
          + "Number of assemblies generated: {2}{0}"
          + "Total number of types generated: {3}{0}"
          + "Time spent in code generation: {4}",
          Environment.NewLine,
          GetCurrentProcessMemoryUsage(),
          CreatedAssemblyCount,
          GetBuiltTypeCount(),
          TimeSpentInCodeGeneration);
    }

    // Working set;Managed before GC;Managed after GC;Assembly count;Type count;Time spent in code generation
    public static string GetCSVStatisticsString ()
    {
      return string.Format ("{0};{1};{2};{3}", 
          GetCurrentProcessMemoryUsage ().ToCSVString(),
          CreatedAssemblyCount,
          GetBuiltTypeCount (),
          TimeSpentInCodeGeneration);
    }
  }
}
