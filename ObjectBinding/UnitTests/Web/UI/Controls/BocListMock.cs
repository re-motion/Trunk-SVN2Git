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
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;

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
  
}

}
