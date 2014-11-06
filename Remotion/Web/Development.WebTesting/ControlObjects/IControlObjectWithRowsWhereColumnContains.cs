// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of rows.
  /// </summary>
  public interface IControlObjectWithRowsWhereColumnContains<TRowControlObject>
      where TRowControlObject : ControlObject
  {
    IControlObjectWithRowsWhereColumnContains<TRowControlObject> GetRowWhere ();
    TRowControlObject GetRowWhere ([NotNull] string columnItemID, [NotNull] string containsCellText);

    TRowControlObject ColumnWithItemIDContains ([NotNull] string itemID, [NotNull] string containsCellText);
    TRowControlObject ColumnWithIndexContains (int index, [NotNull] string containsCellText);
    TRowControlObject ColumnWithTitleContains ([NotNull] string text, [NotNull] string containsCellText);
  }
}