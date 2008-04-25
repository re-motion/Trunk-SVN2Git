using System;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.Web.UnitTests.Configuration;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

[TestFixture]
public class BocDropDownMenuTest: BocTest
{
  private BocDropDownMenuMock _bocDropDownMenu;
  private TypeWithReference _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectReferenceProperty _propertyReferenceValue;

  public BocDropDownMenuTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocDropDownMenu = new BocDropDownMenuMock();
    _bocDropDownMenu.ID = "BocDropDownMenu";
    NamingContainer.Controls.Add (_bocDropDownMenu);
 
    _businessObject = TypeWithReference.Create();
    
    _propertyReferenceValue = 
        (IBusinessObjectReferenceProperty) ((IBusinessObject)_businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = (IBusinessObject) _businessObject;
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocDropDownMenu.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocDropDownMenu.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocDropDownMenu.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocDropDownMenu, WcagHelperMock.Control);
    Assert.IsNull (WcagHelperMock.Property);
  }


  [Test]
  public void SetValueToObject()
  {
    IBusinessObject referencedObject = (IBusinessObject) TypeWithReference.Create();
    _bocDropDownMenu.Value = referencedObject;
    Assert.AreEqual (referencedObject, _bocDropDownMenu.Value);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocDropDownMenu.Value = null;
    Assert.AreEqual (null, _bocDropDownMenu.Value);
  }
    

  [Test]
  public void LoadValueAndInterimTrueWithObject()
  {
    _businessObject.ReferenceValue = TypeWithReference.Create();
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Property = _propertyReferenceValue;
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadValue (true);
    Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueAndInterimTrueWithObjectAndNoProperty()
  {
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadValue (true);
    Assert.AreEqual (_businessObject, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueAndInterimTrueWithNull()
  {
    _businessObject.ReferenceValue = null;
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Property = _propertyReferenceValue;
    _bocDropDownMenu.Value = (IBusinessObject)TypeWithReference.Create();

    _bocDropDownMenu.LoadValue (true);
    Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueAndInterimFalseWithObject()
  {
    _businessObject.ReferenceValue = TypeWithReference.Create();
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Property = _propertyReferenceValue;
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadValue (false);
    Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
  }


  [Test]
  public void LoadValueAndInterimFalseWithObjectAndNoProperty()
  {
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadValue (false);
    Assert.AreEqual (_businessObject, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadValueAndInterimFalseWithNull()
  {
    _businessObject.ReferenceValue = null;
    _bocDropDownMenu.DataSource = _dataSource;
    _bocDropDownMenu.Property = _propertyReferenceValue;
    _bocDropDownMenu.Value = (IBusinessObject) TypeWithReference.Create();

    _bocDropDownMenu.LoadValue (false);
    Assert.AreEqual (_businessObject.ReferenceValue, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadUnboundValueAndInterimTrueWithObject()
  {
    IBusinessObject value = (IBusinessObject) TypeWithReference.Create();
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadUnboundValue (value, true);
    Assert.AreEqual (value, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadUnboundValueAndInterimTrueWithNull()
  {
    IBusinessObject value = null;
    _bocDropDownMenu.Value = (IBusinessObject) TypeWithReference.Create();

    _bocDropDownMenu.LoadUnboundValue (value, true);
    Assert.AreEqual (value, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithObject()
  {
    IBusinessObject value = (IBusinessObject) TypeWithReference.Create();
    _bocDropDownMenu.Value = null;

    _bocDropDownMenu.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocDropDownMenu.Value);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithNull()
  {
    IBusinessObject value = null;
    _bocDropDownMenu.Value = (IBusinessObject) TypeWithReference.Create();

    _bocDropDownMenu.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocDropDownMenu.Value);
  }
}

}
