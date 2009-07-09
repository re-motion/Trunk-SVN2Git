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
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;
using RhinoMocksExtensions=Rhino.Mocks.RhinoMocksExtensions;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering
{
  public class RendererTestBase
  {
    private ControlCollection _controlCollectionMock;
    private MockRepository _mockRepository;
    protected IHttpContext HttpContext { get; private set; }
    protected HtmlHelper Html { get; private set; }

    public ControlCollection ControlCollectionMock
    {
      get { return _controlCollectionMock; }
    }

    public MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    protected RendererTestBase ()
    {
      
    }

    protected virtual void Initialize ()
    {
      Html = new HtmlHelper ();

      HttpContext = MockRepository.GenerateMock<IHttpContext> ();
      IHttpResponse response = MockRepository.GenerateMock<IHttpResponse> ();
      HttpContext.Stub (mock => mock.Response).Return (response);
      response.Stub (mock => mock.ContentType).Return ("text/html");

      HttpBrowserCapabilities browser = new HttpBrowserCapabilities ();
      browser.Capabilities = new Hashtable ();
      browser.Capabilities.Add ("browser", "IE");
      browser.Capabilities.Add ("majorversion", "7");

      var request = MockRepository.GenerateStub<IHttpRequest> ();
      request.Browser = browser;

      HttpContext = MockRepository.GenerateStub<IHttpContext> ();
      HttpContext.Stub (stub => stub.Request).Return (request);
    }

    protected void SetUpAddAndRemoveControlExpectation<T> (IControl control)
        where T:Control
    {
      if (_mockRepository == null)
      {
        _mockRepository = new MockRepository();
      }
      _controlCollectionMock = _mockRepository.PartialMock<ControlCollection> (new Control());
      ControlCollectionMock.BackToRecord ();
      ControlCollectionMock.Expect (mock => mock.Add (Arg<T>.Is.NotNull));
      ControlCollectionMock.Expect (mock => mock.Remove (Arg<T>.Is.NotNull));
      ControlCollectionMock.Replay();
      control.Stub (stub => stub.Controls).Return (ControlCollectionMock);
    }
  }
}