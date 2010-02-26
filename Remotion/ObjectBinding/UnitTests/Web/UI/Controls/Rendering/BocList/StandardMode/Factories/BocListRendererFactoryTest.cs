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
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode.Factories;
using System.Web;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.StandardMode.Factories
{
  [TestFixture]
  public class BocListRendererFactoryTest
  {
    private HttpContextBase HttpContext { get; set; }
    private ObjectBinding.Web.UI.Controls.BocList List { get; set; }

    [SetUp]
    public void SetUp ()
    {
      HttpContext = MockRepository.GenerateMock<HttpContextBase> ();

      List = new ObjectBinding.Web.UI.Controls.BocList();
    }

    [Test]
    public void CreateBocListRenderer ()
    {
      IBocListRendererFactory factory = new BocListRendererFactory();
      IRenderer renderer = factory.CreateRenderer (HttpContext, List, new StubServiceLocator());

      Assert.IsInstanceOfType (typeof (BocListRenderer), renderer);
      Assert.AreSame (List, ((BocListRenderer) renderer).List);

      Assert.IsInstanceOfType (typeof (BocListNavigationBlockRenderer), ((BocListRenderer) renderer).NavigationBlockRenderer);
      Assert.AreSame (List, ((BocListNavigationBlockRenderer) ((BocListRenderer) renderer).NavigationBlockRenderer).List);

      Assert.IsInstanceOfType (typeof (BocListMenuBlockRenderer), ((BocListRenderer) renderer).MenuBlockRenderer);
      Assert.AreSame (List, ((BocListMenuBlockRenderer) ((BocListRenderer) renderer).MenuBlockRenderer).List);

      Assert.IsInstanceOfType (typeof (BocListTableBlockRenderer), ((BocListRenderer) renderer).TableBlockRenderer);
      Assert.AreSame (List, ((BocListTableBlockRenderer) ((BocListRenderer) renderer).TableBlockRenderer).List);

      Assert.IsInstanceOfType (typeof (BocRowRenderer), ((BocListTableBlockRenderer) ((BocListRenderer) renderer).TableBlockRenderer).RowRenderer);
      Assert.AreSame (List, ((BocRowRenderer) ((BocListTableBlockRenderer) ((BocListRenderer) renderer).TableBlockRenderer).RowRenderer).List);
    }
  }
}
