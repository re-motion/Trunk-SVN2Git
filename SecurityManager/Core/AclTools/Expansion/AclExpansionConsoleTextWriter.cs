/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using System.Linq;
using Remotion.Diagnostics.ToText;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  // TODO AE: Remove unused class.
  // TODO AE: Add ctor taking TextWriter and test the class.
  public class AclExpansionConsoleTextWriter : IAclExpansionWriter
  {
    public void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      // TODO AE: Consider inlining.
      WriteHierarchical (aclExpansion); 
    }

    public void WriteHierarchical (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      
      const bool repeatHierarchyEntriesInEachRow = false;

      var aclExpansionHierarchy =
          from expansion in aclExpansion
          group expansion by expansion.User
            into userGroup
            select new
            {
              User = userGroup.Key,
              RoleGroup =
                from user in userGroup // TODO AE: Extract method to avoid nested LINQ statements.
                group user by user.Role
                  into roleGroup
                  select new
                  {
                    Role = roleGroup.Key,
                    ClassGroup =
                    from role in roleGroup // TODO AE: Extract method to avoid nested LINQ statements.
                    group role by role.Class
                      into classGroup
                      select new
                      {
                        Class = classGroup.Key,
                        ClassGroup = classGroup
                      }
                  }
            };

      // TODO AE: Consider extracting methods to avoid deep nesting.
      To.ConsoleLine.nl (10).s ("ACL Expansion");
      To.ConsoleLine.s ("====START====");
      foreach (var userGrouping in aclExpansionHierarchy)
      {
        To.Console.nl (2).e ("user", userGrouping.User);
        foreach (var roleGrouping in userGrouping.RoleGroup)
        {
          To.Console.indent ().nl ().e ("role", roleGrouping.Role);
          foreach (var classGroup in roleGrouping.ClassGroup)
          {

            To.Console.indent ().nl ().e ("class", classGroup.Class);
            foreach (var aclExpansionEntry in classGroup.ClassGroup)
            {
              var stateArray = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates ()).ToArray ();

              To.Console.indent ().nl ().sb ();
              if (repeatHierarchyEntriesInEachRow) // TODO AE: Remove if-block, remove constant.
              {
                To.Console.e ("user", aclExpansionEntry.User.UserName);
                To.Console.e ("role", aclExpansionEntry.Role);
                To.Console.e ("class", aclExpansionEntry.Class);
              }
              To.Console.e ("states", stateArray);
              To.Console.e ("access", aclExpansionEntry.AllowedAccessTypes);
              To.Console.e ("conditions", aclExpansionEntry.AccessConditions);
              To.Console.se ();
              To.Console.unindent ();
            }
            To.Console.unindent ();
          }
          To.Console.unindent ();
        }
      }
      To.ConsoleLine.s ("=====END=====");
    }


    // TODO AE: Remove unused member.
    public void WriteSimple (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
      To.ConsoleLine.nl (10).s ("ACL Expansion");
      To.ConsoleLine.s ("====START====");
      foreach (AclExpansionEntry aclExpansionEntry in aclExpansion)
      {
        var stateArray = aclExpansionEntry.StateCombinations.SelectMany (x => x.GetStates ()).ToArray ();

        To.ConsoleLine.sb ();
        To.Console.e ("user", aclExpansionEntry.User.UserName);
        To.Console.e ("role", aclExpansionEntry.Role);
        To.Console.e ("class", aclExpansionEntry.Class);
        To.Console.e ("states", stateArray);
        To.Console.e ("access", aclExpansionEntry.AllowedAccessTypes);
        To.Console.e ("conditions", aclExpansionEntry.AccessConditions);
        To.Console.se ();
      }
      To.ConsoleLine.s ("=====END=====");
    }
  }
}