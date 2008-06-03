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
using Remotion.Mixins.MixerTool;
using Remotion.Text.CommandLine;
using Remotion.Utilities;

namespace Remotion.Mixins.MixerTool.Console
{
  class Program
  {
    static int Main (string[] args)
    {
      MixerParameters parameters;
      CommandLineClassParser<MixerParameters> parser = new CommandLineClassParser<MixerParameters> ();
      try
      {
        parameters = parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        System.Console.WriteLine (e.Message);
        System.Console.WriteLine ("Usage:");
        System.Console.WriteLine (parser.GetAsciiSynopsis (Environment.GetCommandLineArgs ()[0], System.Console.BufferWidth));
        return 1;
      }

      try
      {
        MixerRunner mixerRunner = new MixerRunner (parameters);
        mixerRunner.Run ();
      }
      catch (Exception e)
      {
				using (ConsoleUtility.EnterColorScope (ConsoleColor.White, ConsoleColor.DarkRed))
				{
					System.Console.Error.WriteLine ("Execution aborted. Exception stack:");
					for (; e != null; e = e.InnerException)
						System.Console.Error.WriteLine ("{0}: {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace);
				}
      	return 1;
      }
      return 0;
    }
  }
}
