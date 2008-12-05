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
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  public class SubMenuTab : MenuTab
  {
    private MainMenuTab _parent;

    public SubMenuTab (string itemID, string text, IconInfo icon)
        : base (itemID , text, icon)
    {
    }

    public SubMenuTab (string itemID, string text)
        : this (itemID , text, null)
    {
    }

    /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
    /// <exclude/>
    [EditorBrowsable (EditorBrowsableState.Never)]
    public SubMenuTab ()
    {
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public MainMenuTab Parent
    {
      get { return _parent; }
    }

    protected internal void SetParent (MainMenuTab parent)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      _parent = parent;
    }
  }
}
