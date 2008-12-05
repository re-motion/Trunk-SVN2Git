// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
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
      To.ConsoleLine.e(aclExpansionEntry.GetStateCombinations()); // TODO AE: To.Console is never executed. Replace by Dev.Null = ...;
    }

    // TODO AE: Remaining TDD-style unit tests are missing.


  }
}
