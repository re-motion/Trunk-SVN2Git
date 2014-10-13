using System;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="LabelControlObject"/>.
  /// </summary>
  public class LabelSelector : RemotionControlSelectorBase<LabelControlObject>
  {
    public LabelSelector ()
        : base ("span", "<not possible>") // Todo RM-6297: do something about <not possible>...we need a second base class...
    {
    }
  }
}