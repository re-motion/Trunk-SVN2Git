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
using System;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class CastClassExpressionTest : SnippetGenerationBaseTest
  {
    [Test]
    public void Cast ()
    {
      var methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (IConvertible));
      methodEmitter.SetParameterTypes (typeof (object));
      methodEmitter.ImplementByReturning (new CastClassExpression (typeof (IConvertible), methodEmitter.ArgumentReferences[0].ToExpression ()));

      Assert.That (InvokeMethod((object) 12), Is.EqualTo (12));
    }
  }
}
