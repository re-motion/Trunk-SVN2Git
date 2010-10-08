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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  public class BocColumnRenderer
  {
    private readonly IBocColumnRenderer _columnRenderer;
    private readonly BocColumnDefinition _columnDefinition;
    private readonly bool _isVisibleColumn;
    private readonly int _columnIndex;
    private readonly bool _showIcon;
    private readonly SortingDirection _sortingDirection;
    private readonly int _orderIndex;

    public BocColumnRenderer (
        IBocColumnRenderer columnRenderer,
        BocColumnDefinition columnDefinition,
        bool isVisibleColumn,
        int columnIndex,
        bool showIcon,
        SortingDirection sortingDirection,
        int orderIndex)
    {
      ArgumentUtility.CheckNotNull ("columnRenderer", columnRenderer);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      _columnRenderer = columnRenderer;
      _columnDefinition = columnDefinition;
      _isVisibleColumn = isVisibleColumn;
      _columnIndex = columnIndex;
      _showIcon = showIcon;
      _sortingDirection = sortingDirection;
      _orderIndex = orderIndex;
    }

    public BocColumnDefinition ColumnDefinition
    {
      get { return _columnDefinition; }
    }

    public bool IsVisibleColumn
    {
      get { return _isVisibleColumn; }
    }

    public int ColumnIndex
    {
      get { return _columnIndex; }
    }

    public bool ShowIcon
    {
      get { return _showIcon; }
    }

    public SortingDirection SortingDirection
    {
      get { return _sortingDirection; }
    }

    public int OrderIndex
    {
      get { return _orderIndex; }
    }

    public void RenderTitleCell (BocColumnRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      _columnRenderer.RenderTitleCell (renderingContext, _sortingDirection, _orderIndex);
    }

    public void RenderDataColumnDeclaration (BocColumnRenderingContext renderingContext, bool isTextXml)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      _columnRenderer.RenderDataColumnDeclaration (renderingContext, isTextXml);
    }

    public void RenderDataCell (BocColumnRenderingContext renderingContext, int rowIndex, BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("dataRowRenderEventArgs", dataRowRenderEventArgs);

      _columnRenderer.RenderDataCell (renderingContext, rowIndex, _showIcon, _isVisibleColumn, dataRowRenderEventArgs);
    }
  }
}