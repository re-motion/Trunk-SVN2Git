using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaListControlObject : BocListControlObject
  {
    public ActaNovaListControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public ScopeControlObject GetTopBlock ()
    {
      var scope = Scope.FindCss ("div.bocListTopBlock");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    public void Filter (string filter)
    {
      var textBox = GetTopBlock().GetControl (new PerLocalIDControlSelectionCommand<TextBoxControlObject> (new TextBoxSelector(), "TextFilterField"));
      textBox.FillWith (filter, FinishInput.Promptly);

      var filterButton =
          GetTopBlock().GetControl (new PerLocalIDControlSelectionCommand<AnchorControlObject> (new AnchorSelector(), "TextFilterButton"));
      filterButton.Click();
    }

    public void ClearFilter ()
    {
      var clearButton =
          GetTopBlock().GetControl (new PerLocalIDControlSelectionCommand<AnchorControlObject> (new AnchorSelector(), "TextFilterClearButton"));
      clearButton.Click();
    }

    public UnspecifiedPageObject OpenColumnConfiguration ()
    {
      var openConfigurationButtonScope = Scope.FindChild ("OpenConfigurationButton");
      var openConfigurationButton = new ImageButtonControlObject (Context.CloneForControl (openConfigurationButtonScope));
      return openConfigurationButton.Click();
    }

    public void SetPreferredView ()
    {
      var setPreferredViewButtonScope = Scope.FindChild ("SetPreferredViewButton");
      var setPreferredViewButton = new ImageButtonControlObject (Context.CloneForControl (setPreferredViewButtonScope));
      setPreferredViewButton.Click();
    }

    protected override ElementScope GetAvailableViewsScope ()
    {
      return Scope.FindChild ("CurrentViewSelector");
    }
  }
}