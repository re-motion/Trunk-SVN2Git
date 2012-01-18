// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Threading;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Web.WxeFunctions;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.UnitTests.DomainObjects.Web
{
  [TestFixture]
  [CLSCompliant (false)]
  public class WxeTransactedFunctionTest : WxeFunctionBaseTest
  {
    [Test]
    public void WxeTransactedFunctionCreateRoot ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateRootTestTransactedFunction (originalScope).Execute (Context);
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateChildIfParent ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateRootWithChildTestTransactedFunction (originalScope.ScopedTransaction, new CreateChildIfParentTestTransactedFunction()).Execute (
            Context);
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));
      }
    }

    [Test]
    public void WxeTransactedFunctionNone ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateNoneTestTransactedFunction (originalScope).Execute (Context);
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateNewAutoCommit ()
    {
      SetDatabaseModifyable();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        SetInt32Property (5, ClientTransaction.CreateRootTransaction());

        new AutoCommitTestTransactedFunction (
            WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit, DomainObjectIDs.ClassWithAllDataTypes1).Execute (Context);
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));

        Assert.That (GetInt32Property (ClientTransaction.CreateRootTransaction()), Is.EqualTo (10));
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateNewNoAutoCommit ()
    {
      SetDatabaseModifyable();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        SetInt32Property (5, ClientTransaction.CreateRootTransaction());

        new NoAutoCommitTestTransactedFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, DomainObjectIDs.ClassWithAllDataTypes1).
            Execute (Context);

        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));

        Assert.That (GetInt32Property (ClientTransaction.CreateRootTransaction()), Is.EqualTo (5));
      }
    }

    [Test]
    public void WxeTransactedFunctionNoneNoAutoCommit ()
    {
      SetDatabaseModifyable();
      SetInt32Property (5, ClientTransaction.CreateRootTransaction());
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;

        new NoAutoCommitTestTransactedFunction (WxeTransactionMode<ClientTransactionFactory>.None, DomainObjectIDs.ClassWithAllDataTypes1).Execute (
            Context);

        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));

        Assert.That (GetInt32Property (ClientTransactionScope.CurrentTransaction), Is.EqualTo (10));
      }

      Assert.That (GetInt32Property (ClientTransaction.CreateRootTransaction()), Is.EqualTo (5));
    }

    [Test]
    public void RemoveCurrentScopeFromWithinFunctionThrows ()
    {
      try
      {
        new RemoveCurrentTransactionScopeFunction().Execute (Context);
      }
      catch (WxeFatalExecutionException ex)
      {
        Assert.IsInstanceOf (typeof (InvalidOperationException), ex.InnerException);
        Assert.That (ex.InnerException.Message, Is.EqualTo ("The ClientTransactionScope has already been left."));
      }
    }

    [Test]
    public void RemoveCurrentScopeFromWithinFunctionThrowsWithPreviouslyExistingScope ()
    {
      try
      {
        ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
        new RemoveCurrentTransactionScopeFunction().Execute (Context);
      }
      catch (WxeFatalExecutionException ex)
      {
        Assert.IsInstanceOf (typeof (InvalidOperationException), ex.InnerException);
        Assert.That (ex.InnerException.Message, Is.EqualTo ("The ClientTransactionScope has already been left."));
      }
    }

    private void SetInt32Property (int value, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterDiscardingScope())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

        objectWithAllDataTypes.Int32Property = value;

        clientTransaction.Commit();
      }
    }

    private int GetInt32Property (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterDiscardingScope())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

        return objectWithAllDataTypes.Int32Property;
      }
    }

    [Test]
    public void AutomaticParameterEnlisting_CreateNone_FunctionCanUseObjectsFromOuterTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var outerTransaction = ClientTransaction.Current;

        var inParameter = ClassWithAllDataTypes.NewObject();
        var inParameterArray = new[] { ClassWithAllDataTypes.NewObject() };
        inParameter.Int32Property = 7;
        inParameterArray[0].Int32Property = 8;

        ClassWithAllDataTypes outParameter;
        ClassWithAllDataTypes[] outParameterArray;
        ExecuteDelegateInWxeFunctionWithParameters (WxeTransactionMode<ClientTransactionFactory>.None, (ctx, f) =>
        {
          var clientTransaction1 = f.Transaction.GetNativeTransaction<ClientTransaction> ();
          Assert.That (clientTransaction1, Is.Null);
          Assert.That (ClientTransaction.Current, Is.SameAs (outerTransaction));

          Assert.That (outerTransaction.IsEnlisted (f.InParameter), Is.True);
          Assert.That (outerTransaction.IsEnlisted (f.InParameterArray[0]));

          Assert.That (f.InParameter.Int32Property, Is.EqualTo (7));
          Assert.That (f.InParameterArray[0].Int32Property, Is.EqualTo (8));
              
          f.OutParameter = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
          f.OutParameter.Int32Property = 12;

          f.OutParameterArray = new[] { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2) };
          f.OutParameterArray[0].Int32Property = 13;
        }, inParameter, inParameterArray, out outParameter, out outParameterArray);

        // Since everything within the function occurred in the same transaction that called the function, all enlisted objects are the same
        // and all changes are visible after the function call.
        Assert.That (ClientTransaction.Current.IsEnlisted (outParameter), Is.True);
        Assert.That (outParameter.Int32Property, Is.EqualTo (12));

        Assert.That (ClientTransaction.Current.IsEnlisted (outParameterArray[0]), Is.True);
        Assert.That (outParameterArray[0].Int32Property, Is.EqualTo (13));
      }
    }

    [Test]
    public void AutomaticParameterEnlisting_CreateNone_FunctionCannotUseParametersNotFromOuterTransaction ()
    {
      var objectFromOtherTransaction = DomainObjectMother.GetObjectInOtherTransaction<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var inParameter = objectFromOtherTransaction;
        var inParameterArray = new[] { objectFromOtherTransaction };

        Assert.That (ClientTransaction.Current.IsEnlisted (objectFromOtherTransaction), Is.False);

        ClassWithAllDataTypes outParameter;
        ClassWithAllDataTypes[] outParameterArray;
        ExecuteDelegateInWxeFunctionWithParameters (WxeTransactionMode<ClientTransactionFactory>.None, (ctx, f) =>
            {
              Assert.That (ClientTransaction.Current.IsEnlisted (f.InParameter), Is.False);
              Assert.That (ClientTransaction.Current.IsEnlisted (f.InParameterArray[0]), Is.False);
              f.OutParameter = f.InParameter;
              f.OutParameterArray = new[] { f.InParameterArray[0] };
            },
            inParameter,
            inParameterArray,
            out outParameter,
            out outParameterArray);
        
        Assert.That (ClientTransaction.Current.IsEnlisted (outParameter), Is.False);
        Assert.That (ClientTransaction.Current.IsEnlisted (outParameterArray[0]), Is.False);
      }
    }

    [Test]
    public void AutomaticParameterEnlisting_CreateRoot_InParametersAreEnlisted ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var outerTransaction = ClientTransaction.Current;
        
        var inParameter = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        var inParameterArray = new[] { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2) };

        inParameter.Int32Property = 7;
        inParameterArray[0].Int32Property = 8;

        ClassWithAllDataTypes outParameter;
        ClassWithAllDataTypes[] outParameterArray;
        ExecuteDelegateInWxeFunctionWithParameters (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
        {
          var clientTransaction = f.Transaction.GetNativeTransaction<ClientTransaction> ();
          Assert.That (clientTransaction, Is.Not.Null);
          Assert.That (ClientTransaction.Current, Is.Not.SameAs (outerTransaction));
          Assert.That (clientTransaction.ParentTransaction, Is.Null);

          Assert.That (outerTransaction.IsEnlisted (f.InParameter), Is.True);
          Assert.That (outerTransaction.IsEnlisted (f.InParameterArray[0]));

          // Since this function is running in a parallel root transaction, the properties set in the outside transaction are not visible from here.
          Assert.That (f.InParameter.Int32Property, Is.Not.EqualTo (7));
          Assert.That (f.InParameterArray[0].Int32Property, Is.Not.EqualTo (8));
        }, inParameter, inParameterArray, out outParameter, out outParameterArray);
      }
    }

    [Test]
    public void AutomaticParameterEnlisting_CreateRoot_OutParameters_NotEnlisted_WithoutSurroundingFunction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        ClassWithAllDataTypes outParameter;
        ClassWithAllDataTypes[] outParameterArray;
        ExecuteDelegateInWxeFunctionWithParameters (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
        {
          f.OutParameter = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
          f.OutParameter.Int32Property = 12;

          f.OutParameterArray = new[] { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2) };
          f.OutParameterArray[0].Int32Property = 13;
        }, null, null, out outParameter, out outParameterArray);

        // Wxe does not enlist parameters in the calling transaction if there is no calling function.
        Assert.That (ClientTransaction.Current.IsEnlisted (outParameter), Is.False);
        Assert.That (ClientTransaction.Current.IsEnlisted (outParameterArray[0]), Is.False);
      }
    }

    [Test]
    public void AutomaticParameterEnlisting_CreateRoot_OutParameters_Enlisted_WithSurroundingFunction ()
    {
      ClientTransaction transactionOfParentFunction = null;
      ClassWithAllDataTypes outParameter;
      ClassWithAllDataTypes[] outParameterArray;
      ExecuteDelegateInSubWxeFunctionWithParameters (
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          (ctx, f) =>
          {
            f.OutParameter = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
            f.OutParameter.Int32Property = 12;

            f.OutParameterArray = new[] { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2) };
            f.OutParameterArray[0].Int32Property = 13;

            transactionOfParentFunction = f.ParentFunction.Transaction.GetNativeTransaction<ClientTransaction>();
          },
          null,
          null,
          out outParameter,
          out outParameterArray);

      // Wxe does enlist parameters in the transaction of the calling function.
      Assert.That (transactionOfParentFunction.IsEnlisted (outParameter), Is.True);
      Assert.That (transactionOfParentFunction.IsEnlisted (outParameterArray[0]), Is.True);
    }

    [Test]
    [ExpectedException (typeof (ObjectsNotFoundException), ExpectedMessage =
        @"Object\(s\) could not be found: 'ClassWithAllDataTypes\|.*\|System.Guid', 'ClassWithAllDataTypes\|.*\|System.Guid'\.",
        MatchType = MessageMatch.Regex)]
    public void AutomaticParameterEnlisting_CreateRoot_WithInvalidInParameter ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var inParameter = ClassWithAllDataTypes.NewObject ();
        var inParameterArray = new[] { ClassWithAllDataTypes.NewObject () };

        // We're passing in new objects which, of course, don't exist in the database.

        try
        {
          ClassWithAllDataTypes outParameter;
          ClassWithAllDataTypes[] outParameterArray;
          ExecuteDelegateInWxeFunctionWithParameters (
              WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) => { }, inParameter, inParameterArray, out outParameter, out outParameterArray);
        }
        catch (WxeUnhandledException ex)
        {
          throw ex.InnerException;
        }
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectsNotFoundException), ExpectedMessage =
        @"Object\(s\) could not be found: 'ClassWithAllDataTypes\|.*\|System.Guid'\.",
        MatchType = MessageMatch.Regex)]
    public void AutomaticParameterEnlisting_CreateRoot_ThrowsWhenInvalidOutParameter ()
    {
      try
      {
        ClassWithAllDataTypes outParameter;
        ClassWithAllDataTypes[] outParameterArray;
        ExecuteDelegateInSubWxeFunctionWithParameters (
            WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
            WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
            (ctx, f) =>
            {
              // These out parameters is of course not valid in the outer transaction
              f.OutParameter = ClassWithAllDataTypes.NewObject();
              f.OutParameterArray = new[] { ClassWithAllDataTypes.NewObject() };
            },
            null,
            null,
            out outParameter,
            out outParameterArray);
      }
      catch (WxeUnhandledException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void AutomaticParameterEnlisting_CreateChild_InAndOutParametersAreEnlisted ()
    {
      ExecuteDelegateInWxeFunction (
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          (parentCtx, parentF) =>
          {
            var inParameter = ClassWithAllDataTypes.NewObject();
            var inParameterArray = new[] { ClassWithAllDataTypes.NewObject() };
            inParameter.Int32Property = 7;
            inParameterArray[0].Int32Property = 8;

            var parentTransaction = parentF.Transaction.GetNativeTransaction<ClientTransaction>();

            var subFunction = new DomainObjectParameterTestTransactedFunction (
            WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent,
            (ctx, f) =>
            {
              var clientTransaction = f.Transaction.GetNativeTransaction<ClientTransaction> ();
              Assert.That (clientTransaction, Is.Not.Null.And.SameAs (ClientTransaction.Current));
              Assert.That (clientTransaction, Is.Not.SameAs (parentTransaction));
              Assert.That (clientTransaction.ParentTransaction, Is.SameAs (parentTransaction));

              Assert.That (clientTransaction.IsEnlisted (f.InParameter), Is.True);
              Assert.That (clientTransaction.IsEnlisted (f.InParameterArray[0]));

              // Since this function is running in a subtransaction, the properties set in the parent transaction are visible from here.
              Assert.That (f.InParameter.Int32Property, Is.EqualTo (7));
              Assert.That (f.InParameterArray[0].Int32Property, Is.EqualTo (8));

              // Since this function is running in a subtransaction, out parameters are visible within the parent function if the transaction is 
              // committed.
              f.OutParameter = ClassWithAllDataTypes.NewObject();
              f.OutParameter.Int32Property = 17;
              f.OutParameterArray = new[] { ClassWithAllDataTypes.NewObject(), ClassWithAllDataTypes.NewObject() };
              f.OutParameterArray[0].Int32Property = 4;

              ClientTransaction.Current.Commit();

              f.OutParameterArray[1].Int32Property = 5;
            },
            inParameter,
            inParameterArray);

            subFunction.SetParentStep (parentF);
            subFunction.Execute (parentCtx);

            var outParameter = subFunction.OutParameter;
            var outParameterArray = subFunction.OutParameterArray;

            Assert.That (parentTransaction.IsEnlisted (outParameter), Is.True);
            Assert.That (outParameter.Int32Property, Is.EqualTo (17));
            Assert.That (parentTransaction.IsEnlisted (outParameterArray[0]), Is.True);
            Assert.That (outParameterArray[0].Int32Property, Is.EqualTo (4));
            Assert.That (parentTransaction.IsEnlisted (outParameterArray[1]), Is.True);
            Assert.That (outParameterArray[1].Int32Property, Is.Not.EqualTo (4));
          });
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException), ExpectedMessage =
        @"Object 'ClassWithAllDataTypes\|.*\|System.Guid' is invalid in this transaction\.",
        MatchType = MessageMatch.Regex)]
    public void AutomaticParameterEnlisting_CreateChild_WithInvalidInParameter ()
    {
      try
      {
        ExecuteDelegateInWxeFunction (
            WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
            (parentCtx, parentF) =>
            {
              var inParameter = ClassWithAllDataTypes.NewObject ();
              inParameter.Delete ();

              var subFunction = new DomainObjectParameterTestTransactedFunction (
                  WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent,
                  (ctx, f) => { },
                  inParameter,
                  null);

              subFunction.SetParentStep (parentF);
              subFunction.Execute (parentCtx);
            });
      }
      catch (WxeUnhandledException e)
      {
        throw e.InnerException;
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException), ExpectedMessage =
        @"Object 'ClassWithAllDataTypes\|.*\|System.Guid' is invalid in this transaction\.",
        MatchType = MessageMatch.Regex)]
    public void AutomaticParameterEnlisting_CreateChild_WithInvalidOutParameter ()
    {
      try 
      {
        ExecuteDelegateInWxeFunction (
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          (parentCtx, parentF) =>
          {
            var subFunction = new DomainObjectParameterTestTransactedFunction (
            WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent,
            (ctx, f) =>
            {
              f.OutParameter = ClassWithAllDataTypes.NewObject ();
            },
            null,
            null);

            subFunction.SetParentStep (parentF);
            subFunction.Execute (parentCtx);
          });
      }
      catch (WxeUnhandledException e)
      {
        throw e.InnerException;
      }
    }

   [Test]
    public void Serialization ()
    {
      var function = new SerializationTestTransactedFunction();
      function.Execute (Context);
      Assert.That (function.FirstStepExecuted, Is.True);
      Assert.That (function.SecondStepExecuted, Is.True);

      var deserializedFunction =
          (SerializationTestTransactedFunction) Serializer.Deserialize (function.SerializedSelf);
      Assert.That (deserializedFunction.FirstStepExecuted, Is.True);
      Assert.That (deserializedFunction.SecondStepExecuted, Is.False);

      deserializedFunction.Execute (Context);

      Assert.That (deserializedFunction.FirstStepExecuted, Is.True);
      Assert.That (deserializedFunction.SecondStepExecuted, Is.True);
    }

    [Test]
    public void ThreadAbortException ()
    {
      var function = new ThreadAbortTestTransactedFunction();
      try
      {
        function.Execute (Context);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      Assert.That (function.FirstStepExecuted, Is.True);
      Assert.That (function.SecondStepExecuted, Is.False);
      Assert.That (function.ThreadAborted, Is.True);

      function.Execute (Context);

      Assert.That (function.FirstStepExecuted, Is.True);
      Assert.That (function.SecondStepExecuted, Is.True);
    }

    [Test]
    public void ThreadAbortExceptionInNestedFunction ()
    {
      var nestedFunction = new ThreadAbortTestTransactedFunction();
      ClientTransactionScope originalScope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
      var parentFunction =
          new CreateRootWithChildTestTransactedFunction (ClientTransactionScope.CurrentTransaction, nestedFunction);

      try
      {
        parentFunction.Execute (Context);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));

      Assert.That (nestedFunction.FirstStepExecuted, Is.True);
      Assert.That (nestedFunction.SecondStepExecuted, Is.False);
      Assert.That (nestedFunction.ThreadAborted, Is.True);

      parentFunction.Execute (Context);

      Assert.That (nestedFunction.FirstStepExecuted, Is.True);
      Assert.That (nestedFunction.SecondStepExecuted, Is.True);

      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));
      originalScope.Leave();
    }

    [Test]
    public void ThreadAbortExceptionInNestedFunctionWithThreadMigration ()
    {
      var nestedFunction = new ThreadAbortTestTransactedFunction();
      var originalScope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
      var parentFunction =
          new CreateRootWithChildTestTransactedFunction (ClientTransactionScope.CurrentTransaction, nestedFunction);

      try
      {
        parentFunction.Execute (Context);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));

      ThreadRunner.Run (
          delegate
          {
            Assert.That (ClientTransactionScope.ActiveScope, Is.Null, "ActiveScope is not null before execute.");
            Assert.That (nestedFunction.FirstStepExecuted, Is.True);
            Assert.That (nestedFunction.SecondStepExecuted, Is.False);
            Assert.That (nestedFunction.ThreadAborted, Is.True);

            parentFunction.Execute (Context);

            Assert.That (nestedFunction.FirstStepExecuted, Is.True);
            Assert.That (nestedFunction.SecondStepExecuted, Is.True);
            Assert.That (ClientTransactionScope.ActiveScope, Is.Null, "ActiveScope is not null after execute.");
            //TODO: Before there was a transaction, now there isn't                           
            //Assert.That ( ClientTransactionScope.CurrentTransaction, Is.SameAs (originalScope.ScopedTransaction)); // but same transaction as on old thread
          });

      originalScope.Leave();
    }

    [Test]
    public void Execute_CreatesTransaction_ThenRestoresOriginal ()
    {
      Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
      
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.True);
        Assert.That (f.Transaction.GetNativeTransaction<ClientTransaction>(), Is.Not.Null.And.SameAs (ClientTransaction.Current));
      });

      Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
    }

    [Test]
    public void Reset_ReplacesCurrentTransaction ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var transactionBefore = ClientTransaction.Current;
        Assert.That (transactionBefore, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction> ()));

        f.Transaction.Reset ();

        Assert.That (transactionBefore, Is.Not.SameAs (ClientTransaction.Current));
        Assert.That (ClientTransaction.Current, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction> ()));
      });
    }

    [Test]
    public void Reset_Variables_AreEnlistedInNewTransaction ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var order = Order.GetObject (DomainObjectIDs.Order1);
        f.Variables.Add ("Order", order);

        f.Transaction.Reset ();

        var transactionAfter = f.Transaction.GetNativeTransaction<ClientTransaction> ();
        Assert.That (transactionAfter.IsEnlisted (order), Is.True);
      });
    }

    [Test]
    [Ignore ("TODO 4591")]
    public void Reset_DoesNotLoadEnlistedVariables_NewTransaction ()
    {
      SetDatabaseModifyable();

      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var existingObject = Order.GetObject (DomainObjectIDs.Order1);
        var nonExistingObject = Employee.GetObject (DomainObjectIDs.Employee1);

        ClientTransaction.CreateRootTransaction ().Execute (() =>
        {
          Employee.GetObject (nonExistingObject.ID).Delete();
          ClientTransaction.Current.Commit();
        });

        ClientTransaction.Current.Rollback();

        f.Variables.Add ("ExistingObject", existingObject);
        f.Variables.Add ("NonExistingObject", nonExistingObject);

        f.Transaction.Reset ();

        Assert.That (existingObject.State, Is.EqualTo (StateType.NotLoadedYet));
        Assert.That (nonExistingObject.State, Is.EqualTo (StateType.NotLoadedYet));

        Assert.That (() => existingObject.EnsureDataAvailable (), Throws.Nothing);
        Assert.That (() => nonExistingObject.EnsureDataAvailable (), Throws.TypeOf<ObjectsNotFoundException> ());
      });
    }

    [Test]
    public void Reset_NonVariables_AreNotEnlistedInNewTransaction ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var order = Order.GetObject (DomainObjectIDs.Order1);

        f.Transaction.Reset ();

        var transactionAfter = f.Transaction.GetNativeTransaction<ClientTransaction> ();
        Assert.That (transactionAfter.IsEnlisted (order), Is.False);
      });
    }

    [Test]
    public void Reset_WithChildTransaction ()
    {
      ExecuteDelegateInSubWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent, (ctx, f) =>
        {
          var transactionBefore = f.Transaction.GetNativeTransaction<ClientTransaction>();
          Assert.That (transactionBefore, Is.Not.Null.And.SameAs (ClientTransaction.Current));
          Assert.That (transactionBefore.ParentTransaction, Is.Not.Null);
          var parentBefore = transactionBefore.ParentTransaction;

          var order = Order.GetObject (DomainObjectIDs.Order1);

          f.Transaction.Reset();

          var transactionAfter = f.Transaction.GetNativeTransaction<ClientTransaction>();
          Assert.That (transactionAfter, Is.Not.SameAs (transactionBefore));
          Assert.That (transactionAfter, Is.Not.Null.And.SameAs (ClientTransaction.Current));
          Assert.That (transactionAfter.ParentTransaction, Is.Not.Null.And.SameAs (parentBefore));

          // This is because it was automatically enlisted in the outer transaction before the reset
          Assert.That (transactionAfter.IsEnlisted (order), Is.True);
        });
    }

    private void ExecuteDelegateInWxeFunction (ITransactionMode transactionMode, Action<WxeContext, DelegateExecutingTransactedFunction> testDelegate)
    {
      var function = new DelegateExecutingTransactedFunction (transactionMode, testDelegate);

      function.Execute (Context);
      Assert.That (function.DelegatesExecuted, Is.True);
    }

    private void ExecuteDelegateInSubWxeFunction (
        ITransactionMode parentFunctionTransactionMode, 
        ITransactionMode subFunctionTransactionMode, 
        Action<WxeContext, DelegateExecutingTransactedFunction> testDelegate)
    {
      var subFunction = new DelegateExecutingTransactedFunction (subFunctionTransactionMode, testDelegate);

      var rootFunction = new TransactedFunctionWithChildFunction (parentFunctionTransactionMode, subFunction);
      rootFunction.Execute (Context);

      Assert.That (subFunction.DelegatesExecuted, Is.True);
    }

    private void ExecuteDelegateInWxeFunctionWithParameters (
        ITransactionMode transactionMode, 
        Action<WxeContext, DomainObjectParameterTestTransactedFunction> testDelegate, 
        ClassWithAllDataTypes inParameter, 
        ClassWithAllDataTypes[] inParameterArray, 
        out ClassWithAllDataTypes outParameter, 
        out ClassWithAllDataTypes[] outParameterArray)
    {
      var function = new DomainObjectParameterTestTransactedFunction (
          transactionMode,
          testDelegate,
          inParameter,
          inParameterArray);
      function.Execute (Context);

      Assert.That (function.DelegatesExecuted, Is.True);

      outParameter = function.OutParameter;
      outParameterArray = function.OutParameterArray;
    }

    private void ExecuteDelegateInSubWxeFunctionWithParameters (
        ITransactionMode parentFunctionTransactionMode,
        ITransactionMode subFunctionTransactionMode,
        Action<WxeContext, DomainObjectParameterTestTransactedFunction> testDelegate,
        ClassWithAllDataTypes inParameter,
        ClassWithAllDataTypes[] inParameterArray,
        out ClassWithAllDataTypes outParameter,
        out ClassWithAllDataTypes[] outParameterArray)
    {
      var subFunction = new DomainObjectParameterTestTransactedFunction (
          subFunctionTransactionMode,
          testDelegate,
          inParameter,
          inParameterArray);
      
      var rootFunction = new TransactedFunctionWithChildFunction (parentFunctionTransactionMode, subFunction);
      rootFunction.Execute (Context);

      Assert.That (subFunction.DelegatesExecuted, Is.True);

      outParameter = subFunction.OutParameter;
      outParameterArray = subFunction.OutParameterArray;
    }
  }
}
