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

    public event EventHandler<ClassContextEventArgs> ClassContextBeingProcessed = delegate { };
    public event EventHandler<ValidationErrorEventArgs> ValidationErrorOccurred = delegate { };
    public event EventHandler<ErrorEventArgs> ErrorOccurred = delegate { };

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

      var builder = CreateConcreteTypeBuilder ();

      ConfigureDirectory (builder.Scope.UnsignedModulePath, builder.Scope.SignedModulePath);
      Generate (builder);
      Save (builder);
      LogStatistics();
    }

    private ConcreteTypeBuilder CreateConcreteTypeBuilder ()
    {
      var builder = new ConcreteTypeBuilder ();
      builder.TypeNameProvider = NameProvider;

      builder.Scope.UnsignedAssemblyName = _unsignedAssemblyName;
      builder.Scope.UnsignedModulePath = Path.Combine (_assemblyOutputDirectory, _unsignedAssemblyName + ".dll");

      builder.Scope.SignedAssemblyName = _signedAssemblyName;
      builder.Scope.SignedModulePath = Path.Combine (_assemblyOutputDirectory, _signedAssemblyName + ".dll");

      return builder;
    }

    private void ConfigureDirectory (string unsignedModulePath, string signedModulePath)
    {
      if (!Directory.Exists (_assemblyOutputDirectory))
        Directory.CreateDirectory (_assemblyOutputDirectory);
      
      if (File.Exists (unsignedModulePath))
        File.Delete (unsignedModulePath);

      if (File.Exists (signedModulePath))
        File.Delete (signedModulePath);
    }

    public void Generate (ConcreteTypeBuilder builder)
    {
      MixinConfiguration configuration = MixinConfiguration.ActiveConfiguration;
      ICollection typesToCheck = ContextAwareTypeDiscoveryUtility.GetInstance().GetTypes (null, false);
      
      s_log.InfoFormat ("Generating types for {0} configured mixin targets and {1} loaded types.", configuration.ClassContexts.Count, typesToCheck.Count);

      GenerateForConfiguredContexts (builder, configuration);
      GenerateForInheritedContexts (builder, configuration, typesToCheck);
    }

    private void GenerateForConfiguredContexts (ConcreteTypeBuilder builder, MixinConfiguration configuration)
    {
      foreach (ClassContext classContext in configuration.ClassContexts)
        GenerateForClassContext (builder, classContext);
    }

    private void GenerateForInheritedContexts (ConcreteTypeBuilder builder, MixinConfiguration configuration, ICollection typesToCheck)
    {
      foreach (Type type in typesToCheck)
      {
        if (!type.IsDefined (typeof (IgnoreForMixinConfigurationAttribute), false))
        {
          ClassContext contextWithoutInheritance = configuration.ClassContexts.GetExact (type);
          ClassContext contextWithInheritance = configuration.ClassContexts.GetWithInheritance (type);
          if (contextWithoutInheritance == null && contextWithInheritance != null)
            GenerateForClassContext (builder, contextWithInheritance);
        }
      }
    }

    private void GenerateForClassContext (ConcreteTypeBuilder builder, ClassContext context)
    {
      if (!ShouldProcessContext (context))
        return;

      _processedContexts.Add (context.Type, context);

      try
      {
        ClassContextBeingProcessed (this, new ClassContextEventArgs (context));
        
        Type concreteType = builder.GetConcreteType (context);
        s_log.InfoFormat ("Created type: {0}.", concreteType.FullName);
        _finishedTypes.Add (context.Type, concreteType);
      }
      catch (ValidationException validationException)
      {
        _errors.Add (new Tuple<ClassContext, Exception> (context, validationException));
        ValidationErrorOccurred (this, new ValidationErrorEventArgs (validationException));
      }
      catch (Exception ex)
      {
        _errors.Add (new Tuple<ClassContext, Exception> (context, ex));
        ErrorOccurred (this, new ErrorEventArgs (ex));
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

    private void Save (ConcreteTypeBuilder builder)
    {
      string[] paths = builder.SaveAndResetDynamicScope();
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
