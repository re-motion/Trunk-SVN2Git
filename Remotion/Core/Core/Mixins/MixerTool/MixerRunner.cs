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
using System.IO;
using Remotion.Logging;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

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
        TargetClassDefinitionCache.Current.GetTargetClassDefinition (e.ClassContext);
      }
      catch (ValidationException ex)
      {
        ConsoleDumper.DumpValidationResults (ex.ValidationLog.GetResults());
        throw;
      }
    }
  }
}
