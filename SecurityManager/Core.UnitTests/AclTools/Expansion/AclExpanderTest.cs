using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Remotion.Utilities;
using Remotion.SecurityManager.AclTools.Expansion;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpanderTest : DomainTest
  {
    private SecurableClassDefinition OrderClass { get; set; }
    private SecurableClassDefinition DummyClass { get; set; }


    public override void SetUp ()
    {
      base.SetUp();
      ClientTransaction.NewRootTransaction().EnterDiscardingScope();
      OrderClass = SecurableClassDefinition.GetObject (SetUpFixture.OrderClassID);
      DummyClass = SecurableClassDefinition.NewObject ();
    }

    [Test]
    public void Create_WithoutStateProperty ()
    {
      ClientTransactionScope.ResetActiveScope ();
      AccessControlTestHelper accessControlTestHelper = new AccessControlTestHelper ();
      accessControlTestHelper.Transaction.EnterDiscardingScope ();
      SecurableClassDefinition orderClass = accessControlTestHelper.CreateOrderClassDefinition ();
      Assert.That (orderClass.StateProperties, Is.Empty);

      StateDefinition[][] expected = new StateDefinition[][] { };

      OuterProductOfStateProperties sut = new OuterProductOfStateProperties (orderClass);

      StateDefinition[][] actual = sut.CreatePropertyProduct ();

      Check (actual, expected);
    }

    [Test]
    public void Create_WithSingleStateProperty ()
    {
      ClientTransactionScope.ResetActiveScope ();
      AccessControlTestHelper accessControlTestHelper = new AccessControlTestHelper ();
      accessControlTestHelper.Transaction.EnterDiscardingScope ();
      SecurableClassDefinition orderClass = accessControlTestHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition orderStateProperty = accessControlTestHelper.CreateOrderStateProperty (orderClass);
      Assert.That (orderClass.StateProperties.Count, Is.EqualTo (1));

      StateDefinition[][] expected =
          new[]
          {
              new[] { orderStateProperty.DefinedStates[0] },
              new[] { orderStateProperty.DefinedStates[1] },
          };

      OuterProductOfStateProperties sut = new OuterProductOfStateProperties (orderClass);

      StateDefinition[][] actual = sut.CreatePropertyProduct ();

      Check (actual, expected);
    }

    [Test]
    public void Create_WithTwoStateProperty ()
    {
      ClientTransactionScope.ResetActiveScope ();
      AccessControlTestHelper accessControlTestHelper = new AccessControlTestHelper ();
      accessControlTestHelper.Transaction.EnterDiscardingScope ();
      SecurableClassDefinition orderClass = accessControlTestHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition orderStateProperty = accessControlTestHelper.CreateOrderStateProperty (orderClass);
      StatePropertyDefinition paymentProperty = accessControlTestHelper.CreatePaymentStateProperty (orderClass);
      Assert.That (orderClass.StateProperties.Count, Is.EqualTo (2));

      StateDefinition[][] expected =
          new[]
          {
              new[] { orderStateProperty.DefinedStates[0], paymentProperty.DefinedStates[0] },
              new[] { orderStateProperty.DefinedStates[0], paymentProperty.DefinedStates[1] },
              new[] { orderStateProperty.DefinedStates[1], paymentProperty.DefinedStates[0] },
              new[] { orderStateProperty.DefinedStates[1], paymentProperty.DefinedStates[1] },
          };

      OuterProductOfStateProperties sut = new OuterProductOfStateProperties (orderClass);

      StateDefinition[][] actual = sut.CreatePropertyProduct ();

      Check (actual, expected);
    }

    private void Check (StateDefinition[][] actual, StateDefinition[][] expected)
    {
      Assert.That (actual.Length, Is.EqualTo (expected.Length));
      for (int i = 0; i < actual.Length; i++)
        Assert.That (actual[0], Is.EquivalentTo (expected[0]));
    }

    //private void Log (string format, params Object[] variables)
    //{
    //  Console.WriteLine (String.Format (format, variables));
    //}


    //private void ClassStateTupleTestHelper (ClassStateTuple classStateTuple, StateCombination stateCombination)
    //{
    //  Assert.That (classStateTuple.Class, Is.EqualTo (OrderClass));
    //  Assert.That (classStateTuple.Class, Is.Not.EqualTo (DummyClass));
    //  Assert.That (classStateTuple.StateList, Is.EqualTo (stateCombination.GetStates()));
    //}

    //[Test]
    //public void ClassStateTupleTest ()
    //{
    //  SecurableClassDefinition testClass = OrderClass;
    //  foreach(StateCombination stateCombination in testClass.StateCombinations)
    //  {
    //    var classStateTuple = new ClassStateTuple (OrderClass, stateCombination);
    //    ClassStateTupleTestHelper (classStateTuple, stateCombination);

    //    var classStateTuple2 = new ClassStateTuple (OrderClass, stateCombination.GetStates ());
    //    ClassStateTupleTestHelper (classStateTuple2, stateCombination);
    //  }
    //}


    //[Test]
    //public void AclSecurityContextHelperTest ()
    //{
    //  SecurableClassDefinition testClass = OrderClass;
    //  var aclSecurityContextHelper = new AclSecurityContextHelper(testClass.Name);
    //  Assert.That (aclSecurityContextHelper.Class, Is.EqualTo (testClass.Name));
      
    //  StatePropertyDefinition statePropertyDefinition = testClass.StateProperties[0];
    //  StateDefinition stateDefinition = statePropertyDefinition.GetState (0);
    //  aclSecurityContextHelper.AddState (statePropertyDefinition, stateDefinition);
    //  Assert.That (aclSecurityContextHelper.GetStateDefinitionList (), Is.EqualTo (new List<StateDefinition>() { stateDefinition } ));
    //  Assert.That (aclSecurityContextHelper.GetStateDefinitionList (), Is.Not.EqualTo (new List<StateDefinition> () { null }));

    //  Assert.That (aclSecurityContextHelper.GetState (statePropertyDefinition.Name), Is.EqualTo (new EnumWrapper (stateDefinition.Name)));
    //}


//    [Test]
//    public void OuterProductOfStatePropertiesTest ()
//    {
//      // DummyClass must have zero state combinations
//      Assert.That (OuterProductOfStateProperties.CalcOuterProductNrStateCombinations (DummyClass), Is.EqualTo (0));

//      SecurableClassDefinition testClass = OrderClass;

//      foreach (var statePropertyDefinition in testClass.StateProperties)
//      {
//        //Console.WriteLine (String.Format ("statePropertyDefinition.Name={0}, statePropertyDefinition.DefinedStates.Count={1}", statePropertyDefinition.Name, statePropertyDefinition.DefinedStates.Count));
//        Log (
//            "statePropertyDefinition.Name={0}, statePropertyDefinition.DefinedStates.Count={1}",
//            statePropertyDefinition.Name,
//            statePropertyDefinition.DefinedStates.Count);

//      }
//      Log ("------------------------------------------");

//      int nStateCombinations = OuterProductOfStateProperties.CalcOuterProductNrStateCombinations (testClass);
//      Console.WriteLine (String.Format ("nStateCombinations={0}", nStateCombinations));
//      Assert.That (nStateCombinations, Is.GreaterThan (3));

//      var outerProductOfStateProperties = new OuterProductOfStateProperties (testClass);
//      //var aclSecurityContextHelperList = new List<AclSecurityContextHelper> ();

//      //Dictionary<string, Enum> leftStates = new Dictionary<string, Enum> ();
//      //leftStates.Add ("State", TestSecurityState.Public);
//      //leftStates.Add ("Confidentiality", TestSecurityState.Public);

//      //foreach (AclSecurityContextHelper aclSecurityContextHelper in outerProductOfStateProperties)
//      //{
//      //  //Assert.That (!aclSecurityContextHelperList.Contains( aclSecurityContextHelper));
//      //  //aclSecurityContextHelperList.Add (aclSecurityContextHelper);
//      //  Log ("{0}", aclSecurityContextHelper);
//      //}

//      Log ("outerProductOfStateProperties: {0}", outerProductOfStateProperties.ToTestString());

//      string outerProductOfStatePropertiesTestString =
//          @"
//{[Delivery,Dhl][State,Received][Payment,None]} {[Dhl][Received][None]}
//{[Delivery,Post][State,Received][Payment,None]} {[Post][Received][None]}
//{[Delivery,Dhl][State,Delivered][Payment,None]} {[Dhl][Delivered][None]}
//{[Delivery,Post][State,Delivered][Payment,None]} {[Post][Delivered][None]}
//{[Delivery,Dhl][State,Received][Payment,Paid]} {[Dhl][Received][Paid]}
//{[Delivery,Post][State,Received][Payment,Paid]} {[Post][Received][Paid]}
//{[Delivery,Dhl][State,Delivered][Payment,Paid]} {[Dhl][Delivered][Paid]}
//{[Delivery,Post][State,Delivered][Payment,Paid]} {[Post][Delivered][Paid]}";

//      //Assert.That (outerProductOfStateProperties.ToTestString (), Is.EqualTo (""));
//      // TODO: Reactivate assertion when states have a sort order.
//      //Assert.That (outerProductOfStateProperties.ToTestString (), Is.EqualTo (outerProductOfStatePropertiesTestString));
//    }

//    [Test]
//    //[Ignore]
//    public void GetallAclsExpanded ()
//    {
//      SecurableClassDefinition orderClass = SecurableClassDefinition.GetObject (SetUpFixture.OrderClassID);
      
//      ClassStateExpander classStateExpander = new ClassStateExpander();
//      //classStateExpander.Init();
      
//      //var aclList = SetUpFixture.aclList;
//      var aclList = orderClass.AccessControlLists;

//      Assert.That (aclList.Count, Is.GreaterThanOrEqualTo (3), "Expected at least 3 ACLs in aclList for testing.");

//      foreach (var acl in aclList)
//      {
//        foreach (var state in acl.StateCombinations)
//        {
//          //Assert.That (classStateExpander.GetACL (new ClassStateTuple (acl.Class, state)), Is.EqualTo (acl));
//          Assert.That (classStateExpander.GetACL (acl.Class, state), Is.EqualTo (acl));
//        }
//      }


//      /*
//      var list = new List<Tuple<SecurableClassDefinition, StateCombination>>();
//      list.Add (new Tuple<SecurableClassDefinition, StateCombination> (orderClass, orderClass.StateCombinations[0]));
//      Dictionary<Tuple<SecurableClassDefinition, StateCombination>, AccessControlList> dictionary = classStateExpander.Find (list);

      
//      Assert.That (dictionary, Is.Not.Empty);
//      foreach (var keyValuePair in dictionary)
//      {
//        Assert.That (keyValuePair.Key.A, Is.SameAs (orderClass));
//        Assert.That (keyValuePair.Key.B, Is.SameAs (orderClass.StateCombinations[0]));
//        Assert.That (keyValuePair.Value, Is.Not.Null);
//        Assert.That (keyValuePair.Value.Class, Is.SameAs (orderClass));
//        var expectedStateCombinations = new List<StateCombination> { orderClass.StateCombinations[0] };
//        Assert.That (keyValuePair.Value.StateCombinations, Is.EquivalentTo (expectedStateCombinations));
//      }
//      */
//    }
  }


}