// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Web;
using System.Web.UI;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocColumnRendererTest
  {
    private BocColumnRenderer _columnRendererAdapter;
    private IBocColumnRenderer _columnRenderMock;
    private StubColumnDefinition _columnDefinition;
    private HtmlTextWriter _htmlTextWriterStub;
    private HttpContextBase _httpContextStub;
    private IBocList _bocListStub;
    private BocColumnRenderingContext<StubColumnDefinition> _renderingContext;

    [SetUp] 
    public void SetUp ()
    {
      _columnDefinition = new StubColumnDefinition();
      _columnRenderMock = MockRepository.GenerateStrictMock<IBocColumnRenderer>();
      _columnRendererAdapter = new BocColumnRenderer (_columnRenderMock, _columnDefinition, true, 0, true, SortingDirection.None, 0);
      _htmlTextWriterStub = MockRepository.GenerateStub<HtmlTextWriter> ();
      _httpContextStub = MockRepository.GenerateStub<HttpContextBase>();
      _bocListStub = MockRepository.GenerateStub<IBocList>();
      _renderingContext = new BocColumnRenderingContext<StubColumnDefinition> (
          new BocColumnRenderingContext(_httpContextStub, _htmlTextWriterStub, _bocListStub, _columnDefinition, 0));
    }

    [Test]
    public void RenderTitleCell ()
    {
      _columnRenderMock.Expect (mock => mock.RenderTitleCell (_renderingContext, SortingDirection.None, 0));
      _columnRenderMock.Replay();

      _columnRendererAdapter.RenderTitleCell (_renderingContext);

      _columnRenderMock.VerifyAllExpectations();
    }

    [Test]
    public void RenderDataColumnDeclaration ()
    {
      _columnRenderMock.Expect (mock => mock.RenderDataColumnDeclaration (_renderingContext, false));
      _columnRenderMock.Replay ();

      _columnRendererAdapter.RenderDataColumnDeclaration(_renderingContext, false);

      _columnRenderMock.VerifyAllExpectations ();
    }

    [Test]
    public void RenderDataCell ()
    {
      var dataRowRenderEventArgs = new BocListDataRowRenderEventArgs (0, null);

      _columnRenderMock.Expect (mock => mock.RenderDataCell (_renderingContext, 0,true, dataRowRenderEventArgs));
      _columnRenderMock.Replay ();

      _columnRendererAdapter.RenderDataCell (_renderingContext, 0, dataRowRenderEventArgs);

      _columnRenderMock.VerifyAllExpectations ();
    }
  }
}