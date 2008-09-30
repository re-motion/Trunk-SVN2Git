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
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionEntryTest : AclToolsTestBase
  {
    [Test]
    public void Ctor ()
    {
      var accessConditions = new AclExpansionAccessConditions();
      var aclExpansionEntry = new AclExpansionEntry (User,Role,accessConditions,AccessTypeDefinitions);
      Assert.That (aclExpansionEntry.User, Is.EqualTo (User));
      Assert.That (aclExpansionEntry.Role, Is.EqualTo (Role));
      Assert.That (aclExpansionEntry.AccessConditions, Is.EqualTo (accessConditions));
      Assert.That (aclExpansionEntry.AccessTypeDefinitions, Is.EqualTo (AccessTypeDefinitions));
    }
  }
}