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

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocEnumValue
{
  public class MockEnumValue : ObjectBinding.Web.UI.Controls.BocEnumValue
  {
    private bool _isReadOnly;
    private bool _isRequired;
    private bool _isDesignMode;

    public MockEnumValue ()
    {
    }

    protected override bool IsDesignMode
    {
      get{return _isDesignMode;}
    }

    public void SetDesignMode (bool value)
    {
      _isDesignMode = value;
    }

    public override bool IsReadOnly
    {
      get { return _isReadOnly; }
    }

    public void SetReadOnly (bool value)
    {
      _isReadOnly = value;
    }

    public override bool IsRequired
    {
      get { return _isRequired; }
    }

    public string CssClassBasePublic
    {
      get { return CssClassBase; }
    }

    public void SetRequired (bool value)
    {
      _isRequired = value;
    }
  }
}