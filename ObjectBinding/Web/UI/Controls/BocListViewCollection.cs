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
using System.Drawing.Design;
using Remotion.ObjectBinding.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

/// <summary> A collection of <see cref="BocListView"/> objects. </summary>
[Editor (typeof (BocListViewCollectionEditor), typeof (UITypeEditor))]
public class BocListViewCollection: BusinessObjectControlItemCollection
{
  public BocListViewCollection (IBusinessObjectBoundWebControl ownerControl)
    : base (ownerControl, new Type[] {typeof (BocListView)})
  {
  }

  public new BocListView[] ToArray()
  {
    return (BocListView[]) InnerList.ToArray (typeof (BocListView));
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new BocListView this[int index]
  {
    get { return (BocListView) List[index]; }
    set { List[index] = value; }
  }
}

}
