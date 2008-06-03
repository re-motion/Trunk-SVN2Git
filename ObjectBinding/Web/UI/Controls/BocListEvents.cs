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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
public delegate void BocListSortingOrderChangeEventHandler (object sender, BocListSortingOrderChangeEventArgs e);

public class BocListSortingOrderChangeEventArgs: EventArgs
{
  private BocListSortingOrderEntry[] _oldSortingOrder;
  private BocListSortingOrderEntry[] _newSortingOrder;

  public BocListSortingOrderChangeEventArgs (
      BocListSortingOrderEntry[] oldSortingOrder, BocListSortingOrderEntry[] newSortingOrder)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("oldSortingOrder", oldSortingOrder);
    ArgumentUtility.CheckNotNullOrItemsNull ("newSortingOrder", newSortingOrder);

    _oldSortingOrder = oldSortingOrder;
    _newSortingOrder = newSortingOrder;
  }

  /// <summary> Gets the old sorting order of the <see cref="BocList"/>. </summary>
  public BocListSortingOrderEntry[] OldSortingOrder
  {
    get { return _oldSortingOrder; }
  }

  /// <summary> Gets the new sorting order of the <see cref="BocList"/>. </summary>
  public BocListSortingOrderEntry[] NewSortingOrder
  {
    get { return _newSortingOrder; }
  }
}

public delegate void BocListItemEventHandler (object sender, BocListItemEventArgs e);

public class BocListItemEventArgs: EventArgs
{
  private int _listIndex;
  private IBusinessObject _businessObject;

  public BocListItemEventArgs (
      int listIndex, 
      IBusinessObject businessObject)
  {
    _listIndex = listIndex;
    _businessObject = businessObject;
  }

  /// <summary> An index that identifies the <see cref="IBusinessObject"/> that has been edited. </summary>
  public int ListIndex
  {
    get { return _listIndex; }
  }

  /// <summary>
  ///   The <see cref="IBusinessObject"/> that has been edited.
  /// </summary>
  public IBusinessObject BusinessObject
  {
    get { return _businessObject; }
  }
}

public delegate void BocListEditableRowChangesEventHandler (object sender, BocListEditableRowChangesEventArgs e);

public class BocListEditableRowChangesEventArgs : BocListItemEventArgs
{
  private IBusinessObjectBoundEditableControl[] _controls;
  private IBusinessObjectDataSource _dataSource;

  public BocListEditableRowChangesEventArgs (
      int listIndex, 
      IBusinessObject businessObject,
      IBusinessObjectDataSource dataSource,
      IBusinessObjectBoundEditableWebControl[] controls)
    : base (listIndex, businessObject)
  {
    _dataSource = dataSource;
    _controls = controls;
  }

  public IBusinessObjectDataSource DataSource
  {
    get { return _dataSource; }
  }

  public IBusinessObjectBoundEditableControl[] Controls
  {
    get { return _controls; }
  }
}

public delegate void BocListDataRowRenderEventHandler (object sender, BocListDataRowRenderEventArgs e);

public class BocListDataRowRenderEventArgs: BocListItemEventArgs
{
  private bool _isEditableRow = true;

  public BocListDataRowRenderEventArgs (int listIndex, IBusinessObject businessObject)
    : base (listIndex, businessObject)
  {
  }

  public bool IsEditableRow
  {
    get { return _isEditableRow; }
    set { _isEditableRow = value; }
  }
}

}
