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
using System.ComponentModel.Design;
using System.Drawing.Design;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Design
{
public class BocListDesigner: WebControlDesigner, IServiceProvider
{
  private DesignerVerbCollection _verbs = null;

	public BocListDesigner()
	{
    _verbs = new DesignerVerbCollection();
    _verbs.Add (new DesignerVerb ("Edit Fixed Columns", new EventHandler(OnVerbEditFixedColumns)));
  }

  private void OnVerbEditFixedColumns (object sender, EventArgs e) 
  {
    BocList bocList = Component as BocList;
    if (bocList == null)
      throw new InvalidOperationException ("Cannot use BocListDesigner for objects other than BocList.");

    PropertyDescriptorCollection propertyDescriptors = TypeDescriptor.GetProperties (bocList);
    PropertyDescriptor propertyDescriptor = propertyDescriptors["FixedColumns"];

    TypeDescriptorContext context = new TypeDescriptorContext (this, this, propertyDescriptor);
    object value = propertyDescriptor.GetValue (Component);
    BocColumnDefinitionCollectionEditor editor = null;
    editor = (BocColumnDefinitionCollectionEditor) TypeDescriptor.GetEditor (value, typeof(UITypeEditor));
    editor.EditValue (context, this, value);
  }

  public override DesignerVerbCollection Verbs 
  {
    get { return _verbs; }
  }

  protected override object GetService (Type serviceType)
  {
    return base.GetService (serviceType);
  }

  object IServiceProvider.GetService (Type serviceType)
  {
    return GetService (serviceType);
  }
}

}
