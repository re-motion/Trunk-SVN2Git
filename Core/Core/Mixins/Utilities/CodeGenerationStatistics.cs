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