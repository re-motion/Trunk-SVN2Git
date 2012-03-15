// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class StatePropertyDefinitionTest : DomainTest
  {
    private MetadataTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();

      _testHelper = new MetadataTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void GetState_ValidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      StateDefinition actualState = stateProperty.GetState (MetadataTestHelper.Confidentiality_ConfidentialName);

      StateDefinition expectedState = _testHelper.CreateConfidentialState ();
      MetadataObjectAssert.AreEqual (expectedState, actualState, "Confidential state");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The state 'New' is not defined for the property 'Confidentiality'.\r\nParameter name: name")]
    public void GetState_InvalidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      StateDefinition actualState = stateProperty.GetState ("New");
    }

    [Test]
    public void ContainsState_ValidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);
      Assert.IsTrue (stateProperty.ContainsState (MetadataTestHelper.Confidentiality_ConfidentialName));
    }

    [Test]
    public void ContainsState_InvalidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);
      Assert.IsFalse (stateProperty.ContainsState ("New"));
    }

    [Test]
    public void GetState_ValidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      StateDefinition actualState = stateProperty.GetState (MetadataTestHelper.Confidentiality_PrivateValue);

      StateDefinition expectedState = _testHelper.CreatePrivateState ();
      MetadataObjectAssert.AreEqual (expectedState, actualState, "Private state");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "A state with the value 42 is not defined for the property 'Confidentiality'.\r\nParameter name: stateValue")]
    public void GetState_InvalidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      StateDefinition actualState = stateProperty.GetState (42);
    }

    [Test]
    public void ContainsState_ValidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);
      Assert.IsTrue (stateProperty.ContainsState (MetadataTestHelper.Confidentiality_PrivateValue));
    }

    [Test]
    public void ContainsState_InvalidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);
      Assert.IsFalse (stateProperty.ContainsState (42));
    }

    [Test]
    public void Indexer_ValidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      StateDefinition actualState = stateProperty[MetadataTestHelper.Confidentiality_ConfidentialName];

      StateDefinition expectedState = _testHelper.CreateConfidentialState ();
      MetadataObjectAssert.AreEqual (expectedState, actualState, "Confidential state");
    }

    [Test]
    public void AddState_AsStateDefinition ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty ("NewProperty");
      StateDefinition newState = _testHelper.CreateState ("NewState", 42);

      stateProperty.AddState (newState);

      Assert.AreEqual (1, stateProperty.DefinedStates.Count);
      MetadataObjectAssert.AreEqual (newState, stateProperty.GetState ("NewState"));
    }

    [Test]
    public void GetDefinedStates ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();
      StatePropertyDefinition expectdPropertyDefinition = _testHelper.CreateConfidentialityProperty (0);
      var expectedStateDefinitions = expectdPropertyDefinition.DefinedStates;
      _testHelper.Transaction.Commit();

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        StatePropertyDefinition actualStatePropertyDefinition = StatePropertyDefinition.GetObject (expectdPropertyDefinition.ID);

        Assert.AreEqual (3, actualStatePropertyDefinition.DefinedStates.Count);
        for (int i = 0; i < actualStatePropertyDefinition.DefinedStates.Count; i++)
          Assert.AreEqual (expectedStateDefinitions[i].ID, actualStatePropertyDefinition.DefinedStates[i].ID);
      }
    }
  }
}
