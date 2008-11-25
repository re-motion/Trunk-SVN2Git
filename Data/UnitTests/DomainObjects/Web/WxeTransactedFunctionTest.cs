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
    //public override void SetUp ()
    //{
    //  base.SetUp ();
    //  ClientTransactionScope.ResetActiveScope ();
    //}

    [Test]
    public void WxeTransactedFunctionCreateRoot ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateRootTestTransactedFunction (originalScope).Execute (Context);
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
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
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
      }
    }

    [Test]
    public void WxeTransactedFunctionNone ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
        new CreateNoneTestTransactedFunction (originalScope).Execute (Context);
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
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
        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

        Assert.AreEqual (10, GetInt32Property (ClientTransaction.CreateRootTransaction()));
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

        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

        Assert.AreEqual (5, GetInt32Property (ClientTransaction.CreateRootTransaction()));
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

        Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

        Assert.AreEqual (10, GetInt32Property (ClientTransactionScope.CurrentTransaction));
      }

      Assert.AreEqual (5, GetInt32Property (ClientTransaction.CreateRootTransaction()));
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
        Assert.IsInstanceOfType (typeof (InvalidOperationException), ex.InnerException);
        Assert.AreEqual ("The ClientTransactionScope has already been left.", ex.InnerException.Message);
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
        Assert.IsInstanceOfType (typeof (InvalidOperationException), ex.InnerException);
        Assert.AreEqual ("The ClientTransactionScope has already been left.", ex.InnerException.Message);
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

        Assert.IsTrue (outParameter.CanBeUsedInTransaction);
        Assert.AreEqual (12, outParameter.Int32Property);

        Assert.IsTrue (outParameterArray[0].CanBeUsedInTransaction);
        Assert.AreEqual (13, outParameterArray[0].Int32Property);
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

        Assert.IsFalse (outParameter.CanBeUsedInTransaction);
        Assert.IsFalse (outParameterArray[0].CanBeUsedInTransaction);
      }
    }

    [Test]
    [ExpectedException (typeof (BulkLoadException), ExpectedMessage =
        "There were errors when loading a bulk of DomainObjects:\r\n"
        + "Object 'ClassWithAllDataTypes|.*|System.Guid' could not be found.\r\nObject 'ClassWithAllDataTypes|.*|System.Guid' could not be found.",
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
    [ExpectedException (typeof (BulkLoadException), ExpectedMessage =
        "There were errors when loading a bulk of DomainObjects:\r\n"
        + "Object 'ClassWithAllDataTypes|.*|System.Guid' could not be found.\r\nObject 'ClassWithAllDataTypes|.*|System.Guid' could not be found.",
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
    [ExpectedException (typeof (ObjectDiscardedException), ExpectedMessage =
        "Remotion.Data.DomainObjects.DataManagement.ObjectDiscardedException : Object 'ClassWithAllDataTypes|.*|System.Guid' is already discarded.",
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
          Assert.AreEqual ("InParameter", aex.ParamName);
          throw;
        }
      }
    }

    [Test]
    [ExpectedException (typeof (BulkLoadException), ExpectedMessage =
        "There were errors when loading a bulk of DomainObjects:\r\n"
        + "Object 'ClassWithAllDataTypes|.*|System.Guid' could not be found.\r\nObject 'ClassWithAllDataTypes|.*|System.Guid' could not be found.",
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
          Assert.AreEqual ("OutParameter", aex.ParamName);
          throw;
        }
      }
    }

    [Test]
    public void Serialization ()
    {
      SerializationTestTransactedFunction function = new SerializationTestTransactedFunction();
      function.Execute (Context);
      Assert.IsTrue (function.FirstStepExecuted);
      Assert.IsTrue (function.SecondStepExecuted);

      SerializationTestTransactedFunction deserializedFunction =
          (SerializationTestTransactedFunction) Serializer.Deserialize (function.SerializedSelf);
      Assert.IsTrue (deserializedFunction.FirstStepExecuted);
      Assert.IsFalse (deserializedFunction.SecondStepExecuted);

      deserializedFunction.Execute (Context);

      Assert.IsTrue (deserializedFunction.FirstStepExecuted);
      Assert.IsTrue (deserializedFunction.SecondStepExecuted);
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

      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

      Assert.IsTrue (nestedFunction.FirstStepExecuted);
      Assert.IsFalse (nestedFunction.SecondStepExecuted);
      Assert.IsTrue (nestedFunction.ThreadAborted);

      parentFunction.Execute (Context);

      Assert.IsTrue (nestedFunction.FirstStepExecuted);
      Assert.IsTrue (nestedFunction.SecondStepExecuted);

      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
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

      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);

      ThreadRunner.Run (
          delegate
          {
            Assert.IsNull (ClientTransactionScope.ActiveScope, "ActiveScope is not null before execute.");
            Assert.IsTrue (nestedFunction.FirstStepExecuted);
            Assert.IsFalse (nestedFunction.SecondStepExecuted);
            Assert.IsTrue (nestedFunction.ThreadAborted);

            parentFunction.Execute (Context);

            Assert.IsTrue (nestedFunction.FirstStepExecuted);
            Assert.IsTrue (nestedFunction.SecondStepExecuted);
            Assert.IsNull (ClientTransactionScope.ActiveScope, "ActiveScope is not null after execute.");
            //TODO: Before there was a transaction, now there isn't                           
            //Assert.AreSame (originalScope.ScopedTransaction, ClientTransactionScope.CurrentTransaction); // but same transaction as on old thread
          });

      originalScope.Leave();
    }

    [Test]
    public void ResetAutoEnlistsObjects ()
    {
      Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
      ResetTestTransactedFunction function = new ResetTestTransactedFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot);
      function.Execute (Context);
      Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
    }

    [Test]
    public void ResetWithChildTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        ResetTestTransactedFunction function = new ResetTestTransactedFunction (WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent);
        CreateRootWithChildTestTransactedFunction rootFunction = new CreateRootWithChildTestTransactedFunction (ClientTransaction.Current, function);
        rootFunction.Execute (Context);
      }
    }

    [Test]
    public void ResetCopiesEventHandlers ()
    {
      ResetTestTransactedFunction function = new ResetTestTransactedFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot);
      function.Execute (Context);
    }
  }
}