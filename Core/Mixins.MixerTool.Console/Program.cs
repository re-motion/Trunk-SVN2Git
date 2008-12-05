// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
