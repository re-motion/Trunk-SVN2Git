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
using System.Reflection;
using System.Web.UI;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.Configuration;
using Remotion.Web.UI.Controls;

namespace OBWTest.UI
{
  /// <summary>
  ///		Summary description for NavigationTabs.
  /// </summary>
  public class NavigationTabs : UserControl
  {
    protected BocEnumValue WaiConformanceLevelField;
    protected TabbedMenu TabbedMenu;

    private void Page_Load (object sender, EventArgs e)
    {
      Type itemType = typeof (WcagConfiguration);
      PropertyInfo propertyInfo = itemType.GetProperty ("ConformanceLevel");
      EnumerationProperty property = new EnumerationProperty (
          new PropertyBase.Parameters (
              (BindableObjectProvider) BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute>(),
              new PropertyInfoAdapter (propertyInfo),
              propertyInfo.PropertyType,
              null,
              false,
              false));

      WaiConformanceLevelField.Property = property;
      WaiConformanceLevelField.LoadUnboundValue (WebConfiguration.Current.Wcag.ConformanceLevel, IsPostBack);
    }

    private void WaiConformanceLevelField_SelectionChanged (object sender, EventArgs e)
    {
      WebConfiguration.Current.Wcag.ConformanceLevel = (WaiConformanceLevel) WaiConformanceLevelField.Value;
      WaiConformanceLevelField.IsDirty = false;
    }

    #region Web Form Designer generated code

    protected override void OnInit (EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit (e);
    }

    /// <summary>
    ///		Required method for Designer support - do not modify
    ///		the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.WaiConformanceLevelField.SelectionChanged += new System.EventHandler (this.WaiConformanceLevelField_SelectionChanged);
      this.Load += new System.EventHandler (this.Page_Load);
    }

    #endregion
  }
}
