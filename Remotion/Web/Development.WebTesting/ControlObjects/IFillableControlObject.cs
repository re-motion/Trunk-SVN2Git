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

using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing something fillable.
  /// </summary>
  public interface IFillableControlObject
  {
    /// <summary>
    /// Returns the current text.
    /// </summary>
    string GetText ();

    /// <summary>
    /// Fills the control with the given <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The text to fill in.</param>
    /// <param name="completionDetection">Required <see cref="ICompletionDetection"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    UnspecifiedPageObject FillWith ([NotNull] string text, [CanBeNull] ICompletionDetection completionDetection = null);

    /// <summary>
    /// Fills the control with the given <paramref name="text"/> and afterwards executes the given <paramref name="finishInputWith"/> action.
    /// </summary>
    /// <param name="text">The text to fill in.</param>
    /// <param name="finishInputWith">What to do after the text has been filled in (see <see cref="FinishInput"/> for supported default options).</param>
    /// <param name="completionDetection">Required <see cref="ICompletionDetection"/>, implementation uses default behavior if <see langword="null" /> is passed.</param>
    /// <returns>An unspecified page object, may be used in case a new page is expected after clicking the control object.</returns>
    UnspecifiedPageObject FillWith (
        [NotNull] string text,
        [NotNull] FinishInputWithAction finishInputWith,
        [CanBeNull] ICompletionDetection completionDetection = null);
  }
}