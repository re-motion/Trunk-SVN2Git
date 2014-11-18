using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations hosting <see cref="ActaNovaBreadCrumbControlObject"/>.
  /// </summary>
  public interface IControlObjectWithBreadCrumbs
  {
    /// <summary>
    /// Returns the number of displayed bread crumbs.
    /// </summary>
    int GetNumberOfBreadCrumbs ();

    /// <summary>
    /// Returns the nth bread crumb, given by a one-based <paramref name="index"/>.
    /// </summary>
    ActaNovaBreadCrumbControlObject GetBreadCrumb (int index);
  }
}