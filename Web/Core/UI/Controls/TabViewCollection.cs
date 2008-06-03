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
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{

public class TabViewCollection: ViewCollection
{
  public TabViewCollection (Control owner)
    : this ((TabbedMultiView.MultiView) owner)
  {
  }

  internal TabViewCollection (TabbedMultiView.MultiView owner)
    : base (owner)
  {
  }

  public override void Add (Control control)
  {
    TabView view = ArgumentUtility.CheckNotNullAndType<TabView> ("control", control);
    base.Add (view);
    Owner.OnTabViewInserted (view);
  }

  public override void AddAt(int index, Control control)
  {
    TabView view = ArgumentUtility.CheckNotNullAndType<TabView> ("control", control);
    base.AddAt (index, view);
    Owner.OnTabViewInserted (view);
  }

  public override void Remove (Control control)
  {
    TabView view = ArgumentUtility.CheckNotNullAndType<TabView> ("control", control);
    Owner.OnTabViewRemove (view);
    base.Remove (control);
    Owner.OnTabViewRemoved (view);
  }

  public override void RemoveAt (int index)
  {
    if (index < 0 || index > this.Count)
      throw new ArgumentOutOfRangeException ("index");
    TabView view = (TabView) this[index];
    Owner.OnTabViewRemove (view);
    base.RemoveAt (index);
    Owner.OnTabViewRemoved (view);
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new TabView this[int index]
  {
    get { return (TabView) base[index]; }
  }

  private new TabbedMultiView.MultiView Owner
  {
    get { return (TabbedMultiView.MultiView) base.Owner; }
  }
}

}
