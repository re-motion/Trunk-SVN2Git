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
using Remotion.Web.UI.Controls.Rendering.SingleView;

namespace Remotion.Web.UnitTests.UI.Controls.Rendering.SingleView.QuirksMode
{
  public class SingleViewMock : Web.UI.Controls.SingleView, ISingleView
  {
    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls ();
    }

    public string CssClassBasePublic
    {
      get { return CssClassBase; }
    }

    public string CssClassBottomControlsPublic
    {
      get { return CssClassBottomControls; }
    }

    public string CssClassContentPublic
    {
      get { return CssClassContent; }
    }

    public string CssClassEmptyPublic
    {
      get { return CssClassEmpty; }
    }

    public string CssClassTopControlsPublic
    {
      get { return CssClassTopControls; }
    }

    public string CssClassViewPublic
    {
      get { return CssClassView; }
    }

    public string CssClassViewBodyPublic
    {
      get { return CssClassViewBody; }
    }

    private bool _isDesignMode;
    internal void SetDesignMode (bool value)
    {
      _isDesignMode = value;
    }

    bool ISingleView.IsDesignMode
    {
      get
      {
        return _isDesignMode;
      }
    }
  }
}