// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

[TestFixture]
public class BocTreeViewTest: BocTest
{
  private BocTreeViewMock _bocTreeView;
  private TypeWithReference _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectReferenceProperty _propertyReferenceValue;
  private IBusinessObjectReferenceProperty _propertyReferenceList;

  public BocTreeViewTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocTreeView = new BocTreeViewMock();
    _bocTreeView.ID = "BocTreeView";
    NamingContainer.Controls.Add (_bocTreeView);

    _businessObject = TypeWithReference.Create();

    _propertyReferenceValue = (IBusinessObjectReferenceProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue");
    _propertyReferenceList = (IBusinessObjectReferenceProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceList");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = (IBusinessObject) _businessObject;
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocTreeView.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocTreeView.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocTreeView.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocTreeView, WcagHelperMock.Control);
    Assert.IsNull (WcagHelperMock.Property);
  }


  [Test]
  public void SetValueToList()
  {
    TypeWithReference[] list = new TypeWithReference[] { TypeWithReference.Create () };
    _bocTreeView.Value = list;
    Assert.AreEqual (list, _bocTreeView.Value);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocTreeView.Value = null;
    Assert.AreEqual (null, _bocTreeView.Value);
  }
    

  [Test]
  public void LoadValueAndInterimTrueWithObject()
  {
    _bocTreeView.DataSource = _dataSource;
    _bocTreeView.Value = null;

    _bocTreeView.LoadValue (true);
    IList actual = (IList) _bocTreeView.Value;
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Count);
    Assert.AreEqual (_businessObject, actual[0]);
  }

  [Test]
  public void LoadValueAndInterimFalseWithObject()
  {
    _bocTreeView.DataSource = _dataSource;
    _bocTreeView.Value = null;

    _bocTreeView.LoadValue (false);
    IList actual = (IList) _bocTreeView.Value;
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Count);
    Assert.AreEqual (_businessObject, actual[0]);
  }

  [Test]
  public void LoadUnboundValueAndInterimTrueWithList()
  {
    TypeWithReference[] value = new TypeWithReference[] {TypeWithReference.Create(), TypeWithReference.Create()};
    _bocTreeView.Value = null;

    _bocTreeView.LoadUnboundValue (value, true);
    Assert.AreEqual (value, _bocTreeView.Value);
  }

  [Test]
  public void LoadUnboundValueAndInterimTrueWithNull()
  {
    TypeWithReference[] value = null;
    _bocTreeView.Value = new TypeWithReference[0];

    _bocTreeView.LoadUnboundValue (value, true);
    Assert.AreEqual (value, _bocTreeView.Value);
  }   

  [Test]
  public void LoadUnboundValueAndInterimFalseWithList()
  {
    TypeWithReference[] value = new TypeWithReference[] {TypeWithReference.Create(), TypeWithReference.Create()};
    _bocTreeView.Value = null;

    _bocTreeView.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocTreeView.Value);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithNull()
  {
    TypeWithReference[] value = null;
    _bocTreeView.Value = new TypeWithReference[0];

    _bocTreeView.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocTreeView.Value);
  }   

}

}
