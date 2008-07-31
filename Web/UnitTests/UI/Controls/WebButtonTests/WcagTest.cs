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
using Remotion.Development.Web.UnitTesting.Configuration;

namespace Remotion.Web.UnitTests.UI.Controls.WebButtonTests
{

[TestFixture]
public class WcagTest : BaseTest
{
  private TestWebButton _webButton;

  protected override void SetUpPage()
  {
    base.SetUpPage();
    _webButton = new TestWebButton();
    _webButton.ID = "WebButton";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _webButton.UseLegacyButton = false;
    _webButton.EvaluateWaiConformity();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _webButton.UseLegacyButton = false;
    _webButton.EvaluateWaiConformity();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithUseLegacyButtonIsFalse()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _webButton.UseLegacyButton = false;
    _webButton.EvaluateWaiConformity();
    
    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_webButton, WcagHelperMock.Control);
    Assert.AreEqual ("UseLegacyButton", WcagHelperMock.Property);
  }


  [Test]
  public void IsLegacyButtonEnabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _webButton.UseLegacyButton = false;
    Assert.IsTrue (_webButton.IsLegacyButtonEnabled);
  }

  [Test]
  public void IsLegacyButtonEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _webButton.UseLegacyButton = false;
    Assert.IsFalse (_webButton.IsLegacyButtonEnabled);
  }
}

}
