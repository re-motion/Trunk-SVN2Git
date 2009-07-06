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
using System.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> A column definition with the possibility of rendering a command in the cell. </summary>
  public abstract class BocCommandEnabledColumnDefinition : BocColumnDefinition
  {
    /// <summary> The <see cref="BocListItemCommand"/> rendered in this column. </summary>
    private readonly SingleControlItemCollection _command;

    protected BocCommandEnabledColumnDefinition ()
    {
      _command = new SingleControlItemCollection (new BocListItemCommand(), new[] { typeof (BocListItemCommand) });
    }

    protected override void OnOwnerControlChanged ()
    {
      base.OnOwnerControlChanged();
      if (Command != null)
        Command.OwnerControl = OwnerControl;
    }

    /// <summary> Gets or sets the <see cref="BocListItemCommand"/> rendered in this column. </summary>
    /// <value> A <see cref="BocListItemCommand"/>. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Category ("Behavior")]
    [Description ("The command rendered in this column.")]
    [NotifyParentProperty (true)]
    public BocListItemCommand Command
    {
      get { return (BocListItemCommand) _command.ControlItem; }
      set
      {
        _command.ControlItem = value;
        if (OwnerControl != null)
          _command.ControlItem.OwnerControl = OwnerControl;
      }
    }

    private bool ShouldSerializeCommand ()
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
    ///   The default value is a <see cref="BocListItemCommand"/> object with a <c>Command.Type</c> set to 
    ///   <see cref="CommandType.None"/>.
    /// </remarks>
    private void ResetCommand ()
    {
      if (Command != null)
      {
        Command = (BocListItemCommand) Activator.CreateInstance (Command.GetType());
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
    ///   Does not persist <see cref="BocListItemCommand"/> objects with a <c>Command.Type</c> set to 
    ///   <see cref="CommandType.None"/>.
    /// </remarks>
    private bool ShouldSerializePersistedCommand ()
    {
      return ShouldSerializeCommand();
    }
  }
}