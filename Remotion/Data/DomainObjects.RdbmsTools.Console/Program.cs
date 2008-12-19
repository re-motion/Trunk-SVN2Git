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

namespace Remotion.Data.DomainObjects.RdbmsTools.Console
{
  public class Program
  {
    private static int Main (string[] args)
    {
      RdbmsToolsParameter rdbmsToolsParameter;
      CommandLineClassParser<RdbmsToolsParameter> parser = new CommandLineClassParser<RdbmsToolsParameter> ();
      try
      {
        rdbmsToolsParameter = parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        System.Console.WriteLine (e.Message);
        System.Console.WriteLine ("Usage:");
        System.Console.WriteLine (parser.GetAsciiSynopsis (Environment.GetCommandLineArgs()[0], System.Console.BufferWidth));
        return 1;
      }

      try
      {
        RdbmsToolsRunner rdbmsToolsRunner = RdbmsToolsRunner.Create (rdbmsToolsParameter);
        rdbmsToolsRunner.Run();
      }
      catch (Exception e)
      {
        if (rdbmsToolsParameter.Verbose)
        {
          System.Console.Error.WriteLine ("Execution aborted. Exception stack:");
          for (; e != null; e = e.InnerException)
            System.Console.Error.WriteLine ("{0}: {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace);
        }
        else
        {
          System.Console.Error.WriteLine ("Execution aborted: {0}", e.Message);
        }
        return 1;
      }
      return 0;
    }
  }
}
