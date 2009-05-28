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
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocBooleanValue;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocBooleanValue
{
  public class MockBooleanValue : ObjectBinding.Web.UI.Controls.BocBooleanValue
  {
    protected override void Render (HtmlTextWriter writer)
    {
      var renderer = new BocBooleanValueRenderer (MockRepository.GenerateMock<IHttpContext> (), writer, this);
      renderer.Render ();
    }

    protected override bool IsDesignMode
    {
      get { return false; }
    }

    private bool _isReadOnly = false;

    public override bool IsReadOnly
    {
      get { return _isReadOnly; }
    }

    public void SetReadOnly (bool value)
    {
      _isReadOnly = value;
    }


    private IPage _page;

    public override IPage Page
    {
      get { return _page; }
    }

    public void SetPage (IPage page)
    {
      _page = page;
    }

    public string ResourceGroup
    {
      get { return "resourceGroup"; }
    }

    public string TrueIconUrl
    {
      get { return "trueIconUrl"; }
    }

    public string FalseIconUrl
    {
      get { return "falseIconUrl"; }
    }

    public string NullIconUrl
    {
      get { return "nullIconUrl"; }
    }

    public string TrueDescriptionText
    {
      get { return "trueDescription"; }
    }

    public string FalseDescriptionText
    {
      get { return "falseDescription"; }
    }

    public string NullDescriptionText
    {
      get { return "nullDescription"; }
    }

    public string CssClassBasePublic
    {
      get { return CssClassBase; }
    }

    public string CssClassDisabledPublic
    {
      get { return CssClassDisabled; }
    }

    public string CssClassReadOnlyPublic
    {
      get { return CssClassReadOnly; }
    }

    protected override BocBooleanValueResourceSet CreateResourceSet ()
    {
      BocBooleanValueResourceSet resourceSet = new BocBooleanValueResourceSet (
          ResourceGroup,
          TrueIconUrl,
          FalseIconUrl,
          NullIconUrl,
          TrueDescriptionText,
          FalseDescriptionText,
          NullDescriptionText
          );

      return resourceSet;
    }

    public BocBooleanValueResourceSet GetResourceSet ()
    {
      return CreateResourceSet();
    }
  }
}