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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   <b>BusinessObjectBoundEditableWebControl</b> is the <see langword="abstract"/> default implementation of 
  ///   <see cref="IBusinessObjectBoundEditableWebControl"/>.
  /// </summary>
  /// <seealso cref="IBusinessObjectBoundEditableWebControl"/>
  public abstract class BusinessObjectBoundEditableWebControl : BusinessObjectBoundWebControl, IBusinessObjectBoundEditableWebControl
  {
    private bool? _required = null;
    private bool? _readOnly = null;
    private List<BaseValidator> _validators;
    private bool _isDirty = false;
    private bool _hasBeenRenderedInPreviousLifecycle = false;
    private bool _isRenderedInCurrentLifecycle = false;

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      if (Page is ISmartPage)
        ((ISmartPage) Page).RegisterControlForDirtyStateTracking (this);
    }

    /// <summary> Gets or sets a flag that specifies whether the value of the control is required. </summary>
    /// <remarks>
    ///   Set this property to <see langword="null"/> in order to use the default value 
    ///   (see <see cref="IsRequired"/>).
    /// </remarks>
    [Description ("Explicitly specifies whether the control is required.")]
    [Category ("Data")]
    [DefaultValue (typeof (bool?), "")]
    public bool? Required
    {
      get { return _required; }
      set { _required = value; }
    }

    /// <summary> Gets or sets a flag that specifies whether the control should be displayed in read-only mode. </summary>
    /// <remarks>
    ///   Set this property to <see langword="null"/> in order to use the default value 
    ///   (see <see cref="IsReadOnly"/>). Note that if the data source is in read-only mode, the
    ///   control is read-only too, even if this property is set to <c>false</c>.
    /// </remarks>
    [Description ("Explicitly specifies whether the control should be displayed in read-only mode.")]
    [Category ("Data")]
    [DefaultValue (typeof (bool?), "")]
    public bool? ReadOnly
    {
      get { return _readOnly; }
      set { _readOnly = value; }
    }

    /// <summary> Gets or sets the dirty flag. </summary>
    /// <remarks>
    ///   <para>
    ///     Initially, the <see cref="IsDirty"/> flag is <see langword="false"/>. It is reset to <see langword="false"/>
    ///     when using <see cref="BusinessObjectBoundWebControl.LoadValue"/> to read the value from, or using
    ///     <see cref="SaveValue"/> to write the value back into the <see cref="IBusinessObject"/> bound to the 
    ///     <see cref="BusinessObjectBoundWebControl.DataSource"/>. It is also reset when using <b>LoadUnboundValue</b>
    ///     to set the value for an unbound control.
    ///   </para><para>
    ///     It is set to <see langword="true"/> when the control's <see cref="BusinessObjectBoundWebControl.Value"/> 
    ///     is set by means other than <see cref="BusinessObjectBoundWebControl.LoadValue"/> or <b>LoadUnboundValue</b>.
    ///     It is also set when value changes due to the user submitting new data in the user interface or through the 
    ///     application using the control to modify the contents of the <see cref="BusinessObjectBoundWebControl.Value"/>. 
    ///     (E.g. a row is added to the list of values by invoking a method on the <see cref="BocList"/>.)
    ///   </para>
    /// </remarks>
    [Browsable (false)]
    public virtual bool IsDirty
    {
      get { return _isDirty; }
      set { _isDirty = value; }
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
    ///   interface.
    /// </summary>
    /// <returns> A string array containing zero or more client ids. </returns>
    public abstract string[] GetTrackedClientIDs ();

    /// <summary>
    ///   Saves the <see cref="IBusinessObjectBoundControl.Value"/> back into the bound <see cref="IBusinessObject"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
    public abstract void SaveValue (bool interim);

    /// <summary>
    ///   Gets a flag that determines whether the control is to be displayed in read-only mode.
    /// </summary>
    /// <remarks>
    ///     In read-only mode, a <see cref="System.Web.UI.WebControls.Label"/> control is used to display the value.
    ///     Otherwise, a <see cref="System.Web.UI.WebControls.TextBox"/> control is used to display and edit the value.
    /// </remarks>
    /// <value>
    ///   <list type="bullet">
    ///     <item>
    ///       Whether the control is bound or unbound, if the value of the <see cref="ReadOnly"/> property is 
    ///       <see langword="true"/>, <see langword="true"/> is returned.
    ///     </item>
    ///     <item>
    ///       If the control is bound to an <see cref="IBusinessObjectDataSourceControl"/> and 
    ///       <see cref="IBusinessObjectDataSource.Mode">DataSource.Mode</see> is set to 
    ///       <see cref="DataSourceMode.Search"/>, <see langword="false"/> is returned.
    ///     </item>
    ///     <item>
    ///       If the control is unbound (<see cref="BusinessObjectBoundWebControl.DataSource"/> or 
    ///       <see cref="BusinessObjectBoundWebControl.Property"/> is <see langword="null"/>) and the
    ///       <see cref="ReadOnly"/> property is not <see langword="true"/>, 
    ///       <see langword="false"/> is returned.
    ///     </item>
    ///     <item>
    ///       If the control is bound (<see cref="BusinessObjectBoundWebControl.DataSource"/> and  
    ///       <see cref="BusinessObjectBoundWebControl.Property"/> are not <see langword="null"/>), 
    ///       the following rules are used to determine the value of this property:
    ///       <list type="bullet">
    ///         <item>
    ///           If the <see cref="IBusinessObjectDataSource.Mode">DataSource.Mode</see> of the control's
    ///           <see cref="BusinessObjectBoundWebControl.DataSource"/> is set to <see cref="DataSourceMode.Read"/>, 
    ///           <see langword="true"/> is returned.
    ///         </item>
    ///         <item>
    ///           If the <see cref="IBusinessObjectDataSource.BusinessObject">DataSource.BusinessObject</see> is 
    ///           <see langword="null"/> and the control is not in <b>Design Mode</b>, 
    ///           <see langword="true"/> is returned.
    ///         </item>
    ///         <item>
    ///           If the control's <see cref="ReadOnly"/> property is <see langword="false"/>, 
    ///           <see langword="false"/> is returned.
    ///         </item>
    ///         <item>
    ///           Otherwise, <see langword="Property.IsReadOnly"/> is evaluated and returned.
    ///         </item>
    ///       </list>
    ///     </item>
    ///   </list>
    /// </value>
    [Browsable (false)]
    public virtual bool IsReadOnly
    {
      get
      {
        if (_readOnly == true) // (Bound Control || Unbound Control) && ReadOnly==true
          return true;
        if (DataSource != null && DataSource.Mode == DataSourceMode.Search) // Search DataSource 
          return false;
        if (Property == null || DataSource == null) // Unbound Control && (ReadOnly==false || ReadOnly==undefined)
          return false;
        if (DataSource.Mode == DataSourceMode.Read) // Bound Control && Reader DataSource
          return true;
        if (! IsDesignMode && DataSource.BusinessObject == null) // Bound Control but no BusinessObject
          return true;
        if (_readOnly == false) // Bound Control && ReadOnly==false
          return false;
        return Property.IsReadOnly (DataSource.BusinessObject); // ReadOnly==undefined: ObjectModel pulls
      }
    }

    /// <summary>
    ///   Gets a flag that determines whether the control is to be treated as a required value.
    /// </summary>
    /// <remarks>
    ///     The value of this property is used to decide whether <see cref="CreateValidators"/> should 
    ///     include a <see cref="RequiredFieldValidator"/> for this control.
    /// </remarks>
    /// <value>
    ///   The following rules are used to determine the value of this property:
    ///   <list type="bullet">
    ///     <item>If the control is read-only, <see langword="false"/> is returned.</item>
    ///     <item>
    ///       If the <see cref="Required"/> property is not <see langword="null"/>, 
    ///       the value of <see cref="Required"/> is returned.
    ///     </item>
    ///     <item>
    ///       If the <see cref="BusinessObjectBoundWebControl.Property"/> contains a property definition with the
    ///       <see cref="IBusinessObjectProperty.IsRequired"/> flag set, <see langword="true"/> is returned. 
    ///     </item>
    ///     <item>Otherwise, <see langword="false"/> is returned.</item>
    ///   </list>
    /// </value>
    [Browsable (false)]
    public virtual bool IsRequired
    {
      get
      {
        if (IsReadOnly)
          return false;
        if (_required != null)
          return _required == true;
        if (Property != null)
          return (bool) Property.IsRequired;
        return false;
      }
    }

    /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
    /// <returns> An (empty) array of <see cref="BaseValidator"/> controls. </returns>
    public virtual BaseValidator[] CreateValidators ()
    {
      return new BaseValidator[0];
    }

    /// <summary> Registers a validator that references this control. </summary>
    /// <remarks> 
    ///   <para>
    ///     The control may choose to ignore this call. 
    ///   </para><para>
    ///     The registered validators are evaluated when <see cref="Validate"/> is called.
    ///   </para>
    /// </remarks>
    public virtual void RegisterValidator (BaseValidator validator)
    {
      if (_validators == null)
        _validators = new List<BaseValidator>();

      _validators.Add (validator);
    }

    public virtual void PrepareValidation ()
    {
    }

    /// <summary> Calls <see cref="BaseValidator.Validate"/> on all registered validators. </summary>
    /// <returns> <see langword="true"/>, if all validators validated. </returns>
    public virtual bool Validate ()
    {
      if (_validators == null)
        return true;

      bool isValid = true;
      for (int i = 0; i < _validators.Count; i++)
      {
        BaseValidator validator = (BaseValidator) _validators[i];
        validator.Validate();
        isValid &= validator.IsValid;
      }
      return isValid;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      _isRenderedInCurrentLifecycle = true;
    }

    protected virtual bool RequiresLoadPostData
    {
      get
      {
        IWxePage wxePage = Page as IWxePage;
        if (wxePage != null)
          return _hasBeenRenderedInPreviousLifecycle || wxePage.IsOutOfSequencePostBack;

        return _hasBeenRenderedInPreviousLifecycle;
      }
    }

    protected override void LoadControlState (object savedState)
    {
      object[] values = (object[]) savedState;
      base.LoadControlState (values[0]);
      _isDirty = (bool) values[1];
      _hasBeenRenderedInPreviousLifecycle = (bool) values[2];
    }

    protected override object SaveControlState ()
    {
      object[] values = new object[3];
      values[0] = base.SaveControlState ();
      values[1] = _isDirty;
      values[2] = _isRenderedInCurrentLifecycle;
      return values;
    }
  }
}
