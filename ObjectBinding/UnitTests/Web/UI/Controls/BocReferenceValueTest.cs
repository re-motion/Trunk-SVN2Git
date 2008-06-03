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
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.Web.UI.Controls;
using Remotion.Web.UnitTests.Configuration;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

[TestFixture]
public class BocReferenceValueTest: BocTest
{
  private BocReferenceValueMock _bocReferenceValue;
  private TypeWithReference _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectReferenceProperty _propertyReferenceValue;

  public BocReferenceValueTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocReferenceValue = new BocReferenceValueMock();
    _bocReferenceValue.ID = "BocReferenceValue";
    _bocReferenceValue.ShowOptionsMenu = false;
    _bocReferenceValue.Command.Type = CommandType.None;
    _bocReferenceValue.Command.Show = CommandShow.Always;
    _bocReferenceValue.InternalValue = Guid.Empty.ToString();
    NamingContainer.Controls.Add (_bocReferenceValue);
 
    _businessObject = TypeWithReference.Create();

    _propertyReferenceValue = (IBusinessObjectReferenceProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = (IBusinessObject) _businessObject;
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocReferenceValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.DropDownListStyle.AutoPostBack = true;
    _bocReferenceValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocReferenceValue, WcagHelperMock.Control);
    Assert.AreEqual ("DropDownListStyle.AutoPostBack", WcagHelperMock.Property);
  }


	[Test]
  public void EvaluateWaiConformityDebugExceptionLevelAWithShowOptionsMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocReferenceValue, WcagHelperMock.Control);
    Assert.AreEqual ("ShowOptionsMenu", WcagHelperMock.Property);
  }


  [Test]
  public void IsOptionsMenuInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsFalse (_bocReferenceValue.HasOptionsMenu);
  }

  [Test]
  public void IsOptionsMenuVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocReferenceValue.ShowOptionsMenu = true;
    _bocReferenceValue.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsTrue (_bocReferenceValue.HasOptionsMenu);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithEventCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command.Type = CommandType.Event;
    _bocReferenceValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocReferenceValue, WcagHelperMock.Control);
    Assert.AreEqual ("Command", WcagHelperMock.Property);
  }

  [Test]
  public void IsEventCommandDisabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.Command.Type = CommandType.Event;
    Assert.IsFalse (_bocReferenceValue.IsCommandEnabled (false));
  }

  [Test]
  public void IsEventCommandEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocReferenceValue.Command.Type = CommandType.Event;
    Assert.IsTrue (_bocReferenceValue.IsCommandEnabled (false));
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithWxeFunctionCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command.Type = CommandType.WxeFunction;
    _bocReferenceValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocReferenceValue, WcagHelperMock.Control);
    Assert.AreEqual ("Command", WcagHelperMock.Property);
  }

  [Test]
  public void IsWxeFunctionCommandDisabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocReferenceValue.Command.Type = CommandType.WxeFunction;
    Assert.IsFalse (_bocReferenceValue.IsCommandEnabled (false));
  }

  [Test]
  public void IsWxeFunctionCommandEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocReferenceValue.Command.Type = CommandType.WxeFunction;
    Assert.IsTrue (_bocReferenceValue.IsCommandEnabled (false));
  }

	
  [Test]
  public void EvaluateWaiConformityDebugLevelAWithHrefCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command.Type = CommandType.Href;
    _bocReferenceValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithoutCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocReferenceValue.Command = null;
    _bocReferenceValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocReferenceValue.ReadOnly = true;
    string[] actual = _bocReferenceValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditMode()
  {
    _bocReferenceValue.ReadOnly = false;
    string[] actual = _bocReferenceValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocReferenceValue.DropDownList.ClientID, actual[0]);
  }


  [Test]
  public void SetValueToObject()
  {
    IBusinessObjectWithIdentity referencedObject = (IBusinessObjectWithIdentity) TypeWithReference.Create();
    _bocReferenceValue.IsDirty = false;
    _bocReferenceValue.Value = referencedObject;
    Assert.AreEqual (referencedObject, _bocReferenceValue.Value);
    Assert.IsTrue (_bocReferenceValue.IsDirty);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocReferenceValue.IsDirty = false;
    _bocReferenceValue.Value = null;
    Assert.AreEqual (null, _bocReferenceValue.Value);
    Assert.IsTrue (_bocReferenceValue.IsDirty);
  }
    

  [Test]
  public void LoadValueAndInterimTrue()
  {
    _businessObject.ReferenceValue = TypeWithReference.Create();
    _bocReferenceValue.DataSource = _dataSource;
    _bocReferenceValue.Property = _propertyReferenceValue;
    _bocReferenceValue.Value = null;
    _bocReferenceValue.IsDirty = true;

    _bocReferenceValue.LoadValue (true);
    Assert.AreEqual (null, _bocReferenceValue.Value);
    Assert.IsTrue (_bocReferenceValue.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithObject()
  {
    _businessObject.ReferenceValue = TypeWithReference.Create();
    _bocReferenceValue.DataSource = _dataSource;
    _bocReferenceValue.Property = _propertyReferenceValue;
    _bocReferenceValue.Value = null;
    _bocReferenceValue.IsDirty = true;

    _bocReferenceValue.LoadValue (false);
    Assert.AreEqual (_businessObject.ReferenceValue, _bocReferenceValue.Value);
    Assert.IsFalse (_bocReferenceValue.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithNull()
  {
    _businessObject.ReferenceValue = null;
    _bocReferenceValue.DataSource = _dataSource;
    _bocReferenceValue.Property = _propertyReferenceValue;
    _bocReferenceValue.Value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
    _bocReferenceValue.IsDirty = true;

    _bocReferenceValue.LoadValue (false);
    Assert.AreEqual (_businessObject.ReferenceValue, _bocReferenceValue.Value);
    Assert.IsFalse (_bocReferenceValue.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimTrue()
  {
    IBusinessObjectWithIdentity value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
    _bocReferenceValue.Property = _propertyReferenceValue;
    _bocReferenceValue.Value = null;
    _bocReferenceValue.IsDirty = true;

    _bocReferenceValue.LoadUnboundValue (value, true);
    Assert.AreEqual (null, _bocReferenceValue.Value);
    Assert.IsTrue (_bocReferenceValue.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithObject()
  {
    IBusinessObjectWithIdentity value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
    _bocReferenceValue.Property = _propertyReferenceValue;
    _bocReferenceValue.Value = null;
    _bocReferenceValue.IsDirty = true;

    _bocReferenceValue.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocReferenceValue.Value);
    Assert.IsFalse (_bocReferenceValue.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithNull()
  {
    IBusinessObjectWithIdentity value = null;
    _bocReferenceValue.Property = _propertyReferenceValue;
    _bocReferenceValue.Value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
    _bocReferenceValue.IsDirty = true;

    _bocReferenceValue.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocReferenceValue.Value);
    Assert.IsFalse (_bocReferenceValue.IsDirty);
  }
}

}
