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
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue"/> control.
  /// </summary>
  [UsedImplicitly]
  public class BocBooleanValueControlObject : BocControlObject
  {
    public BocBooleanValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the current state of the boolean.
    /// </summary>
    public bool? GetState ()
    {
      if (Scope[DiagnosticMetadataAttributes.IsReadOnly] == "true")
        return ParseState (Scope.FindChild ("Value")["data-value"]);

      return ParseState (Scope.FindChild ("Value").Value);
    }

    /// <summary>
    /// Returns whether this instance supports tri-state.
    /// </summary>
    public bool IsTriState ()
    {
      return Scope[DiagnosticMetadataAttributesForObjectBinding.BocBooleanValueIsTriState] == "true";
    }

    /// <summary>
    /// Sets the state of the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValue"/> to <paramref name="newState"/>.
    /// </summary>
    public UnspecifiedPageObject SetTo (bool? newState, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      if (!IsTriState() && !newState.HasValue)
        throw new ArgumentException ("Must not be null for non-tri-state BocBooleanValue controls.", "newState");

      if (GetState() == newState)
        return UnspecifiedPage();

      if (!IsTriState())
        return Click (1, completionDetection);

      var states = new bool?[] { false, null, true, false, null };
      var numberOfClicks = Array.LastIndexOf (states, newState) - Array.IndexOf (states, GetState());
      return Click (numberOfClicks, completionDetection);
    }

    private UnspecifiedPageObject Click (int numberOfClicks, ICompletionDetection completionDetection)
    {
      var linkScope = Scope.FindChild ("DisplayValue");

      for (var i = 0; i < numberOfClicks; ++i)
      {
        var actualCompletionDetector = GetActualCompletionDetector (completionDetection);
        linkScope.ClickAndWait (Context, actualCompletionDetector);
      }

      return UnspecifiedPage();
    }

    private bool? ParseState (string state)
    {
      if (state == "False")
        return false;

      if (state == "True")
        return true;

      return null;
    }
  }
}