// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

[TestFixture]
public class BocListWcagTest: BocTest
{
  private BocListMock _bocList;

  [SetUp]
  public override void SetUp()
  {
    base.SetUp();
    _bocList = new BocListMock();
    _bocList.ID = "BocList";
    _bocList.ShowOptionsMenu = false;
    _bocList.ShowListMenu = false;
    _bocList.ShowAvailableViewsList = false;
    _bocList.PageSize = null;
    _bocList.EnableSorting = false;
    _bocList.RowMenuDisplay = RowMenuDisplay.Disabled;
    _bocList.Selection = RowSelection.Disabled;
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _bocList.PageSize = 1;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.PageSize = 1;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithShowOptionsMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.ShowOptionsMenu = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocList, WcagHelperMock.Control);
    Assert.AreEqual ("ShowOptionsMenu", WcagHelperMock.Property);
  }

  [Test]
  public void IsOptionsMenuInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowOptionsMenu = true;
    _bocList.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsFalse (_bocList.HasOptionsMenu);
  }

  [Test]
  public void IsOptionsMenuVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowOptionsMenu = true;
    _bocList.OptionsMenuItems.Add (new WebMenuItem());
    Assert.IsTrue (_bocList.HasOptionsMenu);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithShowListMenuTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.ShowListMenu = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocList, WcagHelperMock.Control);
    Assert.AreEqual ("ShowListMenu", WcagHelperMock.Property);
  }

  [Test]
  public void IsListMenuInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowListMenu = true;
    _bocList.ListMenuItems.Add (new WebMenuItem());
    Assert.IsFalse (_bocList.HasListMenu);
  }

  [Test]
  public void IsListMenuVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowListMenu = true;
    _bocList.ListMenuItems.Add (new WebMenuItem());
    Assert.IsTrue (_bocList.HasListMenu);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithShowAvailableViewsListTrue()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.ShowAvailableViewsList = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocList, WcagHelperMock.Control);
    Assert.AreEqual ("ShowAvailableViewsList", WcagHelperMock.Property);
  }

  [Test]
  public void IsAvailableViewsListInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.ShowAvailableViewsList = true;
    _bocList.AvailableViews.Add (new BocListView ());
    Assert.IsFalse (_bocList.HasAvailableViewsList);
  }

  [Test]
  public void IsAvailableViewsListVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.ShowAvailableViewsList = true;
    _bocList.AvailableViews.Add (new BocListView ());
    Assert.IsTrue (_bocList.HasAvailableViewsList);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithPageSizeNotNull()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.PageSize = 1;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocList, WcagHelperMock.Control);
    Assert.AreEqual ("PageSize", WcagHelperMock.Property);
  }

  [Test]
  public void IsPagingDisabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.PageSize = 1;
    Assert.IsFalse (_bocList.IsPagingEnabled);
  }

  [Test]
  public void IsPagingEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _bocList.PageSize = 1;
    Assert.IsTrue (_bocList.IsPagingEnabled);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithClientSideSortingEnabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.EnableSorting = true;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

    Assert.IsTrue (WcagHelperMock.HasWarning);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocList, WcagHelperMock.Control);
    Assert.AreEqual ("EnableSorting", WcagHelperMock.Property);
  }

  [Test]
  public void IsClientSideSortingEnabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _bocList.EnableSorting = true;
    Assert.IsTrue (_bocList.IsClientSideSortingEnabled);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithRowMenuDisplayAutomatic()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _bocList.RowMenuDisplay = RowMenuDisplay.Automatic;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocList, WcagHelperMock.Control);
    Assert.AreEqual ("RowMenuDisplay", WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithDropDownMenuColumn()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocDropDownMenuColumnDefinition dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {dropDownMenuColumn});

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocList, WcagHelperMock.Control);
    Assert.AreEqual ("Columns[0]", WcagHelperMock.Property);
  }

  [Test]
  public void IsDropDownMenuColumnInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    BocDropDownMenuColumnDefinition dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
    Assert.IsFalse (_bocList.IsColumnVisible (dropDownMenuColumn));
  }

  [Test]
  public void IsDropDownMenuColumnVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    BocDropDownMenuColumnDefinition dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
    Assert.IsTrue (_bocList.IsColumnVisible (dropDownMenuColumn));
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithRowEditModeColumn()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocRowEditModeColumnDefinition rowEditModeColumn = new BocRowEditModeColumnDefinition();
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {rowEditModeColumn});

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocList, WcagHelperMock.Control);
    Assert.AreEqual ("Columns[0]", WcagHelperMock.Property);
  }

  [Test]
  public void IsRowEditModeColumnInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    BocRowEditModeColumnDefinition rowEditModeColumn = new BocRowEditModeColumnDefinition();
    Assert.IsFalse (_bocList.IsColumnVisible (rowEditModeColumn));
  }

  [Test]
  public void IsRowEditModeColumnVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    BocRowEditModeColumnDefinition rowEditModeColumn = new BocRowEditModeColumnDefinition();
    Assert.IsTrue (_bocList.IsColumnVisible (rowEditModeColumn));
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnSetToEvent()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.Event;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocList, WcagHelperMock.Control);
    Assert.AreEqual ("Columns[0].Command", WcagHelperMock.Property);
  }

  [Test]
  public void IsCommandColumnSetToEventInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.Event;
    Assert.IsFalse (_bocList.IsColumnVisible (commandColumn));
  }

  [Test]
  public void IsCommandColumnSetToEventVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.Event;
    Assert.IsTrue (_bocList.IsColumnVisible (commandColumn));
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnSetToWxeFunction()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.WxeFunction;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (1, WcagHelperMock.Priority);
    Assert.AreSame (_bocList, WcagHelperMock.Control);
    Assert.AreEqual ("Columns[0].Command", WcagHelperMock.Property);
  }

  [Test]
  public void IsCommandColumnSetToWxeFunctionInvisibleWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.WxeFunction;
    Assert.IsFalse (_bocList.IsColumnVisible (commandColumn));
  }

  [Test]
  public void IsCommandColumnSetToWxeFunctionVisibleWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.WxeFunction;
    Assert.IsTrue (_bocList.IsColumnVisible (commandColumn));
  }

	
  [Test]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnSetToHref()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command.Type = CommandType.Href;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithCommandColumnWithoutCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    BocCommandColumnDefinition commandColumn = new BocCommandColumnDefinition();
    commandColumn.Command = null;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[1] {commandColumn});
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionEnabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
    _bocList.Selection = RowSelection.SingleRadioButton;
    _bocList.Index = RowIndex.Disabled;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);

    Assert.IsTrue (WcagHelperMock.HasError);
    Assert.AreEqual (2, WcagHelperMock.Priority);
    Assert.AreSame (_bocList, WcagHelperMock.Control);
    Assert.AreEqual ("Selection", WcagHelperMock.Property);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionDisabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
    _bocList.Selection = RowSelection.Disabled;
    _bocList.Index = RowIndex.Disabled;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelDoubleAWithSelectionAndIndexEnabled()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelDoubleA();
    _bocList.Selection = RowSelection.SingleRadioButton;
    _bocList.Index = RowIndex.InitialOrder;
    _bocList.EvaluateWaiConformity (new BocColumnDefinition[0]);
    
    Assert.IsFalse (WcagHelperMock.HasWarning);
    Assert.IsFalse (WcagHelperMock.HasError);
  }

}

}
