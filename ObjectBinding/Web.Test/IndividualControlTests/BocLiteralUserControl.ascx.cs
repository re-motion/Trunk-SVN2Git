// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
