using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaDetailsArea : ActaNovaControlObject, IControlHost
  {
    public ActaNovaDetailsArea ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    public string FormPageTitle
    {
      get { return Scope.FindCss (".formPageTitleLabel").Text; }
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      return controlSelectionCommand.Select (Context);
    }
  }
}