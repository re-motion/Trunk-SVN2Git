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
using System.ComponentModel;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{
  public class MainMenuTab : MenuTab
  {
    private SubMenuTabCollection _subMenuTabs;
    private MenuTab _activeTab;

    public MainMenuTab (string itemID, string text, IconInfo icon)
        : base (itemID, text, icon)
    {
      _subMenuTabs = new SubMenuTabCollection (OwnerControl);
      _subMenuTabs.SetParent (this);
    }

    public MainMenuTab (string itemID, string text)
        : this (itemID, text, null)
    {
    }

    /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
    /// <exclude/>
    [EditorBrowsable (EditorBrowsableState.Never)]
    public MainMenuTab ()
    {
      _subMenuTabs = new SubMenuTabCollection (OwnerControl);
      _subMenuTabs.SetParent (this);
    }

    [PersistenceMode (PersistenceMode.InnerProperty)]
    [ListBindable (false)]
    [Category ("Behavior")]
    [Description ("")]
    [DefaultValue ((string) null)]
    public SubMenuTabCollection SubMenuTabs
    {
      get { return _subMenuTabs; }
    }

    protected override void OnOwnerControlChanged ()
    {
      base.OnOwnerControlChanged ();
      _subMenuTabs.OwnerControl = OwnerControl;
    }

    protected override void OnSelectionChanged ()
    {
      base.OnSelectionChanged ();
      TabbedMenu.RefreshSubMenuTabStrip ();
    }

    protected override MenuTab GetActiveTab ()
    {
      if (_activeTab != null)
        return _activeTab;

      _activeTab = this;
      if (Command.Type == CommandType.None)
      {
        foreach (SubMenuTab subMenuTab in _subMenuTabs)
        {
          bool isTabActive = subMenuTab.EvaluateVisible () && subMenuTab.EvaluateEnabled ();
          bool isCommandActive = subMenuTab.Command != null && subMenuTab.Command.Type != CommandType.None;
          if (isTabActive && isCommandActive)
          {
            _activeTab = subMenuTab;
            break;
          }
        }
      }

      return _activeTab;
    }
  }
}
