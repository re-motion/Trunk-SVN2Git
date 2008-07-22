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
using System.Threading;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
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
    //public override void SetUp ()
    //{
    //  base.SetUp ();
    //  ClientTransactionScope.ResetActiveScope ();
    //}

    [Test]
    public void WxeTransactedFunctionCreateRoot ()
    {
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateRootTestTransactedFunction (originalScope).Execute (Context);
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateChildIfParent ()
    {
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateRootWithChildTestTransactedFunction (originalScope.ScopedTransaction, new CreateChildIfParentTestTransactedFunction ()).Execute (Context);
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
      }
    }

    [Test]
    public void WxeTransactedFunctionNone ()
    {
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope  = ClientTransactionScope.ActiveScope;
        new CreateNoneTestTransactedFunction (originalScope).Execute (Context);
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateNewAutoCommit ()
    {
      SetDatabaseModifyable ();
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        SetInt32Property (5, ClientTransaction.NewRootTransaction ());

        new AutoCommitTestTransactedFunction (WxeTransactionMode.CreateRoot, DomainObjectIDs.ClassWithAllDataTypes1).Execute (Context);
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

        Assert.AreEqual (10, GetInt32Property (ClientTransaction.NewRootTransaction ()));
      }
    }

    [Test]
    public void WxeTransactedFunctionCreateNewNoAutoCommit()
    {
      SetDatabaseModifyable ();
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        SetInt32Property (5, ClientTransaction.NewRootTransaction ());

        new NoAutoCommitTestTransactedFunction (WxeTransactionMode.CreateRoot, DomainObjectIDs.ClassWithAllDataTypes1).Execute (Context);

        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

        Assert.AreEqual (5, GetInt32Property (ClientTransaction.NewRootTransaction ()));
      }
    }

    [Test]
    public void WxeTransactedFunctionNoneAutoCommit ()
    {
      SetDatabaseModifyable ();
      SetInt32Property (5, ClientTransaction.NewRootTransaction ());
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;

        new AutoCommitTestTransactedFunction (WxeTransactionMode.None, DomainObjectIDs.ClassWithAllDataTypes1).Execute (Context);

        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

        Assert.AreEqual (10, GetInt32Property (ClientTransactionScope.CurrentTransaction));
      }

      Assert.AreEqual (5, GetInt32Property (ClientTransaction.NewRootTransaction ()));
    }

    [Test]
    public void WxeTransactedFunctionNoneNoAutoCommit ()
    {
      SetDatabaseModifyable ();
      SetInt32Property (5, ClientTransaction.NewRootTransaction ());
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;

        new NoAutoCommitTestTransactedFunction (WxeTransactionMode.None, DomainObjectIDs.ClassWithAllDataTypes1).Execute (Context);

        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

        Assert.AreEqual (10, GetInt32Property (ClientTransactionScope.CurrentTransaction));
      }

      Assert.AreEqual (5, GetInt32Property (ClientTransaction.NewRootTransaction ()));
    }

    [Test]
    public void RemoveCurrentScopeFromWithinFunctionThrows ()
    {
      try
      {
        new RemoveCurrentTransactionScopeFunction ().Execute (Context);
      }
      catch (WxeUnhandledException ex)
      {
        Assert.IsTrue (ex.InnerException is WxeNonRecoverableTransactionException);
        Assert.IsTrue (ex.InnerException.InnerException is InconsistentClientTransactionScopeException);
        Assert.AreEqual ("Somebody else has removed the active transaction scope.", ex.InnerException.InnerException.Message);
      }
    }

    [Test]
    public void RemoveCurrentScopeFromWithinFunctionThrowsWithPreviouslyExistingScope ()
    {
      try
      {
        ClientTransaction.NewRootTransaction().EnterDiscardingScope();
        new RemoveCurrentTransactionScopeFunction ().Execute (Context);
      }
      catch (WxeUnhandledException ex)
      {
        Assert.IsTrue (ex.InnerException is WxeNonRecoverableTransactionException);
        Assert.IsTrue (ex.InnerException.InnerException is InconsistentClientTransactionScopeException);
        Assert.AreEqual ("The active transaction scope does not contain the expected scope.", ex.InnerException.InnerException.Message);
      }
    }

    private void SetInt32Property (int value, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterDiscardingScope ())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

        objectWithAllDataTypes.Int32Property = value;

        clientTransaction.Commit ();
      }
    }

    private int GetInt32Property (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterDiscardingScope ())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

        return objectWithAllDataTypes.Int32Property;
      }
    }

    [Test]
    public void AutoEnlistingCreateNone ()
    {
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.NewObject();
        ClassWithAllDataTypes[] inParameterArray = new ClassWithAllDataTypes[] { ClassWithAllDataTypes.NewObject () };
        inParameter.Int32Property = 7;
        inParameterArray[0].Int32Property = 8;

        DomainObjectParameterTestTransactedFunction function =
            new DomainObjectParameterTestTransactedFunction (WxeTransactionMode.None, inParameter, inParameterArray);
        function.Execute (Context);

        ClassWithAllDataTypes outParameter = function.OutParameter;
        ClassWithAllDataTypes[] outParameterArray = function.OutParameterArray;

        Assert.IsTrue (outParameter.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (12, outParameter.Int32Property);
        
        Assert.IsTrue (outParameterArray[0].CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.AreEqual (13, outParameterArray[0].Int32Property);
      }
    }

    [Test]
    public void AutoEnlistingCreateRoot ()
    {
      SetDatabaseModifyable ();

      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.NewObject ();
        inParameter.DateTimeProperty = DateTime.Now;
        inParameter.DateProperty = DateTime.Now.Date;
        inParameter.Int32Property = 4;

        ClassWithAllDataTypes[] inParameterArray = new ClassWithAllDataTypes[] { ClassWithAllDataTypes.NewObject () };
        inParameterArray[0].Int32Property = 5;
        inParameterArray[0].DateTimeProperty = DateTime.Now;
        inParameterArray[0].DateProperty = DateTime.Now.Date;

        ClientTransactionScope.CurrentTransaction.Commit ();

        inParameter.Int32Property = 7;
        inParameterArray[0].Int32Property = 8;

        DomainObjectParameterTestTransactedFunction function = new DomainObjectParameterTestTransactedFunction (WxeTransactionMode.CreateRoot,
                                                                                                                inParameter, inParameterArray);
        function.Execute (Context);
        
        ClassWithAllDataTypes outParameter = function.OutParameter;
        ClassWithAllDataTypes[] outParameterArray = function.OutParameterArray;

        Assert.IsTrue (outParameter.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.AreNotEqual (12, outParameter.Int32Property);
        Assert.AreEqual (9, outParameter.Int32Property);

        Assert.IsTrue (outParameterArray[0].CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.AreNotEqual (13, outParameterArray[0].Int32Property);
        Assert.AreEqual (10, outParameterArray[0].Int32Property);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object 'ClassWithAllDataTypes|.*' cannot be enlisted in the "
                                                                      + "function's transaction. Maybe it was newly created and has not yet been committed, or it was deleted.", MatchType =  MessageMatch.Regex)]
    public void AutoEnlistingCreateRootThrowsWhenInvalidInParameter ()
    {
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.NewObject ();
        ClassWithAllDataTypes[] inParameterArray = new ClassWithAllDataTypes[] { ClassWithAllDataTypes.NewObject () };

        DomainObjectParameterTestTransactedFunction function = new DomainObjectParameterTestTransactedFunction (WxeTransactionMode.CreateRoot,
                                                                                                                inParameter, inParameterArray);
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
            Assert.AreEqual ("InParameter", aex.ParamName);
            throw;
          }
        }
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object 'ClassWithAllDataTypes|.*' cannot be enlisted in the "
                                                                      + "function's transaction. Maybe it was newly created and has not yet been committed, or it was deleted.", MatchType = MessageMatch.Regex)]
    public void AutoEnlistingCreateRootThrowsWhenInvalidOutParameter ()
    {
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        DomainObjectParameterInvalidOutTestTransactedFunction function =
            new DomainObjectParameterInvalidOutTestTransactedFunction (WxeTransactionMode.CreateRoot);
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
            Assert.AreEqual ("OutParameter", aex.ParamName);
            throw;
          }
        }
      }
    }

    [Test]
    public void AutoEnlistingCreateChild()
    {
      SetDatabaseModifyable ();
      DomainObjectParameterWithChildTestTransactedFunction function = new DomainObjectParameterWithChildTestTransactedFunction ();
      function.Execute (Context);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object 'ClassWithAllDataTypes|.*' cannot be enlisted in the "
                                                                      + "function's transaction. Maybe it was newly created and has not yet been committed, or it was deleted.", MatchType = MessageMatch.Regex)]
    public void AutoEnlistingCreateChildWithInvalidInParameter()
    {
      DomainObjectParameterWithChildInvalidInTestTransactedFunction function = new DomainObjectParameterWithChildInvalidInTestTransactedFunction ();
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
          Assert.AreEqual ("InParameter", aex.ParamName);
          throw;
        }
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object 'ClassWithAllDataTypes|.*' cannot be enlisted in the "
                                                                      + "function's transaction. Maybe it was newly created and has not yet been committed, or it was deleted.", MatchType = MessageMatch.Regex)]
    public void AutoEnlistingCreateChildWithInvalidOutParameter ()
    {
      DomainObjectParameterWithChildInvalidOutTestTransactedFunction function = new DomainObjectParameterWithChildInvalidOutTestTransactedFunction ();
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
          Assert.AreEqual ("OutParameter", aex.ParamName);
          throw;
        }
      }
    }

    [Test]
    public void Serialization ()
    {
      SerializationTestTransactedFunction function = new SerializationTestTransactedFunction ();
      function.Execute (Context);
      Assert.IsTrue (function.FirstStepExecuted);
      Assert.IsTrue (function.SecondStepExecuted);

      SerializationTestTransactedFunction deserializedFunction = (SerializationTestTransactedFunction) Serializer.Deserialize (function.SerializedSelf);
      Assert.IsTrue (deserializedFunction.FirstStepExecuted);
      Assert.IsFalse (deserializedFunction.SecondStepExecuted);

      deserializedFunction.Execute (Context);

      Assert.IsTrue (deserializedFunction.FirstStepExecuted);
      Assert.IsTrue (deserializedFunction.SecondStepExecuted);
    }

    [Test]
    public void ThreadAbortException ()
    {
      ThreadAbortTestTransactedFunction function = new ThreadAbortTestTransactedFunction ();
      try
      {
        function.Execute (Context);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      Assert.IsTrue (function.FirstStepExecuted);
      Assert.IsFalse (function.SecondStepExecuted);
      Assert.IsTrue (function.ThreadAborted);

      function.Execute (Context);

      Assert.IsTrue (function.FirstStepExecuted);
      Assert.IsTrue (function.SecondStepExecuted);
    }

    [Test]
    public void ThreadAbortExceptionInNestedFunction ()
    {
      ThreadAbortTestTransactedFunction nestedFunction = new ThreadAbortTestTransactedFunction ();
      ClientTransactionScope originalScope = ClientTransaction.NewRootTransaction().EnterDiscardingScope();
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

      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

      Assert.IsTrue (nestedFunction.FirstStepExecuted);
      Assert.IsFalse (nestedFunction.SecondStepExecuted);
      Assert.IsTrue (nestedFunction.ThreadAborted);

      parentFunction.Execute (Context);

      Assert.IsTrue (nestedFunction.FirstStepExecuted);
      Assert.IsTrue (nestedFunction.SecondStepExecuted);

      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
      originalScope.Leave ();
    }

    [Test]
    public void ThreadAbortExceptionInNestedFunctionWithThreadMigration ()
    {
      ThreadAbortTestTransactedFunction nestedFunction = new ThreadAbortTestTransactedFunction ();
      ClientTransactionScope originalScope = ClientTransaction.NewRootTransaction().EnterDiscardingScope();
      CreateRootWithChildTestTransactedFunction parentFunction =
          new CreateRootWithChildTestTransactedFunction (ClientTransactionScope.CurrentTransaction, nestedFunction);

      try
      {
        parentFunction.Execute (Context);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort ();
      }

      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

      ThreadRunner.Run (delegate {
                                   Assert.IsTrue (nestedFunction.FirstStepExecuted);
                                   Assert.IsFalse (nestedFunction.SecondStepExecuted);
                                   Assert.IsTrue (nestedFunction.ThreadAborted);

                                   parentFunction.Execute (Context);

                                   Assert.IsTrue (nestedFunction.FirstStepExecuted);
                                   Assert.IsTrue (nestedFunction.SecondStepExecuted);

                                   Assert.AreNotSame (originalScope, ClientTransactionScope.ActiveScope); // new scope on new thread
                                   Assert.AreSame (originalScope.ScopedTransaction, ClientTransactionScope.CurrentTransaction); // but same transaction as on old thread
      });

      originalScope.Leave ();
    }

    [Test]
    public void TransactionResettableWhenNotReadOnly ()
    {
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>();
        PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The current transaction cannot be reset as it is read-only. "
                                                                              + "The reason might be an open child transaction.")]
    public void TransactionNotResettableWhenReadOnly ()
    {
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransactionScope.CurrentTransaction.CreateSubTransaction ();

        WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>();
        PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no current transaction.")]
    public void TransactionNotResettableWhenNull ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>();
        PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The current transaction cannot be reset as it is in a dirty state and "
                                                                              + "needs to be committed or rolled back.")]
    public void TransactionNotResettableWhenNewObject ()
    {
      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>();
        Order.NewObject ();
        PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The current transaction cannot be reset as it is in a dirty state and "
                                                                              + "needs to be committed or rolled back.")]
    public void TransactionNotResettableWhenChangedObject ()
    {
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>();
        ++Order.GetObject (DomainObjectIDs.Order1).OrderNumber;
        PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The current transaction cannot be reset as it is in a dirty state and "
                                                                              + "needs to be committed or rolled back.")]
    public void TransactionNotResettableWhenChangedRelation ()
    {
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager> tx = new WxeScopedTransaction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>();
        Order.GetObject (DomainObjectIDs.Order1).OrderItems.Clear();
        PrivateInvoke.InvokeNonPublicMethod (tx, "CheckCurrentTransactionResettable");
      }
    }

    [Test]
    public void ResetAutoEnlistsObjects ()
    {
      Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
      ResetTestTransactedFunction function = new ResetTestTransactedFunction ();
      function.Execute (Context);
      Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
    }

    [Test]
    public void ResetWithChildTransaction ()
    {
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        ResetTestTransactedFunction function = new ResetTestTransactedFunction (WxeTransactionMode.CreateChildIfParent);
        CreateRootWithChildTestTransactedFunction rootFunction = new CreateRootWithChildTestTransactedFunction (ClientTransaction.Current, function);
        rootFunction.Execute (Context);
      }
    }

    [Test]
    public void ResetCopiesEventHandlersWhenToldTo ()
    {
      ResetTestTransactedFunction function = new ResetTestTransactedFunction ();
      function.CopyEventHandlers = true;
      function.Execute (Context);
    }

    [Test]
    public void ResetDoesNotCopyEventHandlersWhenNotToldTo ()
    {
      ResetTestTransactedFunction function = new ResetTestTransactedFunction ();
      function.CopyEventHandlers = false;
      function.Execute (Context);
    }
  }
}