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
using System.IO;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;
using Remotion.Logging;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.MixerTool
{
  [Serializable]
  public class MixerRunner : AppDomainRunnerBase
  {
    private static AppDomainSetup CreateAppDomainSetup (MixerParameters parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      AppDomainSetup setup = new AppDomainSetup ();
      setup.ApplicationName = "Mixer";
      setup.ApplicationBase = parameters.BaseDirectory;
      setup.DynamicBase = Path.Combine (Path.GetTempPath (), "Remotion"); // necessary for AppDomainRunnerBase and AppDomainRunnerBase

      if (!string.IsNullOrEmpty (parameters.ConfigFile))
      {
        setup.ConfigurationFile = parameters.ConfigFile;
        if (!File.Exists (setup.ConfigurationFile))
        {
          throw new FileNotFoundException (
              string.Format (
                  "The configuration file supplied by the 'config' parameter was not found.\r\nFile: {0}",
                  setup.ConfigurationFile),
              setup.ConfigurationFile);
        }
      }
      return setup;
    }

    private readonly MixerParameters _parameters;

    public MixerRunner (MixerParameters parameters)
        : base (CreateAppDomainSetup (ArgumentUtility.CheckNotNull ("parameters", parameters)))
    {
      _parameters = parameters;
    }

    protected override void CrossAppDomainCallbackHandler ()
    {
      LogManager.InitializeConsole ();

      Mixer mixer = new Mixer (_parameters.SignedAssemblyName, _parameters.UnsignedAssemblyName, _parameters.AssemblyOutputDirectory);
      if (_parameters.KeepTypeNames)
        mixer.NameProvider = NamespaceChangingNameProvider.Instance;

      mixer.ClassContextBeingProcessed += Mixer_ClassContextBeingProcessed;
      try
      {
        mixer.Execute (true);
      }
      catch (Exception ex)
      {
				using (ConsoleUtility.EnterColorScope (ConsoleColor.Red, null))
				{
					Console.WriteLine (ex.Message);
				}
      }
    }

    private void Mixer_ClassContextBeingProcessed (object sender, ClassContextEventArgs e)
    {
      try
      {
        Validator.Validate (TargetClassDefinitionCache.Current.GetTargetClassDefinition (e.ClassContext));
      }
      catch (ValidationException ex)
      {
        ConsoleDumper.DumpValidationResults (ex.ValidationLog.GetResults());
        throw;
      }
    }
  }
}
