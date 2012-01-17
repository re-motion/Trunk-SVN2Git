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
    public void AutoEnlistingCreateNone ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.NewObject();
        ClassWithAllDataTypes[] inParameterArray = new[] { ClassWithAllDataTypes.NewObject() };
        inParameter.Int32Property = 7;
        inParameterArray[0].Int32Property = 8;

        DomainObjectParameterTestTransactedFunction function =
            new DomainObjectParameterTestTransactedFunction (WxeTransactionMode<ClientTransactionFactory>.None, inParameter, inParameterArray);
        function.Execute (Context);

        ClassWithAllDataTypes outParameter = function.OutParameter;
        ClassWithAllDataTypes[] outParameterArray = function.OutParameterArray;

        Assert.That (ClientTransaction.Current.IsEnlisted (outParameter), Is.True);
        Assert.That (outParameter.Int32Property, Is.EqualTo (12));

        Assert.That (ClientTransaction.Current.IsEnlisted (outParameterArray[0]), Is.True);
        Assert.That (outParameterArray[0].Int32Property, Is.EqualTo (13));
      }
    }

    [Test]
    public void AutoEnlistingCreateRoot ()
    {
      SetDatabaseModifyable();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.NewObject();
        inParameter.DateTimeProperty = DateTime.Now;
        inParameter.DateProperty = DateTime.Now.Date;
        inParameter.Int32Property = 4;

        ClassWithAllDataTypes[] inParameterArray = new[] { ClassWithAllDataTypes.NewObject() };
        inParameterArray[0].Int32Property = 5;
        inParameterArray[0].DateTimeProperty = DateTime.Now;
        inParameterArray[0].DateProperty = DateTime.Now.Date;

        ClientTransactionScope.CurrentTransaction.Commit();

        inParameter.Int32Property = 7;
        inParameterArray[0].Int32Property = 8;

        DomainObjectParameterTestTransactedFunction function = new DomainObjectParameterTestTransactedFunction (
            WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit, inParameter, inParameterArray);
        function.Execute (Context);

        ClassWithAllDataTypes outParameter = function.OutParameter;
        ClassWithAllDataTypes[] outParameterArray = function.OutParameterArray;

        Assert.That (ClientTransaction.Current.IsEnlisted (outParameter), Is.False);
        Assert.That (ClientTransaction.Current.IsEnlisted (outParameterArray[0]), Is.False);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectsNotFoundException), ExpectedMessage =
        @"Object\(s\) could not be found: 'ClassWithAllDataTypes\|.*\|System.Guid', 'ClassWithAllDataTypes\|.*\|System.Guid'\.",
        MatchType = MessageMatch.Regex)]
    public void AutoEnlistingCreateRootThrowsWhenInvalidInParameter ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.NewObject();
        ClassWithAllDataTypes[] inParameterArray = new[] { ClassWithAllDataTypes.NewObject() };

        var function = new DomainObjectParameterTestTransactedFunction (
            WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit, inParameter, inParameterArray);
        try
        {
          function.Execute (Context);
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
    public void AutoEnlistingCreateRootThrowsWhenInvalidOutParameter ()
    {
      var function = new CreateRootWithChildTestTransactedFunctionBase  (WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          new DomainObjectParameterInvalidOutTestTransactedFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit));
      try
      {
        function.Execute (Context);
      }
      catch (WxeUnhandledException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void AutoEnlistingCreateChild ()
    {
      SetDatabaseModifyable();
      DomainObjectParameterWithChildTestTransactedFunction function = new DomainObjectParameterWithChildTestTransactedFunction();
      function.Execute (Context);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException), ExpectedMessage =
        @"Object 'ClassWithAllDataTypes\|.*\|System.Guid' is invalid in this transaction\.",
        MatchType = MessageMatch.Regex)]
    public void AutoEnlistingCreateChildWithInvalidInParameter ()
    {
      var function = new DomainObjectParameterWithChildInvalidInTestTransactedFunction();
      try
      {
        function.Execute (Context);
      }
      catch (WxeUnhandledException ex)
      {
        try
        {
          throw ex.InnerException;
        }
        catch (ArgumentException aex)
        {
          Assert.That (aex.ParamName, Is.EqualTo ("InParameter"));
          throw;
        }
      }
    }

    [Test]
    [ExpectedException (
        typeof (ObjectInvalidException), 
        ExpectedMessage = @"Object 'ClassWithAllDataTypes\|.*\|System.Guid' is invalid in this transaction\.",
        MatchType = MessageMatch.Regex)]
    public void AutoEnlistingCreateChildWithInvalidOutParameter ()
    {
      var function = new DomainObjectParameterWithChildInvalidOutTestTransactedFunction();
      try
      {
        function.Execute (Context);
      }
      catch (WxeUnhandledException ex)
      {
        try
        {
          throw ex.InnerException;
        }
        catch (ArgumentException aex)
        {
          Assert.That (aex.ParamName, Is.EqualTo ("OutParameter"));
          throw;
        }
      }
    }

    [Test]
    public void Serialization ()
    {
      SerializationTestTransactedFunction function = new SerializationTestTransactedFunction();
      function.Execute (Context);
      Assert.That (function.FirstStepExecuted, Is.True);
      Assert.That (function.SecondStepExecuted, Is.True);

      SerializationTestTransactedFunction deserializedFunction =
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
      ThreadAbortTestTransactedFunction function = new ThreadAbortTestTransactedFunction();
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
      ThreadAbortTestTransactedFunction nestedFunction = new ThreadAbortTestTransactedFunction();
      ClientTransactionScope originalScope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
      CreateRootWithChildTestTransactedFunction parentFunction =
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
      ThreadAbortTestTransactedFunction nestedFunction = new ThreadAbortTestTransactedFunction();
      ClientTransactionScope originalScope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
      CreateRootWithChildTestTransactedFunction parentFunction =
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
      
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, f =>
      {
        Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.True);
        Assert.That (f.Transaction.GetNativeTransaction<ClientTransaction>(), Is.Not.Null.And.SameAs (ClientTransaction.Current));
      });

      Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
    }

    [Test]
    public void Reset_ReplacesCurrentTransaction ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, f =>
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
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, f =>
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

      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, f =>
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
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, f =>
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
        ExecuteDelegateInSubWxeFunctionWithParentTransaction (
            WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent,
            f =>
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

    private void ExecuteDelegateInWxeFunction (ITransactionMode transactionMode, Action<DelegateExecutingTransactedFunction> testDelegate)
    {
      var function = new DelegateExecutingTransactedFunction (
          transactionMode,
          testDelegate);

      function.Execute (Context);
      Assert.That (function.DelegateExecuted, Is.True);
    }

    private void ExecuteDelegateInSubWxeFunctionWithParentTransaction (ITransactionMode transactionMode, Action<DelegateExecutingTransactedFunction> testDelegate)
    {
      var subFunction = new DelegateExecutingTransactedFunction (
          transactionMode,
          testDelegate);

      var rootFunction = new CreateRootWithChildTestTransactedFunction (ClientTransaction.Current, subFunction);
      rootFunction.Execute (Context);

      Assert.That (subFunction.DelegateExecuted, Is.True);
    }
  }
}
