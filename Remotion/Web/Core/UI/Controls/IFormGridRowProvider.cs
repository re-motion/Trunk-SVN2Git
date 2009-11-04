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
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   Interface for allowing the <see cref="FormGridManager"/> to query its parent page
///   for rows to be inserted and rows to be hidden.
/// </summary>
public interface IFormGridRowProvider
{
  /// <summary>
  ///   Returns a list of IDs identifying the rows to be hidden in a form grid.
  /// </summary>
  /// <param name="table"> The <see cref="HtmlTable"/> whose rows will be hidden. </param>
  /// <returns> A <see cref="StringCollection"/> containing the IDs. </returns>
  StringCollection GetHiddenRows (HtmlTable table);

  /// <summary>
  ///   Returns a list of <see cref="FormGridRowInfo"/> objects used to constrtuct and then 
  ///   insert new rows into a form grid.
  /// </summary>
  /// <param name="table"> The <see cref="HtmlTable"/> into which the new rows will be inserted. </param>
  /// <returns> A <see cref="FormGridRowInfoCollection"/> containing the prototypes. </returns>
  FormGridRowInfoCollection GetAdditionalRows (HtmlTable table);
}

}
