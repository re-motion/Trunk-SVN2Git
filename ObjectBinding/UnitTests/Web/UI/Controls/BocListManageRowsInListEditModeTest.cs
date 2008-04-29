using System;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  [TestFixture]
  public class BocListManageRowsInListEditModeTest : BocTest
  {
    private BocListMock _bocList;

    private IBusinessObject[] _values;
    private IBusinessObject[] _newValues;

    private BindableObjectClass _typeWithStringClass;

    private IBusinessObjectPropertyPath _typeWithStringFirstValuePath;
    private IBusinessObjectPropertyPath _typeWithStringSecondValuePath;

    private BocSimpleColumnDefinition _typeWithStringFirstValueSimpleColumn;
    private BocSimpleColumnDefinition _typeWithStringSecondValueSimpleColumn;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _values = new IBusinessObject[5];
      _values[0] = (IBusinessObject) TypeWithString.Create ("0", "A");
      _values[1] = (IBusinessObject) TypeWithString.Create ("1", "A");
      _values[2] = (IBusinessObject) TypeWithString.Create ("2", "B");
      _values[3] = (IBusinessObject) TypeWithString.Create ("3", "B");
      _values[4] = (IBusinessObject) TypeWithString.Create ("4", "C");

      _newValues = new IBusinessObject[2];
      _newValues[0] = (IBusinessObject) TypeWithString.Create ("5", "C");
      _newValues[1] = (IBusinessObject) TypeWithString.Create ("6", "D");

      _typeWithStringClass = BindableObjectProvider.Current.GetBindableObjectClass (typeof (TypeWithString));

      _typeWithStringFirstValuePath = BusinessObjectPropertyPath.Parse (_typeWithStringClass, "FirstValue");
      _typeWithStringSecondValuePath = BusinessObjectPropertyPath.Parse (_typeWithStringClass, "SecondValue");

      _typeWithStringFirstValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithStringFirstValueSimpleColumn.SetPropertyPath (_typeWithStringFirstValuePath);

      _typeWithStringSecondValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithStringSecondValueSimpleColumn.SetPropertyPath (_typeWithStringSecondValuePath);

      _bocList = new BocListMock();
      _bocList.ID = "BocList";
      NamingContainer.Controls.Add (_bocList);

      _bocList.FixedColumns.Add (_typeWithStringFirstValueSimpleColumn);
      _bocList.FixedColumns.Add (_typeWithStringSecondValueSimpleColumn);

      _bocList.LoadUnboundValue (_values, false);
      _bocList.SwitchListIntoEditMode();

      Assert.IsTrue (_bocList.IsListEditModeActive);
    }

    [Test]
    public void AddRow ()
    {
      int index = _bocList.AddRow (_newValues[0]);

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (6, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[2], _bocList.Value[2]);
      Assert.AreSame (_values[3], _bocList.Value[3]);
      Assert.AreSame (_values[4], _bocList.Value[4]);

      Assert.AreEqual (5, index);
      Assert.AreSame (_newValues[0], _bocList.Value[5]);

      Assert.IsTrue (_bocList.IsListEditModeActive);
    }

    [Test]
    public void AddRows ()
    {
      _bocList.AddRows (_newValues);

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (7, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[2], _bocList.Value[2]);
      Assert.AreSame (_values[3], _bocList.Value[3]);
      Assert.AreSame (_values[4], _bocList.Value[4]);

      Assert.AreSame (_newValues[0], _bocList.Value[5]);
      Assert.AreSame (_newValues[1], _bocList.Value[6]);

      Assert.IsTrue (_bocList.IsListEditModeActive);
    }

    [Test]
    public void RemoveRowWithIndex ()
    {
      _bocList.RemoveRow (2);

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (4, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[3], _bocList.Value[2]);
      Assert.AreSame (_values[4], _bocList.Value[3]);

      Assert.IsTrue (_bocList.IsListEditModeActive);
    }

    [Test]
    public void RemoveRowWithBusinessObject ()
    {
      _bocList.RemoveRow (_values[2]);

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (4, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[3], _bocList.Value[2]);
      Assert.AreSame (_values[4], _bocList.Value[3]);

      Assert.IsTrue (_bocList.IsListEditModeActive);
    }

    [Test]
    public void RemoveRowsWithNoRows ()
    {
      _bocList.RemoveRows (new IBusinessObject[0]);

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (5, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[2], _bocList.Value[2]);
      Assert.AreSame (_values[3], _bocList.Value[3]);
      Assert.AreSame (_values[4], _bocList.Value[4]);

      Assert.IsTrue (_bocList.IsListEditModeActive);
    }

    [Test]
    public void RemoveRowsWithSingleRow ()
    {
      _bocList.RemoveRows (new IBusinessObject[] {_values[2]});

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (4, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[3], _bocList.Value[2]);
      Assert.AreSame (_values[4], _bocList.Value[3]);

      Assert.IsTrue (_bocList.IsListEditModeActive);
    }

    [Test]
    public void RemoveRowsWithMultipleRows ()
    {
      _bocList.RemoveRows (new IBusinessObject[] {_values[1], _values[3]});

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (3, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[2], _bocList.Value[1]);
      Assert.AreSame (_values[4], _bocList.Value[2]);

      Assert.IsTrue (_bocList.IsListEditModeActive);
    }
  }
}