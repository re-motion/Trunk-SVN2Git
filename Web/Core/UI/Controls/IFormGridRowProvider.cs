/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
