using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    private AccessControlTestHelper _testHelper;
    private SecurableClassDefinition _orderClass;
    private StateCombinationBuilder _stateCombinationBuilder;
    private OuterProductOfStateProperties _outerProductOfStateProperties;


    public override void SetUp ()
    {
      base.SetUp();
      ClientTransaction.NewRootTransaction().EnterDiscardingScope();
      OrderClass = SecurableClassDefinition.GetObject (SetUpFixture.OrderClassID);
      DummyClass = SecurableClassDefinition.NewObject ();

      _testHelper = new AccessControlTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope ();
      _orderClass = _testHelper.CreateOrderClassDefinition ();
      _stateCombinationBuilder = new StateCombinationBuilder (_orderClass);
      _outerProductOfStateProperties = new OuterProductOfStateProperties (_orderClass);
    }

    private void LogStopwatch(Stopwatch stopwatch, String message)
    {
      Console.Write (Environment.NewLine + Environment.NewLine + message + ": " + String.Format ("{0} ms = {1} s = {2} min", stopwatch.ElapsedMilliseconds, stopwatch.ElapsedMilliseconds / (1000.0), stopwatch.ElapsedMilliseconds / (1000.0 * 60.0)));
    }

    private void LogPropertyStateTuples (PropertyStateTuple[][] propertyStateTuples)
    {
      Console.Write (Environment.NewLine + "LogPropertyStateTuples: ");
      int i = 0;
      foreach (var propertyStateTupleArray in propertyStateTuples)
      {
        Console.Write (Environment.NewLine + String.Format ("{0}) ", i));
        ++i;
        foreach (var propertyStateTuple in propertyStateTupleArray)
          Console.Write(String.Format ("[{0}]", propertyStateTuple.State.Name));
      }
    }

    private delegate PropertyStateTuple[][] CalculatePropertyOuterProductDelegate ();

    private PropertyStateTuple[][] CalculatePropertyOuterProduct (
      string delegateIdentifierName, CalculatePropertyOuterProductDelegate calculatePropertyOuterProduct,
      bool logResult)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start ();
      PropertyStateTuple[][] propertyOuterProduct = calculatePropertyOuterProduct();
      stopwatch.Stop ();
      LogStopwatch (stopwatch, delegateIdentifierName);

      if (logResult)
      {
        LogPropertyStateTuples (propertyOuterProduct);
      }

      return propertyOuterProduct;
    }



    [Test]
    public void TestPropertyOuterProductImplementations ()
    {
      //for (int i = 0; i < 5; i++)
      for (int iProperty = 0; iProperty < 8; iProperty++)
      {
        //StatePropertyDefinition property = _testHelper.CreateStateProperty (Guid.NewGuid ().ToString ());
        StatePropertyDefinition property = _testHelper.CreateStateProperty ("p" + iProperty);
        for (int iState = 0; iState < 4; iState++)
        {
          //property.AddState (Guid.NewGuid ().ToString (), j);
          property.AddState ("s" + iProperty + "-" + iState, iState);
        }
        _orderClass.AddStateProperty (property);
      }

      const bool logResult = false;
      PropertyStateTuple[][] actualSCB = CalculatePropertyOuterProduct ("StateCombinationBuilder.CreatePropertyProduct", _stateCombinationBuilder.CreatePropertyProduct, logResult); //, iProperty, iState);
      PropertyStateTuple[][] actualOPOSP = CalculatePropertyOuterProduct ("OuterProductOfStateProperties.CalculateOuterProduct3", _outerProductOfStateProperties.CalculateOuterProduct3, logResult); //, iProperty, iState);

      Assert.That (actualOPOSP.Length, Is.EqualTo (actualSCB.Length));

      for (int i = 0; i < actualSCB.Length; ++i)
      {
        Assert.That (actualSCB[i], Is.EquivalentTo (actualOPOSP[i]));
      }

    }



    //OuterProductOfStateProperties

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