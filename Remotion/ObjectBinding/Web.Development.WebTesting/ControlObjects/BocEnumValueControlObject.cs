using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue"/> control.
  /// </summary>
  [UsedImplicitly]
  public class BocEnumValueControlObject : BocControlObject, ISelectableControlObject
  {
    private readonly IBocEnumValueControlObjectVariant _variantImpl;

    public BocEnumValueControlObject (string id, TestObjectContext context)
        : base (id, context)
    {
      var style = Scope[DiagnosticMetadataAttributesForObjectBinding.BocEnumValueStyle];
      _variantImpl = CreateVariant (style);
    }

    public string GetSelectedOption ()
    {
      return _variantImpl.GetSelectedOption();
    }

    public UnspecifiedPageObject SelectOption (string itemID, IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return _variantImpl.SelectOption (itemID, actionBehavior);
    }

    public UnspecifiedPageObject SelectOption (int index, IActionBehavior actionBehavior = null)
    {
      return _variantImpl.SelectOption (index, actionBehavior);
    }

    public UnspecifiedPageObject SelectOptionByText (string text, IActionBehavior actionBehavior = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return _variantImpl.SelectOptionByText (text, actionBehavior);
    }

    // Todo RM-6297: CreateVariant factory method, IBocEnumValueControlObjectVariant interface and implementation classes can be moved outside ...
    // as soon as base class functionality like FindChild() has been moved to a helper class or to extension methods.

    /// <summary>
    /// Factory method, creates a <see cref="IBocEnumValueControlObjectVariant"/> from the given <paramref name="style"/>, which must be one of
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.ToString"/>.
    /// </summary>
    /// <param name="style">One of <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.ToString"/>.</param>
    /// <returns>The corresponding <see cref="IBocEnumValueControlObjectVariant"/> implementation.</returns>
    private IBocEnumValueControlObjectVariant CreateVariant ([NotNull] string style)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("style", style);

      switch (style)
      {
        case "DropDownList":
          return new BocEnumValueSelectBasedControlObjectVariant (this);

        case "ListBox":
          return new BocEnumValueSelectBasedControlObjectVariant (this);

        case "RadioButtonList":
          return new BocEnumValueRadioButtonBasedControlObjectVariant (this);
      }

      throw new ArgumentException ("style argument must be one of Remotion.ObjectBinding.Web.UI.Controls.ListControlType.", "style");
    }

    /// <summary>
    /// Declares all methods a boc enum rendering variant must support.
    /// </summary>
    private interface IBocEnumValueControlObjectVariant : ISelectableControlObject
    {
      string GetSelectedOption ();
    }

    /// <summary>
    /// <see cref="IBocEnumValueControlObjectVariant"/> implementation for the
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.DropDownList"/> and
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.ListBox"/> style.
    /// </summary>
    private class BocEnumValueSelectBasedControlObjectVariant : IBocEnumValueControlObjectVariant
    {
      private readonly BocEnumValueControlObject _controlObject;

      public BocEnumValueSelectBasedControlObjectVariant ([NotNull] BocEnumValueControlObject controlObject)
      {
        ArgumentUtility.CheckNotNull ("controlObject", controlObject);

        _controlObject = controlObject;
      }

      public string GetSelectedOption ()
      {
        if (_controlObject.Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
          return _controlObject.FindChild ("Value").Text; // do not trim

        return _controlObject.FindChild ("Value").GetSelectedOptionText();
      }

      public UnspecifiedPageObject SelectOption (string itemID, IActionBehavior actionBehavior = null)
      {
        ArgumentUtility.CheckNotNull ("itemID", itemID);

        Action<ElementScope> selectAction = s => s.SelectOptionByValue (itemID);
        return SelectOption (selectAction, actionBehavior);
      }

      public UnspecifiedPageObject SelectOption (int index, IActionBehavior actionBehavior = null)
      {
        Action<ElementScope> selectAction = s => s.SelectOptionByIndex (index);
        return SelectOption (selectAction, actionBehavior);
      }

      public UnspecifiedPageObject SelectOptionByText (string text, IActionBehavior actionBehavior = null)
      {
        ArgumentUtility.CheckNotNull ("text", text);

        Action<ElementScope> selectAction = s => s.SelectOption (text);
        return SelectOption (selectAction, actionBehavior);
      }

      private UnspecifiedPageObject SelectOption ([NotNull] Action<ElementScope> selectAction, IActionBehavior actionBehavior = null)
      {
        ArgumentUtility.CheckNotNull ("selectAction", selectAction);

        var actualActionBehavior = _controlObject.GetActualActionBehavior (actionBehavior);
        _controlObject.FindChild ("Value").PerformAction (selectAction, _controlObject.Context, actualActionBehavior);
        return _controlObject.UnspecifiedPage();
      }
    }

    /// <summary>
    /// <see cref="IBocEnumValueControlObjectVariant"/> implementation for the
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.RadioButtonList"/> style.
    /// </summary>
    private class BocEnumValueRadioButtonBasedControlObjectVariant : IBocEnumValueControlObjectVariant
    {
      private readonly BocEnumValueControlObject _controlObject;

      public BocEnumValueRadioButtonBasedControlObjectVariant ([NotNull] BocEnumValueControlObject controlObject)
      {
        ArgumentUtility.CheckNotNull ("controlObject", controlObject);

        _controlObject = controlObject;
      }

      public string GetSelectedOption ()
      {
        if (_controlObject.Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
          return _controlObject.FindChild ("Value").Text; // do not trim

        return _controlObject.Scope.FindCss ("input[type='radio'][checked='checked']").Value;
      }

      public UnspecifiedPageObject SelectOption (string itemID, IActionBehavior actionBehavior = null)
      {
        ArgumentUtility.CheckNotNull ("itemID", itemID);

        var scope = _controlObject.Scope.FindDMA ("span", DiagnosticMetadataAttributes.ItemID, itemID).FindCss("input");
        return CheckScope (scope, actionBehavior);
      }

      public UnspecifiedPageObject SelectOption (int index, IActionBehavior actionBehavior = null)
      {
        var scope = _controlObject.Scope.FindDMA ("span", DiagnosticMetadataAttributes.IndexInCollection, index.ToString()).FindCss("input");
        return CheckScope (scope, actionBehavior);
      }

      public UnspecifiedPageObject SelectOptionByText (string text, IActionBehavior actionBehavior = null)
      {
        ArgumentUtility.CheckNotNull ("text", text);

        var scope = _controlObject.Scope.FindDMA ("span", DiagnosticMetadataAttributes.Text, text).FindCss("input");
        return CheckScope (scope, actionBehavior);
      }

      private UnspecifiedPageObject CheckScope ([NotNull] ElementScope scope, IActionBehavior actionBehavior = null)
      {
        ArgumentUtility.CheckNotNull ("scope", scope);

        var actualActionBehavior = _controlObject.GetActualActionBehavior (actionBehavior);
        scope.PerformAction (s => s.Check(), _controlObject.Context, actualActionBehavior);
        return _controlObject.UnspecifiedPage();
      }
    }
  }
}