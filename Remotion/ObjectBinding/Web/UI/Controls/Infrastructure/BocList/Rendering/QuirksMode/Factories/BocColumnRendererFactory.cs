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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode.Factories
{
  public class BocColumnRendererFactory
      :
          IBocColumnRendererFactory<BocSimpleColumnDefinition>,
          IBocColumnRendererFactory<BocCompoundColumnDefinition>,
          IBocColumnRendererFactory<BocCommandColumnDefinition>,
          IBocColumnRendererFactory<BocCustomColumnDefinition>,
          IBocColumnRendererFactory<BocDropDownMenuColumnDefinition>,
          IBocColumnRendererFactory<BocRowEditModeColumnDefinition>,
          IBocIndexColumnRendererFactory,
          IBocSelectorColumnRendererFactory
  {
    public IBocColumnRenderer<BocSimpleColumnDefinition> CreateRenderer
        (HtmlTextWriter writer, Controls.BocList list, BocSimpleColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocSimpleColumnRenderer (writer, list, columnDefinition);
    }

    public IBocColumnRenderer<BocCompoundColumnDefinition> CreateRenderer (
        HtmlTextWriter writer, Controls.BocList list, BocCompoundColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocCompoundColumnRenderer (writer, list, columnDefinition);
    }

    public IBocColumnRenderer<BocCommandColumnDefinition> CreateRenderer (
        HtmlTextWriter writer, Controls.BocList list, BocCommandColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocCommandColumnRenderer (writer, list, columnDefinition);
    }

    public IBocColumnRenderer<BocCustomColumnDefinition> CreateRenderer (
        HtmlTextWriter writer, Controls.BocList list, BocCustomColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocCustomColumnRenderer (writer, list, columnDefinition);
    }

    public IBocColumnRenderer<BocDropDownMenuColumnDefinition> CreateRenderer (
        HtmlTextWriter writer, Controls.BocList list, BocDropDownMenuColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocDropDownMenuColumnRenderer (writer, list, columnDefinition);
    }

    public IBocColumnRenderer<BocRowEditModeColumnDefinition> CreateRenderer (
        HtmlTextWriter writer, Controls.BocList list, BocRowEditModeColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocRowEditModeColumnRenderer (writer, list, columnDefinition);
    }

    IBocIndexColumnRenderer IBocIndexColumnRendererFactory.CreateRenderer (HtmlTextWriter writer, Controls.BocList list)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);

      return new BocIndexColumnRenderer(writer, list);
    }

    IBocSelectorColumnRenderer IBocSelectorColumnRendererFactory.CreateRenderer (HtmlTextWriter writer, Controls.BocList list)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);

      return new BocSelectorColumnRenderer(writer, list);
    }
  }
}