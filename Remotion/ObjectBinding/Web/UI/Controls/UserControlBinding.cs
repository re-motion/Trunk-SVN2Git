// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// Control that allows a User Control to be bound to a business object data source and property.
  /// </summary>
  public class UserControlBinding : BusinessObjectBoundEditableWebControl
  {
    private string _userControlPath = string.Empty;
    private IDataEditControl _userControl;
    private BusinessObjectReferenceDataSourceControl _referenceDataSource;

    public string UserControlPath
    {
      get { return _userControlPath; }
      set { _userControlPath = value; }
    }

    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IDataEditControl UserControl
    {
      get { return _userControl; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      if (! IsDesignMode)
      {
        TemplateControl control = (TemplateControl) Page.LoadControl (_userControlPath);
        Controls.Add (control);
        _userControl = control as IDataEditControl;

        if (_userControl != null && DataSource != null)
        {
          IBusinessObjectDataSource dataSourceControl = DataSource;
          if (Property != null)
          {
            _referenceDataSource = new BusinessObjectReferenceDataSourceControl();
            _referenceDataSource.DataSource = DataSource;
            _referenceDataSource.Property = Property;
            _referenceDataSource.Mode = DataSource.Mode;
            dataSourceControl = _referenceDataSource;
            Controls.Add (_referenceDataSource);
          }

          _userControl.Mode = dataSourceControl.Mode;
          _userControl.BusinessObject = dataSourceControl.BusinessObject;
        }
      }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      if (_referenceDataSource != null)
        _referenceDataSource.Mode = DataSource.Mode;
      _userControl.Mode = DataSource.Mode;
    }


    public override void LoadValue (bool interim)
    {
      if (_referenceDataSource == null)
        throw new NotImplementedException();

      _referenceDataSource.LoadValue (interim);
      _userControl.BusinessObject = _referenceDataSource.BusinessObject;
      _userControl.LoadValues (interim);
    }

    public override void SaveValue (bool interim)
    {
      _userControl.SaveValues (interim);
      _referenceDataSource.BusinessObject = _userControl.BusinessObject;
      _referenceDataSource.SaveValue (interim);
    }

    protected override object ValueImplementation
    {
      get
      {
        if (_referenceDataSource == null)
          throw new InvalidOperationException ("Cannot get value if no property is set.");
        return _referenceDataSource.Value;
      }
      set
      {
        if (_referenceDataSource == null)
          throw new InvalidOperationException ("Cannot set value if no property is set.");
        _referenceDataSource.Value = value;
      }
    }

    public override bool IsDirty
    {
      get
      {
        for (int i = 0; i < _userControl.DataSource.BoundControls.Length; i++)
        {
          IBusinessObjectBoundControl control = _userControl.DataSource.BoundControls[i];
          BusinessObjectBoundEditableWebControl editableControl = control as BusinessObjectBoundEditableWebControl;
          if (editableControl != null && editableControl.IsDirty)
            return true;
        }
        return false;
      }
      set { throw new NotSupportedException(); }
    }

    public override string[] GetTrackedClientIDs ()
    {
      return new string[0];
    }

    protected override void Render (HtmlTextWriter writer)
    {
      if (IsDesignMode)
      {
        string type = "Unknown";
        IBusinessObjectReferenceProperty property = Property as IBusinessObjectReferenceProperty;
        if (property != null && property.ReferenceClass != null)
          type = property.ReferenceClass.Identifier;

        writer.Write (
            "<table style=\"font-family: arial; font-size: x-small; BORDER-RIGHT: gray 1px solid; BORDER-TOP: white 1px solid; BORDER-LEFT: white 1px solid; BORDER-BOTTOM: gray 1px solid; BACKGROUND-COLOR: #d4d0c8\">"
            + "<tr><td colspan=\"2\"><b>User Control</b></td></tr>"
            + "<tr><td>Data Source:</td><td>{0}</td></tr>"
            + "<tr><td>Property:</td><td>{1}</td></tr>"
            + "<tr><td>Type:</td><td>{2}</td></tr>"
            + "<tr><td>User Control:</td><td>{3}</td></tr>",
            DataSourceControl,
            PropertyIdentifier,
            type,
            _userControlPath);
      }

      base.Render (writer);
    }

    public override void RegisterValidator (BaseValidator validator)
    {
      throw new NotSupportedException();
    }

    public override void PrepareValidation ()
    {
      _userControl.PrepareValidation();
    }

    public override bool Validate ()
    {
      return _userControl.Validate();
    }

    protected override Type[] SupportedPropertyInterfaces
    {
      get { return new Type[] { typeof (IBusinessObjectReferenceProperty) }; }
    }
  }
}
