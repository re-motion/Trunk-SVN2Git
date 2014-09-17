using System;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Todo RM-6297: Docs
  /// </summary>
  public class ControlSelectionParameters
  {
    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    public string ID { get; set; }

    /// <summary>
    /// Selection by index (one-based).
    /// </summary>
    public int? Index { get; set; }

    /// <summary>
    /// Selection by title (e.g. for container-structured objects like form grids).
    /// </summary>
    public string Title { get; set; }
  }
}