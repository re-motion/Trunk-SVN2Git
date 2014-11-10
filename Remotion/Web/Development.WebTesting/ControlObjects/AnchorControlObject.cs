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
using System.Web.UI.WebControls;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for <see cref="LinkButton"/>, <see cref="HyperLink"/> and all their derivatives (e.g.
  /// <see cref="T:Remotion.Web.UI.Controls.WebLinkButton"/> or <see cref="T:Remotion.Web.UI.Controls.SmartHyperLink"/>). Also represents a simple
  /// HTML anchor &lt;a&gt; control within a re-motion applicaiton.
  /// </summary>
  [UsedImplicitly]
  public class AnchorControlObject : WebFormsControlObject, IClickableControlObject
  {
    public AnchorControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject Click (ICompletionDetection completionDetection = null)
    {
      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
      Scope.ClickAndWait (Context, actualCompletionDetector);
      return UnspecifiedPage();
    }

    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      if (IsPostBackLink (scope))
        return Continue.When (Wxe.PostBackCompleted);

      // TODO RM-6297: Add integration test for IsSimpleJavaScriptLink() case.
      if (IsSimpleJavaScriptLink (scope))
        return Continue.Immediately();

      return Continue.When (Wxe.Reset);
    }

    private bool IsPostBackLink (ElementScope scope)
    {
      const string remotionDoPostBackScript = "DoPostBackWithOptions";
      const string aspNetDoPostBackScript = "__doPostBack";

      // Note: unfortunately, Selenium sometimes reports wrong href contents, therefore we cannot check for href in the third condition.
      return scope["href"].Contains (remotionDoPostBackScript) ||
             scope["href"].Contains (aspNetDoPostBackScript) ||
             (/*scope["href"].Equals ("#") &&*/ scope["onclick"] != null && scope["onclick"].Contains (aspNetDoPostBackScript));
    }

    private bool IsSimpleJavaScriptLink (ElementScope scope)
    {
      return scope["href"].Equals ("#") && scope["onclick"] != null && scope["onclick"].Contains ("javascript:");
    }
  }
}