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
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class TryFinallyStatementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void TryFinallyWithoutException ()
    {
      FieldReference tryField = ClassEmitter.CreateField ("TryExecuted", typeof (bool));
      FieldReference finallyField = ClassEmitter.CreateField ("FinallyExecuted", typeof (bool));

      var methodEmitter = GetMethodEmitter (false);
      Statement[] tryBlock = new Statement[]
      {
        new AssignStatement (tryField, new ConstReference (true).ToExpression())
      };
      Statement[] finallyBlock = new Statement[]
      {
        new AssignStatement (finallyField, new ConstReference (true).ToExpression())
      };

      methodEmitter.AddStatement (new TryFinallyStatement (tryBlock, finallyBlock));
      methodEmitter.AddStatement (new ReturnStatement ());

      InvokeMethod ();
      Assert.IsTrue ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), tryField.Reference.Name));
      Assert.IsTrue ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), finallyField.Reference.Name));
    }

    [Test]
    public void TryFinallyWithException ()
    {
      FieldReference tryField = ClassEmitter.CreateField ("TryExecuted", typeof (bool));
      FieldReference finallyField = ClassEmitter.CreateField ("FinallyExecuted", typeof (bool));

      var methodEmitter = GetMethodEmitter (false);
      Statement[] tryBlock = new Statement[]
      {
        new ThrowStatement (typeof (Exception), "Expected exception"),
        new AssignStatement (tryField, new ConstReference (true).ToExpression())
      };
      Statement[] finallyBlock = new Statement[]
      {
        new AssignStatement (finallyField, new ConstReference (true).ToExpression())
      };

      methodEmitter.AddStatement (new TryFinallyStatement (tryBlock, finallyBlock));
      methodEmitter.AddStatement (new ReturnStatement ());

      try
      {
        InvokeMethod ();
        Assert.Fail ("Expected exception");
      }
      catch (Exception ex)
      {
        Assert.AreEqual (typeof (Exception), ex.GetType ());
        Assert.AreEqual ("Expected exception", ex.Message);
      }
      Assert.IsFalse ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), tryField.Reference.Name));
      Assert.IsTrue ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), finallyField.Reference.Name));
    }
  }
}
