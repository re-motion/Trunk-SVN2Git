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

namespace Remotion.Development.UnitTesting.ObjectMothers
{
  /// <summary>
  /// Provides boolean values for unit tests.
  /// </summary>
  public static class BooleanObjectMother
  {
    private static readonly Random s_random = new Random ();

    /// <summary>
    /// Gets a random <see cref="bool"/> value. This is used by unit tests when they need code to work with arbitrary boolean values. Rather than
    /// duplicating the test, once for <see langword="true" /> and once for <see langword="false" />, the test is written once and is executed 
    /// with both <see langword="true" /> and <see langword="false" /> values chosen at random.
    /// </summary>
    /// <returns>A random <see cref="bool"/> value.</returns>
    public static bool GetRandomBoolean ()
    {
      return s_random.Next (2) == 1;
    }
  }
}