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
using System.Collections.Generic;
using System.IO;
using Remotion.Data.DomainObjects;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.Text.CommandLine;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expander
{
  //public class Program
  //{
  //  public static int Main (string[] args)
  //  {
  //    AclExpanderApplicationSettings arguments = GetArguments (args);
  //    if (arguments == null)
  //    {
  //      To.Console.nl().s ("No arguments passed => aborting. Press any-key...");
  //      Console.ReadKey ();
  //      return 1;
  //    }

  //    AclExpanderApplication program = new AclExpanderApplication (arguments);
  //    int result = program.Run ();
  //    To.Console.nl ().s ("Press any-key...");
  //    Console.ReadKey ();
  //    return result;
  //  }
  //}

  public interface IApplicationRunner<TApplicationSettings>
  {
    void Init (TApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter);
    //int Run (TApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter);
    int Run ();
  }


  public class ConsoleApplication<TApplication, TApplicationSettings> 
    where TApplication: IApplicationRunner<TApplicationSettings>, new()
    where TApplicationSettings : class
  {
    public static int MainDo (string[] args)
    {
      int result = 0;

      TApplicationSettings settings = null;
      CommandLineClassParser<TApplicationSettings> parser = new CommandLineClassParser<TApplicationSettings> ();
      try
      {
        settings = parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        System.Console.WriteLine (e.Message);
        System.Console.WriteLine ("Usage:");
        System.Console.WriteLine (parser.GetAsciiSynopsis (Environment.GetCommandLineArgs ()[0], System.Console.BufferWidth));
        result = 1;
      }

      if (result == 0)
      {
        try
        {
          TApplication application = new TApplication();
          application.Init (settings, System.Console.Error, System.Console.Out);
          result = application.Run();
        }
        catch (Exception e)
        {
          using (ConsoleUtility.EnterColorScope (ConsoleColor.White, ConsoleColor.DarkRed))
          {
            System.Console.Error.WriteLine ("Execution aborted. Exception stack:");
            for (; e != null; e = e.InnerException)
              System.Console.Error.WriteLine ("{0}: {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace);
          }
          result = 1;
        }
      }

      To.Console.nl ().s ("Press any-key...");
      Console.ReadKey ();
      return result;
    }
  }


  public class Program : ConsoleApplication<AclExpanderApplication, AclExpanderApplicationSettings>
  {
    public static int Main (string[] args)
    {
      return MainDo (args);
    }
  }
  


  public class AclExpanderApplication : IApplicationRunner<AclExpanderApplicationSettings> 
  {
    AclExpanderApplicationSettings _settings;
    private TextWriter _errorWriter;
    private TextWriter _logWriter;

    public AclExpanderApplication ()
    {
    }


    //public AclExpanderApplication (AclExpanderApplicationSettings arguments)
    //{
    //  ArgumentUtility.CheckNotNull ("arguments", arguments);
    //  _settings = arguments;
    //}


    //public static AclExpanderApplicationSettings GetArguments (string[] args)
    //{
    //  CommandLineClassParser parser = new CommandLineClassParser (typeof (AclExpanderApplicationSettings));

    //  try
    //  {
    //    return (AclExpanderApplicationSettings) parser.Parse (args);
    //  }
    //  catch (CommandLineArgumentException e)
    //  {
    //    Console.WriteLine (e.Message);
    //    WriteUsage (parser);

    //    return null;
    //  }
    //}

    //private static void WriteUsage (CommandLineClassParser parser)
    //{
    //  Console.WriteLine ("Usage:");

    //  string commandName = Environment.GetCommandLineArgs ()[0];
    //  Console.WriteLine (parser.GetAsciiSynopsis (commandName, Console.BufferWidth));
    //}


    public void Init (AclExpanderApplicationSettings settings, TextWriter errorWriter, TextWriter logWriter)
    {
      ArgumentUtility.CheckNotNull ("settings", settings);
      ArgumentUtility.CheckNotNull ("errorWriter", errorWriter);
      ArgumentUtility.CheckNotNull ("logWriter", logWriter);

      _settings = settings;
      _errorWriter = errorWriter;
      _logWriter = logWriter;
    }


    public int Run ()
    {
      try
      {
        using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
        {
          List<AclExpansionEntry> aclExpansion = GetAclExpansion();

          //var aclExpansionWriter = new AclExpansionConsoleTextWriter();
          //aclExpansionWriter.WriteHierarchical (aclExpansion);
          WriteAclExpansionAsHtmlSpikeToStreamWriter (aclExpansion);
        }

        return 0;
      }
      catch (Exception e)
      {
        HandleException (e);
        return 1;
      }
    }


    public void WriteAclExpansionAsHtmlSpikeToStringWriter (List<AclExpansionEntry> aclExpansion)
    {
      var stringWriter = new StringWriter ();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (stringWriter, true);
      aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansion);
      To.ConsoleLine.s (stringWriter.ToString ());
    }

    public void WriteAclExpansionAsHtmlSpikeToStreamWriter (List<AclExpansionEntry> aclExpansion)
    {
      string aclExpansionFileName = "c:\\temp\\AclExpansion_" + FileNameTimestampNow () + ".html";
      using (var streamWriter = new StreamWriter (aclExpansionFileName))
      {
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (streamWriter, true);
        aclExpansionHtmlWriter.WriteAclExpansionAsHtml (aclExpansion);
      }
    }

    private string FileNameTimestamp (DateTime dt)
    {
      return StringUtility.ConcatWithSeparator (new [] { dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond }, "_");
    }

    private string FileNameTimestampNow ()
    {
      return FileNameTimestamp (DateTime.Now);
    }


    private List<AclExpansionEntry> GetAclExpansion ()
    {
      var aclExpander = 
        new AclExpander (
          new AclExpanderUserFinder (_settings.UserFirstName, _settings.UserLastName, _settings.UserName), new AclExpanderAclFinder ()
        );

      return aclExpander.GetAclExpansionEntryListSortedAndDistinct();
      //return aclExpander.GetAclExpansionEntryList ();
    }

 
    private void HandleException (Exception exception)
    {
      Assertion.IsNotNull (_errorWriter, "Error writer not initialzed.");
      if (_settings.Verbose)
      {
        _errorWriter.WriteLine ("Execution aborted. Exception stack:");

        for (; exception != null; exception = exception.InnerException)
        {
          _errorWriter.WriteLine ("{0}: {1}\n{2}", exception.GetType ().FullName, exception.Message, exception.StackTrace);
        }
      }
      else
      {
        _errorWriter.WriteLine ("Execution aborted: {0}", exception.Message);
      }
    }

    //private void WriteInfo (string text, params object[] args)
    //{
    //  if (_settings.Verbose)
    //    Console.WriteLine (text, args);
    //}


    //public void Run (AclExpanderApplicationSettings parameters)
    //{
    //  throw new System.NotImplementedException();
    //}

  }

}