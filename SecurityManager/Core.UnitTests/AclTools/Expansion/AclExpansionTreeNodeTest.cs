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