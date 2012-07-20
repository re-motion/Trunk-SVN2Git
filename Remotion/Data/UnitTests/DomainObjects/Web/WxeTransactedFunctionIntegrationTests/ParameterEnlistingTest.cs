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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class ParameterEnlistingTest : WxeTransactedFunctionIntegrationTestBase
  {
    [Test]
    public void AutomaticParameterEnlisting_CreateNone_FunctionCanUseObjectsFromOuterTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var outerTransaction = ClientTransaction.Current;

        var inParameter = ClassWithAllDataTypes.NewObject ();
        var inParameterArray = new[] { ClassWithAllDataTypes.NewObject () };
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
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
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

            transactionOfParentFunction = f.ParentFunction.Transaction.GetNativeTransaction<ClientTransaction> ();
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
    public void AutomaticParameterEnlisting_CreateRoot_WithNonLoadableInParameter ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var inParameter = ClassWithAllDataTypes.NewObject ();
        var inParameterArray = new[] { ClassWithAllDataTypes.NewObject () };

        // We're passing in new objects which, of course, don't exist in the database.

        ClassWithAllDataTypes outParameter;
        ClassWithAllDataTypes[] outParameterArray;
        ExecuteDelegateInWxeFunctionWithParameters (
            WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
            {
              Assert.That (f.Transaction.GetNativeTransaction<ClientTransaction> ().IsEnlisted (f.InParameter), Is.True);
              Assert.That (f.InParameter.State, Is.EqualTo (StateType.NotLoadedYet));
              Assert.That (() => f.InParameter.EnsureDataAvailable(), Throws.TypeOf<ObjectsNotFoundException>());

              Assert.That (f.Transaction.GetNativeTransaction<ClientTransaction> ().IsEnlisted (f.InParameterArray[0]), Is.True);
              Assert.That (f.InParameterArray[0].State, Is.EqualTo (StateType.NotLoadedYet));
              Assert.That (() => f.InParameterArray[0].EnsureDataAvailable (), Throws.TypeOf<ObjectsNotFoundException> ());
            }, inParameter, inParameterArray, out outParameter, out outParameterArray);
      }
    }

    [Test]
    public void AutomaticParameterEnlisting_CreateRoot_WithNonLoadableOutParameter ()
    {
      ClassWithAllDataTypes outParameter;
      ClassWithAllDataTypes[] outParameterArray;
      ClientTransaction transactionOfParentFunction = null;

      ExecuteDelegateInSubWxeFunctionWithParameters (
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          (ctx, f) =>
          {
            // These out parameters is of course not valid in the outer transaction
            f.OutParameter = ClassWithAllDataTypes.NewObject ();
            f.OutParameterArray = new[] { ClassWithAllDataTypes.NewObject () };

            transactionOfParentFunction = f.ParentFunction.Transaction.GetNativeTransaction<ClientTransaction>();
          },
          null,
          null,
          out outParameter,
          out outParameterArray);

      Assert.That (transactionOfParentFunction.IsEnlisted (outParameter), Is.True);
      Assert.That (() => transactionOfParentFunction.Execute (() => outParameter.State), Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (transactionOfParentFunction.IsEnlisted (outParameterArray[0]), Is.True);
      Assert.That (() => transactionOfParentFunction.Execute (() => outParameterArray[0].State), Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void AutomaticParameterEnlisting_CreateChild_InAndOutParametersAreEnlisted ()
    {
      ExecuteDelegateInWxeFunction (
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          (parentCtx, parentF) =>
          {
            var inParameter = ClassWithAllDataTypes.NewObject ();
            var inParameterArray = new[] { ClassWithAllDataTypes.NewObject () };
            inParameter.Int32Property = 7;
            inParameterArray[0].Int32Property = 8;

            var parentTransaction = parentF.Transaction.GetNativeTransaction<ClientTransaction> ();

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
                  f.OutParameter = ClassWithAllDataTypes.NewObject ();
                  f.OutParameter.Int32Property = 17;
                  f.OutParameterArray = new[] { ClassWithAllDataTypes.NewObject (), ClassWithAllDataTypes.NewObject () };
                  f.OutParameterArray[0].Int32Property = 4;

                  ClientTransaction.Current.Commit ();

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
    public void AutomaticParameterEnlisting_CreateChild_WithNonLoadableInParameter ()
    {
      ExecuteDelegateInWxeFunction (
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          (parentCtx, parentF) =>
          {
            var inParameter = ClassWithAllDataTypes.NewObject ();
            inParameter.Delete ();

            var inParameterArray = new[] { ClassWithAllDataTypes.NewObject () };
            inParameterArray[0].Delete ();

            var subFunction = new DomainObjectParameterTestTransactedFunction (
                WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent,
                (ctx, f) =>
                {
                  Assert.That (f.InParameter.State, Is.EqualTo (StateType.Invalid));
                  Assert.That (() => f.InParameter.EnsureDataAvailable (), Throws.TypeOf<ObjectInvalidException> ());

                  Assert.That (f.InParameterArray[0].State, Is.EqualTo (StateType.Invalid));
                  Assert.That (() => f.InParameterArray[0].EnsureDataAvailable (), Throws.TypeOf<ObjectInvalidException> ());
                },
                inParameter,
                inParameterArray);

            subFunction.SetParentStep (parentF);
            subFunction.Execute (parentCtx);
          });
    }

    [Test]
    public void AutomaticParameterEnlisting_CreateChild_WithNonLoadableOutParameter ()
    {
      ExecuteDelegateInWxeFunction (
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          (parentCtx, parentF) =>
          {
            var subFunction = new DomainObjectParameterTestTransactedFunction (
                WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent,
                (ctx, f) =>
                {
                  f.OutParameter = ClassWithAllDataTypes.NewObject();
                  f.OutParameterArray = new[] { ClassWithAllDataTypes.NewObject() };
                },
                null,
                null);

            subFunction.SetParentStep (parentF);
            subFunction.Execute (parentCtx);

            var parentTransaction = parentF.Transaction.GetNativeTransaction<ClientTransaction>();
            Assert.That (parentTransaction.IsEnlisted (subFunction.OutParameter), Is.True);
            Assert.That (subFunction.OutParameter.State, Is.EqualTo (StateType.Invalid));
            Assert.That (() => subFunction.OutParameter.EnsureDataAvailable(), Throws.TypeOf<ObjectInvalidException>());

            Assert.That (parentTransaction.IsEnlisted (subFunction.OutParameterArray[0]), Is.True);
            Assert.That (subFunction.OutParameterArray[0].State, Is.EqualTo (StateType.Invalid));
            Assert.That (() => subFunction.OutParameterArray[0].EnsureDataAvailable(), Throws.TypeOf<ObjectInvalidException>());
          });
    }
  }
}