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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class for all control objects. Much like <see cref="PageObject"/>s, control objects hide the actual HTML structure from the web test
  /// developer and instead provide a semantic interface. In contrast to <see cref="PageObject"/>s, control objects represent a specific
  /// ASP.NET (custom) control and not a whole page.
  /// </summary>
  public abstract class ControlObject : WebTestObject<ControlObjectContext>
  {
    protected ControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Provides access to child controls of the control.
    /// </summary>
    public IControlHost Children
    {
      get { return new ControlHost (Context); }
    }

    /// <summary>
    /// Return's the control's HTML ID.
    /// </summary>
    /// <exception cref="MissingHtmlException">If the DOM element does not bear the ID attribute.</exception>
    public string GetHtmlID ()
    {
      return Scope.Id;
    }

    /// <summary>
    /// Returns the actual <see cref="ICompletionDetector"/> to be used when acting on the control object's scope.
    /// </summary>
    /// <param name="userDefinedCompletionDetection">User-provided <see cref="ICompletionDetection"/>.</param>
    /// <returns>The <see cref="ICompletionDetector"/> to be used.</returns>
    protected ICompletionDetector GetActualCompletionDetector ([CanBeNull] ICompletionDetection userDefinedCompletionDetection)
    {
      return GetActualCompletionDetector (Scope, userDefinedCompletionDetection);
    }

    /// <summary>
    /// Returns the actual <see cref="ICompletionDetector"/>, which should be used when interacting with the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope of the DOM element on which the interaction is going to take place.</param>
    /// <param name="userDefinedCompletionDetection">User-provided <see cref="ICompletionDetection"/>.</param>
    /// <returns>The <see cref="ICompletionDetector"/> to be used.</returns>
    protected ICompletionDetector GetActualCompletionDetector (
        [NotNull] ElementScope scope,
        [CanBeNull] ICompletionDetection userDefinedCompletionDetection)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      var actualCompletionDetection = userDefinedCompletionDetection ?? GetDefaultCompletionDetection (scope);
      return actualCompletionDetection.Build();
    }

    /// <summary>
    /// Returns the control's default <see cref="ICompletionDetection"/> builder configuration, which should be used when interacting with the given
    /// <paramref name="scope"/>.
    /// </summary>
    protected abstract ICompletionDetection GetDefaultCompletionDetection ([NotNull] ElementScope scope);

    /// <summary>
    /// Convinience method which returns a new <see cref="UnspecifiedPageObject"/>.
    /// </summary>
    /// <returns>A new unspecified page object.</returns>
    protected UnspecifiedPageObject UnspecifiedPage ()
    {
      return new UnspecifiedPageObject (Context);
    }
  }
}