// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System.ComponentModel;
using System.Web.UI;
using Microsoft.Practices.ServiceLocation;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> A column definition used for switching between edit mode and returning from it via save and cancel. </summary>
  public class BocRowEditModeColumnDefinition : BocColumnDefinition
  {
    private string _editText = string.Empty;
    private IconInfo _editIcon = new IconInfo ();
    private string _saveText = string.Empty;
    private IconInfo _saveIcon = new IconInfo ();
    private string _cancelText = string.Empty;
    private IconInfo _cancelIcon = new IconInfo ();
    private BocRowEditColumnDefinitionShow _show = BocRowEditColumnDefinitionShow.EditMode;

    public BocRowEditModeColumnDefinition ()
    {
    }

    /// <summary>
    ///   Determines when the column is shown to the user in regard of the <see cref="BocList"/>'s read-only setting.
    /// </summary>
    /// <value> 
    ///   One of the <see cref="BocRowEditColumnDefinitionShow"/> enumeration values. 
    ///   The default is <see cref="BocRowEditColumnDefinitionShow.EditMode"/>.
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("Determines when to show the column to the user in regard to the BocList's read-only setting.")]
    [DefaultValue (BocRowEditColumnDefinitionShow.EditMode)]
    [NotifyParentProperty (true)]
    public BocRowEditColumnDefinitionShow Show
    {
      get { return _show; }
      set { _show = value; }
    }

    /// <summary> Gets or sets the text representing the edit command in the rendered page. </summary>
    /// <value> A <see cref="string"/> representing the edit command. </value>
    /// <remarks> The value will not be HTML encoded. </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The text representing the edit command in the rendered page. The value will not be HTML encoded.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string EditText
    {
      get { return _editText; }
      set { _editText = StringUtility.NullToEmpty (value); }
    }

    /// <summary>
    ///  Gets or sets the image representing the edit command in the rendered page. Must not be <see langword="null"/>. 
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the edit command. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The image representing the edit command in the rendered page.")]
    [NotifyParentProperty (true)]
    public IconInfo EditIcon
    {
      get { return _editIcon; }
      set
      {
        ArgumentUtility.CheckNotNull ("EditIcon", value);
        _editIcon = value;
      }
    }

    private bool ShouldSerializeEditIcon ()
    {
      return IconInfo.ShouldSerialize (_editIcon);
    }

    private void ResetEditIcon ()
    {
      _editIcon.Reset ();
    }


    /// <summary> Gets or sets the text representing the save command in the rendered page. </summary>
    /// <value> A <see cref="string"/> representing the save command. </value>
    /// <remarks> The value will not be HTML encoded. </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The text representing the save command in the rendered page. The value will not be HTML encoded.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string SaveText
    {
      get { return _saveText; }
      set { _saveText = StringUtility.NullToEmpty (value); }
    }

    /// <summary> 
    ///   Gets or sets the image representing the save command in the rendered page. Must not be <see langword="null"/>.
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the save command. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The relative url to image representing the save command in the rendered page.")]
    [NotifyParentProperty (true)]
    public IconInfo SaveIcon
    {
      get { return _saveIcon; }
      set
      {
        ArgumentUtility.CheckNotNull ("SaveIcon", value);
        _saveIcon = value;
      }
    }

    private bool ShouldSerializeSaveIcon ()
    {
      return IconInfo.ShouldSerialize (_saveIcon);
    }

    private void ResetSaveIcon ()
    {
      _saveIcon.Reset ();
    }

    /// <summary> Gets or sets the text representing the cancel command in the rendered page. </summary>
    /// <value> A <see cref="string"/> representing the cancel command. </value>
    /// <remarks> The value will not be HTML encoded. </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The text representing the cancel command in the rendered page. The value will not be HTML encoded.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string CancelText
    {
      get { return _cancelText; }
      set { _cancelText = StringUtility.NullToEmpty (value); }
    }

    /// <summary> 
    ///   Gets or sets the image representing the cancel command in the rendered page. Must not be <see langword="null"/>. 
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the cancel command. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The image representing the cancel command in the rendered page.")]
    [NotifyParentProperty (true)]
    public IconInfo CancelIcon
    {
      get { return _cancelIcon; }
      set
      {
        ArgumentUtility.CheckNotNull ("CancelIcon", value);
        _cancelIcon = value;
      }
    }

    private bool ShouldSerializeCancelIcon ()
    {
      return IconInfo.ShouldSerialize (_cancelIcon);
    }

    private void ResetCancelIcon ()
    {
      _cancelIcon.Reset ();
    }

    protected override IBocColumnRenderer GetRendererInternal (IServiceLocator locator, IHttpContext context, HtmlTextWriter writer, IBocList list)
    {
      var factory = locator.GetInstance<IBocColumnRendererFactory<BocRowEditModeColumnDefinition>> ();
      return factory.CreateRenderer (context, writer, list, this);
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "RowEditModeColumnDefinition"; }
    }

    public override void LoadResources (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      base.LoadResources (resourceManager);

      string key = ResourceManagerUtility.GetGlobalResourceKey (EditText);
      if (!string.IsNullOrEmpty (key))
        EditText = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (SaveText);
      if (!string.IsNullOrEmpty (key))
        SaveText = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (CancelText);
      if (!string.IsNullOrEmpty (key))
        CancelText = resourceManager.GetString (key);

      if (EditIcon != null)
        EditIcon.LoadResources (resourceManager);
      if (SaveIcon != null)
        SaveIcon.LoadResources (resourceManager);
      if (CancelIcon != null)
        CancelIcon.LoadResources (resourceManager);
    }
  }
}