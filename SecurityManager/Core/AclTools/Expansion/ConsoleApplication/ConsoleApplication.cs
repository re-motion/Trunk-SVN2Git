// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication;
using Remotion.Text.CommandLine;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication
{
  public class ConsoleApplication<TApplication, TApplicationSettings> 
      where TApplication: IApplicationRunner<TApplicationSettings>, new()
      where TApplicationSettings : class
  {
    public static int MainDo (string[] args)
    {
      int result = 0;
      TApplicationSettings settings = ParseCommandLineArguments(args, ref result);
      RunApplication(ref result, settings);
      WaitForKeypress();
      return result;
    }

    private static void WaitForKeypress ()
    {
      To.ConsoleLine.nl(2).s ("Press any-key...");
      Console.ReadKey ();
    }

    private static void RunApplication (ref int result, TApplicationSettings settings)
    {
      if (result == 0)
      {
        try
        {
          TApplication application = new TApplication();
          application.Init (settings, System.Console.Error, System.Console.Out);
          application.Run();
        }
        catch (Exception e)
        {
          using (ConsoleUtility.EnterColorScope (ConsoleColor.White, ConsoleColor.DarkRed))
          {
            //System.Console.Error.WriteLine ("Execution aborted. Exception stack:");
            To.Error.s("Execution aborted. Exception stack:");
            for (; e != null; e = e.InnerException)
            {
              //System.Console.Error.WriteLine ("{0}: {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace);
              To.Error.s(e.GetType ().FullName).s(": ").s(e.Message).s(e.StackTrace);
            }
          }
          result = 1;
        }
      }
    }

    private static TApplicationSettings ParseCommandLineArguments (string[] args, ref int result)
    {
      TApplicationSettings settings = null;
      CommandLineClassParser<TApplicationSettings> parser = new CommandLineClassParser<TApplicationSettings> ();
      try
      {
        settings = parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        To.ConsoleLine.s (e.Message);
        To.ConsoleLine.s ("Usage:");
        To.ConsoleLine.s (parser.GetAsciiSynopsis (Environment.GetCommandLineArgs ()[0], System.Console.BufferWidth));
        result = 1;
      }
      return settings;
    }
  }
}