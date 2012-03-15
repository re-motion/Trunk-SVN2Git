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
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class StatePropertyDefinitionTest : DomainTest
  {
    private MetadataTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new MetadataTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void GetState_ValidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      StateDefinition actualState = stateProperty.GetState (MetadataTestHelper.Confidentiality_ConfidentialName);

      StateDefinition expectedState = _testHelper.CreateConfidentialState();
      MetadataObjectAssert.AreEqual (expectedState, actualState, "Confidential state");
    }

    [Test]
    public void GetState_InvalidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      Assert.That (
          () => stateProperty.GetState ("New"),
          Throws.ArgumentException.And.Message.StartsWith ("The state 'New' is not defined for the property 'Confidentiality'."));
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

      StateDefinition expectedState = _testHelper.CreatePrivateState();
      MetadataObjectAssert.AreEqual (expectedState, actualState, "Private state");
    }

    [Test]
    public void GetState_InvalidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      Assert.That (
          () => stateProperty.GetState (42),
          Throws.ArgumentException.And.Message.StartsWith ("A state with the value 42 is not defined for the property 'Confidentiality'."));
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

      StateDefinition expectedState = _testHelper.CreateConfidentialState();
      MetadataObjectAssert.AreEqual (expectedState, actualState, "Confidential state");
    }

    [Test]
    public void AddState ()
    {
      var state1 = _testHelper.CreateState ("State 1", 1);
      var state2 = _testHelper.CreateState ("State 2", 2);
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty ("NewProperty");
      stateProperty.AddState (state1);
      stateProperty.AddState (state2);

      Assert.That (stateProperty.DefinedStates, Is.EqualTo (new[] { state1, state2 }));
    }

    [Test]
    public void AddState_DuplicateName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty ("NewProperty");
      stateProperty.AddState (_testHelper.CreateState ("State 1", 1));

      Assert.That (
          () => stateProperty.AddState (_testHelper.CreateState ("State 1", 2)),
          Throws.ArgumentException.And.Message.StartsWith ("A state with the name 'State 1' was already added to the property 'NewProperty'."));
    }

    [Test]
    public void AddState_DuplicateValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty ("NewProperty");
      stateProperty.AddState (_testHelper.CreateState ("State 1", 1));

      Assert.That (
          () => stateProperty.AddState (_testHelper.CreateState ("State 2", 1)),
          Throws.ArgumentException.And.Message.StartsWith ("A state with the value 1 was already added to the property 'NewProperty'."));
    }

    [Test]
    public void RemoveState ()
    {
      var state1 = _testHelper.CreateState ("State 1", 1);
      var state2 = _testHelper.CreateState ("State 2", 2);
      var state3 = _testHelper.CreateState ("State 3", 3);
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty ("NewProperty");
      stateProperty.AddState (state1);
      stateProperty.AddState (state2);
      stateProperty.AddState (state3);

      stateProperty.RemoveState (state2);

      Assert.That (stateProperty.DefinedStates, Is.EqualTo (new[] { state1, state3 }));
    }

    [Test]
    public void GetDefinedStates ()
    {
      var state1 = _testHelper.CreateState ("State 1", 1);
      var state2 = _testHelper.CreateState ("State 2", 2);
      var state3 = _testHelper.CreateState ("State 3", 3);
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty ("NewProperty");
      stateProperty.AddState (state1);
      stateProperty.AddState (state3);
      stateProperty.AddState (state2);

      Assert.That (stateProperty.DefinedStates, Is.EqualTo (new[] { state1, state2, state3 }));
    }
  }
}