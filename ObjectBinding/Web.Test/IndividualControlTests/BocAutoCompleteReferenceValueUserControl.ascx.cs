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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI;
using Remotion.Web.UI.Globalization;

namespace OBWTest.IndividualControlTests
{

  [WebMultiLingualResources ("OBWTest.Globalization.IndividualControlTests.BocReferenceValueUserControl")]
  public partial class BocAutoCompleteReferenceValueUserControl : BaseUserControl
  {

    protected override void RegisterEventHandlers ()
    {
      base.RegisterEventHandlers ();
      
      PartnerField.SelectionChanged += new EventHandler (PartnerField_SelectionChanged);
      PartnerTestSetNullButton.Click += new EventHandler (PartnerTestSetNullButton_Click);
      PartnerTestSetNewItemButton.Click += new EventHandler (PartnerTestSetNewItemButton_Click);
      ReadOnlyPartnerTestSetNullButton.Click += new EventHandler (ReadOnlyPartnerTestSetNullButton_Click);
      ReadOnlyPartnerTestSetNewItemButton.Click += new EventHandler (ReadOnlyPartnerTestSetNewItemButton_Click);
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    override protected void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      Person person = (Person) CurrentObject.BusinessObject;

      UnboundPartnerField.Property = (IBusinessObjectReferenceProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition("Partner");
      //UnboundPartnerField.LoadUnboundValue (person.Partner, IsPostBack);
      UnboundReadOnlyPartnerField.Property = (IBusinessObjectReferenceProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Partner");
      UnboundReadOnlyPartnerField.LoadUnboundValue ((IBusinessObjectWithIdentity) person.Partner, IsPostBack);
      DisabledUnboundPartnerField.Property = (IBusinessObjectReferenceProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Partner");
      DisabledUnboundPartnerField.LoadUnboundValue ((IBusinessObjectWithIdentity) person.Partner, IsPostBack);
      DisabledUnboundReadOnlyPartnerField.Property = (IBusinessObjectReferenceProperty) CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Partner");
      DisabledUnboundReadOnlyPartnerField.LoadUnboundValue ((IBusinessObjectWithIdentity) person.Partner, IsPostBack);

      if (!IsPostBack)
      {
        if (Page is ISmartNavigablePage)
          ((ISmartNavigablePage) Page).SetFocus (PartnerField);
      }
    }

    override protected void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      SetDebugLabel (PartnerField, PartnerFieldValueLabel);
      SetDebugLabel (ReadOnlyPartnerField, ReadOnlyPartnerFieldValueLabel);
      SetDebugLabel (UnboundPartnerField, UnboundPartnerFieldValueLabel);
      SetDebugLabel (UnboundReadOnlyPartnerField, UnboundReadOnlyPartnerFieldValueLabel);
      SetDebugLabel (DisabledPartnerField, DisabledPartnerFieldValueLabel);
      SetDebugLabel (DisabledReadOnlyPartnerField, DisabledReadOnlyPartnerFieldValueLabel);
      SetDebugLabel (DisabledUnboundPartnerField, DisabledUnboundPartnerFieldValueLabel);
      SetDebugLabel (DisabledUnboundReadOnlyPartnerField, DisabledUnboundReadOnlyPartnerFieldValueLabel);
    }

    private void SetDebugLabel (IBusinessObjectBoundWebControl control, Label label)
    {
      if (control.Value != null)
        label.Text = control.Value.ToString ();
      else
        label.Text = "not set";
    }

    public override bool Validate ()
    {
      bool isValid = true;

      isValid &= base.Validate ();
      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    private void PartnerTestSetNullButton_Click (object sender, EventArgs e)
    {
      PartnerField.Value = null;
    }

    private void PartnerTestSetNewItemButton_Click (object sender, EventArgs e)
    {
      Person person = Person.CreateObject ();
      person.LastName = person.ID.ToByteArray ()[15].ToString ();
      person.FirstName = "--";

      PartnerField.Value = (IBusinessObjectWithIdentity) person;
    }

    private void ReadOnlyPartnerTestSetNullButton_Click (object sender, EventArgs e)
    {
      ReadOnlyPartnerField.Value = null;
    }

    private void ReadOnlyPartnerTestSetNewItemButton_Click (object sender, EventArgs e)
    {
      Person person = Person.CreateObject ();
      person.LastName = person.ID.ToByteArray ()[15].ToString ();
      person.FirstName = "--";

      ReadOnlyPartnerField.Value = (IBusinessObjectWithIdentity) person;
    }

    private void PartnerField_SelectionChanged (object sender, EventArgs e)
    {
      if (PartnerField.Value != null)
        PartnerFieldSelectionChangedLabel.Text = PartnerField.Value.ToString ();
      else
        PartnerFieldSelectionChangedLabel.Text = "not set";
    }
  }
}
