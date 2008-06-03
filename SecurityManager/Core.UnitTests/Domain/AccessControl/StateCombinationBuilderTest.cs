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
using NUnit.Framework;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StateCombinationBuilderTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();

      _testHelper = new AccessControlTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void CreateAndAttach_FromClassWithoutStateProperties ()
    {

      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StateCombinationBuilder builder = new StateCombinationBuilder ();

      List<StateCombination> actualStateCombinations = builder.CreateAndAttach (orderClass);
     
      Assert.AreEqual (1, actualStateCombinations.Count);
      StateCombination actualStatelessStateCombination = actualStateCombinations[0];
      CheckStateCombination (orderClass, actualStatelessStateCombination, "Stateless");
      Assert.AreEqual (0, actualStatelessStateCombination.StateUsages.Count);
    }

    [Test]
    [Ignore ("Most likely obsolete")]
    public void CreateAndAttach_FromClassWithSingleStateProperties ()
    {
      SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition orderStateProperty = _testHelper.CreateOrderStateProperty (orderClass);
      StateCombinationBuilder builder = new StateCombinationBuilder ();

      List<StateCombination> actualStateCombinations = builder.CreateAndAttach (orderClass);
    
      Assert.AreEqual (3, actualStateCombinations.Count);

      StateCombination actualStatelessStateCombination = actualStateCombinations[0];
      CheckStateCombination (orderClass, actualStatelessStateCombination, "Stateless");
      Assert.AreEqual (0, actualStatelessStateCombination.StateUsages.Count);

      StateCombination actualReceivedStateCombination = actualStateCombinations[1];
      CheckStateCombination (orderClass, actualReceivedStateCombination, "Received State");
      Assert.AreEqual (1, actualReceivedStateCombination.StateUsages.Count);

      StateCombination actualDeliveredStateCombination = actualStateCombinations[2];
      CheckStateCombination (orderClass, actualDeliveredStateCombination, "Delivered State");
      Assert.AreEqual (1, actualDeliveredStateCombination.StateUsages.Count);
    }

    private static void CheckStateCombination (SecurableClassDefinition orderClass,StateCombination actualStateCombination, string message, params object[] parameters)
    {
      Assert.AreSame (orderClass, actualStateCombination.Class, message, parameters);
      Assert.IsNotNull (actualStateCombination.AccessControlList, message, parameters);
      Assert.AreSame (orderClass, actualStateCombination.AccessControlList.Class, message, parameters);
      Assert.Contains (actualStateCombination, actualStateCombination.AccessControlList.StateCombinations, message, parameters);
    }


    //[Test]
    //public void Create_From ()
    //{
    //  SecurableClassDefinition orderClass = _testHelper.CreateOrderClassDefinitionWithProperties ();
    //  List<StateCombination> expectedStateCombinations = _testHelper.CreateStateCombinationsForOrder ();

    //  List<StateDefinition> states = new List<StateDefinition> ();
    //  StateCombinationBuilder builder = new StateCombinationBuilder ();

    //  List<StateCombination> actualStateCombinations = builder.Create (states);

    //  Assert.AreEqual (5, expectedStateCombinations.Count);
    //  Assert.AreEqual (5, actualStateCombinations.Count);
    //}

  }
}
