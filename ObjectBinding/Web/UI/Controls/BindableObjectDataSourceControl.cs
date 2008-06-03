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
using System.Web.UI;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Design.BindableObject;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  [Designer (typeof (BindableObjectDataSourceDesigner))]
  public class BindableObjectDataSourceControl : BusinessObjectDataSourceControl
  {
    private readonly BindableObjectDataSource _dataSource = new BindableObjectDataSource();

    public BindableObjectDataSourceControl ()
    {
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Data")]
    [DefaultValue (null)]
    [Editor (typeof (BindableObjectTypePickerEditor), typeof (UITypeEditor))]
    [TypeConverter (typeof (TypeNameConverter))]
    public Type Type
    {
      get { return _dataSource.Type; }
      set { _dataSource.Type = value; }
    }

    protected override IBusinessObjectDataSource GetDataSource ()
    {
      return _dataSource;
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      _dataSource.Site = Site;
    }
  }
}
