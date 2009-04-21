// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
    public enum WaiConformanceLevel
    {
      Undefined = 0,
      A = 1,
      DoubleA = 3,
      TripleA = 7
    }

    protected BocEnumValue WaiConformanceLevelField;
    protected TabbedMenu TabbedMenu;
    
    public WaiConformanceLevel ConformanceLevel
    {
      get { return (WaiConformanceLevel) WebConfiguration.Current.Wcag.ConformanceLevel; }
      set { WebConfiguration.Current.Wcag.ConformanceLevel = (Remotion.Web.Configuration.WaiConformanceLevel) value; }
    }

    private void Page_Load (object sender, EventArgs e)
    {
      Type itemType = GetType();
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
      WaiConformanceLevelField.LoadUnboundValue (ConformanceLevel, IsPostBack);
    }

    private void WaiConformanceLevelField_SelectionChanged (object sender, EventArgs e)
    {
      ConformanceLevel = (WaiConformanceLevel) WaiConformanceLevelField.Value;
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
