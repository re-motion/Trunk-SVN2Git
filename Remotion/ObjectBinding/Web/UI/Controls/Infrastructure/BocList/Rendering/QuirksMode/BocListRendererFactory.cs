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

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  public class BocListRendererFactory
  {
    private readonly Controls.BocList _list;
    private readonly HtmlTextWriter _writer;

    public BocListRendererFactory (Controls.BocList list, HtmlTextWriter writer)
    {
      _list = list;
      _writer = writer;
    }

    public BocListRenderer CreateRenderer ()
    {
      return new BocListRenderer (List, Writer, this);
    }

    public BocListMenuBlockRenderer CreateMenuBlockRenderer ()
    {
      return new BocListMenuBlockRenderer (List, Writer);
    }

    public BocListNavigatorRenderer CreateNavigatorRenderer ()
    {
      return new BocListNavigatorRenderer (List, Writer);
    }

    public BocRowRenderer CreateRowRenderer ()
    {
      return new BocRowRenderer (List, Writer, this);
    }

    private Controls.BocList List
    {
      get { return _list; }
    }

    private HtmlTextWriter Writer
    {
      get { return _writer; }
    }
  }
}