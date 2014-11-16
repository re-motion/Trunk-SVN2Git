using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations hosting <see cref="ActaNovaBreadCrumbControlObject"/>.
  /// </summary>
  public interface IControlObjectWithBreadCrumbs
  {
    int GetNumberOfBreadCrumbs ();
    ActaNovaBreadCrumbControlObject GetBreadCrumb (int index);
  }
}