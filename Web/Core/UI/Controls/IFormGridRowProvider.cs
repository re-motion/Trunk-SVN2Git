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
