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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StatefulAccessControlListTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void GetClass ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition ("SecurableClass");
      StatefulAccessControlList acl = _testHelper.CreateStatefulAcl (classDefinition);

      Assert.That (acl.Class, Is.SameAs (classDefinition));
    }

    [Test]
    public void CreateStateCombination ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition ("SecurableClass");
      StatefulAccessControlList acl = _testHelper.CreateStatefulAcl (classDefinition);
      using (_testHelper.Transaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, classDefinition.State);
        Assert.AreEqual (StateType.Unchanged, acl.State);

        StateCombination stateCombination = acl.CreateStateCombination ();

        Assert.AreSame (acl, stateCombination.AccessControlList);
        Assert.AreEqual (acl.Class, stateCombination.Class);
        Assert.IsEmpty (stateCombination.StateUsages);
        Assert.AreEqual (StateType.Changed, classDefinition.State);
        Assert.AreEqual (StateType.Changed, acl.State);
      }
    }

    [Test]
    public void CreateStateCombination_WithoutClassDefinition ()
    {
      StatefulAccessControlList acl = _testHelper.CreateStatefulAcl (SecurableClassDefinition.NewObject ());
      using (_testHelper.Transaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, acl.State);

        acl.StateCombinations.Add (StateCombination.NewObject ());

        Assert.AreEqual (StateType.Changed, acl.State);
      }
    }

    [Test]
    public void CreateStateCombination_TwoNewEntries ()
    {
      StatefulAccessControlList acl = StatefulAccessControlList.NewObject (_testHelper.CreateClassDefinition ("SecurableClass"));
      using (_testHelper.Transaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, acl.State);

        StateCombination stateCombination0 = acl.CreateStateCombination ();
        StateCombination stateCombination1 = acl.CreateStateCombination ();

        Assert.AreEqual (2, acl.StateCombinations.Count);
        Assert.AreSame (stateCombination0, acl.StateCombinations[0]);
        Assert.AreEqual (0, stateCombination0.Index);
        Assert.AreSame (stateCombination1, acl.StateCombinations[1]);
        Assert.AreEqual (1, stateCombination1.Index);
        Assert.AreEqual (StateType.Changed, acl.State);
      }
    }

    [Test]
    public void Get_StateCombinationsFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      StatefulAccessControlList expectedAcl = dbFixtures.CreateAndCommitAccessControlListWithStateCombinations (10, ClientTransactionScope.CurrentTransaction);
      ObjectList<StateCombination> expectedStateCombinations = expectedAcl.StateCombinations;

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        StatefulAccessControlList actualAcl = StatefulAccessControlList.GetObject (expectedAcl.ID);

        Assert.AreEqual (10, actualAcl.StateCombinations.Count);
        for (int i = 0; i < 10; i++)
          Assert.AreEqual (expectedStateCombinations[i].ID, actualAcl.StateCombinations[i].ID);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Cannot create StateCombination if no SecurableClassDefinition is assigned to this StatefulAccessControlList.")]
    public void CreateStateCombination_BeforeClassIsSet ()
    {
      StatefulAccessControlList acl = StatefulAccessControlList.NewObject (_testHelper.CreateClassDefinition ("SecurableClass"));
      PropertyIndexer propertyIndexer = new PropertyIndexer (acl);
      propertyIndexer[typeof (StatefulAccessControlList), "MyClass"].SetValue ((SecurableClassDefinition) null);
      acl.CreateStateCombination ();
    }
  }
}