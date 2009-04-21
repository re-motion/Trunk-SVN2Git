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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class IfStatementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void IfWithTrueCondition ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));
      methodEmitter.AddStatement (new IfStatement (new SameConditionExpression (NullExpression.Instance, NullExpression.Instance),
          new ReturnStatement (new ConstReference ("True"))));
      methodEmitter.AddStatement (new ReturnStatement (new ConstReference ("False")));

      Assert.AreEqual ("True", InvokeMethod());
    }

    [Test]
    public void FalseCondition ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));
      methodEmitter.AddStatement (new IfStatement (new SameConditionExpression (NullExpression.Instance, new ConstReference ("5").ToExpression()),
          new ReturnStatement (new ConstReference ("True"))));
      methodEmitter.AddStatement (new ReturnStatement (new ConstReference ("False")));

      Assert.AreEqual ("False", InvokeMethod ());
    }
  }
}
