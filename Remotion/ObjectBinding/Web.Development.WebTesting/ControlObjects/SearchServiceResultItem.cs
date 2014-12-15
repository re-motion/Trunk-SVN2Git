using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Represents an item of an auto complete search service query result (an item of the auto completion set).
  /// </summary>
  public class SearchServiceResultItem
  {
    public string DisplayName { get; set; }
    public string IconUrl { get; set; }
    public string UniqueIdentifier { get; set; }
    public string Type { get; set; }
  }
}