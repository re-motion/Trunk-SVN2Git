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
              f.OutParameter = ClassWithAllDataTypes.NewObject ();
              f.OutParameterArray = new[] { ClassWithAllDataTypes.NewObject () };
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
  }
}