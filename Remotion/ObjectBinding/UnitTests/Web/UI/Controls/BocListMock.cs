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
using System.ComponentModel;
using System.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocList"/> type. </summary>
[ToolboxItem (false)]
public class BocListMock: BocList
{
	public new void EvaluateWaiConformity (BocColumnDefinition[] columns)
  {
    base.EvaluateWaiConformity (columns);
  }

  public new bool HasOptionsMenu
  {
    get { return base.HasOptionsMenu; }
  }

  public new bool HasListMenu
  {
    get { return base.HasListMenu; }
  }

  public new bool HasAvailableViewsList
  {
    get { return base.HasAvailableViewsList; }
  }

  public new bool IsSelectionEnabled
  {
    get { return base.IsSelectionEnabled; }
  }

  public new bool IsPagingEnabled
  {
    get { return base.IsPagingEnabled; }
  }

  public new bool IsColumnVisible (BocColumnDefinition column)
  {
    return base.IsColumnVisible (column);
  }


  public new bool IsClientSideSortingEnabled
  {
    get { return base.IsClientSideSortingEnabled; }
  }

  protected override bool IsDesignMode
  {
    get
    {
      return IsDesignModeOverrideValue.HasValue ? IsDesignModeOverrideValue.Value : base.IsDesignMode;
    }
  }

  protected override bool IsInternetExplorer55OrHigher ()
  {
    return true;
  }

  protected override WebMenuItem[] InitializeRowMenuItems (IBusinessObject businessObject, int listIndex)
  {

    return new[]
           {
               new WebMenuItem (
                   "item0",
                   null,
                   "WebMenuItem1",
                   new IconInfo ("~/Images/RowMenuItem.gif", 16, 16),
                   new IconInfo ("~/Images/RowMenuItemDisabled.gif", 16, 16),
                   WebMenuItemStyle.Text,
                   RequiredSelection.Any,
                   false,
                   null)
           };
  }

  public bool? IsDesignModeOverrideValue { get; set; }

  public void OnLoad ()
  {
    base.OnLoad (EventArgs.Empty);
  }

  public void OnPreRender ()
  {
    base.OnPreRender (EventArgs.Empty);
  }
}

}
