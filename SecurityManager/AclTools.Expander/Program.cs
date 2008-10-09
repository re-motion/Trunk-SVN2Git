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
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Text.CommandLine;
using Remotion.Text.StringExtensions;
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
        using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
        {
          List<AclExpansionEntry> aclExpansion = GetAclExpansion();
          OutputAclExpansion(aclExpansion);
        }

        return 0;
      }
      catch (Exception e)
      {
        HandleException (e);
        return 1;
      }
    }

    private void OutputAclExpansion(List<AclExpansionEntry> aclExpansion)
    {
      OutputAclExpansionHierarchical (aclExpansion);
    }


    private void OutputAclExpansionSimple (List<AclExpansionEntry> aclExpansion)
    { 
      To.ConsoleLine.nl(10).s ("ACL Expansion");
      To.ConsoleLine.s ("====START====");
      foreach (AclExpansionEntry aclExpansionEntry in aclExpansion)
      {
        var stateArray = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates()).ToArray();

        //To.ConsoleLine.e (aclExpansionEntry);
        To.ConsoleLine.sb();
        To.Console.e ("user",aclExpansionEntry.User.UserName);
        To.Console.e ("role",aclExpansionEntry.Role);
        To.Console.e ("class", aclExpansionEntry.Class);
        //To.Console.e (aclExpansionEntry.StateCombinations[0].GetStates());
        To.Console.e ("states", stateArray);
        To.Console.e ("access", aclExpansionEntry.AccessTypeDefinitions);
        To.Console.e ("conditions", aclExpansionEntry.AccessConditions);
        To.Console.se();
      }
      To.ConsoleLine.s ("=====END=====");
    }

    private void OutputAclExpansionHierarchical (List<AclExpansionEntry> aclExpansion)
    {
      const bool repeatHierarchyEntriesInEachRow = false;

      var aclExpansionHierarchy =
          from expansion in aclExpansion
          group expansion by expansion.User
          into userGroup
              select new { User = userGroup.Key,
                RoleGroup =
              from user in userGroup
              group user by user.Role
              into roleGroup
                  select new { Role = roleGroup.Key, RoleGroup = roleGroup }};
         
               

      To.ConsoleLine.nl (10).s ("ACL Expansion");
      To.ConsoleLine.s ("====START====");
      foreach (var userGrouping in aclExpansionHierarchy)
      {
        To.Console.nl (2).e ("user", userGrouping.User);
        foreach (var roleGrouping in userGrouping.RoleGroup)
        {
          To.Console.indent().nl ().e ("role", roleGrouping.Role);
          foreach (var aclExpansionEntry in roleGrouping.RoleGroup)
          {

            var stateArray = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates()).ToArray();

            To.Console.indent ().nl ().sb ();
            if (repeatHierarchyEntriesInEachRow)
            {
              To.Console.e ("user", aclExpansionEntry.User.UserName);
              To.Console.e ("role", aclExpansionEntry.Role);
            }
            To.Console.e ("class", aclExpansionEntry.Class);
            //To.Console.e (aclExpansionEntry.StateCombinations[0].GetStates());
            To.Console.e ("states", stateArray);
            To.Console.e ("access", aclExpansionEntry.AccessTypeDefinitions);
            To.Console.e ("conditions", aclExpansionEntry.AccessConditions);
            To.Console.se();
            To.Console.unindent();
          }
          To.Console.unindent ();
        }
      }
      To.ConsoleLine.s ("=====END=====");
    }

    private List<AclExpansionEntry> GetAclExpansion ()
    {
      var aclExpander = new AclExpander ();

      //var aclExpansion = aclExpander.GetAclExpansionEntryListSortedAndDistinct();
      return aclExpander.GetAclExpansionEntryList ();
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