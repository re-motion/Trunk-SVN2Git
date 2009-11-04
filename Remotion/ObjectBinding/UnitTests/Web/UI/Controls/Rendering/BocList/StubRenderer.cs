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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList
{
    public class StubRenderer : IBocListTableBlockRenderer, IBocListMenuBlockRenderer, IBocListNavigationBlockRenderer
    {
        public StubRenderer (HtmlTextWriter writer)
        {
            Writer = writer;
            List = null;
        }

        public HtmlTextWriter Writer { get; private set; }
        public IBocList List { get; private set; }

        public void Render ()
        {
            Writer.RenderBeginTag (HtmlTextWriterTag.Div);
            Writer.RenderEndTag();
        }

        void IBocListTableBlockRenderer.Render ()
        {
            Render();
        }

        IBocList IBocListNavigationBlockRenderer.List
        {
            get { return List; }
        }

        HtmlTextWriter IBocListNavigationBlockRenderer.Writer
        {
            get { return Writer; }
        }

        void IBocListNavigationBlockRenderer.Render ()
        {
            Render();
        }

        IBocList IBocListMenuBlockRenderer.List
        {
            get { return List; }
        }

        HtmlTextWriter IBocListMenuBlockRenderer.Writer
        {
            get { return Writer; }
        }

        void IBocListMenuBlockRenderer.Render ()
        {
            Render();
        }

        IBocList IBocListTableBlockRenderer.List
        {
            get { return List; }
        }

        HtmlTextWriter IBocListTableBlockRenderer.Writer
        {
            get { return Writer; }
        }
    }
}
