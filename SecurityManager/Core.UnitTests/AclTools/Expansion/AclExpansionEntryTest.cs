// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionEntryTest : AclToolsTestBase
  {
    [Test]
    public void CtorTest ()
    {
      var accessConditions = new AclExpansionAccessConditions();
      var aclExpansionEntry = new AclExpansionEntry (User, Role, Acl, accessConditions, AccessTypeDefinitions, AccessTypeDefinitions2);
      Assert.That (aclExpansionEntry.User, Is.EqualTo (User));
      Assert.That (aclExpansionEntry.Role, Is.EqualTo (Role));
      Assert.That (aclExpansionEntry.Class, Is.EqualTo (Acl.Class));
      Assert.That (aclExpansionEntry.GetStateCombinations(), Is.EqualTo (Acl.StateCombinations));
      Assert.That (aclExpansionEntry.AccessConditions, Is.EqualTo (accessConditions));
      Assert.That (aclExpansionEntry.AllowedAccessTypes, Is.EqualTo (AccessTypeDefinitions));
      Assert.That (aclExpansionEntry.DeniedAccessTypes, Is.EqualTo (AccessTypeDefinitions2));
    }

      
    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = @"StateCombinations not defined for StatelessAccessControlList. Test for ""is StatefulAccessControlList"" in calling code.") ]
    public void StateCombinationsForStatelessAclThrowsTest ()
    {
      SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition ();
      var statlessAcl = TestHelper.CreateStatelessAcl (classDefinition);

      var accessConditions = new AclExpansionAccessConditions ();
      var aclExpansionEntry = new AclExpansionEntry (User, Role, statlessAcl, accessConditions, AccessTypeDefinitions, AccessTypeDefinitions2);
      Dev.Null = aclExpansionEntry.GetStateCombinations(); 
    }


    [Test]
    public void GetStateCombinationsTest ()
    {
      SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition ();
      var aclExpansionEntry = new AclExpansionEntry (User, Role, Acl, new AclExpansionAccessConditions (), AccessTypeDefinitions, AccessTypeDefinitions2);
      var result = aclExpansionEntry.GetStateCombinations ();
      Assert.That (result, Is.EqualTo(Acl.StateCombinations));
    }

    [Test]
    public void ToTextTest ()
    {
      var aclExpansionEntry = new AclExpansionEntry (User, Role, Acl, new AclExpansionAccessConditions (), AccessTypeDefinitions, AccessTypeDefinitions2);
      var resultEpected = @"[user=""DaUs"",role=[""DaUs"",""Da Group"",""Supreme Being""],allowed={[""Read""],[""Write""],[""Delete""]},denied={[""Read""],[""Delete""]},conditions=[]]";
      var result = To.String.e (aclExpansionEntry).ToString ();
      Assert.That (result, Is.EqualTo (resultEpected));
    }

  }
}
