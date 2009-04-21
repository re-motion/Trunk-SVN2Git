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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{

  public abstract class MenuTab : WebTab
  {
    private SingleControlItemCollection _command = null;
    /// <summary> The command being rendered by this menu item. </summary>
    private NavigationCommand _renderingCommand = null;
    private MissingPermissionBehavior _missingPermissionBehavior;

    protected MenuTab (string itemID, string text, IconInfo icon)
      : base (itemID, text, icon)
    {
      Initialize ();
    }

    protected MenuTab ()
    {
      Initialize ();
    }

    private void Initialize ()
    {
      _command = new SingleControlItemCollection (new NavigationCommand (), new Type[] { typeof (NavigationCommand) });
    }

    protected TabbedMenu TabbedMenu
    {
      get { return (TabbedMenu) OwnerControl; }
    }

    /// <summary> Gets or sets the <see cref="NavigationCommand"/> rendered for this menu item. </summary>
    /// <value> A <see cref="NavigationCommand"/>. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Category ("Behavior")]
    [Description ("The command rendered for this menu item.")]
    [NotifyParentProperty (true)]
    public virtual NavigationCommand Command
    {
      get
      {
        return (NavigationCommand) _command.ControlItem;
      }
      set
      {
        _command.ControlItem = value;
      }
    }

    protected bool ShouldSerializeCommand ()
    {
      if (Command == null)
        return false;

      if (Command.IsDefaultType)
        return false;
      else
        return true;
    }

    /// <summary> Sets the <see cref="Command"/> to its default value. </summary>
    /// <remarks> 
    ///   The default value is a <see cref="Command"/> object with a <c>Command.Type</c> set to 
    ///   <see cref="CommandType.None"/>.
    /// </remarks>
    protected void ResetCommand ()
    {
      if (Command != null)
      {
        Command = (NavigationCommand) Activator.CreateInstance (Command.GetType ());
        Command.Type = CommandType.None;
      }
    }

    [PersistenceMode (PersistenceMode.InnerProperty)]
    [Browsable (false)]
    public SingleControlItemCollection PersistedCommand
    {
      get { return _command; }
    }

    /// <summary> Controls the persisting of the <see cref="Command"/>. </summary>
    /// <remarks> 
    ///   Does not persist <see cref="Command"/> objects with a <c>Command.Type</c> set to 
    ///   <see cref="CommandType.None"/>.
    /// </remarks>
    protected bool ShouldSerializePersistedCommand ()
    {
      return ShouldSerializeCommand ();
    }

    protected override void OnOwnerControlChanged ()
    {
      base.OnOwnerControlChanged ();

      if (OwnerControl != null && !(OwnerControl is TabbedMenu))
        throw new InvalidOperationException ("A SubMenuTab can only be added to a WebTabStrip that is part of a TabbedMenu.");

      if (Command != null)
        Command.OwnerControl = OwnerControl;
    }

    public override void LoadResources (IResourceManager resourceManager)
    {
      base.LoadResources (resourceManager);
      if (Command != null)
        Command.LoadResources (resourceManager);
    }

    public override void RenderBeginTagForCommand (HtmlTextWriter writer, bool isEnabled, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("style", style);

      MenuTab activeTab = null;
      if (isEnabled && EvaluateEnabled ())
      {
        activeTab = GetActiveTab ();
        _renderingCommand = activeTab.Command;
      }
      else
      {
        _renderingCommand = null;
      }

      if (_renderingCommand != null)
      {
        NameValueCollection additionalUrlParameters = TabbedMenu.GetUrlParameters (activeTab);
        _renderingCommand.RenderBegin (writer, GetPostBackClientEvent (), new string[0], string.Empty, null, additionalUrlParameters, false, style);
      }
      else
      {
        style.AddAttributesToRender (writer);
        writer.RenderBeginTag (HtmlTextWriterTag.A);
      }
    }

    public override void RenderEndTagForCommand (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      if (_renderingCommand != null)
        _renderingCommand.RenderEnd (writer);
      else
        writer.RenderEndTag ();
      _renderingCommand = null;
    }

    protected virtual MenuTab GetActiveTab ()
    {
      return this;
    }

    public override void OnClick ()
    {
      base.OnClick ();
      if (!IsSelected)
        IsSelected = true;
    }

    public override bool EvaluateVisible ()
    {
      if (!base.EvaluateVisible ())
        return false;

      if (Command != null)
      {
        if (WcagHelper.Instance.IsWaiConformanceLevelARequired ()
            && Command.Type == CommandType.Event)
        {
          return false;
        }
        if (MissingPermissionBehavior == MissingPermissionBehavior.Invisible)
          return Command.HasAccess (null);
      }

      return true;
    }

    public override bool EvaluateEnabled ()
    {
      if (!base.EvaluateEnabled ())
        return false;

      if (Command != null)
      {
        if (MissingPermissionBehavior == MissingPermissionBehavior.Disabled)
          return Command.HasAccess (null);
      }

      return true;
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [NotifyParentProperty (true)]
    [DefaultValue (MissingPermissionBehavior.Invisible)]
    public MissingPermissionBehavior MissingPermissionBehavior
    {
      get { return _missingPermissionBehavior; }
      set { _missingPermissionBehavior = value; }
    }
  }
}
