// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;
using System.Linq;

namespace Remotion.Data.UnitTests.Linq.Parsing.Structure.IntermediateModel
{
  [TestFixture]
  public class SingleOrDefaultExpressionNodeTest : ExpressionNodeTestBase
  {
    [Test]
    public void SupportedMethod_WithoutPredicate ()
    {
      MethodInfo method = GetGenericMethodDefinition (q => q.SingleOrDefault ());
      Assert.That (SingleOrDefaultExpressionNode.SupportedMethods, List.Contains (method));
    }

    [Test]
    public void SupportedMethod_WithPredicate ()
    {
      MethodInfo method = GetGenericMethodDefinition (q => q.SingleOrDefault (i => i > 5));
      Assert.That (SingleOrDefaultExpressionNode.SupportedMethods, List.Contains (method));
    }
  }
}