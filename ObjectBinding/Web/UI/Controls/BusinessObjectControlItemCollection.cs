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
using System.Web.UI;
using Remotion.Globalization;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
 
/// <summary> A collection of <see cref="BusinessObjectControlItem"/> objects. </summary>
public abstract class BusinessObjectControlItemCollection : ControlItemCollection
{
  public BusinessObjectControlItemCollection (IBusinessObjectBoundWebControl ownerControl, Type[] supportedTypes)
    : base ((Control) ownerControl, supportedTypes)
  {
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public new IBusinessObjectBoundWebControl OwnerControl
  {
    get { return (IBusinessObjectBoundWebControl) base.OwnerControl; }
    set { base.OwnerControl = (Control) value; }
  }
}

/// <summary>
///   Base class for non-UI items of business object controls. 
/// </summary>
/// <remarks>
///   Derived classes: <see cref="BocColumnDefinition"/>, <see cref="BocListView"/>, <see cref="PropertyPathBinding"/>.
/// </remarks>
public abstract class BusinessObjectControlItem: IControlItem
{
  private IBusinessObjectBoundWebControl _ownerControl;

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  /// <summary> Gets or sets the <see cref="IBusinessObjectBoundWebControl"/> to which this item belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public IBusinessObjectBoundWebControl OwnerControl
  {
    get { return _ownerControl; }
    set
    {
      if (_ownerControl != value)
      {
        _ownerControl = value;
        OnOwnerControlChanged();
      }
    }
  }

  Control IControlItem.OwnerControl
  {
    get { return (Control) _ownerControl; }
    set { OwnerControl = (IBusinessObjectBoundWebControl) value; }
  }

  /// <summary> Not supported by base implementation. </summary>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public virtual string ItemID
  {
    get { return null; }
    set { throw new NotSupportedException ("Implement ItemID in a specialized class, if the class supports IDs."); }
  }

  public virtual void LoadResources (IResourceManager resourceManager)
  {
  }
}

}
