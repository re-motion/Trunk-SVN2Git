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
      CommandLineArguments arguments = GetArguments (args);
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

    private static CommandLineArguments GetArguments (string[] args)
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (CommandLineArguments));

      try
      {
        return (CommandLineArguments) parser.Parse (args);
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

    readonly CommandLineArguments _arguments;

    private Program (CommandLineArguments arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);
      _arguments = arguments;
    }

    public int Run ()
    {
      try
      {
        var aclExpander = new AclExpander();

        //var aclExpansion = aclExpander.GetAclExpansionEntryListSortedAndDistinct();
        var aclExpansion = aclExpander.GetAclExpansionEntryList (); // TODO: Use GetAclExpansionEntryListSortedAndDistinct again

        To.ConsoleLine.s ("ACL Expansion");
        To.ConsoleLine.s ("====START====");
        foreach (AclExpansionEntry aclExpansionEntry in aclExpansion)
        {
          To.ConsoleLine.e (aclExpansionEntry);
        }
        To.ConsoleLine.s ("=====END=====");

        return 0;
      }
      catch (Exception e)
      {
        HandleException (e);
        return 1;
      }
    }

    //private void ImportMetadata (ClientTransaction transaction)
    //{
    //  MetadataImporter importer = new MetadataImporter (transaction);
    //  WriteInfo ("Importing metadata file '{0}'.", _arguments.MetadataFile);
    //  importer.Import (_arguments.MetadataFile);
    //}

    //private void ImportLocalization (ClientTransaction transaction)
    //{
    //  CultureImporter importer = new CultureImporter (transaction);
    //  LocalizationFileNameStrategy localizationFileNameStrategy = new LocalizationFileNameStrategy ();
    //  string[] localizationFileNames = localizationFileNameStrategy.GetLocalizationFileNames (_arguments.MetadataFile);

    //  foreach (string localizationFileName in localizationFileNames)
    //  {
    //    WriteInfo ("Importing localization file '{0}'.", localizationFileName);
    //    importer.Import (localizationFileName);
    //  }

    //  if (localizationFileNames.Length == 0)
    //    WriteInfo ("Localization files not found.");
    //}

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