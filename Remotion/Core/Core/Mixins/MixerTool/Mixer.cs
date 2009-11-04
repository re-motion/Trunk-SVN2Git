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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Utilities;
using Remotion.Mixins.Validation;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.Mixins.MixerTool
{
  public class Mixer
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (Mixer));

    public static Mixer Create (
        string signedAssemblyName, 
        string unsignedAssemblyName,
        IConcreteMixedTypeNameProvider typeNameProvider, 
        string assemblyOutputDirectory)
    {
      var builderFactory = new ConcreteTypeBuilderFactory (typeNameProvider, signedAssemblyName, unsignedAssemblyName);
      
      var typeDiscoveryService = ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService ();
      var finder = new ClassContextFinder (typeDiscoveryService);

      return new Mixer (finder, builderFactory, assemblyOutputDirectory);
    }

    public event EventHandler<ClassContextEventArgs> ClassContextBeingProcessed = delegate { };
    public event EventHandler<ValidationErrorEventArgs> ValidationErrorOccurred = delegate { };
    public event EventHandler<ErrorEventArgs> ErrorOccurred = delegate { };

    private readonly List<Tuple<ClassContext, Exception>> _errors = new List<Tuple<ClassContext, Exception>> ();
    private readonly Dictionary<Type, ClassContext> _processedContexts = new Dictionary<Type, ClassContext> ();
    private readonly Dictionary<Type, Type> _finishedTypes = new Dictionary<Type, Type> ();

    public Mixer (IClassContextFinder classContextFinder, IConcreteTypeBuilderFactory concreteTypeBuilderFactory, string assemblyOutputDirectory)
    {
      ArgumentUtility.CheckNotNull ("classContextFinder", classContextFinder);
      ArgumentUtility.CheckNotNull ("concreteTypeBuilderFactory", concreteTypeBuilderFactory);
      ArgumentUtility.CheckNotNull ("assemblyOutputDirectory", assemblyOutputDirectory);

      ClassContextFinder = classContextFinder;
      ConcreteTypeBuilderFactory = concreteTypeBuilderFactory;
      AssemblyOutputDirectory = assemblyOutputDirectory;
    }

    public IClassContextFinder ClassContextFinder { get; private set; }
    public IConcreteTypeBuilderFactory ConcreteTypeBuilderFactory { get; private set; }
    public string AssemblyOutputDirectory { get; private set; }

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

    public void PrepareOutputDirectory ()
    {
      if (!Directory.Exists (AssemblyOutputDirectory))
      {
        s_log.InfoFormat ("Preparing output directory '{0}'.", AssemblyOutputDirectory);
        Directory.CreateDirectory (AssemblyOutputDirectory);
      }

      CleanupIfExists (ConcreteTypeBuilderFactory.GetSignedModulePath (AssemblyOutputDirectory));
      CleanupIfExists (ConcreteTypeBuilderFactory.GetUnsignedModulePath (AssemblyOutputDirectory));
    }

    // The MixinConfiguration is passed to Execute in order to be able to call PrepareOutputDirectory before analyzing the configuration (and potentially
    // locking old generated files).
    public void Execute (MixinConfiguration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Time needed to mix and save all types: {elapsed}."))
      {
        _errors.Clear();
        _processedContexts.Clear();
        _finishedTypes.Clear();

        s_log.InfoFormat ("The base directory is '{0}'.", AppDomain.CurrentDomain.BaseDirectory);

        var builder = ConcreteTypeBuilderFactory.CreateTypeBuilder (AssemblyOutputDirectory);

        foreach (var classContext in ClassContextFinder.FindClassContexts (configuration))
          Generate (classContext, builder);

        Save (builder);
      }
      LogStatistics();
    }

    private void Generate (ClassContext classContext, IConcreteTypeBuilder concreteTypeBuilder)
    {
      _processedContexts.Add (classContext.Type, classContext);

      try
      {
        ClassContextBeingProcessed (this, new ClassContextEventArgs (classContext));

        Type concreteType = concreteTypeBuilder.GetConcreteType (classContext);
        _finishedTypes.Add (classContext.Type, concreteType);
      }
      catch (ValidationException validationException)
      {
        _errors.Add (new Tuple<ClassContext, Exception> (classContext, validationException));
        ValidationErrorOccurred (this, new ValidationErrorEventArgs (validationException));
      }
      catch (Exception ex)
      {
        _errors.Add (new Tuple<ClassContext, Exception> (classContext, ex));
        ErrorOccurred (this, new ErrorEventArgs (ex));
      }
    }

    private void Save (IConcreteTypeBuilder builder)
    {
      string[] paths = builder.SaveAndResetDynamicScope ();
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

    private void CleanupIfExists (string path)
    {
      if (File.Exists (path))
      {
        s_log.InfoFormat ("Removing file '{0}'.", path);
        File.Delete (path);
      }
    }
  }
}
