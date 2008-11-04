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
using System.IO;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication;
using Remotion.Text.CommandLine;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication
{
  public class ConsoleApplication<TApplication, TApplicationSettings> 
      where TApplication: IApplicationRunner<TApplicationSettings>, new()
      where TApplicationSettings : ConsoleApplicationSettings
  {

    private readonly ToTextBuilder _logToTextBuilder;
    private readonly ToTextBuilder _errorToTextBuilder;
    private readonly CommandLineClassParser<TApplicationSettings> _parser = new CommandLineClassParser<TApplicationSettings> ();
    private readonly int _bufferWidth;
    private readonly IWait _waitAtEnd;


    public ConsoleApplication (TextWriter errorWriter, TextWriter logWriter, int bufferWidth, IWait waitAtEnd)
    {
      _logToTextBuilder = new ToTextBuilder (To.ToTextProvider, logWriter);
      _errorToTextBuilder = new ToTextBuilder (To.ToTextProvider, errorWriter);
      _bufferWidth = bufferWidth;
      _waitAtEnd = waitAtEnd;
    }


    public ConsoleApplication () : this (System.Console.Error, System.Console.Out, System.Console.BufferWidth, new WaitForConsoleKeypress()) {}


    public int Main (string[] args)
    {
      int result = 0;
      TApplicationSettings settings = ParseCommandLineArguments(args, ref result);
      RunApplication(ref result, settings);
      WaitForKeypress();
      return result;
    }

    private void WaitForKeypress ()
    {
      _logToTextBuilder.nl (2).s ("Press any-key...");
      //Console.ReadKey ();
      _waitAtEnd.Wait();
    }

    private void RunApplication (ref int result, TApplicationSettings settings)
    {
      if (result == 0)
      {
        try
        {
          TApplication application = new TApplication();
          //application.Init (settings, System.Console.Error, System.Console.Out);
          application.Run (settings, System.Console.Error, System.Console.Out);
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

    private TApplicationSettings ParseCommandLineArguments (string[] args, ref int result)
    {
      TApplicationSettings settings = null;
      //_parser = new CommandLineClassParser<TApplicationSettings> ();
      try
      {
        settings = _parser.Parse (args);
        if (settings.Mode == ConsoleApplicationSettings.ShowUsageMode.ShowUsage)
        {
          _logToTextBuilder.nl (2).s ("Application Usage: ");
          _logToTextBuilder.nl().s (GetSynopsis (args));
        }
      }
      catch (CommandLineArgumentException e)
      {
        _errorToTextBuilder.s (e.Message);
        _errorToTextBuilder.s ("Usage:");
        _errorToTextBuilder.s (GetSynopsis (args));
        result = 1;
      }

      return settings;
    }

    //public string GetSynopsis (CommandLineClassParser<TApplicationSettings> parser)
    public string GetSynopsis (string[] args)
    {
      //return _parser.GetAsciiSynopsis (Environment.GetCommandLineArgs ()[0], System.Console.BufferWidth);
      return _parser.GetAsciiSynopsis (args[0], _bufferWidth);
    }
  }

  public interface IWait
  {
    void Wait ();
  }

  public class WaitForConsoleKeypress : IWait
  {
    public void Wait ()
    {
      Console.ReadKey ();
    }
  }

}