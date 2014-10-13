using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.Infrastructure
{
  /// <summary>
  /// <see cref="IActionBehavior"/> implementations specific to Acta Nova.
  /// </summary>
  public static class ActaNovaBehavior
  {
    public static readonly IActionBehavior WaitForOuterInnerOuterUpdate = new ActionBehavior().WaitFor (WaitForActaNova.OuterInnerOuterUpdate);
  }
}