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
// Requires:
// System.dll, System.Design.dll, System.Drawing.dll, System.Web.dll
// Remotion.dll, Remotion.ObjectBinding.dll, Remotion.ObjectBinding.Web.dll, Remotion.Web.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Remotion.Collections;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Design;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Sample
{
  public class ObjectBoundRepeater: Repeater, IBusinessObjectBoundEditableWebControl
  {
    protected class ObjectBoundRepeaterInternal: BusinessObjectBoundEditableWebControl
    {
      private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
          typeof (IBusinessObjectReferenceProperty) };

    
      private IList _value;
      private ObjectBoundRepeater _parent;


      public ObjectBoundRepeaterInternal (ObjectBoundRepeater parent)
      {
        ArgumentUtility.CheckNotNull ("parent", parent);
        _parent = parent;
      }

      public override void LoadValue (bool interim)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null)
          ValueImplementation = DataSource.BusinessObject.GetProperty (Property);

        if (! interim)
          IsDirty = false;

        _parent.LoadValueInternal (interim);
      }

      public override void SaveValue (bool interim)
      {
        if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
          DataSource.BusinessObject.SetProperty (Property, Value);
      
        _parent.SaveValueInternal (interim);
      }

      public override bool IsDirty
      {
        get { return _parent.IsDirtyInternal; }
        set { _parent.IsDirtyInternal = value; }
      }

      public override string[] GetTrackedClientIDs()
      {
        return _parent.GetTrackedClientIDsInternal();
      }

      /// <summary> Gets or sets the current value. </summary>
      /// <value> An object implementing <see cref="IList"/>. </value>
      [Browsable (false)]
      public new IList Value
      {
        get 
        {
          return _value; 
        }
        set 
        {
          _value = value; 
          ((Repeater) _parent).DataSource = value;
        }
      }

      /// <summary> Gets or sets the current value when <see cref="Value"/> through polymorphism. </summary>
      /// <value> The value must be of type <see cref="IList"/>. </value>
      protected override object ValueImplementation
      {
        get { return Value; }
        set { Value = (IList) value; }
      }

      /// <summary>
      ///   Gets or sets the list of <see cref="Type"/> objects for the <see cref="IBusinessObjectProperty"/> 
      ///   implementations that can be bound to this control.
      /// </summary>
      protected override Type[] SupportedPropertyInterfaces
      {
        get { return s_supportedPropertyInterfaces; }
      }

      /// <summary> Gets a value that indicates whether properties with the specified multiplicity are supported. </summary>
      /// <returns> <b>true</b> if the multiplicity specified by <paramref name="isList"/> is supported. </returns>
      protected override bool SupportsPropertyMultiplicity (bool isList)
      {
        return isList;
      }


      protected override void Render (HtmlTextWriter writer)
      {
      }

      public override Control NamingContainer
      {
        get { return _parent.NamingContainer; }
      }

      public override bool Validate()
      {
        bool isValid = base.Validate();
        if (! isValid)
          return false;
        return _parent.ValidateInternal();
      }

      protected override void LoadViewState(object savedState)
      {
        if (savedState != null)
          base.LoadViewState (savedState);
      }

    }

    #region BusinessObjectBoundEditableWebControl implementation

    [Browsable(false)]
    public BusinessObjectBinding Binding
    {
      get { return _repeaterInternal.Binding; }
    }

    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> this <see cref="ObjectBoundRepeater"/> is bound to.
    /// </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public new IBusinessObjectDataSource DataSource
    {
      get { return _repeaterInternal.DataSource; }
      set { _repeaterInternal.DataSource = value; }
    }

    [Category ("Data")]
    [Description ("The string representation of the Property.")]
    [Editor (typeof (PropertyPickerEditor), typeof (UITypeEditor))]
    [DefaultValue ("")]
    [MergableProperty (false)]
    public string PropertyIdentifier
    {
      get { return _repeaterInternal.PropertyIdentifier; }
      set { _repeaterInternal.PropertyIdentifier = value; }
    }

    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObjectProperty"/> used for accessing the data to be loaded into 
    ///   <see cref="Value"/>.
    /// </summary>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectProperty Property
    {
      get { return _repeaterInternal.Property; }
      set { _repeaterInternal.Property = value; }
    }

    [TypeConverter (typeof (BusinessObjectDataSourceControlConverter))]
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Data")]
    [Description ("The ID of the BusinessObjectDataSourceControl control used as data source.")]
    [DefaultValue ("")]
    public string DataSourceControl
    {
      get { return _repeaterInternal.DataSourceControl; }
      set { _repeaterInternal.DataSourceControl = value; }
    }


    public void LoadValue (bool interim)
    {
      _repeaterInternal.LoadValue (interim);
    }
  
    public void SaveValue (bool interim)
    {
      _repeaterInternal.SaveValue (interim);
    }

    object IBusinessObjectBoundControl.Value
    {
      get { return _repeaterInternal.Value; }
      set { _repeaterInternal.Value = (IList) value; }
    }

    [Browsable (false)]
    public bool IsDirty
    {
      get { return _repeaterInternal.IsDirty; }
      set { _repeaterInternal.IsDirty = value; }
    }

    public string[] GetTrackedClientIDs()
    {
      return _repeaterInternal.GetTrackedClientIDs();
    }

    public void PrepareValidation ()
    {
      _repeaterInternal.PrepareValidation();
    }

    public bool Validate()
    {
      return _repeaterInternal.Validate();
    }

    [Browsable (false)]
    public bool HasValidBinding
    {
      get
      {
        EnsureChildControls();
        return _repeaterInternal.HasValidBinding; 
      }
    }

    public override bool Visible
    {
      get { return HasValidBinding && base.Visible; }
      set { base.Visible = value; }
    }


    [Browsable(false)]
    public string DisplayName 
    {
      get { return _repeaterInternal.DisplayName; }
    }

    [Browsable(false)]
    public virtual HelpInfo HelpInfo
    {
      get { return null; }
    }

    [Browsable(false)]
    public virtual Control TargetControl
    {
      get { return this; }
    }

    [Browsable(false)]
    public virtual bool UseLabel
    {
      get { return false; }
    }


    /// <summary> Gets or sets a flag that specifies whether the value of the control is required. </summary>
    [Description("Explicitly specifies whether the control is required.")]
    [Category ("Data")]
    [DefaultValue (typeof (bool?), "")]
    public bool? Required
    {
      get { return _repeaterInternal.Required; }
      set { _repeaterInternal.Required = value; }
    }

    /// <summary> Gets or sets a flag that specifies whether the control should be displayed in read-only mode. </summary>
    [Description("Explicitly specifies whether the control should be displayed in read-only mode.")]
    [Category ("Data")]
    [DefaultValue (typeof (bool?), "")]
    public bool? ReadOnly
    {
      get { return _repeaterInternal.ReadOnly; }
      set { _repeaterInternal.ReadOnly = value; }
    }

    [Browsable(false)]
    public bool IsReadOnly
    {
      get { return _repeaterInternal.IsReadOnly; }
    }

    [Browsable(false)]
    public bool IsRequired 
    {
      get { return _repeaterInternal.IsRequired; }
    }
 

    public virtual BaseValidator[] CreateValidators()
    {
      return _repeaterInternal.CreateValidators();
    }

    public virtual void RegisterValidator (BaseValidator validator)
    {
      _repeaterInternal.RegisterValidator (validator);
    }



    public bool SupportsProperty (IBusinessObjectProperty property)
    {
      return _repeaterInternal.SupportsProperty (property);
    }
  
    void IControlWithDesignTimeSupport.PreRenderForDesignMode()
    {
      ((IControlWithDesignTimeSupport) _repeaterInternal).PreRenderForDesignMode();
    }

    #endregion
  
    private ObjectBoundRepeaterInternal _repeaterInternal;
    private ArrayList _dataSources = new ArrayList();
    private ArrayList _dataEditControls = new ArrayList();
    private Boolean _isDirty = true;

    public ObjectBoundRepeater()
    {
      _repeaterInternal = CreateRepeaterInternal();
    }

    protected virtual ObjectBoundRepeaterInternal CreateRepeaterInternal()
    {
      return new ObjectBoundRepeaterInternal (this);
    }

    protected ObjectBoundRepeaterInternal RepeaterInternal
    {
      get { return _repeaterInternal; }
    }


    protected override void CreateChildControls()
    {
      base.CreateChildControls();

      _repeaterInternal.ID = ID + "_RepeaterInternal";
      Controls.Add (_repeaterInternal);
    }

    public override ControlCollection Controls
    {
      get
      {
        EnsureChildControls();
        return base.Controls;
      }
    }


    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public override string DataMember
    {
      get { return string.Empty; }
      set { throw new NotSupportedException(); }
    }


    protected override void OnItemDataBound (RepeaterItemEventArgs e)
    {
      base.OnItemDataBound (e);
      if (IsDesignMode)
        return;

      IBusinessObject obj = (IBusinessObject) e.Item.DataItem;
    
      foreach (Control control in e.Item.Controls)
      {
        if (control is BusinessObjectDataSourceControl)
        {
          BusinessObjectDataSourceControl dataSource = (BusinessObjectDataSourceControl) control;
          _dataSources.Add (dataSource);

          dataSource.BusinessObject = obj;
        }
        else if (control is IDataEditControl)
        {
          DataEditUserControl dataEditControl = (DataEditUserControl) control;
          _dataEditControls.Add (dataEditControl);
    
          dataEditControl.BusinessObject = obj;        
          if (IsReadOnly)
            dataEditControl.Mode = IsReadOnly ? DataSourceMode.Read : DataSourceMode.Edit;
        }
      }
    }
  
    protected virtual void LoadValueInternal (bool interim)
    {
      Controls.Clear();
      Controls.Add (_repeaterInternal);
      if (! interim)
        base.ClearChildViewState();
      this.CreateControlHierarchy (true);
      base.ChildControlsCreated = true;

      foreach (BusinessObjectDataSourceControl dataSource in _dataSources)
        dataSource.LoadValues (interim);

      foreach (IDataEditControl control in _dataEditControls)
        control.LoadValues (interim);
    }
  
    protected virtual void SaveValueInternal (bool interim)
    {
      foreach (BusinessObjectDataSourceControl dataSource in _dataSources)
        dataSource.SaveValues (interim);

      foreach (IDataEditControl control in _dataEditControls)
        control.SaveValues (interim);
    }


    /// <summary> Gets or sets the current value. </summary>
    /// <value> 
    ///   An object implementing <see cref="IList"/>, containing objects of implementing <see cref="IBusinessObject"/>. 
    /// </value>
    [Browsable (false)]
    public IList Value
    {
      get { return _repeaterInternal.Value; }
      set { _repeaterInternal.Value = value; }
    }


    /// <summary> Gets or sets the dirty flag. </summary>
    /// <remarks>
    ///   Initially, the value of <c>IsDirty</c> is <b>true</b>. The value is set to <b>false</b> during loading
    ///   and saving values. Text changes by the user cause <b>IsDirty</b> to be reset to <b>false</b> during the
    ///   loading phase of the request (i.e., before the page's <b>Load</b> event is raised).
    /// </remarks>
    protected virtual bool IsDirtyInternal
    {
      get { return _isDirty; }
      set { _isDirty = value; }
    }

    protected virtual string[] GetTrackedClientIDsInternal()
    {
      return new string[0];
    }


    protected virtual bool ValidateInternal()
    {
      foreach (IBusinessObjectDataSourceControl dataSource in _dataSources)
      {
        bool isValid = dataSource.Validate();
        if (! isValid)
          return false;
      }

      foreach (IDataEditControl control in _dataEditControls)
      {
        bool isValid = control.Validate();
        if (! isValid)
          return false;
      }

      return true;
    }

    /// <summary> Evalutes whether this control is in <b>Design Mode</b>. </summary>
    protected bool IsDesignMode
    {
      get { return ControlHelper.IsDesignMode (this, Context); }
    }

    protected override void LoadViewState(object savedState)
    {
      object[] values = (object[]) savedState;
      base.LoadViewState (values[0]);
      _isDirty = (bool)  values[1];
    }

    protected override object SaveViewState()
    {
      object[] values = new object[2];
      values[0] = base.SaveViewState();
      values[1] = _isDirty;
      return values;
    }

    protected override void Render(HtmlTextWriter writer)
    {
      if (IsDesignMode)
        return;

      base.Render (writer);
    }

    void ISmartControl.RegisterHtmlHeadContents (HttpContext context)
    {
    }
  }
}
