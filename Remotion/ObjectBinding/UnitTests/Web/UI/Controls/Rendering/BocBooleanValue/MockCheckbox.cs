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
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocBooleanValue
{
  public class MockCheckbox : BocCheckBox
  {
    public string DefaultTrueDescription
    {
      get
      {
        IResourceManager resourceManager = GetResourceManager();
        return resourceManager.GetString (ResourceIdentifier.TrueDescription);
      }
    }

    public string DefaultFalseDescription
    {
      get
      {
        IResourceManager resourceManager = GetResourceManager();
        return resourceManager.GetString (ResourceIdentifier.FalseDescription);
      }
    }

    private IPage _page;
    public new IPage  Page
    {
      get { return _page; }
    }

    public void SetPage (IPage page)
    {
      _page = page;
    }

    protected override bool IsDesignMode
    {
      get { return false; }
    }

    private bool _isReadOnly;
    public override bool IsReadOnly
    {
      get { return _isReadOnly; }
    }

    public string CssClassBasePublic
    {
      get { return CssClassBase; }
    }

    public string CssClassReadOnlyPublic
    {
      get { return CssClassReadOnly; }
    }

    public string CssClassDisabledPublic
    {
      get { return CssClassDisabled; }
    }

    public void SetReadOnly (bool value)
    {
      _isReadOnly = value;
    }
  }
}