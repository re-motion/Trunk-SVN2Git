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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [ToTextSpecificHandler]
  public class UserToTextSpecificTypeHandler : ToTextSpecificTypeHandler<User>
  {
    public override void ToText (User u, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<User> ().e ("user", u.UserName).e ("title", u.Title).e ("first", u.FirstName).e ("last", u.LastName);
      toTextBuilder.e ("display", u.DisplayName).e ("owning group", u.OwningGroup).e ("tenat", u.Tenant);
      toTextBuilder.nl().e ("roles", u.Roles);
      toTextBuilder.ie();
    }
  }

  [TestFixture]
  public class AclExpansionTest : AclToolsTestBase
  {
    [Test]
    public void Enumerable ()
    {
      AclExpansionEntry[] aclExpansionEntries = GetAclExpansionEntries();
      var aclExpansion = new AclExpansion ((AclExpansionEntry[]) aclExpansionEntries.Clone ());

      int i = 0;
      foreach (AclExpansionEntry aclExpansionEntry in aclExpansion)
      {
        //To.Console.e (i + 1).s (") ").e (aclExpansionEntry.User).nl ();
        Assert.That (aclExpansionEntry, Is.EqualTo (aclExpansionEntries[i]));
        ++i;
      }
    }

    [Test]
    [Explicit]
    public void Linq ()
    {
      AclExpansionEntry[] aclExpansionEntries = GetAclExpansionEntries ();
      var aclExpansion = new AclExpansion ((AclExpansionEntry[]) aclExpansionEntries.Clone ());

      var query = from e in aclExpansion
                  where e.User.Title != ""
                  select e.User.DisplayName;

      To.Console.e (query.ToArray());
    }

    private AclExpansionEntry[] GetAclExpansionEntries ()
    {
      var accessConditions = new AclExpansionAccessConditions();
      return new AclExpansionEntry[] { new AclExpansionEntry (User, Role, accessConditions, AccessTypeDefinitions), new AclExpansionEntry (User2, Role2, new AclExpansionAccessConditions(), AccessTypeDefinitions2), new AclExpansionEntry (User3, Role3, new AclExpansionAccessConditions(), AccessTypeDefinitions3) };
    }
  }
}