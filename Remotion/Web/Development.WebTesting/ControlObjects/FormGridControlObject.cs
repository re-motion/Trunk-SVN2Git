using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.FormGridManager"/>.
  /// </summary>
  public class FormGridControlObject : RemotionControlObject, IControlHost
  {
    public FormGridControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    // Todo RM-6297: ControlHostingRemotionControlObject to remove code duplication with other IControlHost implementations?
    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return controlSelectionCommand.Select (Context);
    }
  }
}