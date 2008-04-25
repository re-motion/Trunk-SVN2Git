using System;
using System.Collections.Generic;
using System.ComponentModel;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary> 
  ///   The <see langword="abstract"/> default implementation of the <see cref="IBusinessObjectDataSource"/> interface. 
  /// </summary>
  /// <remarks>
  ///   Any specialized version of the <b>BusinessObjectDataSource</b> requires an override for the
  ///   <see cref="BusinessObjectClass"/> property. It is also necessary to provide a way for specifying which 
  ///   <see cref="IBusinessObjectClass"/> will be returned by this property. See the remarks section of the
  ///   <see cref="IBusinessObjectDataSource"/> interface documentation for details on how to implement this feature.
  /// </remarks>
  /// <seealso cref="IBusinessObjectDataSource"/>
  public abstract class BusinessObjectDataSource : Component, IBusinessObjectDataSource
  {
    private readonly List<IBusinessObjectBoundControl> _boundControls = new List<IBusinessObjectBoundControl>();
    private DataSourceMode _mode = DataSourceMode.Edit;

    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObject"/> connected to this <see cref="IBusinessObjectDataSource"/>
    /// </summary>
    /// <value>
    ///   An <see cref="IBusinessObject"/> or <see langword="null"/>. Must be compatible with
    ///   the <see cref="BusinessObjectClass"/> assigned to this <see cref="BusinessObjectDataSource"/>.
    /// </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public abstract IBusinessObject BusinessObject { get; set; }

    /// <summary> 
    ///   Gets the <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/> connected to this <see cref="BusinessObjectDataSource"/>. 
    /// </summary>
    /// <value> The <see cref="IBusinessObjectClass"/> of the connected <see cref="IBusinessObject"/>. </value>
    /// <remarks>
    ///   Usually set before the <see cref="IBusinessObject"/> is connected to the <see cref="BusinessObjectDataSource"/>. 
    /// </remarks>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public abstract IBusinessObjectClass BusinessObjectClass { get; }

    /// <summary> Gets or sets the current <see cref="DataSourceMode"/>. </summary>
    /// <value> A value of the <see cref="DataSourceMode"/> enumeration. </value>
    [Category ("Data")]
    [DefaultValue (DataSourceMode.Edit)]
    public DataSourceMode Mode
    {
      get { return _mode; }
      set { _mode = value; }
    }

    /// <summary> Loads the values of the <see cref="BusinessObject"/> into all bound controls. </summary>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    public void LoadValues (bool interim)
    {
      foreach (IBusinessObjectBoundControl control in _boundControls)
      {
        if (control.HasValidBinding)
          control.LoadValue (interim);
      }
    }

    /// <summary> 
    ///   Saves the values of the <see cref="BusinessObject"/> from all bound controls implementing <see cref="IBusinessObjectBoundEditableControl"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
    public void SaveValues (bool interim)
    {
      foreach (IBusinessObjectBoundControl control in _boundControls)
      {
        IBusinessObjectBoundEditableControl writeableControl = control as IBusinessObjectBoundEditableControl;
        if (writeableControl != null && writeableControl.HasValidBinding)
          writeableControl.SaveValue (interim);
      }
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectProvider"/> used for accessing supplementary information on the connected
    ///   <see cref="IBusinessObject"/> and assigned <see cref="IBusinessObjectClass"/>.
    /// </summary>
    /// <value> The <see cref="IBusinessObjectProvider"/> for the current <see cref="BusinessObjectClass"/>. </value>
    /// <remarks>
    ///   <note type="inheritinfo">
    ///     Must not return <see langword="null"/> if the <see cref="BusinessObjectClass"/> is set.
    ///   </note>
    /// </remarks>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public virtual IBusinessObjectProvider BusinessObjectProvider
    {
      get { return (BusinessObjectClass == null) ? null : BusinessObjectClass.BusinessObjectProvider; }
    }

    /// <summary>Gets the <see cref="IBusinessObjectBoundControl"/> objects bound to this <see cref="BusinessObjectDataSource"/>.</summary>
    /// <value> An array of <see cref="IBusinessObjectBoundControl"/> objects. </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectBoundControl[] BoundControls
    {
      get { return _boundControls.FindAll (delegate (IBusinessObjectBoundControl control) { return control.HasValidBinding; }).ToArray(); }
    }

    /// <summary>
    ///   Adds the passed <see cref="IBusinessObjectBoundControl"/> to the list of controls bound to this <see cref="BusinessObjectDataSource"/>.
    /// </summary>
    /// <param name="control">The <see cref="IBusinessObjectBoundControl"/> to be added to <see cref="BoundControls"/>.</param>
    public void Register (IBusinessObjectBoundControl control)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      if (!_boundControls.Contains (control))
        _boundControls.Add (control);
    }

    /// <summary>
    ///   Removes the passed <see cref="IBusinessObjectBoundControl"/> from the list of controls bound to this <see cref="BusinessObjectDataSource"/>.
    /// </summary>
    /// <param name="control">The <see cref="IBusinessObjectBoundControl"/> to be removed from <see cref="BoundControls"/>.</param>
    public void Unregister (IBusinessObjectBoundControl control)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      _boundControls.Remove (control);
    }
  }
}