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