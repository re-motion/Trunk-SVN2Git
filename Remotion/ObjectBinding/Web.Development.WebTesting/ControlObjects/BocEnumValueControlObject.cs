﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Collections.Generic;
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue"/> control.
  /// </summary>
  public class BocEnumValueControlObject
      : BocControlObject, IControlObjectWithSelectableOptions, IFluentControlObjectWithSelectableOptions, IControlObjectWithLoadTestingSupport
  {
    private readonly IBocEnumValueControlObjectVariant _variantImpl;

    public BocEnumValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      var style = Scope[DiagnosticMetadataAttributesForObjectBinding.BocEnumValueStyle];
      _variantImpl = CreateVariant (style);
    }

    /// <summary>
    /// Returns the currently selected option's value.
    /// </summary>
    public string GetSelectedOption ()
    {
      return _variantImpl.GetSelectedOption();
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithSelectableOptions SelectOption ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SelectOption (string itemID, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SelectOption().WithItemID (itemID, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithItemID (string itemID, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return _variantImpl.SelectOption (itemID, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithIndex (int index, IWebTestActionOptions actionOptions)
    {
      return _variantImpl.SelectOption (index, actionOptions);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableOptions.WithDisplayText (string displayText, IWebTestActionOptions actionOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      return _variantImpl.SelectOptionByText (displayText, actionOptions);
    }

    /// <summary>
    /// See <see cref="IControlObjectWithLoadTestingSupport.GetFormElementNames"/>. Returns the select (value) as only element if the display style is
    /// either <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.DropDownList"/> or
    /// <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.ListBox"/>. Returns the common name of the input[type=radio] elements if
    /// the display style is <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.ListControlType.RadioButtonList"/>.
    /// </summary>
    ICollection<string> IControlObjectWithLoadTestingSupport.GetFormElementNames ()
    {
      return new[] { string.Format ("{0}_Value", GetHtmlID()) };
    }

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
    private interface IBocEnumValueControlObjectVariant
    {
      string GetSelectedOption ();

      UnspecifiedPageObject SelectOption ([NotNull] string itemID, IWebTestActionOptions actionOptions);
      UnspecifiedPageObject SelectOption (int index, IWebTestActionOptions actionOptions);
      UnspecifiedPageObject SelectOptionByText ([NotNull] string text, IWebTestActionOptions actionOptions);
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
          return _controlObject.Scope.FindChild ("Value").Text; // do not trim

        return _controlObject.Scope.FindChild ("Value").GetSelectedOptionText();
      }

      public UnspecifiedPageObject SelectOption (string itemID, IWebTestActionOptions actionOptions)
      {
        ArgumentUtility.CheckNotNull ("itemID", itemID);

        Action<ElementScope> selectAction = s => s.SelectOptionByValue (itemID);
        return SelectOption (selectAction, actionOptions);
      }

      public UnspecifiedPageObject SelectOption (int index, IWebTestActionOptions actionOptions)
      {
        Action<ElementScope> selectAction = s => s.SelectOptionByIndex (index);
        return SelectOption (selectAction, actionOptions);
      }

      public UnspecifiedPageObject SelectOptionByText (string text, IWebTestActionOptions actionOptions)
      {
        ArgumentUtility.CheckNotNull ("text", text);

        Action<ElementScope> selectAction = s => s.SelectOption (text);
        return SelectOption (selectAction, actionOptions);
      }

      private UnspecifiedPageObject SelectOption ([NotNull] Action<ElementScope> selectAction, IWebTestActionOptions actionOptions)
      {
        ArgumentUtility.CheckNotNull ("selectAction", selectAction);

        var actualActionOptions = _controlObject.MergeWithDefaultActionOptions (_controlObject.Scope, actionOptions);
        new CustomAction (_controlObject, _controlObject.Scope.FindChild ("Value"), "Select", selectAction).Execute (actualActionOptions);
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
          return _controlObject.Scope.FindChild ("Value").Text; // do not trim

        return _controlObject.Scope.FindCss ("input[type='radio'][checked='checked']").Value;
      }

      public UnspecifiedPageObject SelectOption (string itemID, IWebTestActionOptions actionOptions)
      {
        ArgumentUtility.CheckNotNull ("itemID", itemID);

        var scope = _controlObject.Scope.FindTagWithAttribute ("span", DiagnosticMetadataAttributes.ItemID, itemID).FindCss ("input");
        return CheckScope (scope, actionOptions);
      }

      public UnspecifiedPageObject SelectOption (int index, IWebTestActionOptions actionOptions)
      {
        var scope =
            _controlObject.Scope.FindTagWithAttribute ("span", DiagnosticMetadataAttributes.IndexInCollection, index.ToString()).FindCss ("input");
        return CheckScope (scope, actionOptions);
      }

      public UnspecifiedPageObject SelectOptionByText (string text, IWebTestActionOptions actionOptions)
      {
        ArgumentUtility.CheckNotNull ("text", text);

        var scope = _controlObject.Scope.FindTagWithAttribute ("span", DiagnosticMetadataAttributes.Content, text).FindCss ("input");
        return CheckScope (scope, actionOptions);
      }

      private UnspecifiedPageObject CheckScope ([NotNull] ElementScope scope, IWebTestActionOptions actionOptions)
      {
        ArgumentUtility.CheckNotNull ("scope", scope);

        var actualActionOptions = _controlObject.MergeWithDefaultActionOptions (_controlObject.Scope, actionOptions);
        new CheckAction (_controlObject, scope).Execute (actualActionOptions);
        return _controlObject.UnspecifiedPage();
      }
    }
  }
}