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
using System.Diagnostics;
using System.IO;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication;
using Remotion.Text.CommandLine;
using Remotion.Text.StringExtensions;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.ConsoleApplication
{
  // TODO QAE: Rationale for /wait+?. MGi: For all cases where comand window would otherwise close, without anyone having the chance to seing the program output.
  // TODO QAE: Too generic for AclExpander, evaluate moving to Remotion.Core or combine to make less generic (and less complex). MGi: Was not planned to use it only for AclExpander, but is planned as a general base class for command line apps

  /// <summary>
  /// Console application class: Supplies command line parsing (including standard command line switches; 
  /// see <see cref="ConsoleApplicationSettings"/>) and standardized error handling and output.
  /// </summary>
  /// <remarks>
  /// Standard command line switches: "/?" to show the usage information, "/wait+" to wait for a keypress at the end of program execution.
  /// </remarks>
  /// <example>
  /// <code>
  /// <![CDATA[
  /// public class Program 
  /// {
  ///   public static int Main (string[] args)
  ///   {
  ///     var consoleApplication = new ConsoleApplication<AclExpanderApplication, AclExpanderApplicationSettings>();
  ///     return consoleApplication.Main (args);
  ///   }
  /// }
  /// ]]>
  /// </code>
  /// </example>
  /// <typeparam name="TApplication">The application implementation class. Supplied with an log- and error-output-stream
  /// by the <see cref="ConsoleApplication"/>.
  /// Needs to implement <see cref="IApplicationRunner{TApplicationSettings}"/>.
  /// </typeparam>
  /// <typeparam name="TApplicationSettings">The settings for the <see cref="TApplication"/>. 
  /// Needs to derive from <see cref="ConsoleApplicationSettings"/>.
  /// </typeparam>

  public class ConsoleApplication<TApplication, TApplicationSettings> 
      where TApplication: IApplicationRunner<TApplicationSettings>, new()
      where TApplicationSettings : ConsoleApplicationSettings, new()
  {
    private readonly ToTextBuilder _logToTextBuilder;
    private readonly ToTextBuilder _errorToTextBuilder;
    private readonly CommandLineClassParser<TApplicationSettings> _parser = new CommandLineClassParser<TApplicationSettings> ();
    private readonly TextWriter _errorWriter;
    private readonly TextWriter _logWriter;
    private readonly int _bufferWidth;
    private readonly IWait _waitAtEnd;
    private int _result;
    private string _synopsis = "(Application synopsis not yet retrieved)";

    public ConsoleApplication (TextWriter errorWriter, TextWriter logWriter, int bufferWidth, IWait waitAtEnd)
    {
      _errorWriter = errorWriter;
      _logWriter = logWriter;
      _errorToTextBuilder = new ToTextBuilder (To.ToTextProvider, errorWriter);
      _logToTextBuilder = new ToTextBuilder (To.ToTextProvider, logWriter);
      _bufferWidth = bufferWidth;
      _waitAtEnd = waitAtEnd;
    }

    public ConsoleApplication () : this (System.Console.Error, System.Console.Out, 80, new WaitForConsoleKeypress()) {}

    public TApplicationSettings Settings { get; set; }


    public int Main (string[] args)
    {
      _result = 0;
      ParseSynopsis (args);
      ParseCommandLineArguments (args);
      ConsoleApplicationMain();
      WaitForKeypress();
      return _result;
    }

    private void ConsoleApplicationMain ()
    {
      if (_result == 0)
      {
        if (Settings.Mode == ConsoleApplicationSettings.ShowUsageMode.ShowUsage)
        {
          OutputApplicationUsage();
        }
        else
        {
          RunApplication (Settings);
        }
      }
    }


    public string Synopsis
    {
      get { return _synopsis; }
    }

 
    public int BufferWidth
    {
      get { return _bufferWidth; }
    }


    private void OutputApplicationUsage ()
    {
      _logToTextBuilder.nl (2).s ("Application Usage: ");
      _logToTextBuilder.nl (2).s (Synopsis).nl(2);
    }

    private void WaitForKeypress ()
    {
      if (Settings.WaitForKeypress)
      {
        _logToTextBuilder.nl (2).s ("Press any-key...");
        _waitAtEnd.Wait();
      }
    }

    public virtual void RunApplication (TApplicationSettings settings)
    {
      try
      {
        var application = CreateApplication();
        application.Run (settings, _errorWriter, _logWriter);
      }
      catch (Exception e)
      {
        _result = 1;
        using (ConsoleUtility.EnterColorScope (ConsoleColor.White, ConsoleColor.DarkRed))
        {
          _errorToTextBuilder.s ("Execution aborted. Exception stack:");
          for (; e != null; e = e.InnerException)
          {
            _errorToTextBuilder.s (e.GetType ().FullName).s (": ").s (e.Message).s (e.StackTrace);
          }
        }
      }
    }


    public virtual IApplicationRunner<TApplicationSettings> CreateApplication ()
    {
      return new TApplication();
    }


    public virtual void ParseCommandLineArguments (string[] args)
    {
      try
      {
        Settings = _parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        _errorToTextBuilder.nl ().s ("An error occured: ").s (e.Message);
        OutputApplicationUsage ();
        // TODO AE: Return bool or throw exception instead of setting a global flag.
        // TODO AE: Remove flag.
        _result = 1;
        Settings = new TApplicationSettings(); // Use default settings
      }
    }

    public void ParseSynopsis (string[] args)
    {
      try
      {
        string applicationName = Process.GetCurrentProcess().MainModule.FileName.RightUntilChar('\\');
        _synopsis = _parser.GetAsciiSynopsis (applicationName, _bufferWidth);
      }
      catch (Exception e)
      {
        _synopsis = "(An error occured while retrieving the application usage synopsis: " + e.Message + ")";  
      }
    }


  }
}