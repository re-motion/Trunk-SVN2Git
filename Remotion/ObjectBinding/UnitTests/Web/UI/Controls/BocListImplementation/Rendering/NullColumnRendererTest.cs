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
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class NullColumnRendererTest
  {
    private NullColumnRenderer _nullColumnRenderer;
    private HtmlTextWriter _htmlTextWriterMock;

    [SetUp]
    public void SetUp ()
    {
      _nullColumnRenderer = new NullColumnRenderer();
      _htmlTextWriterMock = MockRepository.GenerateStrictMock<HtmlTextWriter> ();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_nullColumnRenderer.IsNull, Is.True);
      Assert.That (_nullColumnRenderer.Column, Is.Null);
    }

    [Test]
    public void RenderTitleCell ()
    {
      _htmlTextWriterMock.Replay();

      _nullColumnRenderer.RenderTitleCell (_htmlTextWriterMock, SortingDirection.None, 0);

      _htmlTextWriterMock.VerifyAllExpectations();
    }

    [Test]
    public void RenderDataCell ()
    {
      _htmlTextWriterMock.Replay ();

      _nullColumnRenderer.RenderDataCell (_htmlTextWriterMock, 0, true, true, null);

      _htmlTextWriterMock.VerifyAllExpectations ();
    }

    [Test]
    public void RenderDataColumnDeclaration ()
    {
      _htmlTextWriterMock.Replay();

      _nullColumnRenderer.RenderDataColumnDeclaration (_htmlTextWriterMock, false, new BocSimpleColumnDefinition());

      _htmlTextWriterMock.VerifyAllExpectations();
    }

  }
}