﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure.Factories;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class FormGridControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>), "GetTests")]
    [TestCaseSource (typeof (IndexControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>), "GetTests")]
    [TestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>), "GetTests")]
    [TestCaseSource (typeof (TitleControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>), "GetTests")]
    [TestCaseSource (typeof (FirstControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>), "GetTests")]
    [TestCaseSource (typeof (SingleControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>), "GetTests")]
    public void TestControlSelectors (TestCaseFactoryBase.TestSetupAction<FormGridSelector, FormGridControlObject> testSetupAction)
    {
      testSetupAction (Helper, e => e.FormGrids(), "formGrid");
    }

    // Exists as unused member for future FormGrid tests.
    // ReSharper disable once UnusedMember.Local
    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("FormGridTest.aspx");
    }
  }
}