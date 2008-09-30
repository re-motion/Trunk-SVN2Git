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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionTest : AclToolsTestBase
  {
    [Test]
    public void Enumerable ()
    {
      var accessConditions = new AclExpansionAccessConditions();
      AclExpansionEntry[] aclExpansionEntries =
      {
        new AclExpansionEntry (User, Role, accessConditions, AccessTypeDefinitions),
        new AclExpansionEntry (User2, Role2, new AclExpansionAccessConditions(), AccessTypeDefinitions2),
        new AclExpansionEntry (User3, Role3, new AclExpansionAccessConditions(), AccessTypeDefinitions3)
      };

      var aclExpansion = new AclExpansion ((AclExpansionEntry[]) aclExpansionEntries.Clone ());

      //aclExpansionEntries[2] = null;

      int i = 0;
      foreach (AclExpansionEntry aclExpansionEntry in aclExpansion)
      {
        Assert.That (aclExpansionEntry, Is.EqualTo (aclExpansionEntries[i]));
        ++i;
      }

      //Assert.That (aclExpansionEntries, Is.EquivalentTo (aclExpansion));
    }
  }
}