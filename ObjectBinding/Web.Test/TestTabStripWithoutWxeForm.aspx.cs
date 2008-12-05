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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using Remotion.Collections;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Reflection;
using Remotion.ObjectBinding.Web.Controls;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace OBWTest
{

public class TestTabStripWithoutWxeForm : Page, IWindowStateManager
{
  protected Remotion.Web.UI.Controls.HtmlHeadContents HtmlHeadContents;
  private IDataEditControl[] _dataEditControls;
  protected Remotion.Web.UI.Controls.WebTabStrip PagesTabStrip;
  protected Remotion.Web.UI.Controls.TabStripMenu NavigationTabs;
  protected Remotion.Web.UI.Controls.ValidationStateViewer ValidationStateViewer;
  protected Remotion.Web.UI.Controls.TabbedMultiView MultiView;
  private bool _currentObjectSaved = false;

	private void Page_Load(object sender, System.EventArgs e)
	{
    // add tabs 
    AddTab ("1", "Test Tab 1", null);
    AddTab ("2", "Test Tab 2 foo bar", null);
    AddTab ("3", "Test Tab 3 foo", null);
    AddTab ("4", "Test Tab 4 foo foo bar", null);
    AddTab ("5", "Test Tab 5", null);
    AddTab ("6", "Test Tab 6 foo", null);
    AddTab ("7", "Test Tab 7 foo foo bar", null);

//    AddMainMenuTab ("1", "Main Tab 1", null);
//    AddMainMenuTab ("2", "Main Tab 2 foo bar", null);
//    AddMainMenuTab ("3", "Main Tab 3 foo", null);
//    AddMainMenuTab ("4", "Main Tab 4 foo foo bar", null);
//    AddMainMenuTab ("5", "Main Tab 5", null);
//    AddMainMenuTab ("6", "Main Tab 6 foo", null);
//    AddMainMenuTab ("7", "Main Tab 7 foo foo bar", null);

    TypedArrayList dataEditControls = new TypedArrayList (typeof (IDataEditControl));
    // load editor pages
    IDataEditControl dataEditControl;
    dataEditControl = AddPage ("TestTabbedPersonDetailsUserControl", "Person Details", new IconInfo ("Images/OBRTest.Person.gif"), "TestTabbedPersonDetailsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add (dataEditControl);
    dataEditControl = AddPage ("TestTabbedPersonJobsUserControl", "Jobs", new IconInfo ("Images/OBRTest.Job.gif"), "TestTabbedPersonJobsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add (dataEditControl);
    _dataEditControls = (IDataEditControl[]) dataEditControls.ToArray();

    int activeViewIndex = 0;
    TabView activeView = (TabView) MultiView.Views[activeViewIndex];
    if (activeViewIndex > 0)
      MultiView.SetActiveView (activeView);
  }

  private void AddTab (string id, string text, IconInfo icon)
  {
    WebTab tab = new WebTab ();
    tab.Text = text;
    tab.ItemID = id ;
    tab.Icon = icon;
    PagesTabStrip.Tabs.Add (tab);
  }

  private void AddMainMenuTab (string id, string text, IconInfo icon)
  {  
    WebTab tab = new WebTab ();
    tab.Text = text;
    tab.ItemID = id ;
    tab.Icon = icon;
    NavigationTabs.Tabs.Add (tab);
  }

  private IDataEditControl AddPage (string id, string title, IconInfo icon, string path)
  {
    TabView view = new TabView();
    view.ID = id+ "_View";
    view.Title = title;
    view.Icon = icon;
    MultiView.Views.Add (view);

    UserControl control = (UserControl) this.LoadControl (path);
    control.ID = Remotion.Text.IdentifierGenerator.HtmlStyle.GetValidIdentifier (System.IO.Path.GetFileNameWithoutExtension (path));

    //EgoFormPageUserControl formPageControl = control as EgoFormPageUserControl;
    //if (formPageControl != null)
    //  formPageControl.FormPageObject = formPage;

    view.Controls.Add (control);

    IDataEditControl dataEditControl = control as IDataEditControl;
    if (dataEditControl != null)
    {
      return dataEditControl;
    }

    return null;
  }

	override protected void OnInit(EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();

    WebButton saveButton = new WebButton ();
    saveButton.ID = "SaveButton";
    saveButton.Text = "Save";
    saveButton.Style["margin-right"] = "10pt";
    saveButton.Click += new EventHandler(SaveButton_Click);
    MultiView.TopControls.Add (saveButton);

    WebButton cancelButton = new WebButton ();
    cancelButton.ID = "CancelButton";
    cancelButton.Text = "Cancel";
    cancelButton.Style["margin-right"] = "10pt";
    cancelButton.Click += new EventHandler(CancelButton_Click);
    MultiView.TopControls.Add (cancelButton);

    WebButton postBackButton = new WebButton();
    postBackButton.ID = "PostBackButton";
    postBackButton.Text = "Postback";
    postBackButton.Style["margin-right"] = "10pt";
    MultiView.BottomControls.Add (postBackButton);

    WebButton validateButton = new WebButton();
    validateButton.ID = "ValidateButton";
    validateButton.Text = "Validate";
    validateButton.Style["margin-right"] = "10pt";
    validateButton.Click += new EventHandler(ValidateButton_Click);
    MultiView.BottomControls.Add (validateButton);
		
    base.OnInit(e);
	}
	#region Web Form Designer generated code

	
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{    
    this.Unload += new System.EventHandler(this.Page_Unload);
    this.Load += new System.EventHandler(this.Page_Load);

  }
	#endregion

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);

    string key = GetType().FullName + "_Style";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (ResourceUrlResolver), ResourceType.Html, "Style.css");
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, url);
    }

    key = GetType().FullName + "_Global";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, "Html/global.css");
    }
  }

  private void Page_Unload (object sender, System.EventArgs e)
  {
    if (_currentObjectSaved)
      return;

    foreach (IDataEditControl control in _dataEditControls)
      control.DataSource.SaveValues (true);
  }

  private void CancelButton_Click(object sender, System.EventArgs e)
  {
  }

  private void SaveButton_Click(object sender, System.EventArgs e)
  {
    // validate all tabs
    foreach (IDataEditControl control in _dataEditControls)
    {
      bool isValid = control.Validate();
      if (! isValid)
        return;
    }

    // save all tabs
    foreach (IDataEditControl control in _dataEditControls)
      control.DataSource.SaveValues (false);
  }

  private void ValidateButton_Click(object sender, System.EventArgs e)
  {
    foreach (UserControl control in _dataEditControls)
    {
      Remotion.Web.UI.Controls.FormGridManager formGridManager = 
          control.FindControl("FormGridManager") as Remotion.Web.UI.Controls.FormGridManager;
      if (formGridManager != null)
        formGridManager.Validate();
    }
  }

  private void MultiView_ActiveViewChanged(object sender, System.EventArgs e)
  {
  
  }

  object IWindowStateManager.GetData (string key)
  {
    return this.Request.Params[key];
  }

  void IWindowStateManager.SetData (string key, object value)
  {
    ArgumentUtility.CheckType ("value", value, typeof (string));
    Page.RegisterHiddenField (key, (string) value);
  }

}

}

