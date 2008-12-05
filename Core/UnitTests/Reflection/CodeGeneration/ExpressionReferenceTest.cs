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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class ExpressionReferenceTest : SnippetGenerationBaseTest
  {
    [Test]
    public void ExpressionReference ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));
      
      ExpressionReference expressionReference = new ExpressionReference (typeof (string), new ConstReference ("bla").ToExpression(), methodEmitter);
      methodEmitter.ImplementByReturning (new ReferenceExpression (expressionReference));

      Assert.AreEqual ("bla", InvokeMethod());
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Expressions cannot be assigned to.")]
    public void ExpressionReferenceCannotBeStored ()
    {
      CustomMethodEmitter methodEmitter = GetUnsavedMethodEmitter (false);
      ExpressionReference expressionReference = new ExpressionReference (typeof (string), new ConstReference ("bla").ToExpression (), methodEmitter);
      expressionReference.StoreReference (null);
    }

    [Test]
    public void LoadAddressOfExpressionReference ()
    {
      CustomMethodEmitter methodEmitter = GetMethodEmitter (false);
      methodEmitter.SetReturnType (typeof (string));

      ExpressionReference expressionReference =
          new ExpressionReference (typeof (StructWithMethod), new InitObjectExpression (methodEmitter, typeof (StructWithMethod)), methodEmitter);
      ExpressionReference addressReference =
          new ExpressionReference (typeof (StructWithMethod).MakeByRefType(), expressionReference.ToAddressOfExpression(), methodEmitter);
      MethodInvocationExpression methodCall =
          new MethodInvocationExpression (addressReference, typeof (StructWithMethod).GetMethod ("Method"));

      methodEmitter.ImplementByReturning (methodCall);

      Assert.AreEqual ("StructMethod", InvokeMethod());
    }
  }
}
