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
using System.ComponentModel.Design;
using System.IO;
using Remotion.Logging;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Validation;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.Mixins.MixerTool
{
  [Serializable]
  public class MixerRunner : AppDomainRunnerBase
  {
    public static AppDomainSetup CreateAppDomainSetup (MixerParameters parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      var setup = new AppDomainSetup
                  {
                      ApplicationName = "Mixer",
                      ApplicationBase = parameters.BaseDirectory
                  };

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

      Mixer mixer = CreateMixer();

      try
      {
        mixer.PrepareOutputDirectory ();
        mixer.Execute (MixinConfiguration.ActiveConfiguration);
      }
      catch (Exception ex)
      {
				using (ConsoleUtility.EnterColorScope (ConsoleColor.Red, null))
				{
					Console.WriteLine (ex.Message);
				}
      }
    }

    private Mixer CreateMixer ()
    {
      var typeNameProvider = 
          _parameters.KeepTypeNames 
          ? (IConcreteMixedTypeNameProvider) NamespaceChangingNameProvider.Instance 
          : GuidNameProvider.Instance;

      var mixer = Mixer.Create (
          _parameters.SignedAssemblyName, 
          _parameters.UnsignedAssemblyName, 
          _parameters.AssemblyOutputDirectory, 
          typeNameProvider);
      
      mixer.ValidationErrorOccurred += Mixer_ValidationErrorOccurred;
      mixer.ErrorOccurred += Mixer_ErrorOccurred;
      return mixer;
    }

    private void Mixer_ValidationErrorOccurred (object sender, ValidationErrorEventArgs e)
    {
      ConsoleDumper.DumpValidationResults (e.ValidationException.ValidationLog.GetResults ());
    }

    void Mixer_ErrorOccurred (object sender, ErrorEventArgs e)
    {
      using (ConsoleUtility.EnterColorScope (ConsoleColor.Red, null))
      {
        Console.WriteLine (e.ToString ());
      }
    }
  }
}
