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
using System.ComponentModel;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   <b>BusinessObjectReferenceDataSourceControl</b> provides an <see cref="IBusinessObjectReferenceDataSource"/>
  ///   to controls of type <see cref="IBusinessObjectBoundWebControl"/> inside an <b>ASPX Web Form</b> or 
  ///   <b>ASCX User Control</b>.
  /// </summary>
  /// <seealso cref="IBusinessObjectReferenceDataSource"/>
  /// <seealso cref="IBusinessObjectDataSourceControl"/>
  [NonVisualControl]
  [Designer (typeof (BocDataSourceDesigner))]
  public class BusinessObjectReferenceDataSourceControl :
      BusinessObjectBoundEditableWebControl, IBusinessObjectDataSourceControl, IBusinessObjectReferenceDataSource
  {
    private class InternalBusinessObjectReferenceDataSource : BusinessObjectReferenceDataSourceBase
    {
      private BusinessObjectReferenceDataSourceControl _parent;

      public InternalBusinessObjectReferenceDataSource (BusinessObjectReferenceDataSourceControl parent)
      {
        _parent = parent;
      }

      public override IBusinessObjectReferenceProperty ReferenceProperty
      {
        get { return _parent.ReferenceProperty; }
        set { _parent.ReferenceProperty = value; }
      }

      public override IBusinessObjectDataSource ReferencedDataSource
      {
        get { return _parent.DataSource; }
      }

      public bool IsDirty
      {
        get
        {
          if (HasBusinessObjectChanged)
            return true;

          foreach (IBusinessObjectBoundControl control in BoundControls)
          {
            IBusinessObjectBoundEditableWebControl editableControl =
                control as IBusinessObjectBoundEditableWebControl;
            if (editableControl != null && editableControl.IsDirty)
              return true;
          }
          return false;
        }
      }
    }

    private InternalBusinessObjectReferenceDataSource _internalDataSource;

    /// <summary>
    ///   <see cref="BusinessObjectReferenceDataSourceControl"/> supports properties of type
    ///   <see cref="IBusinessObjectReferenceProperty"/>.
    /// </summary>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return new Type[] { typeof (IBusinessObjectReferenceProperty) }; }
    }

    // Default summary will be created.
    public BusinessObjectReferenceDataSourceControl ()
    {
      _internalDataSource = new InternalBusinessObjectReferenceDataSource (this);
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    /// <value> The value must be of type <see cref="IBusinessObject"/>. </value>
    protected override object ValueImplementation
    {
      get { return _internalDataSource.BusinessObject; }
      set { _internalDataSource.BusinessObject = (IBusinessObject) value; }
    }

    /// <summary> Gets or sets the dirty flag. </summary>
    /// <value> 
    ///   Evaluates <see langword="true"/> if either the <see cref="BusinessObjectReferenceDataSourceControl"/> or one 
    ///   of the bound controls is dirty.
    /// </value>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.IsDirty">BusinessObjectBoundEditableWebControl.IsDirty</seealso>
    public override bool IsDirty
    {
      get
      {
        return base.IsDirty || _internalDataSource.IsDirty;
      }
      set
      {
        base.IsDirty = value;
      }
    }

    /// <summary> 
    ///   Returns the <see cref="System.Web.UI.Control.ClientID"/> values of all controls whose value can be modified 
    ///   in the user interface.
    /// </summary>
    /// <returns> An empty <see cref="String"/> <see cref="Array"/>. </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      return new string[0];
    }

    /// <summary> 
    ///   Loads the <see cref="BusinessObject"/> from the <see cref="ReferencedDataSource"/> using 
    ///   <see cref="ReferenceProperty"/> and populates the bound controls using <see cref="LoadValues"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    public override void LoadValue (bool interim) // inherited from control interface
    {
      _internalDataSource.LoadValue (interim);
    }

    /// <summary> Loads the values of the <see cref="BusinessObject"/> into all bound controls. </summary>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    public virtual void LoadValues (bool interim) // inherited from data source interface
    {
      _internalDataSource.LoadValues (interim);
    }

    /// <summary> 
    ///   Saves the values from the bound controls using <see cref="SaveValues"/>
    ///   and writes the <see cref="BusinessObject"/> back into the <see cref="ReferencedDataSource"/> using 
    ///   <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
    /// <remarks> 
    ///   Actual saving only occurs if <see cref="BusinessObjectBoundEditableWebControl.IsReadOnly"/> evaluates 
    ///   <see langword="false"/>. 
    /// </remarks>
    public override void SaveValue (bool interim) // inherited from control interface
    {
      if (!IsReadOnly && IsDirty)
        _internalDataSource.SaveValue (interim);
    }

    /// <summary> 
    ///   Saves the values of the <see cref="BusinessObject"/> from all bound controls implementing
    ///   <see cref="IBusinessObjectBoundEditableControl"/>.
    /// </summary>
    /// <param name="interim"> Spefifies whether this is the final saving, or an interim saving. </param>
    /// <remarks>
    ///   Actual saving only occurs if <see cref="BusinessObjectBoundEditableWebControl.IsReadOnly"/> evaluates 
    ///  <see langword="false"/>. 
    /// </remarks>
    public virtual void SaveValues (bool interim) // inherited data source interface
    {
      if (!IsReadOnly)
        _internalDataSource.SaveValues (interim);
    }

    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used to access the 
    ///   <see cref="IBusinessObject"/> to which this <see cref="BusinessObjectReferenceDataSourceControl"/> connects.
    /// </summary>
    /// <value> 
    ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the 
    ///   <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>.
    /// </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectReferenceProperty ReferenceProperty
    {
      get { return (IBusinessObjectReferenceProperty) Property; }
      set { Property = value; }
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> 
    ///   to which this <see cref="BusinessObjectReferenceDataSourceControl"/> connects.
    /// </summary>
    /// <value> 
    ///   The <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> to which this
    ///   <see cref="BusinessObjectReferenceDataSourceControl"/> connects.
    ///  </value>
    /// <remarks> Identical to <see cref="BusinessObjectBoundWebControl.DataSource"/>. </remarks>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectDataSource ReferencedDataSource
    {
      get { return _internalDataSource.ReferencedDataSource; }
    }


    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObject"/> accessed through the <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <value> An <see cref="IBusinessObject"/> or <see langword="null"/>. </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IBusinessObject BusinessObject
    {
      get { return _internalDataSource.BusinessObject; }
      set { _internalDataSource.BusinessObject = value; }
    }

    /// <summary> 
    ///   Gets the <see cref="IBusinessObjectReferenceProperty.ReferenceClass"/> of the <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <value> 
    ///   An <see cref="IBusinessObjectClass"/> or <see langword="null"/> if no <see cref="ReferenceProperty"/> is set.
    /// </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectClass BusinessObjectClass
    {
      get { return _internalDataSource.BusinessObjectClass; }
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectProvider"/> of this 
    ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </summary>
    /// <value> The <see cref="IBusinessObjectProvider"/> for the current <see cref="BusinessObjectClass"/>. </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectProvider BusinessObjectProvider
    {
      get { return _internalDataSource.BusinessObjectProvider; }
    }

    /// <summary>
    ///   Adds the passed <see cref="IBusinessObjectBoundControl"/> to the list of controls bound to this
    ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </summary>
    /// <param name="control"> 
    ///   The <see cref="IBusinessObjectBoundControl"/> to be registered with this
    ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </param>
    public void Register (IBusinessObjectBoundControl control)
    {
      _internalDataSource.Register (control);
    }

    /// <summary>
    ///   Removes the passed <see cref="IBusinessObjectBoundControl"/> from the list of controls bound to this
    ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </summary>
    /// <param name="control"> 
    ///   The <see cref="IBusinessObjectBoundControl"/> to be unregistered from this 
    ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </param>
    public void Unregister (IBusinessObjectBoundControl control)
    {
      _internalDataSource.Unregister (control);
    }

    /// <summary>
    ///   Gets or sets the current <see cref="DataSourceMode"/> of this 
    ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </summary>
    /// <value> A value of the <see cref="DataSourceMode"/> enumeration. </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public virtual DataSourceMode Mode
    {
      get
      {
        // return IsReadOnly ? DataSourceMode.Read : DataSourceMode.Edit; 
        if (IsReadOnly)
          return DataSourceMode.Read;
        if (DataSource != null && DataSource.Mode == DataSourceMode.Search)
          return DataSourceMode.Search;
        return DataSourceMode.Edit;
      }
      set
      {
        // "search" needs edit mode
        ReadOnly = value == DataSourceMode.Read;
      }
    }

    /// <summary>
    ///   Gets an array of <see cref="IBusinessObjectBoundControl"/> objects bound to this 
    ///   <see cref="IBusinessObjectDataSource"/>.
    /// </summary>
    /// <value> An array of <see cref="IBusinessObjectBoundControl"/> objects. </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectBoundControl[] BoundControls
    {
      get { return _internalDataSource.BoundControls; }
    }

    /// <summary> Prepares all bound controls implementing <see cref="IValidatableControl"/> for validation. </summary>
    public override void PrepareValidation ()
    {
      base.PrepareValidation ();
      for (int i = 0; i < BoundControls.Length; i++)
      {
        IBusinessObjectBoundControl control = _internalDataSource.BoundControls[i];
        IValidatableControl validateableControl = control as IValidatableControl;
        if (validateableControl != null)
          validateableControl.PrepareValidation ();
      }
    }

    /// <summary> Validates all bound controls implementing <see cref="IValidatableControl"/>. </summary>
    /// <returns> <see langword="true"/> if no validation errors where found. </returns>
    public override bool Validate ()
    {
      bool isValid = base.Validate ();
      for (int i = 0; i < _internalDataSource.BoundControls.Length; i++)
      {
        IBusinessObjectBoundControl control = _internalDataSource.BoundControls[i];
        IValidatableControl validateableControl = control as IValidatableControl;
        if (validateableControl != null)
          isValid &= validateableControl.Validate ();
      }
      return isValid;
    }

    /// <summary>
    ///   Overrides the implementation of <see cref="System.Web.UI.Control.Render">Control.Render</see>. 
    ///   Does not render any output.
    /// </summary>
    /// <param name="writer">
    ///   The <see cref="System.Web.UI.HtmlTextWriter"/> object that receives the server control content. 
    /// </param>
    protected override void Render (System.Web.UI.HtmlTextWriter writer)
    {
      //  No output, control is invisible
    }
  }
}
