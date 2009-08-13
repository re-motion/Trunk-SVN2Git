// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.Mixins.Validation;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.MixerTool
{
  public class Mixer
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (Mixer));

    public event EventHandler<ClassContextEventArgs> ClassContextBeingProcessed = delegate {};

    private readonly string _signedAssemblyName;
    private readonly string _unsignedAssemblyName;
    private readonly string _assemblyOutputDirectory;
    private INameProvider _nameProvider = GuidNameProvider.Instance;

    private readonly List<Tuple<ClassContext, Exception>> _errors = new List<Tuple<ClassContext, Exception>> ();
    private readonly Dictionary<Type, ClassContext> _processedContexts = new Dictionary<Type, ClassContext> ();
    private readonly Dictionary<Type, Type> _finishedTypes = new Dictionary<Type, Type> ();

    public Mixer (string signedAssemblyName, string unsignedAssemblyName, string assemblyOutputDirectory)
    {
      ArgumentUtility.CheckNotNull ("signedAssemblyName", signedAssemblyName);
      ArgumentUtility.CheckNotNull ("unsignedAssemblyName", unsignedAssemblyName);
      ArgumentUtility.CheckNotNull ("assemblyOutputDirectory", assemblyOutputDirectory);

      _signedAssemblyName = signedAssemblyName;
      _unsignedAssemblyName = unsignedAssemblyName;
      _assemblyOutputDirectory = assemblyOutputDirectory;
    }

    public INameProvider NameProvider
    {
      get { return _nameProvider; }
      set { _nameProvider = value; }
    }

    public ReadOnlyCollection<Tuple<ClassContext, Exception>> Errors
    {
      get { return _errors.AsReadOnly(); }
    }

    public Dictionary<Type, ClassContext> ProcessedContexts
    {
      get { return _processedContexts; }
    }

    public Dictionary<Type, Type> FinishedTypes
    {
      get { return _finishedTypes; }
    }

    public void Execute (bool saveGeneratedTypes)
    {
      _errors.Clear();
      _processedContexts.Clear();
      _finishedTypes.Clear();

      s_log.InfoFormat ("Base directory is '{0}'.", AppDomain.CurrentDomain.BaseDirectory);
      ConcreteTypeBuilder originalBuilder = ConcreteTypeBuilder.Current;
      try
      {
        Configure();
        Generate ();
        Save();
        LogStatistics();
      }
      finally
      {
        ConcreteTypeBuilder.SetCurrent (originalBuilder);
      }
    }

    private void Configure ()
    {
      ConcreteTypeBuilder.SetCurrent (new ConcreteTypeBuilder ());

      ConcreteTypeBuilder.Current.TypeNameProvider = NameProvider;

      if (!Directory.Exists (_assemblyOutputDirectory))
        Directory.CreateDirectory (_assemblyOutputDirectory);

      ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName = _unsignedAssemblyName;
      ConcreteTypeBuilder.Current.Scope.UnsignedModulePath = Path.Combine (_assemblyOutputDirectory,
          ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName + ".dll");

      ConcreteTypeBuilder.Current.Scope.SignedAssemblyName = _signedAssemblyName;
      ConcreteTypeBuilder.Current.Scope.SignedModulePath = Path.Combine (_assemblyOutputDirectory,
          ConcreteTypeBuilder.Current.Scope.SignedAssemblyName + ".dll");

      if (File.Exists (ConcreteTypeBuilder.Current.Scope.UnsignedModulePath))
        File.Delete (ConcreteTypeBuilder.Current.Scope.UnsignedModulePath);

      if (File.Exists (ConcreteTypeBuilder.Current.Scope.SignedModulePath))
        File.Delete (ConcreteTypeBuilder.Current.Scope.SignedModulePath);
    }

    public void Generate ()
    {
      MixinConfiguration configuration = MixinConfiguration.ActiveConfiguration;
      ICollection typesToCheck = ContextAwareTypeDiscoveryUtility.GetInstance().GetTypes (null, false);
      
      s_log.InfoFormat ("Generating types for {0} configured mixin targets and {1} loaded types.", configuration.ClassContexts.Count, typesToCheck.Count);
      GenerateForConfiguredContexts(configuration);
      GenerateForInheritedContexts(configuration, typesToCheck);
    }

    private void GenerateForConfiguredContexts (MixinConfiguration configuration)
    {
      foreach (ClassContext classContext in configuration.ClassContexts)
        GenerateForClassContext (classContext);
    }

    private void GenerateForInheritedContexts (MixinConfiguration configuration, ICollection typesToCheck)
    {
      foreach (Type type in typesToCheck)
      {
        if (!type.IsDefined (typeof (IgnoreForMixinConfigurationAttribute), false))
        {
          ClassContext contextWithoutInheritance = configuration.ClassContexts.GetExact (type);
          ClassContext contextWithInheritance = configuration.ClassContexts.GetWithInheritance (type);
          if (contextWithoutInheritance == null && contextWithInheritance != null)
            GenerateForClassContext (contextWithInheritance);
        }
      }
    }

    private void GenerateForClassContext (ClassContext context)
    {
      if (!ShouldProcessContext (context))
        return;

      _processedContexts.Add (context.Type, context);

      try
      {
        ClassContextBeingProcessed (this, new ClassContextEventArgs (context));
        Type concreteType = TypeFactory.GetConcreteType (context.Type);
        s_log.InfoFormat ("Created type: {0}.", concreteType.FullName);
        _finishedTypes.Add (context.Type, concreteType);
      }
      catch (ValidationException validationException)
      {
        _errors.Add (new Tuple<ClassContext, Exception> (context, validationException));
        s_log.ErrorFormat (validationException, "{0} : Validation problem when generating type", context.ToString());
        ConsoleDumper.DumpValidationResults (validationException.ValidationLog.GetResults());
      }
      catch (Exception ex)
      {
        _errors.Add (new Tuple<ClassContext, Exception> (context, ex));
        s_log.ErrorFormat (ex, "{0} : Unexpected error when generating type", context.ToString ());
        using (ConsoleUtility.EnterColorScope (ConsoleColor.Red, null))
        {
          Console.WriteLine (ex.ToString());
        }
      }
    }

    protected virtual bool ShouldProcessContext (ClassContext context)
    {
      if (context.Type.IsGenericTypeDefinition)
      {
        s_log.WarnFormat ("Type {0} is a generic type definition and is thus ignored.", context.Type);
        return false;
      }

      if (context.Type.IsInterface)
      {
        s_log.WarnFormat ("Type {0} is an interface and is thus ignored.", context.Type);
        return false;
      }

      return true;
    }

    private void Save ()
    {
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope();
      if (paths.Length == 0)
        s_log.Info ("No assemblies generated.");
      else
      {
        foreach (string path in paths)
          s_log.InfoFormat ("Generated assembly file {0}.", path);
      }
    }

    private void LogStatistics ()
    {
      s_log.Info (CodeGenerationStatistics.GetStatisticsString());
    }
  }
}
