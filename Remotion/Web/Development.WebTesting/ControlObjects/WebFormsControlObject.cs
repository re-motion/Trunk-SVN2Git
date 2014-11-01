using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Base class for control objects representing an ASP.NET WebForms control.
  /// </summary>
  public abstract class WebFormsControlObject : ControlObject
  {
    protected WebFormsControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }
  }
}