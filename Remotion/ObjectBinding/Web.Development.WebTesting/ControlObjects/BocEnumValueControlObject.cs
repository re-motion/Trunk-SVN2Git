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

    public UnspecifiedPageObject SelectOption (string itemID, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return _variantImpl.SelectOption (itemID, completionDetection);
    }

    public UnspecifiedPageObject SelectOption (int index, ICompletionDetection completionDetection = null)
    {
      return _variantImpl.SelectOption (index, completionDetection);
    }

    public UnspecifiedPageObject SelectOptionByText (string text, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      return _variantImpl.SelectOptionByText (text, completionDetection);
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

      public UnspecifiedPageObject SelectOption (string itemID, ICompletionDetection completionDetection = null)
      {
        ArgumentUtility.CheckNotNull ("itemID", itemID);

        Action<ElementScope> selectAction = s => s.SelectOptionByValue (itemID);
        return SelectOption (selectAction, completionDetection);
      }

      public UnspecifiedPageObject SelectOption (int index, ICompletionDetection completionDetection = null)
      {
        Action<ElementScope> selectAction = s => s.SelectOptionByIndex (index);
        return SelectOption (selectAction, completionDetection);
      }

      public UnspecifiedPageObject SelectOptionByText (string text, ICompletionDetection completionDetection = null)
      {
        ArgumentUtility.CheckNotNull ("text", text);

        Action<ElementScope> selectAction = s => s.SelectOption (text);
        return SelectOption (selectAction, completionDetection);
      }

      private UnspecifiedPageObject SelectOption ([NotNull] Action<ElementScope> selectAction, ICompletionDetection completionDetection = null)
      {
        ArgumentUtility.CheckNotNull ("selectAction", selectAction);

        var actualCompletionDetection = _controlObject.DetermineActualCompletionDetection (completionDetection);
        _controlObject.FindChild ("Value").PerformAction (selectAction, _controlObject.Context, actualCompletionDetection);
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

      public UnspecifiedPageObject SelectOption (string itemID, ICompletionDetection completionDetection = null)
      {
        ArgumentUtility.CheckNotNull ("itemID", itemID);

        var scope = _controlObject.Scope.FindDMA ("span", DiagnosticMetadataAttributes.ItemID, itemID).FindCss("input");
        return CheckScope (scope, completionDetection);
      }

      public UnspecifiedPageObject SelectOption (int index, ICompletionDetection completionDetection = null)
      {
        var scope = _controlObject.Scope.FindDMA ("span", DiagnosticMetadataAttributes.IndexInCollection, index.ToString()).FindCss("input");
        return CheckScope (scope, completionDetection);
      }

      public UnspecifiedPageObject SelectOptionByText (string text, ICompletionDetection completionDetection = null)
      {
        ArgumentUtility.CheckNotNull ("text", text);

        var scope = _controlObject.Scope.FindDMA ("span", DiagnosticMetadataAttributes.Text, text).FindCss("input");
        return CheckScope (scope, completionDetection);
      }

      private UnspecifiedPageObject CheckScope ([NotNull] ElementScope scope, ICompletionDetection completionDetection = null)
      {
        ArgumentUtility.CheckNotNull ("scope", scope);

        var actualCompletionDetection = _controlObject.DetermineActualCompletionDetection (completionDetection);
        scope.PerformAction (s => s.Check(), _controlObject.Context, actualCompletionDetection);
        return _controlObject.UnspecifiedPage();
      }
    }
  }
}