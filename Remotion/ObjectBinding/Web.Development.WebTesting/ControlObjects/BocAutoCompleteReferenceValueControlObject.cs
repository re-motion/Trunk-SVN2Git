// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocAutoCompleteReferenceValue"/>.
  /// </summary>
  public class BocAutoCompleteReferenceValueControlObject
      : BocControlObject, IFillableControlObject, ICommandHost, IDropDownMenuHost, IControlObjectWithFormElements
  {
    public BocAutoCompleteReferenceValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Invokes the associated search service of the represented <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocAutoCompleteReferenceValue"/>
    /// and returns its results.
    /// </summary>
    /// <param name="searchText">Text to search for.</param>
    /// <param name="completionSetCount">Auto completion set count.</param>
    /// <returns>The completion set as list of <see cref="SearchServiceResultItem"/> or an empty list if the completion set has been empty.</returns>
    public IReadOnlyList<SearchServiceResultItem> GetSearchServiceResults ([NotNull] string searchText, int completionSetCount)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("searchText", searchText);

      var inputScopeId = GetHtmlID() + "_TextValue";

      var response =
          Context.Browser.ExecuteScript (CommonJavaScripts.CreateAutoCompleteWebServiceRequest (inputScopeId, searchText, completionSetCount));
      var responseAsCollection = (IReadOnlyCollection<object>) response;
      return responseAsCollection.Cast<IDictionary<string, object>>().Select (
          d => new SearchServiceResultItem
               {
                   DisplayName = (string) d["DisplayName"],
                   IconUrl = (string) d["IconUrl"],
                   UniqueIdentifier = (string) d["UniqueIdentifier"],
                   Type = (string) d["__type"]
               }).ToList();
    }

    public SearchServiceResultItem GetExactSearchServiceResult ([NotNull] string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);

      // Todo RM-6297: Implement GetExactSearchResult
      throw new NotImplementedException ("TODO RM-6297: Implement GetExactSearchServiceResult() + docs + integration test.");
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return Scope.FindChild ("Label").Text; // do not trim

      return Scope.FindChild ("TextValue").Value; // do not trim
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (string text, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);

      return FillWith (text, FinishInput.WithTab, actionOptions);
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject FillWith (string text, FinishInputWithAction finishInputWith, IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("text", text);
      ArgumentUtility.CheckNotNull ("finishInputWith", finishInputWith);

      var actualActionOptions = MergeWithDefaultActionOptions (Scope, actionOptions);
      new FillWithAction (this, Scope.FindChild ("TextValue"), text, finishInputWith).Execute (actualActionOptions);
      return UnspecifiedPage();
    }

    /// <inheritdoc/>
    public CommandControlObject GetCommand ()
    {
      var commandScope = Scope.FindChild ("Command");
      return new CommandControlObject (Context.CloneForControl (commandScope));
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject ExecuteCommand (IWebTestActionOptions acitonOptions = null)
    {
      return GetCommand().Click (acitonOptions);
    }

    /// <inheritdoc/>
    public DropDownMenuControlObject GetDropDownMenu ()
    {
      var dropDownMenuScope = Scope.FindChild ("Boc_OptionsMenu");
      return new DropDownMenuControlObject (Context.CloneForControl (dropDownMenuScope));
    }

    /// <summary>
    /// See <see cref="IControlObjectWithFormElements.GetFormElementNames"/>. Returns the input[type=text] (text value) as first element, the
    /// input[type=hidden] (key value) as second element.
    /// </summary>
    ICollection<string> IControlObjectWithFormElements.GetFormElementNames ()
    {
      var htmlID = GetHtmlID();
      return new[] { string.Format ("{0}_TextValue", htmlID), string.Format ("{0}_KeyValue", htmlID) };
    }
  }
}