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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.AclTools.Expansion.StateCombinationBuilder;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;


namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.SecurityContextHelper
{
  [TestFixture]
  public class SecurityContextHelperTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;
    private SecurableClassDefinition _orderClass;
    private SecurableClassDefinition _dummyClass;
    private StateCombinationBuilder _stateCombinationBuilder;
    private StateCombinationBuilderFast _outerProductOfStateProperties;
    private SecurableClassDefinition _orderClassWithProperties;


    public override void SetUp ()
    {
      base.SetUp();
      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
      _orderClass = SecurableClassDefinition.GetObject (SetUpFixture.OrderClassID);
      _dummyClass = SecurableClassDefinition.NewObject ();

      _testHelper = new AccessControlTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope ();
      _orderClass = _testHelper.CreateOrderClassDefinition ();

      _orderClassWithProperties = _testHelper.CreateOrderClassDefinitionWithProperties ();

      _stateCombinationBuilder = new StateCombinationBuilder (_orderClass);
      _outerProductOfStateProperties = new StateCombinationBuilderFast (_orderClass);
    }

 

    private void ClassStateTupleTestHelper (ClassStateTuple classStateTuple, StateCombination stateCombination)
    {
      Assert.That (classStateTuple.Class, Is.EqualTo (_orderClassWithProperties));
      Assert.That (classStateTuple.Class, Is.Not.EqualTo (_dummyClass));
      Assert.That (classStateTuple.StateList, Is.EqualTo (stateCombination.GetStates ()));
    }

    [Test]
    public void ClassStateTupleTest ()
    {
      SecurableClassDefinition testClass = _orderClassWithProperties;
      foreach (StateCombination stateCombination in testClass.StateCombinations)
      {
        var classStateTuple = new ClassStateTuple (_orderClassWithProperties, stateCombination);
        ClassStateTupleTestHelper (classStateTuple, stateCombination);

        var classStateTuple2 = new ClassStateTuple (_orderClassWithProperties, stateCombination);
        ClassStateTupleTestHelper (classStateTuple2, stateCombination);
      }
    }


    [Test]
    public void AclSecurityContextHelperTest ()
    {
      SecurableClassDefinition testClass = _orderClassWithProperties;
      var aclSecurityContextHelper = new AclSecurityContextHelper (testClass.Name);
      Assert.That (aclSecurityContextHelper.Class, Is.EqualTo (testClass.Name));

      StatePropertyDefinition statePropertyDefinition = testClass.StateProperties[0];
      StateDefinition stateDefinition = statePropertyDefinition.GetState (0);
      aclSecurityContextHelper.AddState (statePropertyDefinition, stateDefinition);
      Assert.That (aclSecurityContextHelper.GetStateDefinitionList (), Is.EqualTo (new List<StateDefinition> () { stateDefinition }));
      Assert.That (aclSecurityContextHelper.GetStateDefinitionList (), Is.Not.EqualTo (new List<StateDefinition> () { null }));

      Assert.That (aclSecurityContextHelper.GetState (statePropertyDefinition.Name), Is.EqualTo (new EnumWrapper (stateDefinition.Name)));
    }



  }
}