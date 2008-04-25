using System;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.Web.UnitTests.Configuration;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

[TestFixture]
public class BocEnumValueTest: BocTest
{
  private BocEnumValueMock _bocEnumValue;
  private TypeWithEnum _businessObject;
  private BusinessObjectReferenceDataSource _dataSource;
  private IBusinessObjectEnumerationProperty _propertyEnumValue;

  public BocEnumValueTest()
  {
  }

  
  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocEnumValue = new BocEnumValueMock();
    _bocEnumValue.ID = "BocEnumValue";
    NamingContainer.Controls.Add (_bocEnumValue);

    _businessObject = TypeWithEnum.Create();

    _propertyEnumValue = (IBusinessObjectEnumerationProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("EnumValue");
    
    _dataSource = new BusinessObjectReferenceDataSource();
    _dataSource.BusinessObject = (IBusinessObject) _businessObject;
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocEnumValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocEnumValue.ListControlStyle.AutoPostBack = true;
    _bocEnumValue.EvaluateWaiConformity ();
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithListControlStyleAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocEnumValue.ListControlStyle.AutoPostBack = true;
    _bocEnumValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocEnumValue, WcagHelperMock.Control);
    Assert.AreEqual ("ListControlStyle.AutoPostBack", WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithListControlAutoPostBackTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocEnumValue.ListControl.AutoPostBack = true;
    _bocEnumValue.EvaluateWaiConformity ();

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocEnumValue, WcagHelperMock.Control);
    Assert.AreEqual ("ListControl.AutoPostBack", WcagHelperMock.Property);
  }


  [Test]
  public void GetTrackedClientIDsInReadOnlyMode()
  {
    _bocEnumValue.ReadOnly = true;
    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (0, actual.Length);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAsDropDownList()
  {
    _bocEnumValue.ReadOnly = false;
    _bocEnumValue.ListControlStyle.ControlType = ListControlType.DropDownList;
    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocEnumValue.ListControl.ClientID, actual[0]);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAsListBox()
  {
    _bocEnumValue.ReadOnly = false;
    _bocEnumValue.ListControlStyle.ControlType = ListControlType.ListBox;
    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (1, actual.Length);
    Assert.AreEqual (_bocEnumValue.ListControl.ClientID, actual[0]);
  }

  [Test]
  public void GetTrackedClientIDsInEditModeAsRadioButtonList()
  {
    _bocEnumValue.ReadOnly = false;
    _bocEnumValue.ListControlStyle.ControlType = ListControlType.RadioButtonList;
    Assert.IsNotNull (_propertyEnumValue, "Could not find property 'EnumValue'.");
    Assert.IsTrue (
        typeof (IBusinessObjectEnumerationProperty).IsAssignableFrom (_propertyEnumValue.GetType()), 
        "Property 'EnumValue' of invalid type.");
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.RefreshEnumList();

    string[] actual = _bocEnumValue.GetTrackedClientIDs();
    Assert.IsNotNull (actual);
    Assert.AreEqual (3, actual.Length);
    Assert.AreEqual (_bocEnumValue.ListControl.ClientID + "_0", actual[0]);
    Assert.AreEqual (_bocEnumValue.ListControl.ClientID + "_1", actual[1]);
    Assert.AreEqual (_bocEnumValue.ListControl.ClientID + "_2", actual[2]);
  }


  [Test]
  public void SetValueToEnum()
  {
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.IsDirty = false;
    _bocEnumValue.Value = TestEnum.Second;
    Assert.AreEqual (TestEnum.Second, _bocEnumValue.Value);
    Assert.IsTrue (_bocEnumValue.IsDirty);
  }
    
  [Test]
  public void SetValueToNull()
  {
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.IsDirty = false;
    _bocEnumValue.Value = null;
    Assert.AreEqual (null, _bocEnumValue.Value);
    Assert.IsTrue (_bocEnumValue.IsDirty);
  }
    

  [Test]
  public void LoadValueAndInterimTrue()
  {
    _businessObject.EnumValue = TestEnum.Second;
    _bocEnumValue.DataSource = _dataSource;
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = null;
    _bocEnumValue.IsDirty = true;

    _bocEnumValue.LoadValue (true);
    Assert.AreEqual (null, _bocEnumValue.Value);
    Assert.IsTrue (_bocEnumValue.IsDirty);
  }

  [Test]
  public void LoadValueAndInterimFalseWithEnum()
  {
    _businessObject.EnumValue = TestEnum.Second;
    _bocEnumValue.DataSource = _dataSource;
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = null;
    _bocEnumValue.IsDirty = true;

    _bocEnumValue.LoadValue (false);
    Assert.AreEqual (_businessObject.EnumValue, _bocEnumValue.Value);
    Assert.IsFalse (_bocEnumValue.IsDirty);
  }


  [Test]
  public void LoadUnboundValueAndInterimTrue()
  {
    TestEnum value = TestEnum.Second;
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = null;
    _bocEnumValue.IsDirty = true;

    _bocEnumValue.LoadUnboundValue (value, true);
    Assert.AreEqual (null, _bocEnumValue.Value);
    Assert.IsTrue (_bocEnumValue.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithEnum()
  {
    TestEnum value = TestEnum.Second;
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = null;
    _bocEnumValue.IsDirty = true;

    _bocEnumValue.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocEnumValue.Value);
    Assert.IsFalse (_bocEnumValue.IsDirty);
  }

  [Test]
  public void LoadUnboundValueAndInterimFalseWithNull()
  {
    TestEnum? value = null;
    _bocEnumValue.Property = _propertyEnumValue;
    _bocEnumValue.Value = TestEnum.Second;
    _bocEnumValue.IsDirty = true;

    _bocEnumValue.LoadUnboundValue (value, false);
    Assert.AreEqual (value, _bocEnumValue.Value);
    Assert.IsFalse (_bocEnumValue.IsDirty);
  }
}

}
