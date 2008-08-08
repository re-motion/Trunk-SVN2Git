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
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateCombinationBuilderTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;
    private SecurableClassDefinition _orderClass;
    private StateCombinationBuilder _stateCombinationBuilder;
    //private StateCombinationBuilderFast stateCombinationBuilderFast;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
      _orderClass = _testHelper.CreateOrderClassDefinition();
      _stateCombinationBuilder = new StateCombinationBuilder (_orderClass);
    }

    [Test]
    public void Create_WithoutStateProperty ()
    {
      Assert.That (_orderClass.StateProperties, Is.Empty);

      PropertyStateTuple[][] expected = new PropertyStateTuple[][] { };

      PropertyStateTuple[][] actual = _stateCombinationBuilder.CreatePropertyProduct();

      Check (actual, expected);
    }

    [Test]
    public void Create_WithSingleStateProperty ()
    {
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (_orderClass);
      Assert.That (_orderClass.StateProperties.Count, Is.EqualTo (1));

      PropertyStateTuple[][] expected =
          new[]
          {
              new[] { CreateTuple (orderStateProperty, 0) },
              new[] { CreateTuple (orderStateProperty, 1) },
          };

      PropertyStateTuple[][] actual = _stateCombinationBuilder.CreatePropertyProduct();

      Check (actual, expected);
    }

    [Test]
    public void Create_WithTwoStateProperties ()
    {
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (_orderClass);
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (_orderClass);
      Assert.That (_orderClass.StateProperties.Count, Is.EqualTo (2));

      PropertyStateTuple[][] expected =
          new[]
          {
              new[] { CreateTuple (orderStateProperty, 0), CreateTuple (paymentProperty, 0) },
              new[] { CreateTuple (orderStateProperty, 0), CreateTuple (paymentProperty, 1) },
              new[] { CreateTuple (orderStateProperty, 1), CreateTuple (paymentProperty, 0) },
              new[] { CreateTuple (orderStateProperty, 1), CreateTuple (paymentProperty, 1) },
          };

      PropertyStateTuple[][] actual = _stateCombinationBuilder.CreatePropertyProduct();

      Check (actual, expected);
    }

    [Test]
    public void Create_WithThreeStatePropertiesAndOneOfThemEmpty ()
    {
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (_orderClass);
      StatePropertyDefinition emptyProperty = _testHelper.CreateStateProperty ("Empty");
      _orderClass.AddStateProperty (emptyProperty);
      StatePropertyDefinition paymentProperty = _testHelper.CreatePaymentStateProperty (_orderClass);
      Assert.That (_orderClass.StateProperties.Count, Is.EqualTo (3));

      PropertyStateTuple[][] expected =
          new[]
          {
              new[] { CreateTuple (orderStateProperty, 0), new PropertyStateTuple (emptyProperty, null), CreateTuple (paymentProperty, 0) },
              new[] { CreateTuple (orderStateProperty, 0), new PropertyStateTuple (emptyProperty, null), CreateTuple (paymentProperty, 1) },
              new[] { CreateTuple (orderStateProperty, 1), new PropertyStateTuple (emptyProperty, null), CreateTuple (paymentProperty, 0) },
              new[] { CreateTuple (orderStateProperty, 1), new PropertyStateTuple (emptyProperty, null), CreateTuple (paymentProperty, 1) },
          };

      PropertyStateTuple[][] actual = _stateCombinationBuilder.CreatePropertyProduct();

      Check (actual, expected);
    }


    [Test]
    [Explicit]
    public void TestStateCombinationBuildersPerformance ()
    {
      const int numberProperty = 8;
      const int numberState = 4;
      for (int iProperty = 0; iProperty < numberProperty; iProperty++)
      {
        StatePropertyDefinition property = _testHelper.CreateStateProperty ("p" + iProperty);
        for (int iState = 0; iState < numberState; iState++)
        {
          property.AddState ("s" + iProperty + "-" + iState, iState);
        }
        _orderClass.AddStateProperty (property);
      }

      // Must not initialize StateCombinationBuilderFast before states/properties in class exist !
      var stateCombinationBuilderFast = new StateCombinationBuilderFast (_orderClass);

      const bool logResult = false;
      var actualOPOSP = CalculatePropertyOuterProduct<PropertyStateTuple[][]> ("StateCombinationBuilderFast.CalculateOuterProduct3", stateCombinationBuilderFast.CalculateOuterProduct3, logResult, numberProperty, numberState);
      var actualOPOSP4 = CalculatePropertyOuterProduct<PropertyStateTuple[,]> ("StateCombinationBuilderFast.CalculateOuterProduct4", stateCombinationBuilderFast.CalculateOuterProduct4, logResult, numberProperty, numberState);
      var actualOPOSP5 = CalculatePropertyOuterProduct<PropertyStateTuple[,]> ("StateCombinationBuilderFast.CalculateOuterProduct5", stateCombinationBuilderFast.CalculateOuterProduct5, logResult, numberProperty, numberState);
      var actualOPOSP6 = CalculatePropertyOuterProduct<PropertyStateTuple[,]> ("StateCombinationBuilderFast.CalculateOuterProduct6", stateCombinationBuilderFast.CalculateOuterProduct6, logResult, numberProperty, numberState);
      var actualSCB = CalculatePropertyOuterProduct<PropertyStateTuple[][]> ("StateCombinationBuilder.CreatePropertyProduct", _stateCombinationBuilder.CreatePropertyProduct, logResult, numberProperty, numberState);
      Console.WriteLine();

      Assert.That (actualOPOSP.Length, Is.EqualTo (actualSCB.Length));

      for (int i = 0; i < actualSCB.Length; ++i)
      {
        Assert.That (actualSCB[i], Is.EquivalentTo (actualOPOSP[i]));
      }

    }

    private void LogStopwatch (Stopwatch stopwatch, String message)
    {
      Console.WriteLine();
      Console.WriteLine (message + ": " + String.Format ("{0} ms = {1} s = {2} min", stopwatch.ElapsedMilliseconds, stopwatch.ElapsedMilliseconds / (1000.0), stopwatch.ElapsedMilliseconds / (1000.0 * 60.0)));
    }

    private void LogPropertyStateTuples (PropertyStateTuple[][] propertyStateTuples)
    //private void LogPropertyStateTuples<T> (T propertyStateTuples) where T : IEnumerable<T>
    {
      Console.Write (Environment.NewLine + "LogPropertyStateTuples: ");
      int i = 0;
      foreach (var propertyStateTupleArray in propertyStateTuples)
      {
        Console.Write (Environment.NewLine + String.Format ("{0}) ", i));
        ++i;
        foreach (var propertyStateTuple in propertyStateTupleArray)
          Console.Write (String.Format ("[{0}]", propertyStateTuple.State.Name));
      }
    }

    private T CalculatePropertyOuterProduct<T> (
      string delegateIdentifierName, Func<T> calculatePropertyOuterProduct,
      bool logResult, int numberProperty, int numberState)
    {
      Stopwatch stopwatch = new Stopwatch ();

      GC.Collect (2);
      GC.WaitForPendingFinalizers ();

      stopwatch.Start ();
      T propertyOuterProduct = calculatePropertyOuterProduct ();
      stopwatch.Stop ();
      LogStopwatch (stopwatch, delegateIdentifierName + String.Format (" (numberProperty={0},numberState={1})", numberProperty, numberState));

      if (logResult)
      {
        //LogPropertyStateTuples (propertyOuterProduct);
      }

      return propertyOuterProduct;
    }


    private PropertyStateTuple CreateTuple (StatePropertyDefinition stateProperty, int stateIndex)
    {
      return new PropertyStateTuple (stateProperty, stateProperty.DefinedStates[stateIndex]);
    }

    private void Check (PropertyStateTuple[][] actual, PropertyStateTuple[][] expected)
    {
      Assert.That (actual.Length, Is.EqualTo (expected.Length));
      for (int i = 0; i < actual.Length; i++)
        Assert.That (actual[0], Is.EquivalentTo (expected[0]));
    }
  }
}