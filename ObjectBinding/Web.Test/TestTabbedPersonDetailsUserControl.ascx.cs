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
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using Remotion.Collections;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace OBWTest
{
public class TestTabbedPersonDetailsUserControl : 
    DataEditUserControl, IControl, IFormGridRowProvider
{
  protected BocTextValue FirstNameField;
  protected BocTextValue LastNameField;
  protected FormGridManager FormGridManager;
  protected BindableObjectDataSourceControl CurrentObject;
  protected BocDateTimeValue DateOfBirthField;
  protected BocReferenceValue PartnerField;
  protected BocBooleanValue DeceasedField;
  protected BocDateTimeValue DateOfDeathField;
  protected BocEnumValue MarriageStatusField;
  protected HtmlTable FormGrid;
  private AutoInitHashtable _listOfFormGridRowInfos =
      new AutoInitHashtable (typeof (FormGridRowInfoCollection));
  private AutoInitHashtable _listOfHiddenRows = 
      new AutoInitHashtable (typeof (StringCollection));

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

  public virtual StringCollection GetHiddenRows (HtmlTable table)
  {
    return (StringCollection) _listOfHiddenRows[table];
  }

  public virtual FormGridRowInfoCollection GetAdditionalRows (HtmlTable table)
  {
    return (FormGridRowInfoCollection) _listOfFormGridRowInfos[table];
  }

	override protected void OnInit(EventArgs e)
	{
    StringCollection hiddenRows = (StringCollection)_listOfHiddenRows[FormGrid];
    FormGridRowInfoCollection newRows = (FormGridRowInfoCollection)_listOfFormGridRowInfos[FormGrid];

		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();
		base.OnInit(e);
    InitalizePartnerFieldMenuItems();
  }
	
  private void InitalizePartnerFieldMenuItems()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Open";
    menuItem.Text = "Open";
    menuItem.Category = "Object";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.WxeFunction;
    menuItem.Command.WxeFunctionCommand.Parameters = "objects";
    menuItem.Command.WxeFunctionCommand.TypeName = "OBWTest.ViewPersonsWxeFunction,OBWTest";
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Text = "Copy";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Cut";
    menuItem.Text = "Cut";
    menuItem.Category = "Edit";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Text = "Paste";
    menuItem.Category = "Edit";
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Delete";
    menuItem.Text = "Delete";
    menuItem.Category = "Edit";
    menuItem.Icon.Url = "Images/DeleteItem.gif";
    menuItem.DisabledIcon.Url = "Images/DeleteItemDisabled.gif";
    menuItem.RequiredSelection = RequiredSelection.OneOrMore;
    menuItem.Style = WebMenuItemStyle.Icon;
    menuItem.Command.Type = CommandType.Event;
    PartnerField.OptionsMenuItems.Add (menuItem);
  }

	#region Web Form Designer generated code

	/// <summary>
	///		Required method for Designer support - do not modify
	///		the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{

  }
	#endregion
	}
}
