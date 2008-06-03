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
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Remotion.ObjectBinding.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
 
/// <summary> A collection of <see cref="PropertyPathBinding"/> objects. </summary>
[Editor (typeof (PropertyPathBindingCollectionEditor), typeof (UITypeEditor))]
public class PropertyPathBindingCollection : BusinessObjectControlItemCollection
{
  public PropertyPathBindingCollection (IBusinessObjectBoundWebControl ownerControl)
    : base (ownerControl, new Type[] {typeof (PropertyPathBinding)})
  {
  }

  public new PropertyPathBinding[] ToArray()
  {
    ArrayList arrayList = new ArrayList (List);
    return (PropertyPathBinding[]) arrayList.ToArray (typeof (PropertyPathBinding));
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new PropertyPathBinding this[int index]
  {
    get { return (PropertyPathBinding) List[index]; }
    set { List[index] = value; }
  }
}

}
