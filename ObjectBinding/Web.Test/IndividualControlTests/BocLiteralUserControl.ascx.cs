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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace OBWTest.IndividualControlTests
{

  public partial class BocLiteralUserControl : BaseUserControl
  {
    protected override void RegisterEventHandlers ()
    {
      base.RegisterEventHandlers ();

      this.CVTestSetNullButton.Click += new EventHandler (this.CVTestSetNullButton_Click);
      this.CVTestSetNewValueButton.Click += new EventHandler (this.CVTestSetNewValueButton_Click);
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    override protected void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      Person person = (Person) CurrentObject.BusinessObject;

      UnboundCVField.Property = (IBusinessObjectStringProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("CVString");
      UnboundCVField.LoadUnboundValue (person.CVString, IsPostBack);
    }

    override protected void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
    }

    private void CVTestSetNullButton_Click (object sender, EventArgs e)
    {
      CVField.Value = null;
    }

    private void CVTestSetNewValueButton_Click (object sender, EventArgs e)
    {
      CVField.Value = "Foo<br/>Bar";
    }
  }

}
