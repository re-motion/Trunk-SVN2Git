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
using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionTreeNodeTest
  {
    // TODO: Ctor test
    // TODO AE: Good idea.

    // TODO AE: Remove this method.
    [Test]
    [Explicit]
    public void UsageTest ()
    {
      var data = List.New (List.New ("a", "b"), List.New ("c", "d"));
      var test = AclExpansionTreeNode.New ("a", 2, List.New ("a", "b"));
    }
  }
}
