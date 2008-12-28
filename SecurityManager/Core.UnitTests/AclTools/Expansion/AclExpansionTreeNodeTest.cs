// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
 

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionTreeNodeTest : AclToolsTestBase
  {
    [Test]
    public void CtorTest ()
    {
      List<Role> children = ListMother.New (Role,Role2,Role3);
      var aclExpansionTreeNode = new AclExpansionTreeNode<User, Role> (User, 17, children);
      Assert.That (aclExpansionTreeNode.Key,Is.EqualTo(User));
      Assert.That (aclExpansionTreeNode.NumberLeafNodes, Is.EqualTo (17));
      Assert.That (aclExpansionTreeNode.Children, Is.EqualTo (children));
    }

    [Test]
    public void FactoryTest ()
    {
      List<Role> children = ListMother.New (Role, Role2, Role3);
      var aclExpansionTreeNode = AclExpansionTreeNode.New (User, 17, children);
      Assert.That (aclExpansionTreeNode.Key, Is.EqualTo (User));
      Assert.That (aclExpansionTreeNode.NumberLeafNodes, Is.EqualTo (17));
      Assert.That (aclExpansionTreeNode.Children, Is.EqualTo (children));
    }


    [Test]
    public void ToTextTest ()
    {
      List<Role> children = ListMother.New (Role, Role2, Role3);
      var aclExpansionTreeNode = AclExpansionTreeNode.New (User, 17, children);
      var result = To.String.e (aclExpansionTreeNode).ToString ();
      var resultExpected =
      #region
 @"
([""DaUs""],17,
{[""DaUs"",""Da Group"",""Supreme Being""],[""mr.smith"",""Anotha Group"",""Working Drone""],[""ryan_james"",""Da 3rd Group"",""Combatant""]})";
      #endregion
      Assert.That (result, Is.EqualTo (resultExpected));
    }
  }
}
