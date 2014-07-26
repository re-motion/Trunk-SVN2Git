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
using JetBrains.Annotations;
using Remotion.Context;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a scope within no security will be evaluated. Use <see cref="SecurityFreeSection"/>.<see cref="Create"/> to enter the scope 
  /// and <see cref="Scope.Dispose"/> to leave the scope.
  /// </summary>
  public static class SecurityFreeSection
  {
    /// <summary>
    /// The <see cref="Scope"/> struct can be used to mark a section of code where no security will be evaluated. 
    /// The section should be exited by invoking the <see cref="Dispose"/> method.
    /// </summary>
    [CannotApplyEqualityOperator]
    public struct Scope : IDisposable
    {
      private readonly int _activeSectionsCount;
      private bool _isDisposed;

      internal Scope (int activeSectionsCount)
      {
        Assertion.DebugAssert (activeSectionsCount > 0);

        _activeSectionsCount = activeSectionsCount;
        _isDisposed = false;
      }

      /// <summary>
      /// Exits the <see cref="SecurityFreeSection"/> <see cref="Scope"/>.
      /// </summary>
      /// <exception cref="InvalidOperationException">
      /// The <see cref="Scope"/> was not exited in the correct nesting order.
      /// <para>- or -</para>
      /// The <see cref="Scope"/> was initialized using the value type's default construtor.
      /// </exception>
      public void Dispose ()
      {
        if (!_isDisposed)
        {
          if (_activeSectionsCount == 0)
            throw new InvalidOperationException ("The SecurityFreeSection scope has not been entered by invoking SecurityFreeSection.Create().");

          DecrementActiveSections (_activeSectionsCount);
          _isDisposed = true;
        }
      }

      [Obsolete ("Use Dispose() instead. (Version 1.15.21.0)")]
      public void Leave ()
      {
        Dispose();
      }
    }

    private class ActiveSections
    {
      // Mutable to avoid object allocation during increment and decrement
      public int Count { get; set; }

      public ActiveSections ()
      {
        Count = 0;
      }
    }

    private static readonly string s_activeSectionCountKey = SafeContextKeys.SecuritySecurityFreeSection;

    /// <summary>
    /// Enters a new <see cref="SecurityFreeSection"/> <see cref="Scope"/>. 
    /// The <see cref="Scope"/> should always be used via a using-block, or if that is not possible, disposed inside a finally-block.
    /// </summary>
    public static Scope Create ()
    {
      var activeSectionsCount = IncrementActiveSections();
      return new Scope (activeSectionsCount);
    }

    public static bool IsActive
    {
      get
      {
        var activeSections = GetActiveSections();
        return activeSections.Count > 0;
      }
    }

    private static ActiveSections GetActiveSections ()
    {
      var activeSections = (ActiveSections) SafeContext.Instance.GetData (s_activeSectionCountKey);
      if (activeSections == null)
      {
        activeSections = new ActiveSections();
        SafeContext.Instance.SetData (s_activeSectionCountKey, activeSections);
      }
      return activeSections;
    }

    private static int IncrementActiveSections ()
    {
      var activeSections = GetActiveSections();
      activeSections.Count++;
      return activeSections.Count;
    }

    private static void DecrementActiveSections (int numberOfActiveSectionsExpected)
    {
      var activeSections = GetActiveSections();

      if (activeSections.Count != numberOfActiveSectionsExpected)
      {
        throw new InvalidOperationException (
            "Nested SecurityFreeSection scopes have been exited out-of-sequence. "
            + "Entering a SecurityFreeSection should always be combined with a using-block for the scope, or if this is not possible, a finally-block for leaving the scope.");
      }

      activeSections.Count--;
    }
  }
}