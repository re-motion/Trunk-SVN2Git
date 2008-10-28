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
  public class Program
  {
    public static int Main (string[] args)
    {
      AclExpanderCommandLineArguments arguments = GetArguments (args);
      if (arguments == null)
      {
        To.Console.nl().s ("No arguments passed => aborting. Press any-key...");
        Console.ReadKey ();
        return 1;
      }

      Program program = new Program (arguments);
      int result = program.Run ();
      To.Console.nl ().s ("Press any-key...");
      Console.ReadKey ();
      return result;
    }

    private static AclExpanderCommandLineArguments GetArguments (string[] args)
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (AclExpanderCommandLineArguments));

      try
      {
        return (AclExpanderCommandLineArguments) parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        Console.WriteLine (e.Message);
        WriteUsage (parser);

        return null;
      }
    }

    private static void WriteUsage (CommandLineClassParser parser)
    {
      Console.WriteLine ("Usage:");

      string commandName = Environment.GetCommandLineArgs ()[0];
      Console.WriteLine (parser.GetAsciiSynopsis (commandName, Console.BufferWidth));
    }

    readonly AclExpanderCommandLineArguments _arguments;

    private Program (AclExpanderCommandLineArguments arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);
      _arguments = arguments;
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
      string aclExpansionFileName = "c:\\temp\\AclExpansion_" + FileNameTimestamp(DateTime.Now) + ".html";
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

    private List<AclExpansionEntry> GetAclExpansion ()
    {
      var aclExpander = new AclExpander ();

      return aclExpander.GetAclExpansionEntryListSortedAndDistinct();
      //return aclExpander.GetAclExpansionEntryList ();
    }

 
    private void HandleException (Exception exception)
    {
      if (_arguments.Verbose)
      {
        Console.Error.WriteLine ("Execution aborted. Exception stack:");

        for (; exception != null; exception = exception.InnerException)
        {
          Console.Error.WriteLine ("{0}: {1}\n{2}", exception.GetType ().FullName, exception.Message, exception.StackTrace);
        }
      }
      else
      {
        Console.Error.WriteLine ("Execution aborted: {0}", exception.Message);
      }
    }

    //private void WriteInfo (string text, params object[] args)
    //{
    //  if (_arguments.Verbose)
    //    Console.WriteLine (text, args);
    //}
  }


}