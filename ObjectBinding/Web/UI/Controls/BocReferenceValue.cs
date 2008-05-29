using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Logging;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
/// <summary> This control can be used to display or edit reference values. </summary>
/// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/Class/*' />
// TODO: see "Doc\Bugs and ToDos.txt"
[ValidationProperty ("BusinessObjectID")]
[DefaultEvent ("SelectionChanged")]
[ToolboxItemFilter("System.Web.UI")]
[Designer (typeof (BocReferenceValueDesigner))]
public class BocReferenceValue: 
    BusinessObjectBoundEditableWebControl, 
    IPostBackEventHandler, 
    IPostBackDataHandler,
    IBocMenuItemContainer,
    IResourceDispatchTarget, 
    IFocusableControl
{
  // constants
	
  private const string c_nullIdentifier = "==null==";

  /// <summary> The text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
  private const string c_designModeEmptyLabelContents = "##";
  private const string c_defaultControlWidth = "150pt";

  private const string c_scriptFileUrl = "BocReferenceValue.js";
  private const string c_styleFileUrl = "BocReferenceValue.css";

  /// <summary> The key identifying a options menu item resource entry. </summary>
  private const string c_resourceKeyOptionsMenuItems = "OptionsMenuItems";
  /// <summary> The key identifying the command resource entry. </summary>
  private const string c_resourceKeyCommand = "Command";

  // types

  /// <summary> A list of control specific resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocReferenceValue")]
  protected enum ResourceIdentifier
  {
    /// <summary> Label displayed in the OptionsMenu. </summary>
    OptionsTitle,
    /// <summary> The validation error message displayed when the null item is selected. </summary>
    NullItemValidationMessage
  }

  // static members

  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { 
      typeof (IBusinessObjectReferenceProperty) };
  
  private static readonly ILog s_log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  private static readonly object s_selectionChangedEvent = new object();
  private static readonly object s_menuItemClickEvent = new object();
  private static readonly object s_commandClickEvent = new object();

  private static readonly string s_scriptFileKey = typeof (BocReferenceValue).FullName + "_Script";
  private static readonly string s_startUpScriptKey = typeof (BocReferenceValue).FullName+ "_Startup";
  private static readonly string s_styleFileKey = typeof (BocReferenceValue).FullName + "_Style";

	// member fields

  private bool _isBusinessObejectListPopulated = false;

  private DropDownList _dropDownList;
  private Label _label;
  private Image _icon ;

  private Style _commonStyle;
  private DropDownListStyle _dropDownListStyle;
  private Style _labelStyle;

  /// <summary> 
  ///   The object returned by <see cref="BocReferenceValue"/>. 
  ///   Does not require <see cref="System.Runtime.Serialization.ISerializable"/>. 
  /// </summary>
  private IBusinessObjectWithIdentity _value = null;

  /// <summary> The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the current object. </summary>
  private string _internalValue = null;
  private string _displayName = null;

  private bool _enableIcon = true;
  private string _select = String.Empty;
  private bool? _enableSelectStatement = null;

  private DropDownMenu _optionsMenu;
  private string _optionsTitle;
  private bool _showOptionsMenu = true;
  private Unit _optionsMenuWidth = Unit.Empty;
  private bool? _hasValueEmbeddedInsideOptionsMenu = null;
  private string[] _hiddenMenuItems = null;

  /// <summary> The command rendered for this reference value. </summary>
  private SingleControlItemCollection _command = null;

  private string _errorMessage;
  private ArrayList _validators;

  // construction and disposing

	public BocReferenceValue()
	{
    _commonStyle = new Style();
    _dropDownListStyle = new DropDownListStyle();
    _labelStyle = new Style();
    _icon = new Image();
    _dropDownList = new DropDownList();
    _label = new Label();
    _optionsMenu = new DropDownMenu (this);
    _validators = new ArrayList();
    _command = new SingleControlItemCollection (new BocCommand(), new Type[] {typeof (BocCommand)});
  }

	// methods and properties

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);

    if (!IsDesignMode)
    {
      Page.RegisterRequiresPostBack (this);
      InitializeMenusItems();
    }
  }

  public override void RegisterHtmlHeadContents (HttpContext context)
  {
    base.RegisterHtmlHeadContents (context);

    if (!HtmlHeadAppender.Current.IsRegistered (s_scriptFileKey))
    {
      string scriptUrl = ResourceUrlResolver.GetResourceUrl (this, context, typeof (BocReferenceValue), ResourceType.Html, c_scriptFileUrl);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);
    }

    if (!HtmlHeadAppender.Current.IsRegistered (s_styleFileKey))
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          this, Context, typeof (BocReferenceValue), ResourceType.Html, c_styleFileUrl);
      HtmlHeadAppender.Current.RegisterStylesheetLink (s_styleFileKey, url, HtmlHeadAppender.Priority.Library);
    }
  }

  /// <remarks>
  ///   If the <see cref="DropDownList"/> could not be created from <see cref="DropDownListStyle"/>,
  ///   the control is set to read-only.
  /// </remarks>
  protected override void CreateChildControls()
  {
    _icon.ID = ID + "_Boc_Icon";
    _icon.EnableViewState = false;
    Controls.Add (_icon);

    _dropDownList.ID = ID + "_Boc_DropDownList";
    _dropDownList.EnableViewState = false;
    ((IStateManager) _dropDownList.Items).TrackViewState ();
    Controls.Add (_dropDownList);

    _label.ID = ID + "_Boc_Label";
    _label.EnableViewState = false;
    Controls.Add (_label);
    
    _optionsMenu.ID = ID + "_Boc_OptionsMenu";
    Controls.Add (_optionsMenu);
    _optionsMenu.EventCommandClick += new WebMenuItemClickEventHandler (OptionsMenu_EventCommandClick);
    _optionsMenu.WxeFunctionCommandClick += new WebMenuItemClickEventHandler (OptionsMenu_WxeFunctionCommandClick);
  }

  /// <remarks> Populates the list. </remarks>
  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    if (! ControlExistedInPreviousRequest)
      EnsureBusinessObjectListPopulated();
  }

  /// <summary> Invokes the <see cref="LoadPostData"/> method. </summary>
  bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
  {
    if (RequiresLoadPostData)
      return LoadPostData (postDataKey, postCollection);
    else
      return false;
  }

  /// <summary> Invokes the <see cref="RaisePostDataChangedEvent"/> method. </summary>
  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
    RaisePostDataChangedEvent();
  }

  /// <summary>
  ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed
  ///   between postbacks.
  /// </summary>
  /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadPostData/*' />
  protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
  {
    string newValue = PageUtility.GetPostBackCollectionItem (Page, _dropDownList.UniqueID);
    bool isDataChanged = false;
    if (newValue != null)
    {
      if (_internalValue == null && newValue != c_nullIdentifier)
        isDataChanged = true;
      else if (_internalValue != null && newValue != _internalValue)
        isDataChanged = true;
    }

    if (isDataChanged)
    {
      if (newValue == c_nullIdentifier)
        InternalValue = null;
      else
        InternalValue = newValue;
      IsDirty = true;
    }
    return isDataChanged;
  }

  /// <summary> Called when the state of the control has changed between postbacks. </summary>
  protected virtual void RaisePostDataChangedEvent()
  {
    if (_internalValue == null)
      _displayName = null;
    else
      _displayName = _dropDownList.SelectedItem.Text;

    if (! IsReadOnly && Enabled)
      OnSelectionChanged();
  }

  /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
  protected virtual void OnSelectionChanged ()
  {
    EventHandler eventHandler = (EventHandler) Events[s_selectionChangedEvent];
    if (eventHandler != null)
      eventHandler (this, EventArgs.Empty);
  }

  /// <summary> Calls the <see cref="RaisePostBackEvent"/> method. </summary>
  void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
  {
    RaisePostBackEvent (eventArgument);
  }

  /// <summary> Called when the control caused a post back. </summary>
  /// <param name="eventArgument"> an empty <see cref="String"/>. </param>
  protected virtual void RaisePostBackEvent (string eventArgument)
  {
    HandleCommand();
  }

  /// <summary> Handles post back events raised by the value's <see cref="Command"/>. </summary>
  private void HandleCommand()
  {
    switch (Command.Type)
    {
      case CommandType.Event:
      {
        OnCommandClick (Value);
        break;
      }
      case CommandType.WxeFunction:
      {
        if (Page is IWxePage)
          Command.ExecuteWxeFunction ((IWxePage) Page, Value);
        //else
        //  Command.ExecuteWxeFunction (Page, Value);
        break;
      }
      default:
      {
        break;
      }
    }
  }

  /// <summary> Fires the <see cref="CommandClick"/> event. </summary>
  /// <param name="businessObject"> 
  ///   The current <see cref="Value"/>, which corresponds to the clicked <see cref="IBusinessObjectWithIdentity"/>,
  ///   unless somebody changed the <see cref="Value"/> in the code behind before the event fired.
  /// </param>
  protected virtual void OnCommandClick (IBusinessObjectWithIdentity businessObject)
  {
    if (Command != null)
    {
      Command.OnClick (businessObject);
      BocCommandClickEventHandler commandClickHandler = (BocCommandClickEventHandler) Events[s_commandClickEvent];
      if (commandClickHandler != null)
      {
        BocCommandClickEventArgs e = new BocCommandClickEventArgs (Command, businessObject);
        commandClickHandler (this, e);
      }
    }
  }

  /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  void IResourceDispatchTarget.Dispatch (IDictionary values)
  {
    ArgumentUtility.CheckNotNull ("values", values);
    Dispatch (values);
  }

  /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
  /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
  protected virtual void Dispatch  (IDictionary values)
  {
    HybridDictionary optionsMenuItemValues = new HybridDictionary();
    HybridDictionary propertyValues = new HybridDictionary();
    HybridDictionary commandValues = new HybridDictionary();

    //  Parse the values

    foreach (DictionaryEntry entry in values)
    {
      string key = (string) entry.Key;
      string[] keyParts = key.Split (new Char[] {':'}, 3);

      //  Is a property/value entry?
      if (keyParts.Length == 1)
      {
        string property = keyParts[0];
        propertyValues.Add (property, entry.Value);
      }
        //  Is compound element entry
      else if (keyParts.Length == 2)
      {
        //  Compound key: "elementID:property"
        string elementID = keyParts[0];
        string property = keyParts[1];

         //  Switch to the right collection
        switch (elementID)
        {
          case c_resourceKeyCommand:
          {
            commandValues.Add (property, entry.Value);
            break;
          }
          default:
          {
            //  Invalid collection property
            s_log.Debug ("BocReferenceValue '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain an element named '" + elementID + "'.");
            break;
          }
        }       
      }
        //  Is collection entry?
      else if (keyParts.Length == 3)
      {    
        //  Compound key: "collectionID:elementID:property"
        string collectionID = keyParts[0];
        string elementID = keyParts[1];
        string property = keyParts[2];

        IDictionary currentCollection = null;

        //  Switch to the right collection
        switch (collectionID)
        {
          case c_resourceKeyOptionsMenuItems:
          {
            currentCollection = optionsMenuItemValues;
            break;
          }
          default:
          {
            //  Invalid collection property
            s_log.Debug ("BocReferenceValue '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' does not contain a collection property named '" + collectionID + "'.");
            break;
          }
        }       

        //  Add the property/value pair to the collection
        if (currentCollection != null)
        {
          //  Get the dictonary for the current element
          IDictionary elementValues = (IDictionary) currentCollection[elementID];

          //  If no dictonary exists, create it and insert it into the elements hashtable.
          if (elementValues == null)
          {
            elementValues = new HybridDictionary();
            currentCollection[elementID] = elementValues;
          }

          //  Insert the argument and resource's value into the dictonary for the specified element.
          elementValues.Add (property, entry.Value);
        }
      }
      else
      {
        //  Not supported format or invalid property
        s_log.Debug ("BocReferenceValue '" + ID + "' in naming container '" + NamingContainer.GetType().FullName + "' on page '" + Page.ToString() + "' received a resource with an invalid or unknown key '" + key + "'. Required format: 'property' or 'collectionID:elementID:property'.");
      }
    }

    //  Dispatch simple properties
    ResourceDispatcher.DispatchGeneric (this, propertyValues);

    //  Dispatch compound element properties
    if (Command != null)
      ResourceDispatcher.DispatchGeneric (Command, commandValues);

    //  Dispatch to collections
    OptionsMenuItems.Dispatch (optionsMenuItemValues, this, "OptionsMenuItems");
  }

  /// <summary> Loads the resources into the control's properties. </summary>
  protected override void LoadResources  (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;
    if (IsDesignMode)
      return;
    base.LoadResources (resourceManager);

    string key;
    key = ResourceManagerUtility.GetGlobalResourceKey (Select);
    if (! StringUtility.IsNullOrEmpty (key))
      Select = resourceManager.GetString (key);
    
    key = ResourceManagerUtility.GetGlobalResourceKey (OptionsTitle);
    if (! StringUtility.IsNullOrEmpty (key))
      OptionsTitle = resourceManager.GetString (key);

    key = ResourceManagerUtility.GetGlobalResourceKey (ErrorMessage);
    if (! StringUtility.IsNullOrEmpty (key))
      ErrorMessage = resourceManager.GetString (key);

    if (Command != null)
      Command.LoadResources (resourceManager);

    OptionsMenuItems.LoadResources (resourceManager);
  }

  /// <summary> Checks whether the control conforms to the required WAI level. </summary>
  /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
  protected virtual void EvaluateWaiConformity()
  {
    if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
    {
      if (_showOptionsMenu)
        WcagHelper.Instance.HandleError (1, this, "ShowOptionsMenu");
      bool hasPostBackCommand =     Command != null
                                && (   Command.Type == CommandType.Event 
                                    || Command.Type == CommandType.WxeFunction);
      if (hasPostBackCommand)
        WcagHelper.Instance.HandleError (1, this, "Command");

      if (DropDownListStyle.AutoPostBack == true)
        WcagHelper.Instance.HandleWarning (1, this, "DropDownListStyle.AutoPostBack");

      if (DropDownList.AutoPostBack)
        WcagHelper.Instance.HandleWarning (1, this, "DropDownList.AutoPostBack");
    }
  }

  public override void PrepareValidation ()
  {
    base.PrepareValidation();

    if (! IsReadOnly)
      SetEditModeValue ();
  }

  private void SetEditModeValue ()
  {
    bool isNullItem = InternalValue == null;

    if (isNullItem || ! IsRequired)
    {
      //  No null item in the list?
      if (_dropDownList.Items.FindByValue (c_nullIdentifier) == null)
        _dropDownList.Items.Insert (0, CreateNullItem());
    }

    //  Check if null item is to be selected
    if (isNullItem)
    {
      _dropDownList.SelectedValue = c_nullIdentifier;
    }
    else
    {
      if (_dropDownList.Items.FindByValue (InternalValue) != null)
      {
        _dropDownList.SelectedValue = InternalValue;
      }
      else if (Value != null)
      {
        //  Item not yet in the list but is a valid item.
        IBusinessObjectWithIdentity businessObject = Value;

        ListItem item = new ListItem (GetDisplayName (businessObject), businessObject.UniqueIdentifier);
        _dropDownList.Items.Add (item);

        _dropDownList.SelectedValue = InternalValue;
      }
    }
  }

  protected override void OnPreRender (EventArgs e)
  {
    EnsureChildControls();
    base.OnPreRender (e);

    if (!IsDesignMode && !Page.ClientScript.IsStartupScriptRegistered (s_startUpScriptKey))
    {
      const string script = "BocReferenceValue_InitializeGlobals ('" + c_nullIdentifier + "');";
      ScriptUtility.RegisterStartupScriptBlock (Page, s_startUpScriptKey, script);
    }

    LoadResources (GetResourceManager());

    if (!IsDesignMode)
    {
      PreRenderMenuItems();
    }

    PreRenderIcon();

    if (HasOptionsMenu)
      PreRenderOptionsMenu();

    if (IsReadOnly)
      PreRenderReadOnlyValue();
    else
      PreRenderEditModeValue();

    if (Command != null)
      Command.RegisterForSynchronousPostBack (this, null, string.Format ("BocReferenceValue '{0}', Object Command", ID));
  }

  /// <summary> Gets a <see cref="HtmlTextWriterTag.Div"/> as the <see cref="WebControl.TagKey"/>. </summary>
  protected override HtmlTextWriterTag TagKey
  {
    get { return HtmlTextWriterTag.Div; }
  }

  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    bool isReadOnly = IsReadOnly;
    bool isDisabled = ! Enabled;

    string backUpCssClass = CssClass; // base.CssClass and base.ControlStyle.CssClass
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (CssClass))
    {
      if (isReadOnly)
        CssClass += " " + CssClassReadOnly;
      else if (isDisabled)
        CssClass += " " + CssClassDisabled;
    }
    string backUpAttributeCssClass = Attributes["class"];
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (Attributes["class"]))
    {
      if (isReadOnly)
        Attributes["class"] += " " + CssClassReadOnly;
      else if (isDisabled)
        Attributes["class"] += " " + CssClassDisabled;
    }
    
    base.AddAttributesToRender (writer);

    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (CssClass))
      CssClass = backUpCssClass;
    if ((isReadOnly || isDisabled) && ! StringUtility.IsNullOrEmpty (Attributes["class"]))
      Attributes["class"] = backUpAttributeCssClass;
    
    if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
    {
      string cssClass = CssClassBase;
      if (isReadOnly)
        cssClass += " " + CssClassReadOnly;
      else if (isDisabled)
        cssClass += " " + CssClassDisabled;
      writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
    }

    writer.AddStyleAttribute ("display", "inline");
  }

  protected override void RenderContents (HtmlTextWriter writer)
  {
    EvaluateWaiConformity();

    if (   _hasValueEmbeddedInsideOptionsMenu == true && HasOptionsMenu
        || _hasValueEmbeddedInsideOptionsMenu == null && IsReadOnly && HasOptionsMenu)
    {
      RenderContentsWithIntegratedOptionsMenu (writer);
    }
    else
    {
      RenderContentsWithSeparateOptionsMenu (writer);
    }
  }

  private void RenderContentsWithSeparateOptionsMenu (HtmlTextWriter writer)
  {
    bool isReadOnly = IsReadOnly;

    bool isControlHeightEmpty = Height.IsEmpty && StringUtility.IsNullOrEmpty (Style["height"]);
    bool isDropDownListHeightEmpty =   _dropDownList.Height.IsEmpty 
                                    && StringUtility.IsNullOrEmpty (_dropDownList.Style["height"]);
    bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
    bool isLabelWidthEmpty =    _label.Width.IsEmpty 
                             && StringUtility.IsNullOrEmpty (_label.Style["width"]);
    bool isDropDownListWidthEmpty =    _dropDownList.Width.IsEmpty 
                                    && StringUtility.IsNullOrEmpty (_dropDownList.Style["width"]);
    if (isReadOnly)
    {
      if (isLabelWidthEmpty && ! isControlWidthEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    }
    else
    {
      if (! isControlHeightEmpty && isDropDownListHeightEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    
      if (isDropDownListWidthEmpty)
      {
        if (isControlWidthEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }
    }

    writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
    writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
    writer.AddStyleAttribute ("display", "inline");
    writer.RenderBeginTag (HtmlTextWriterTag.Table);  // Begin table
    writer.RenderBeginTag (HtmlTextWriterTag.Tr); //  Begin tr

    bool isCommandEnabled = IsCommandEnabled (isReadOnly);

    string argument = string.Empty;
    string postBackEvent = null; 
    if (! IsDesignMode)
      postBackEvent = Page.ClientScript.GetPostBackEventReference (this, argument) + ";";
    string objectID = string.Empty;
    if (InternalValue != c_nullIdentifier)
      objectID = InternalValue;

    if (isReadOnly)
    {
      RenderReadOnlyValue (writer, isCommandEnabled, postBackEvent, string.Empty, objectID);
    }
    else
    {
      if (_icon.Visible)
        RenderSeparateIcon (writer, isCommandEnabled, postBackEvent, string.Empty, objectID);
      RenderEditModeValue (writer, isControlHeightEmpty, isDropDownListHeightEmpty, isDropDownListWidthEmpty);
    }

    bool hasOptionsMenu = HasOptionsMenu;
    if (hasOptionsMenu)
    {
      writer.AddStyleAttribute ("padding-left", "0.3em");
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      //writer.AddAttribute ("align", "right");
      writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
      _optionsMenu.Width = _optionsMenuWidth;
      _optionsMenu.RenderControl (writer);
      writer.RenderEndTag();  //  End td
    }

    //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
    if (   !IsDesignMode && !isReadOnly && !hasOptionsMenu && !_icon.Visible 
        && Context.Request.Browser.Browser == "Opera")
    {
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
      writer.Write ("&nbsp;");
      writer.RenderEndTag(); // End td
    }

    writer.RenderEndTag();
    writer.RenderEndTag();
  }

  private void RenderContentsWithIntegratedOptionsMenu (HtmlTextWriter writer)
  {
    bool isReadOnly = IsReadOnly;

    bool isControlHeightEmpty = Height.IsEmpty && StringUtility.IsNullOrEmpty (Style["height"]);
    bool isDropDownListHeightEmpty = StringUtility.IsNullOrEmpty (_dropDownList.Style["height"]);
    bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
    bool isLabelWidthEmpty = StringUtility.IsNullOrEmpty (_label.Style["width"]);
    bool isDropDownListWidthEmpty = StringUtility.IsNullOrEmpty (_dropDownList.Style["width"]);

    if (isReadOnly)
    {
      if (isLabelWidthEmpty && ! isControlWidthEmpty)
        _optionsMenu.Style["width"] = "100%";
      else
        _optionsMenu.Style["width"] = "0%";
    }
    else
    {
      if (! isControlHeightEmpty && isDropDownListHeightEmpty)
        _optionsMenu.Style["height"] = "100%";
    
      if (isDropDownListWidthEmpty)
      {
        if (isControlWidthEmpty)
          _optionsMenu.Style["width"] = c_defaultControlWidth;
        else
          _optionsMenu.Style["width"] = "100%";
      }
    }

    _optionsMenu.SetRenderHeadTitleMethodDelegate (new RenderMethod (RenderOptionsMenuTitle));
    _optionsMenu.RenderControl (writer);
    _optionsMenu.SetRenderHeadTitleMethodDelegate (null);
  }

  private void RenderOptionsMenuTitle (HtmlTextWriter writer, Control control)
  {
    bool isReadOnly = IsReadOnly;

    bool isControlHeightEmpty = Height.IsEmpty && StringUtility.IsNullOrEmpty (Style["height"]);
    bool isDropDownListHeightEmpty = StringUtility.IsNullOrEmpty (_dropDownList.Style["height"]);
    bool isControlWidthEmpty = Width.IsEmpty && StringUtility.IsNullOrEmpty (Style["width"]);
    bool isLabelWidthEmpty = StringUtility.IsNullOrEmpty (_label.Style["width"]);
    bool isDropDownListWidthEmpty = StringUtility.IsNullOrEmpty (_dropDownList.Style["width"]);

    bool isCommandEnabled = IsCommandEnabled (isReadOnly);

    string argument = string.Empty;
    string postBackEvent = Page.ClientScript.GetPostBackEventReference (this, argument) + ";";
    string objectID = string.Empty;
    if (InternalValue != c_nullIdentifier)
      objectID = InternalValue;


    if (isReadOnly)
    {
      RenderReadOnlyValue (writer, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
      if (! isControlWidthEmpty)
      {
        writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");
        writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        writer.RenderEndTag();
      }
    }
    else
    {
      if (_icon.Visible)
        RenderSeparateIcon (writer, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
      _dropDownList.Attributes.Add ("onClick", DropDownMenu.OnHeadTitleClickScript);
      RenderEditModeValue (writer, isControlHeightEmpty, isDropDownListHeightEmpty, isDropDownListWidthEmpty);
    }
  }

  private void RenderSeparateIcon (
      HtmlTextWriter writer, 
      bool isCommandEnabled, 
      string postBackEvent, 
      string onClick,
      string objectID)
  {
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
    writer.AddStyleAttribute ("padding-right", "0.3em");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

    if (isCommandEnabled)
    {
      Command.RenderBegin (writer, postBackEvent, onClick, objectID, null);
      if (! StringUtility.IsNullOrEmpty (Command.ToolTip))
        _icon.ToolTip = Command.ToolTip;
    }
    _icon.RenderControl (writer);
    if (isCommandEnabled)
      Command.RenderEnd (writer);

    writer.RenderEndTag();  //  End td
  }

  private void RenderReadOnlyValue (
      HtmlTextWriter writer, 
      bool isCommandEnabled, 
      string postBackEvent, 
      string onClick,
      string objectID)
  {
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

    if (isCommandEnabled)
      Command.RenderBegin (writer, postBackEvent, onClick, objectID, null);
    if (_icon.Visible)
    {
      _icon.RenderControl (writer);
      writer.Write ("&nbsp;");
    }
    _label.RenderControl (writer);
    if (isCommandEnabled)
      Command.RenderEnd (writer);
    
    writer.RenderEndTag();  //  End td
  }

  private void RenderEditModeValue (
      HtmlTextWriter writer, 
      bool isControlHeightEmpty, 
      bool isDropDownListHeightEmpty,
      bool isDropDownListWidthEmpty)
  {
    writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
    writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

    if (! isControlHeightEmpty && isDropDownListHeightEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
    if (isDropDownListWidthEmpty)
      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
    _dropDownList.RenderControl (writer);
    
    writer.RenderEndTag();  //  End td

    RenderEditModeValueExtension (writer);
  }

  /// <summary> Called after the edit mode value's cell is rendered. </summary>
  /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
  protected virtual void RenderEditModeValueExtension (HtmlTextWriter writer)
  {
  }

  protected bool IsCommandEnabled (bool isReadOnly)
  {
    if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
      return false;

    bool isCommandEnabled = false;
    if (Command != null)
    {
      bool isActive =    Command.Show == CommandShow.Always
                      || isReadOnly && Command.Show == CommandShow.ReadOnly
                      || ! isReadOnly && Command.Show == CommandShow.EditMode;
      bool isCommandLinkPossible = (IsReadOnly || _icon.Visible) && InternalValue != null;
      if (   isActive
          && Command.Type != CommandType.None
          && isCommandLinkPossible)
      {
        isCommandEnabled = Enabled;
      }
    }
    return isCommandEnabled;
  }

  protected override void LoadControlState (object savedState)
  {
    object[] values = (object[]) savedState;

    base.LoadControlState (values[0]);
    if (values[1] != null)    
      InternalValue = (string) values[1];
    _displayName = (string) values[2];
    ((IStateManager) _dropDownList.Items).LoadViewState (values[3]);
    _dropDownList.SelectedIndex = (int) values[4];
  }

  protected override object SaveControlState ()
  {
    object[] values = new object[5];

    values[0] = base.SaveControlState ();
    values[1] = _internalValue;
    values[2] = _displayName;
    values[3] = ((IStateManager) _dropDownList.Items).SaveViewState ();
    values[4] = _dropDownList.SelectedIndex;

    return values;
  }


  /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadValue/*' />
  public override void LoadValue (bool interim)
  {
    if (! interim)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null)
      {
        IBusinessObjectWithIdentity value = 
            (IBusinessObjectWithIdentity) DataSource.BusinessObject.GetProperty (Property);
        LoadValueInternal (value, interim);
      }
    }
  }

  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <param name="value"> 
  ///   The object implementing <see cref="IBusinessObjectWithIdentity"/> to load, or <see langword="null"/>. 
  /// </param>
  /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadUnboundValue/*' />
  public void LoadUnboundValue (IBusinessObjectWithIdentity value, bool interim)
  {
    LoadValueInternal (value, interim);
  }

  /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
  protected virtual void LoadValueInternal (IBusinessObjectWithIdentity value, bool interim)
  {
    if (! interim)
    {
      Value = value;
      IsDirty = false;
    }
  }

  /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/SaveValue/*' />
  public override void SaveValue (bool interim)
  {
    if (!interim && IsDirty)
    {
      if (Property != null && DataSource != null && DataSource.BusinessObject != null && ! IsReadOnly)
      {
        DataSource.BusinessObject.SetProperty (Property, Value);
        IsDirty = false;
      }
    }
  }

  /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
  protected virtual IResourceManager GetResourceManager()
  {
    return GetResourceManager (typeof (ResourceIdentifier));
  }

  /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
  /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/CreateValidators/*' />
  public override BaseValidator[] CreateValidators()
  {
    if (IsReadOnly || ! IsRequired)
      return new BaseValidator[0];

    BaseValidator[] validators = new BaseValidator[1];
    
    CompareValidator notNullItemValidator = new CompareValidator();
    notNullItemValidator.ID = ID + "_ValidatorNotNullItem";
    notNullItemValidator.ControlToValidate = TargetControl.ID;
    notNullItemValidator.ValueToCompare = c_nullIdentifier;
    notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
    if (StringUtility.IsNullOrEmpty (_errorMessage))
    {
      notNullItemValidator.ErrorMessage = 
          GetResourceManager().GetString (ResourceIdentifier.NullItemValidationMessage);
    }
    else
    {
      notNullItemValidator.ErrorMessage = _errorMessage;
    }      
    validators[0] = notNullItemValidator;

    _validators.AddRange (validators);
    return validators;
  }

  /// <summary> Sets the <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode. </summary>
  /// <remarks>
  ///   Use this method to set the listed items, e.g. from the parent control, if no <see cref="Select"/>
  ///   statement was provided.
  /// </remarks>
  /// <param name="businessObjects">
  ///   An array of <see cref="IBusinessObjectWithIdentity"/> objects to be used as list of possible values.
  ///   Must not be <see langword="null"/>.
  /// </param>
  public void SetBusinessObjectList (IBusinessObjectWithIdentity[] businessObjects)
  {
    ArgumentUtility.CheckNotNull ("businessObjects", businessObjects);
    RefreshBusinessObjectList (businessObjects);
  }

  /// <summary> Sets the <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode. </summary>
  /// <remarks>
  ///   Use this method to set the listed items, e.g. from the parent control, if no <see cref="Select"/>
  ///   statement was provided.
  /// </remarks>
  /// <param name="businessObjects">
  ///   An <see cref="IList"/> of <see cref="IBusinessObjectWithIdentity"/> objects to be used as list of possible 
  ///   values. Must not be <see langword="null"/>.
  /// </param>
  public void SetBusinessObjectList (IList businessObjects)
  {
    ArgumentUtility.CheckNotNull ("businessObjects", businessObjects);
    ArgumentUtility.CheckItemsNotNullAndType ("businessObjects", businessObjects, typeof (IBusinessObjectWithIdentity));
    RefreshBusinessObjectList (businessObjects);
  }

  /// <summary> Clears the list of <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode. </summary>
  /// <remarks> If the value is not required, the null item will displayed anyway. </remarks>
  public void ClearBusinessObjectList()
  {
    RefreshBusinessObjectList (null);
  }

  /// <summary> Calls <see cref="PopulateBusinessObjectList"/> if the list has not yet been populated. </summary>
  protected void EnsureBusinessObjectListPopulated()
  {
    if (_isBusinessObejectListPopulated)
      return;
    PopulateBusinessObjectList();
  }


  /// <summary>
  ///   Queries <see cref="IBusinessObjectReferenceProperty.SearchAvailableObjects"/> for the
  ///   <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode and sets the list with the
  ///   objects returned by the query.
  /// </summary>
  /// <remarks> 
  ///   <para>
  ///     Uses the <see cref="Select"/> statement to query the <see cref="Property"/>'s 
  ///     <see cref="IBusinessObjectReferenceProperty.SearchAvailableObjects"/> method for the list contents.
  ///   </para><para>
  ///     Only populates the list if <see cref="EnableSelectStatement"/> is not <see langword="false"/>.
  ///     Otherwise the list will be left empty.
  ///   </para>  
  /// </remarks>
  protected void PopulateBusinessObjectList()
  {
    if (! IsSelectStatementEnabled)
      return;

    if (Property == null)
      return;

    IBusinessObject[] businessObjects = null;

    //  Get all matching business objects
    if (DataSource != null)
      businessObjects = Property.SearchAvailableObjects (DataSource.BusinessObject, _select);

    RefreshBusinessObjectList (ArrayUtility.Convert<IBusinessObject, IBusinessObjectWithIdentity> (businessObjects));
  }

  /// <summary> Populates the <see cref="DropDownList"/> with the items passed in <paramref name="businessObjects"/>. </summary>
  /// <param name="businessObjects">
  ///   The <see cref="IList"/> of <see cref="IBusinessObjectWithIdentity"/> objects to populate the 
  ///   <see cref="DropDownList"/>. Use <see langword="null"/> to clear the list.
  /// </param>
  /// <remarks> This method controls the actual refilling of the <see cref="DropDownList"/>. </remarks>
  protected virtual void RefreshBusinessObjectList (IList businessObjects)
  {
    _isBusinessObejectListPopulated = true;
    _dropDownList.Items.Clear();

    //  Add Undefined item
    bool isNullItem = InternalValue == null;
    if (isNullItem || ! IsRequired)
      _dropDownList.Items.Add (CreateNullItem());

    if (businessObjects != null)
    {
      //  Populate _dropDownList
      for (int i = 0; i < businessObjects.Count; i++)
      {
        IBusinessObjectWithIdentity businessObject = (IBusinessObjectWithIdentity) businessObjects[i];
        ListItem item = new ListItem (GetDisplayName (businessObject), businessObject.UniqueIdentifier);
        _dropDownList.Items.Add (item);
      }
    }
  }

  protected virtual void InitializeMenusItems()
  {
  }

  protected virtual void PreRenderMenuItems()
  {
    if (_hiddenMenuItems == null)
      return;

    BocDropDownMenu.HideMenuItems (OptionsMenuItems, _hiddenMenuItems);
  }

  private void PreRenderReadOnlyValue()
  {
    string text;
    if (InternalValue != null)
      text = HttpUtility.HtmlEncode (_displayName);
    else
      text = String.Empty;
    if (StringUtility.IsNullOrEmpty (text))
    {
      if (IsDesignMode)
      {
        text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  _label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else
      {
        text = "&nbsp;";
      }
    }
    _label.Text = text;

    _label.Height = Unit.Empty;
    _label.Width = Unit.Empty;
    _label.ApplyStyle (_commonStyle);
    _label.ApplyStyle (_labelStyle);
  }

  private void PreRenderEditModeValue()
  {
    SetEditModeValue ();

    _dropDownList.Enabled = Enabled;
    _dropDownList.Height = Unit.Empty;
    _dropDownList.Width = Unit.Empty;
    _dropDownList.ApplyStyle (_commonStyle);
    _dropDownListStyle.ApplyStyle (_dropDownList);
  }

  private void PreRenderIcon()
  {
    _icon.Visible = false;

    //  Get icon
    if (_enableIcon && Property != null)
    {
      IconInfo iconInfo = BusinessObjectBoundWebControl.GetIcon (Value, Property.ReferenceClass.BusinessObjectProvider);

      if (iconInfo != null)
      {
        _icon.ImageUrl = iconInfo.Url;
        _icon.Width = iconInfo.Width;
        _icon.Height = iconInfo.Height;

        _icon.Visible = _enableIcon;
        _icon.Style["vertical-align"] = "middle";
        _icon.Style["border-style"] = "none";

        if (IsCommandEnabled (IsReadOnly))
        {
          if (StringUtility.IsNullOrEmpty (iconInfo.AlternateText))
          {
            if (Value == null)
              _icon.AlternateText = String.Empty;
            else
              _icon.AlternateText = HttpUtility.HtmlEncode (GetDisplayName (Value));
          }
          else
          {
            _icon.AlternateText = iconInfo.AlternateText;
          }

        }
      }
    }
  }

  private void PreRenderOptionsMenu()
  {
    _optionsMenu.Enabled = Enabled;
    _optionsMenu.IsReadOnly = IsReadOnly;
    if (StringUtility.IsNullOrEmpty (_optionsTitle))
      _optionsMenu.TitleText = GetResourceManager().GetString (ResourceIdentifier.OptionsTitle);
    else
      _optionsMenu.TitleText = _optionsTitle;
    _optionsMenu.Style["vertical-align"] = "middle";

    if (! IsDesignMode)
    {
      string getSelectionCount;
      if (IsReadOnly)
      {
        if (InternalValue != null)
          getSelectionCount = "function() { return 1; }";
        else 
          getSelectionCount = "function() { return 0; }";
      }
      else
      {
        getSelectionCount = "function() { return BocReferenceValue_GetSelectionCount ('" + _dropDownList.ClientID + "'); }";
      }
      _optionsMenu.GetSelectionCount = getSelectionCount;
    }
  }

  private bool IsInternetExplorer55OrHigher()
  {
    if (IsDesignMode)
      return true;
    
    bool isVersionGreaterOrEqual55 = 
            Context.Request.Browser.MajorVersion >= 6
        ||    Context.Request.Browser.MajorVersion == 5 
            && Context.Request.Browser.MinorVersion >= 0.5;
    bool isInternetExplorer55AndHigher = 
        Context.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

    return isInternetExplorer55AndHigher;
  }

  /// <summary> Gets a flag describing whether the <see cref="OptionsMenu"/> is visible. </summary>
  protected bool HasOptionsMenu
  {
    get 
    {
      return   ! WcagHelper.Instance.IsWaiConformanceLevelARequired() 
            && _showOptionsMenu && (OptionsMenuItems.Count > 0 || IsDesignMode)
            && IsInternetExplorer55OrHigher(); 
    }
  }

  /// <summary> Creates the <see cref="ListItem"/> symbolizing the undefined selection. </summary>
  /// <returns> A <see cref="ListItem"/>. </returns>
  private ListItem CreateNullItem()
  {
    ListItem emptyItem = new ListItem (string.Empty, c_nullIdentifier);
    return emptyItem;
  }

  /// <summary> 
  ///   Handles the <see cref="DropDownMenu.EventCommandClick"/> event of the <see cref="OptionsMenu"/>.
  /// </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="WebMenuItemClickEventArgs"/> object that contains the event data. </param>
  private void OptionsMenu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemEventCommandClick (e.Item);
  }

  /// <summary> 
  ///   Calls the <see cref="BocMenuItemCommand.OnClick"/> method of the <paramref name="menuItem"/>'s 
  ///   <see cref="BocMenuItem.Command"/> and raises <see cref="MenuItemClick"/> event. 
  /// </summary>
  /// <param name="menuItem"> The <see cref="BocMenuItem"/> that has been clicked. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.Event"/>. </remarks>
  protected virtual void OnMenuItemEventCommandClick (WebMenuItem menuItem)
  {
    WebMenuItemClickEventHandler menuItemClickHandler = (WebMenuItemClickEventHandler) Events[s_menuItemClickEvent];
    if (menuItem != null && menuItem.Command != null)
    {
      if (menuItem is BocMenuItem)
        ((BocMenuItemCommand) menuItem.Command).OnClick ((BocMenuItem) menuItem);
      else
        menuItem.Command.OnClick();
    }
    if (menuItemClickHandler != null)
    {
      WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs (menuItem);
      menuItemClickHandler (this, e);
    }
  }

  /// <summary> Is raised when a menu item with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
  [Category ("Action")]
  [Description ("Is raised when a menu item with a command of type Event is clicked.")]
  public event WebMenuItemClickEventHandler MenuItemClick
  {
    add { Events.AddHandler (s_menuItemClickEvent, value); }
    remove { Events.RemoveHandler (s_menuItemClickEvent, value); }
  }

  /// <summary> Handles the <see cref="DropDownMenu.WxeFunctionCommandClick"/> event of the <see cref="OptionsMenu"/>. </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="WebMenuItemClickEventArgs"/> object that contains the event data. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.Event"/>. </remarks>
  private void OptionsMenu_WxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemWxeFunctionCommandClick (e.Item);
  }

  /// <summary> 
  ///   Calls the <see cref="BocMenuItemCommand.ExecuteWxeFunction"/> method of the <paramref name="menuItem"/>'s 
  ///   <see cref="BocMenuItem.Command"/>.
  /// </summary>
  /// <param name="menuItem"> The <see cref="BocMenuItem"/> that has been clicked. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.WxeFunction"/>. </remarks>
  protected virtual void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem)
  {
    if (menuItem != null && menuItem.Command != null)
    {
      if (menuItem is BocMenuItem)
      {
        int[] indices = new int[0];
        IBusinessObject[] businessObjects;
        if (Value != null)
          businessObjects = new IBusinessObject[] { Value };
        else
          businessObjects = new IBusinessObject[0];
   
        BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction ((IWxePage) Page, indices, businessObjects);
        //else
        //  command.ExecuteWxeFunction (Page, indices, businessObjects);
      }
      else
      {
        Command command = menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction ((IWxePage) Page, null);
        //else
        //  command.ExecuteWxeFunction (Page, null, new NameValueCollection (0));
      }
    }
  }

  /// <summary> Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> object this control is bound to. </summary>
  /// <value> An <see cref="IBusinessObjectReferenceProperty"/> object. </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectReferenceProperty Property
  {
    get { return (IBusinessObjectReferenceProperty) base.Property; }
    set { base.Property = value; }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <include file='doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/Value/*' />
  [Browsable (false)]
  public new IBusinessObjectWithIdentity Value
  {
    get 
    {
      if (InternalValue == null)
      {
        _value = null;
      }
      //  Only reload if value is outdated
      else if (   Property != null
               && (   _value == null
                   || _value.UniqueIdentifier != InternalValue))
      {
        _value = ((IBusinessObjectClassWithIdentity) Property.ReferenceClass).GetObject (InternalValue);
      }

      return _value;
    }
    set 
    { 
      IsDirty = true;

      _value = value;

      if (value != null)
      {
        InternalValue = value.UniqueIdentifier;
        _displayName = GetDisplayName (value);
      }
      else
      {
        InternalValue = null;
        _displayName = null;
      }
    }
  }

  /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
  /// <value> The value must be of type <see cref="IBusinessObjectWithIdentity"/>. </value>
  protected override object ValueImplementation
  {
    get { return Value; }
    set { Value = (IBusinessObjectWithIdentity) value; }
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> for the current 
  ///   <see cref="IBusinessObjectWithIdentity"/> object 
  ///   or <see langword="null"/> if no item / the null item is selected.
  /// </value>
  protected string InternalValue
  {
    get { return _internalValue; }
    set 
    {
      //  Don't update identical values, unless they are null
      if (_internalValue == value && _internalValue != null)
        return;

      bool isOldInternalValueNull = _internalValue == null;

      _internalValue = StringUtility.EmptyToNull (value);

      bool removeNullItem =    IsRequired 
                            && isOldInternalValueNull
                            && _internalValue != null;
      
      if (removeNullItem)
      {
        ListItem itemToRemove = _dropDownList.Items.FindByValue (c_nullIdentifier);
        _dropDownList.Items.Remove (itemToRemove);
      }
      else if (_internalValue == null)
      {
        //  No null item in the list ?
        if (_dropDownList.Items.FindByValue (c_nullIdentifier) == null)
          _dropDownList.Items.Insert (0, CreateNullItem());
        _dropDownList.SelectedValue = c_nullIdentifier;
      }
    }
  }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/> of the selected 
  ///   <see cref="IBusinessObjectWithIdentity"/>.
  /// </summary>
  /// <value> A string or <see langword="null"/> if no  <see cref="IBusinessObjectWithIdentity"/> is selected. </value>
  [Browsable (false)]
  public string BusinessObjectID
  {
    get
    {
      if (InternalValue == c_nullIdentifier)
        return null;
      return _internalValue;
    }
  }

  bool IBocMenuItemContainer.IsReadOnly
  {
    get { return IsReadOnly; }
  }

  bool IBocMenuItemContainer.IsSelectionEnabled
  {
    get { return true; }
  }

  IBusinessObject[] IBocMenuItemContainer.GetSelectedBusinessObjects()
  {
    return (Value == null) ? new IBusinessObject[0] : new IBusinessObject[] {Value};
  }

  void IBocMenuItemContainer.RemoveBusinessObjects (IBusinessObject[] businessObjects)
  {
    RemoveBusinessObjects (businessObjects);
  }

  void IBocMenuItemContainer.InsertBusinessObjects (IBusinessObject[] businessObjects)
  {
    InsertBusinessObjects (businessObjects);
  }

  /// <summary> Removes the <paramref name="businessObjects"/> from the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  protected virtual void RemoveBusinessObjects (IBusinessObject[] businessObjects)
  {
    if (Value == null)
      return;

    if (businessObjects.Length > 0 && businessObjects[0] is IBusinessObjectWithIdentity)
    {
      if (((IBusinessObjectWithIdentity) businessObjects[0]).UniqueIdentifier == Value.UniqueIdentifier)
      {
        Value = null;
        IsDirty = true;
      }
    }
  }

  /// <summary> Adds the <paramref name="businessObjects"/> to the <see cref="Value"/> collection. </summary>
  /// <remarks> Sets the dirty state. </remarks>
  protected virtual void InsertBusinessObjects (IBusinessObject[] businessObjects)
  {
    if (businessObjects.Length > 0)
    {
      Value = (IBusinessObjectWithIdentity) businessObjects[0];
      IsDirty = true;
    }
  }

  /// <summary>
  ///   Returns the string to be used in the drop down list for the specified <see cref="IBusinessObjectWithIdentity"/>.
  /// </summary>
  /// <param name="businessObject"> The <see cref="IBusinessObjectWithIdentity"/> to get the display name for. </param>
  /// <returns> The display name for the specified <see cref="IBusinessObjectWithIdentity"/>. </returns>
  /// <remarks> 
  ///   <para>
  ///     Override this method to change the way the display name is composed. 
  ///   </para><para>
  ///     The default implementation used the <see cref="IBusinessObject.DisplayName"/> property to get the display name.
  ///   </para>
  /// </remarks>
  protected virtual string GetDisplayName (IBusinessObjectWithIdentity businessObject)
  {
    return businessObject.DisplayNameSafe;
  }

  /// <summary> 
  ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user interface.
  /// </summary>
  /// <returns> 
  ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
  ///   <see cref="DropDownList"/> if the control is in edit mode, or an empty array if the control is read-only.
  /// </returns>
  /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
  public override string[] GetTrackedClientIDs()
  {
    return IsReadOnly ? new string[0] : new string[1] { _dropDownList.ClientID };
  }

  /// <summary> The <see cref="BocReferenceValue"/> supports only scalar properties. </summary>
  /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return ! isList;
  }

  /// <summary>
  ///   The <see cref="BocReferenceValue"/> supports properties of types <see cref="IBusinessObjectReferenceProperty"/>.
  /// </summary>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  public override bool SupportsProperty (IBusinessObjectProperty property)
  {
    ArgumentUtility.CheckNotNull ("property", property);
    if (!base.SupportsProperty (property))
      return false;
    return ((IBusinessObjectReferenceProperty) property).ReferenceClass is IBusinessObjectClassWithIdentity;
  }

  /// <summary>
  ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
  ///   <see cref="TargetControl"/>.
  /// </summary>
  /// <value> Always <see langword="false"/>. </value>
  public override bool UseLabel
  {
    get { return false; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
  ///   <see cref="Control.ClientID"/>.
  /// </summary>
  /// <value> The <see cref="DropDownList"/> if the control is in edit mode, otherwise the control itself. </value>
  public override Control TargetControl 
  {
    get { return IsReadOnly ? (Control) this : _dropDownList; }
  }

  /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
  /// <value>
  ///   Returns the <see cref="Control.ClientID"/> of the <see cref="DropDownList"/> if the control is in edit mode, 
  ///   otherwise <see langword="null"/>. 
  /// </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public string FocusID
  { 
    get { return IsReadOnly ? null : _dropDownList.ClientID; }
  }

  /// <summary> This event is fired when the selection is changed between postbacks. </summary>
  [Category ("Action")]
  [Description ("Fires when the value of the control has changed.")]
  public event EventHandler SelectionChanged
  {
    add { Events.AddHandler (s_selectionChangedEvent, value); }
    remove { Events.RemoveHandler (s_selectionChangedEvent, value); }
  }

  /// <summary> This event is fired when the value's command is clicked. </summary>
  [Category ("Action")]
  [Description ("Fires when the value's command is clicked.")]
  public event BocCommandClickEventHandler CommandClick
  {
    add { Events.AddHandler (s_commandClickEvent, value); }
    remove { Events.RemoveHandler (s_commandClickEvent, value); }
  }

  /// <summary>
  ///   Gets the style that you want to apply to the <see cref="DropDownList"/> (edit mode) 
  ///   and the <see cref="Label"/> (read-only mode).
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="DropDownListStyle"/> and <see cref="LabelStyle"/> to assign individual 
  ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
  ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
  ///   <see cref="DropDownListStyle"/> and <see cref="LabelStyle"/>  properties.
  /// </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the DropDownList (edit mode) and the Label (read-only mode).")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style CommonStyle
  {
    get { return _commonStyle; }
  }

  /// <summary> Gets the style that you want to apply to the <see cref="DropDownList"/> (edit mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the DropDownList (edit mode) only.")]
  [NotifyParentProperty( true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public DropDownListStyle DropDownListStyle
  {
    get { return _dropDownListStyle; }
  }

  /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
  /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
  [Category ("Style")]
  [Description ("The style that you want to apply to the Label (read-only mode) only.")]
  [NotifyParentProperty (true)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style LabelStyle
  {
    get { return _labelStyle; }
  }

  /// <summary> Gets the <see cref="DropDownList"/> used in edit mode. </summary>
  [Browsable (false)]
  public DropDownList DropDownList
  {
    get { return _dropDownList; }
  }

  /// <summary> Gets the <see cref="Label"/> used in read-only mode. </summary>
  [Browsable (false)]
  public Label Label
  {
    get { return _label; }
  }

  /// <summary> Gets the <see cref="Image"/> used as an icon. </summary>
  [Browsable (false)]
  public Image Icon
  {
    get { return _icon; }
  }

  /// <summary> Gets the <see cref="DropDownMenu"/> offering additional commands for the current <see cref="Value"/>. </summary>
  protected DropDownMenu OptionsMenu
  {
    get { return _optionsMenu; }
  }

  /// <summary> Gets or sets the <see cref="BocCommand"/> for this control's <see cref="Value"/>. </summary>
  /// <value> A <see cref="BocCommand"/>. </value>
  /// <remarks> This property is used for accessing the <see cref="BocCommand"/> at run time and for Designer support. </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Menu")]
  [Description ("The command rendered for this control's Value.")]
  [NotifyParentProperty (true)]
  public BocCommand Command
  {
    get { return (BocCommand) _command.ControlItem; }
    set 
    { 
      _command.ControlItem = value; 
      if (value != null)
        _command.ControlItem.OwnerControl = this;
    }
  }

  /// <summary> Controls the persisting of the <see cref="Command"/>. </summary>
  /// <remarks> 
  ///   <para>
  ///     Does not persist <see cref="BocCommand"/> objects with a <see cref="Remotion.Web.UI.Controls.Command.Type"/> 
  ///     set to <see cref="CommandType.None"/>.
  ///   </para><para>
  ///     Used by <see cref="ShouldSerializePersistedCommand"/>.
  ///   </para>
  /// </remarks>
  private bool ShouldSerializeCommand()
  {
    if (Command == null)
      return false;

    if (Command.Type == CommandType.None)
      return false;
    else
      return true;
  }

  /// <summary> Sets the <see cref="Command"/> to its default value. </summary>
  /// <remarks> 
  ///   The default value is a <see cref="BocCommand"/> object with a 
  ///   <see cref="Remotion.Web.UI.Controls.Command.Type"/> set to <see cref="CommandType.None"/>.
  /// </remarks>
  private void ResetCommand()
  {
    if (Command != null)
    {
      Command = (BocCommand) Activator.CreateInstance (Command.GetType());
      Command.Type = CommandType.None;
    }
  }

  /// <summary> Gets or sets the encapsulated <see cref="BocCommand"/> for this control's <see cref="Value"/>. </summary>
  /// <value> 
  ///   A <see cref="SingleControlItemCollection"/> containing a <see cref="BocCommand"/> in its 
  ///   <see cref="SingleControlItemCollection.ControlItem"/> property.
  /// </value>
  /// <remarks> This property is used for persisting the <see cref="Command"/> into the <b>ASPX</b> source code. </remarks>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [Browsable (false)]
  [NotifyParentProperty (true)]
  public SingleControlItemCollection PersistedCommand
  {
    get { return _command; }
  }

  /// <summary> Controls the persisting of the <see cref="PersistedCommand"/>. </summary>
  /// <remarks> Returns <see cref="ShouldSerializeCommand"/>. </remarks>
  private bool ShouldSerializePersistedCommand()
  {
    return ShouldSerializeCommand();
  }

  /// <summary> 
  ///   Gets or sets a flag that determines whether the <see cref="Icon"/> is shown in front of the <see cref="Value"/>.
  /// </summary>
  /// <value> <see langword="true"/> to show the <see cref="Icon"/>. The default value is <see langword="true"/>. </value>
  /// <remarks> 
  ///   An icon is only shown if the <see cref="Property"/>'s 
  ///   <see cref="IBusinessObjectClass.BusinessObjectProvider">ReferenceClass.BusinessObjectProvider</see>
  ///   provides an instance of type <see cref="IBusinessObjectWebUIService"/> and 
  ///   <see cref="IBusinessObjectWebUIService.GetIcon"/> returns not <see langword="null"/>.
  /// </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("Flag that determines whether to show the icon in front of the value.")]
  [DefaultValue (true)]
  public bool EnableIcon
  {
    get { return _enableIcon; }
    set { _enableIcon = value; }
  }

  /// <summary> The search expression used to populate the selection list in edit mode. </summary>
  /// <value> A <see cref="String"/> with a valid search expression. The default value is an empty <see cref="String"/>. </value>
  /// <remarks> A valid <see cref="Property"/> is required in order to populate the list using the search statement. </remarks>
  [Category ("Data")]
  [Description ("Set the search expression for populating the selection list.")]
  [DefaultValue ("")]
  public string Select
  {
    get { return _select; }
    set 
    { 
      if (value == null)
        _select = null;
      else
        _select = value.Trim(); 
    }
  }

  /// <summary> Gets or sets the flag that determines whether to evaluate the <see cref="Select"/> statement. </summary>
  /// <value> 
  ///   <see langword="true"/> to evaluate the select statement. 
  ///   Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsSelectStatementEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to evaluate the Select statement. Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (typeof (bool?), "")]
  public bool? EnableSelectStatement
  {
    get { return _enableSelectStatement; }
    set { _enableSelectStatement = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableSelectStatement"/> property. </summary>
  /// <value>
  ///   <see langowrd="false"/> if <see cref="EnableSelectStatement"/> is <see langword="false"/>. 
  /// </value>
  protected bool IsSelectStatementEnabled
  {
    get { return _enableSelectStatement != false;}
  }

  /// <summary> Gets the <see cref="BocMenuItem"/> objects displayed in the <see cref="OptionsMenu"/>. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Menu")]
  [Description ("The menu items displayed by options menu.")]
  [DefaultValue ((string) null)]
  [Editor (typeof (BocMenuItemCollectionEditor), typeof (System.Drawing.Design.UITypeEditor))]
  public WebMenuItemCollection OptionsMenuItems
  {
    get { return _optionsMenu.MenuItems; }
  }

  /// <summary> Gets or sets the text that is rendered as a label for the <see cref="OptionsMenu"/>. </summary>
  /// <value> 
  ///   The text rendered as the <see cref="OptionsMenu"/>'s label. The default value is an empty <see cref="String"/>. 
  /// </value>
  [Category ("Menu")]
  [Description ("The text that is rendered as a label for the options menu.")]
  [DefaultValue ("")]
  public string OptionsTitle
  {
    get { return _optionsTitle; }
    set { _optionsTitle = value; }
  }

  /// <summary> Gets or sets a flag that determines whether to display the <see cref="OptionsMenu"/>. </summary>
  /// <value> <see langword="true"/> to show the <see cref="OptionsMenu"/>. The default value is <see langword="true"/>. </value>
  [Category ("Menu")]
  [Description ("Enables the options menu.")]
  [DefaultValue (true)]
  public bool ShowOptionsMenu
  {
    get { return _showOptionsMenu; }
    set { _showOptionsMenu = value; }
  }

  /// <summary> Gets or sets the width of the options menu. </summary>
  /// <value> The <see cref="Unit"/> value used for the option menu's width. The default value is <b>undefined</b>. </value>
  [Category ("Menu")]
  [Description ("The width of the options menu.")]
  [DefaultValue (typeof (Unit), "")]
  public Unit OptionsMenuWidth
  {
    get { return _optionsMenuWidth; }
    set { _optionsMenuWidth = value; }
  }

  /// <summary> Gets or sets flag that determines whether to use the value as the <see cref="OptionsMenu"/>'s head. </summary>
  /// <value> 
  ///   <see langword="true"/> to embed the value inside the options menu's head. 
  ///   The default value is <see langword="true"/>. 
  /// </value>
  [Category ("Menu")]
  [Description ("Determines whether to use the value as the options menu's head.")]
  [DefaultValue (typeof (bool?), "")]
  public bool? HasValueEmbeddedInsideOptionsMenu
  {
    get { return _hasValueEmbeddedInsideOptionsMenu; }
    set { _hasValueEmbeddedInsideOptionsMenu = value; }
  }

  /// <summary> Gets or sets the list of menu items to be hidden. </summary>
  /// <value> The <see cref="WebMenuItem.ItemID"/> values of the menu items to hide. </value>
  [Category ("Menu")]
  [Description ("The list of menu items to be hidden, identified by their ItemIDs.")]
  [DefaultValue ((string) null)]
  [PersistenceMode (PersistenceMode.Attribute)]
  [TypeConverter (typeof (Remotion.Web.UI.Design.StringArrayConverter))]
  public string[] HiddenMenuItems 
  {
    get 
    {
      if (_hiddenMenuItems == null)
        return new string[0];
      return _hiddenMenuItems;
    }
    set {_hiddenMenuItems = value;}
  }

  /// <summary> Gets or sets the validation error message. </summary>
  /// <value> 
  ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
  ///   In case of the default value, the text is read from the resources for this control.
  /// </value>
  [Description("Validation message displayed if there is an error.")]
  [Category ("Validator")]
  [DefaultValue("")]
  public string ErrorMessage
  {
    get
    { 
      return _errorMessage; 
    }
    set 
    {
      _errorMessage = value; 
      for (int i = 0; i < _validators.Count; i++)
      {
        BaseValidator validator = (BaseValidator) _validators[i];
        validator.ErrorMessage = _errorMessage;
      }
    }
  }

  #region protected virtual string CssClass...
  /// <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/> itself. </summary>
  /// <remarks> 
  ///   <para> Class: <c>bocReferenceValue</c>. </para>
  ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
  /// </remarks>
  protected virtual string CssClassBase
  { get { return "bocReferenceValue"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/>'s value. </summary>
  /// <remarks> Class: <c>bocReferenceValueContent</c> </remarks>
  protected virtual string CssClassContent
  { get { return "bocReferenceValueContent"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/> when it is displayed in read-only mode. </summary>
  /// <remarks> 
  ///   <para> Class: <c>readOnly</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocReferenceValue.readOnly</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassReadOnly
  { get { return "readOnly"; } }

  /// <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/> when it is displayed in read-only mode. </summary>
  /// <remarks> 
  ///   <para> Class: <c>disabled</c>. </para>
  ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocReferenceValue.disabled</c> as a selector. </para>
  /// </remarks>
  protected virtual string CssClassDisabled
  { get { return "disabled"; } }
  #endregion
}

}
