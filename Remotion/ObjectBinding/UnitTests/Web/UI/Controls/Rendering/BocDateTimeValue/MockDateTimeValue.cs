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
using System.Collections;
using System.Web;
using Remotion.Development.UnitTesting;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocDateTimeValue
{
  public class MockDateTimeValue : ObjectBinding.Web.UI.Controls.BocDateTimeValue
  {
    private bool _isReadOnly;
    private const string c_defaultControlWidth = "150pt";

    public MockDateTimeValue ()
    {
      PrivateInvoke.SetNonPublicField (this, "_datePickerButton", new MockHyperLink());
    }

    public new string CssClassBase
    {
      get { return base.CssClassBase; }
    }

    public new string CssClassReadOnly
    {
      get { return base.CssClassReadOnly; }
    }

    public new string CssClassDisabled
    {
      get { return base.CssClassDisabled; }
    }

    public string DefaultWidth
    {
      get { return c_defaultControlWidth; }
    }

    protected override HttpContext Context
    {
      get
      {
        var context = new HttpContext (new HttpRequest ("Test.aspx", "http://www.example.com/", ""), new HttpResponse (null));
        context.Request.Browser = new HttpBrowserCapabilities();
        context.Request.Browser.Capabilities = new Hashtable();
        context.Request.Browser.Capabilities.Add ("browser", "IE");
        context.Request.Browser.Capabilities.Add ("majorversion", "7");
        return context;
      }
    }

    public void SetReadOnly (bool readOnly)
    {
      _isReadOnly = readOnly;
    }

    public override bool IsReadOnly
    {
      get
      {
        return _isReadOnly;
      }
    }
  }
}