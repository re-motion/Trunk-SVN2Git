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
using System.ComponentModel;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Design;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   <b>BusinessObjectBoundWebControl</b> is the <see langword="abstract"/> default implementation of 
  ///   <see cref="IBusinessObjectBoundWebControl"/>.
  /// </summary>
  /// <seealso cref="IBusinessObjectBoundWebControl"/>
  // It is required to use a Designer from the same assambly as is the control (or the GAC etc), 
  // otherwise the VS 2003 Toolbox will have trouble loading the assembly.
  [Designer (typeof (BocDesigner))]
  public abstract class BusinessObjectBoundWebControl : WebControl, IExtendedBusinessObjectBoundWebControl
  {
    #region BusinessObjectBinding implementation

    /// <summary>Gets the <see cref="BusinessObjectBinding"/> object used to manage the binding for this <see cref="BusinessObjectBoundWebControl"/>.</summary>
    /// <value> The <see cref="BusinessObjectBinding"/> instance used to manage this control's binding. </value>
    [Browsable (false)]
    public BusinessObjectBinding Binding
    {
      get { return _binding; }
    }

    /// <summary>Gets or sets the <see cref="IBusinessObjectDataSource"/> this <see cref="IBusinessObjectBoundWebControl"/> is bound to.</summary>
    /// <value> An <see cref="IBusinessObjectDataSource"/> providing the current <see cref="IBusinessObject"/>. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public IBusinessObjectDataSource DataSource
    {
      get { return _binding.DataSource; }
      set { _binding.DataSource = value; }
    }

    /// <summary>Gets or sets the string representation of the <see cref="Property"/>.</summary>
    /// <value> 
    ///   A string that can be used to query the <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for the 
    ///   <see cref="IBusinessObjectProperty"/>. 
    /// </value>
    [Category ("Data")]
    [Description ("The string representation of the Property.")]
    [Editor (typeof (PropertyPickerEditor), typeof (UITypeEditor))]
    [DefaultValue ("")]
    [MergableProperty (false)]
    public string PropertyIdentifier
    {
      get { return _binding.PropertyIdentifier; }
      set { _binding.PropertyIdentifier = value; }
    }

    /// <summary>Gets or sets the <see cref="IBusinessObjectProperty"/> used for accessing the data to be loaded into <see cref="Value"/>.</summary>
    /// <value>An <see cref="IBusinessObjectProperty"/> that is part of the bound <see cref="IBusinessObject"/>'s <see cref="IBusinessObjectClass"/>.</value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectProperty Property
    {
      get { return _binding.Property; }
      set { _binding.Property = value; }
    }

    /// <summary>
    ///   Gets or sets the <b>ID</b> of the <see cref="IBusinessObjectDataSourceControl"/> encapsulating the <see cref="IBusinessObjectDataSource"/> 
    ///   this  <see cref="IBusinessObjectBoundWebControl"/> is bound to.
    /// </summary>
    /// <value>A string set to the <b>ID</b> of an <see cref="IBusinessObjectDataSourceControl"/> inside the current naming container.</value>
    [TypeConverter (typeof (BusinessObjectDataSourceControlConverter))]
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Data")]
    [Description ("The ID of the BusinessObjectDataSourceControl control used as data source.")]
    [DefaultValue ("")]
    public string DataSourceControl
    {
      get { return _binding.DataSourceControl; }
      set { _binding.DataSourceControl = value; }
    }

    /// <summary>Tests whether this <see cref="BusinessObjectBoundWebControl"/> can be bound to the <paramref name="property"/>.</summary>
    /// <param name="property">The <see cref="IBusinessObjectProperty"/> to be tested. Must not be <see langword="null"/>.</param>
    /// <returns>
    ///   <list type="bullet">
    ///     <item><see langword="true"/> is <see cref="SupportedPropertyInterfaces"/> is null.</item>
    ///     <item><see langword="false"/> if the <see cref="DataSource"/> is in <see cref="DataSourceMode.Search"/> mode.</item>
    ///     <item>Otherwise, <see langword="IsPropertyInterfaceSupported"/> is evaluated and returned as result.</item>
    ///   </list>
    /// </returns>
    public virtual bool SupportsProperty (IBusinessObjectProperty property)
    {
      return _binding.SupportsProperty (property);
    }

    /// <summary>Gets a flag specifying whether the <see cref="IBusinessObjectBoundControl"/> has a valid binding configuration.</summary>
    /// <remarks>
    ///   The configuration is considered invalid if data binding is configured for a property that is not available for the bound class or object.
    /// </remarks>
    /// <value> 
    ///   <list type="bullet">
    ///     <item><see langword="true"/> if the <see cref="DataSource"/> or the <see cref="Property"/> is <see langword="null"/>.</item>
    ///     <item>The result of the <see cref="IBusinessObjectProperty.IsAccessible">IBusinessObjectProperty.IsAccessible</see> method.</item>
    ///     <item>Otherwise, <see langword="false"/> is returned.</item>
    ///   </list>
    /// </value>
    [Browsable (false)]
    public bool HasValidBinding
    {
      get { return _binding.HasValidBinding; }
    }

    #endregion

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectService"/> from the <paramref name="businessObjectProvider"/> and queries it for an <see cref="IconInfo"/> 
    ///   object.
    /// </summary>
    /// <param name="businessObject"> 
    ///   The <see cref="IBusinessObject"/> to be passed to the <see cref="IBusinessObjectWebUIService"/>'s 
    ///   <see cref="IBusinessObjectWebUIService.GetIcon"/> method.
    /// </param>
    /// <param name="businessObjectProvider"> 
    ///   The <see cref="IBusinessObjectProvider"/> to be used to get the <see cref="IconInfo"/> object. Must not be <see langowrd="null"/>. 
    /// </param>
    public static IconInfo GetIcon (IBusinessObject businessObject, IBusinessObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      var webUIService = businessObjectProvider.GetService<IBusinessObjectWebUIService> ();

      if (webUIService != null)
        return webUIService.GetIcon (businessObject);

      return null;
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectService"/> from the <paramref name="businessObjectProvider"/> and queries it for a <see cref="string"/>
    ///   to be used as tool-tip.
    /// </summary>
    /// <param name="businessObject"> 
    ///   The <see cref="IBusinessObject"/> to be passed to the <see cref="IBusinessObjectWebUIService"/>'s 
    ///   <see cref="IBusinessObjectWebUIService.GetIcon"/> method.
    /// </param>
    /// <param name="businessObjectProvider"> 
    ///   The <see cref="IBusinessObjectProvider"/> to be used to get the <see cref="IconInfo"/> object. Must not be <see langowrd="null"/>. 
    /// </param>
    public static string GetToolTip (IBusinessObject businessObject, IBusinessObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      var webUIService = businessObjectProvider.GetService<IBusinessObjectWebUIService>();

      if (webUIService != null)
        return webUIService.GetToolTip (businessObject);

      return null;
    }

    public static HelpInfo GetHelpInfo (IBusinessObjectBoundWebControl control)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (control.DataSource == null)
        return null;

      var businessObjectProvider = control.DataSource.BusinessObjectProvider;
      if (businessObjectProvider == null)
        return null;

      var webUIService = businessObjectProvider.GetService<IBusinessObjectWebUIService>();
      if (webUIService != null)
        return webUIService.GetHelpInfo (control, control.DataSource.BusinessObjectClass, control.Property, control.DataSource.BusinessObject);

      return null;
    }

    private BusinessObjectBinding _binding;
    /// <summary> Caches the <see cref="ResourceManagerSet"/> for this control. </summary>
    private ResourceManagerSet _cachedResourceManager;
    private bool _controlExistedInPreviousRequest = false;

    /// <summary> Creates a new instance of the BusinessObjectBoundWebControl type. </summary>
    public BusinessObjectBoundWebControl ()
    {
      _binding = new BusinessObjectBinding (this);
    }

    /// <remarks>Calls <see cref="Control.EnsureChildControls"/> and the <see cref="BusinessObjectBinding.EnsureDataSource"/> method.</remarks>
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      EnsureChildControls ();
      _binding.EnsureDataSource ();
      if (!IsDesignMode)
      {
        Page.RegisterRequiresControlState (this);
      }

      RegisterHtmlHeadContents (Context);
    }

    /// <value> 
    ///   <para>
    ///     The <b>set accessor</b> passes the value to the base class's <b>Visible</b> property.
    ///   </para><para>
    ///     The <b>get accessor</b> ANDs the base class's <b>Visible</b> setting with the value of the <see cref="HasValidBinding"/> property.
    ///   </para>
    /// </value>
    /// <remarks>
    ///   The control only saves the set value of <b>Visible</b> into the view state. Therefor the control can change its visibilty during during 
    ///   subsequent postbacks.
    /// </remarks>
    public override bool Visible
    {
      get
      {
        if (!base.Visible)
          return false;

        if (IsDesignMode)
          return true;

        return HasValidBinding;
      }
      set { base.Visible = value; }
    }

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <seealso cref="IBusinessObjectBoundControl.LoadValue">IBusinessObjectBoundControl.LoadValue</seealso>
    public abstract void LoadValue (bool interim);

    /// <summary> Gets or sets the value provided by the <see cref="IBusinessObjectBoundControl"/>. </summary>
    /// <value> An object or boxed value. </value>
    /// <remarks>
    ///   <para>
    ///     Override <see cref="ValueImplementation"/> to define the behaviour of <c>Value</c>. 
    ///   </para><para>
    ///     Redefine <see cref="Value"/> using the keyword <see langword="new"/> to provide a typesafe implementation in derived classes.
    ///   </para>
    /// </remarks>
    [Browsable (false)]
    public object Value
    {
      get { return ValueImplementation; }
      set { ValueImplementation = value; }
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    [Browsable (false)]
    protected abstract object ValueImplementation { get; set; }

    /// <summary> Calls <see cref="Control.OnPreRender"/> on every invocation. </summary>
    /// <remarks> Used by the <see cref="BocDesigner"/>. </remarks>
    void IControlWithDesignTimeSupport.PreRenderForDesignMode ()
    {
      if (!IsDesignMode)
        throw new InvalidOperationException ("PreRenderChildControlsForDesignMode may only be called during design time.");
      EnsureChildControls ();
      OnPreRender (EventArgs.Empty);
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      bool tempEnabled = Enabled;
      if (!tempEnabled)
        Enabled = true;
      base.AddAttributesToRender (writer);
      if (!tempEnabled)
        Enabled = false;
    }

    /// <remarks> Calls <see cref="Control.EnsureChildControls"/>. </remarks>
    public override ControlCollection Controls
    {
      get
      {
        EnsureChildControls ();
        return base.Controls;
      }
    }

    Type[] IExtendedBusinessObjectBoundWebControl.SupportedPropertyInterfaces
    {
      get { return SupportedPropertyInterfaces; }
    }

    /// <summary>
    ///   Gets the interfaces derived from <see cref="IBusinessObjectProperty"/> supported by this control, or <see langword="null"/> if no 
    ///   restrictions are made.
    /// </summary>
    /// <value> <see langword="null"/> in the default implementation. </value>
    /// <remarks> Used by <see cref="SupportsProperty"/>. </remarks>
    [Browsable (false)]
    protected virtual Type[] SupportedPropertyInterfaces
    {
      get { return null; }
    }

    bool IExtendedBusinessObjectBoundWebControl.SupportsPropertyMultiplicity (bool isList)
    {
      return SupportsPropertyMultiplicity (isList);
    }

    /// <summary> Indicates whether properties with the specified multiplicity are supported. </summary>
    /// <remarks> Used by <see cref="SupportsProperty"/>. </remarks>
    /// <param name="isList"> <see langword="true"/> if the property is a list property. </param>
    /// <returns> <see langword="true"/> if the multiplicity specified by <paramref name="isList"/> is supported. </returns>
    protected virtual bool SupportsPropertyMultiplicity (bool isList)
    {
      return !isList;
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this control. </summary>
    /// <param name="localResourcesType"> 
    ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it. Typically an <b>enum</b> or the derived class itself.
    /// </param>
    protected IResourceManager GetResourceManager (Type localResourcesType)
    {
      Remotion.Utilities.ArgumentUtility.CheckNotNull ("localResourcesType", localResourcesType);

      //  Provider has already been identified.
      if (_cachedResourceManager != null)
        return _cachedResourceManager;

      //  Get the resource managers

      IResourceManager localResourceManager = MultiLingualResources.GetResourceManager (localResourcesType, true);
      IResourceManager namingContainerResourceManager = ResourceManagerUtility.GetResourceManager (NamingContainer, true);

      if (namingContainerResourceManager == null)
        _cachedResourceManager = new ResourceManagerSet (localResourceManager);
      else
        _cachedResourceManager = new ResourceManagerSet (localResourceManager, namingContainerResourceManager);

      return _cachedResourceManager;
    }

    /// <summary> Gets the text to be written into the label for this control. </summary>
    /// <value> <see langword="null"/> for the default implementation. </value>
    [Browsable (false)]
    public virtual string DisplayName
    {
      get { return (Property != null) ? Property.DisplayName : null; }
    }

    /// <summary>Regsiteres stylesheet and script files with the <see cref="HtmlHeadAppender"/>.</summary>
    public virtual void RegisterHtmlHeadContents (HttpContext context)
    {
    }

    /// <summary>Gets an instance of the <see cref="HelpInfo"/> type, which contains all information needed for rendering a help-link.</summary>
    [Browsable (false)]
    public virtual HelpInfo HelpInfo
    {
      get { return GetHelpInfo (this); }
    }

    /// <summary>Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its <see cref="Control.ClientID"/>.</summary>
    /// <value> This instance for the default implementation. </value>
    [Browsable (false)]
    public virtual Control TargetControl
    {
      get { return this; }
    }

    /// <summary>Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the <see cref="TargetControl"/>.</summary>
    /// <value>
    ///   <see langword="true"/> unsless the <see cref="TargetControl"/> is a <see cref="DropDownList"/> or an 
    ///   <see cref="System.Web.UI.HtmlControls.HtmlSelect"/> control.
    /// </value>
    [Browsable (false)]
    public virtual bool UseLabel
    {
      get { return !(TargetControl is DropDownList || TargetControl is System.Web.UI.HtmlControls.HtmlSelect); }
    }

    /// <summary> Evalutes whether this control is in <b>Design Mode</b>. </summary>
    [Browsable (false)]
    protected virtual bool IsDesignMode
    {
      get { return ControlHelper.IsDesignMode (this, Context); }
    }

    bool ISmartControl.IsRequired
    {
      get { return false; }
    }

    BaseValidator[] ISmartControl.CreateValidators ()
    {
      return new BaseValidator[0];
    }

    /// <summary> Gets a flag whether the control already existed in the previous page life cycle. </summary>
    /// <remarks> 
    ///   This property utilizes the <see cref="LoadControlState"/> method for determining a post back. 
    /// It is therefor only useful after the load control state phase of the page life cycle.
    /// </remarks>
    /// <value> <see langword="true"/> if the control has been on the page in the previous life cycle. </value>
    protected bool ControlExistedInPreviousRequest
    {
      get { return _controlExistedInPreviousRequest; }
    }

    protected override void LoadControlState (object savedState)
    {
      base.LoadControlState (savedState);
      _controlExistedInPreviousRequest = true;
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected virtual void LoadResources (IResourceManager resourceManager)
    {
      if (resourceManager == null)
        return;
      if (IsDesignMode)
        return;

      string key;
      key = ResourceManagerUtility.GetGlobalResourceKey (AccessKey);
      if (!StringUtility.IsNullOrEmpty (key))
        AccessKey = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (ToolTip);
      if (!StringUtility.IsNullOrEmpty (key))
        ToolTip = resourceManager.GetString (key);
    }

    //  /// <summary>
    //  ///   Occurs after either the <see cref="Property"/> property or the <see cref="PropertyIdentifier"/> property is assigned a new value.
    //  /// </summary>
    //  /// <remarks>
    //  ///   Note that this event does not occur if the property path is modified, only if a new one is assigned.
    //  /// </remarks>
    //  public event BindingChangedEventHandler BindingChanged;

    //  private bool _onLoadCalled = false;
    //  private bool _propertyBindingChangedBeforeOnLoad = false;

    //  protected override void OnLoad (EventArgs e)
    //  {
    //    base.OnLoad (e);
    //    _onLoadCalled = true;
    //    if (_propertyBindingChangedBeforeOnLoad)
    //      OnBindingChanged (null, null);
    //  }

    //  /// <summary>
    //  /// Raises the <see cref="PropertyChanged"/> event.
    //  /// </summary>
    //  protected virtual void OnBindingChanged (IBusinessObjectProperty previousProperty, IBusinessObjectDataSource previousDataSource)
    //  {
    //    if (! _onLoadCalled)
    //    {
    //      _propertyBindingChangedBeforeOnLoad = true;
    //      return;
    //    }
    //    if (BindingChanged != null)
    //      BindingChanged (this, new BindingChangedEventArgs (previousProperty, previousDataSource));
    //  }
  }

  ///// <summary>
  /////   Provides data for the <cBindingChanged</c> event.
  /////   <seealso cref="BusinessObjectBoundControl.BindingChanged"/>
  ///// </summary>
  //public class BindingChangedEventArgs: EventArgs
  //{
  //  /// <summary>
  //  ///   The value of the <c>PropertyPath</c> property before the change took place.
  //  /// </summary>
  //  public readonly IBusinessObjectProperty PreviousProperty;
  //  public readonly IBusinessObjectDataSource PreviuosDataSource;
  //
  //  public BindingChangedEventArgs (IBusinessObjectProperty previousProperty, IBusinessObjectDataSource previousDataSource)
  //  {
  //    PreviousProperty = previousProperty;
  //    PreviuosDataSource = previousDataSource;
  //  }
  //}
  //
  //public delegate void BindingChangedEventHandler (object sender, BindingChangedEventArgs e);

}
