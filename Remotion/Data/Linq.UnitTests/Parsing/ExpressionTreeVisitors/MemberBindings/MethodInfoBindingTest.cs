// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Data.Linq.Parsing.ExpressionTreeVisitors.MemberBindings;

namespace Remotion.Data.Linq.UnitTests.Parsing.ExpressionTreeVisitors.MemberBindings
{
  [TestFixture]
  public class MethodInfoBindingTest : MemberBindingTestBase
  {
    [Test]
    public void MatchesReadAccess_True ()
    {
      var binding = new MethodInfoBinding (Method, AssociatedExpression);
      Assert.That (binding.MatchesReadAccess (Method), Is.True);
    }

    [Test]
    public void MatchesReadAccess_True_WithProperty ()
    {
      var binding = new MethodInfoBinding (Method, AssociatedExpression);
      Assert.That (binding.MatchesReadAccess (Property), Is.True);
    }

    [Test]
    public void MatchesReadAccess_False_WithOtherProperty ()
    {
      var binding = new MethodInfoBinding (Method, AssociatedExpression);
      Assert.That (binding.MatchesReadAccess (OtherProperty), Is.False);
    }

    [Test]
    public void MatchesReadAccess_False_WithReadOnlyProperty ()
    {
      var binding = new MethodInfoBinding (Method, AssociatedExpression);
      Assert.That (binding.MatchesReadAccess (WriteOnlyProperty), Is.False);
    }

    [Test]
    public void MatchesReadAccess_False ()
    {
      var binding = new MethodInfoBinding (Method, AssociatedExpression);
      Assert.That (binding.MatchesReadAccess (Field), Is.False);
    }
  }
}
