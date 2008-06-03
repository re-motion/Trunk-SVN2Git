/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
