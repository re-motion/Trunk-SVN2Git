// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System.Collections.Generic;
using Remotion.Linq.Utilities;

namespace Remotion.Linq
{
  /// <summary>
  /// Generates unique identifiers based on a set of known identifiers.
  /// An identifier is generated by appending a number to a given prefix. The identifier is considered unique when no known identifier
  /// exists which equals the prefix/number combination.
  /// </summary>
  public class UniqueIdentifierGenerator
  {
    private readonly HashSet<string> _knownIdentifiers = new HashSet<string>();
    private int _identifierCounter;

    /// <summary>
    /// Adds the given <paramref name="identifier"/> to the set of known identifiers.
    /// </summary>
    /// <param name="identifier">The identifier to add.</param>
    public void AddKnownIdentifier (string identifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("identifier", identifier);
      _knownIdentifiers.Add (identifier);
    }

    private bool IsKnownIdentifier (string identifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("identifier", identifier);
      return _knownIdentifiers.Contains (identifier);
    }

    public void Reset ()
    {
      _knownIdentifiers.Clear();
      _identifierCounter = 0;
    }

    /// <summary>
    /// Gets a unique identifier starting with the given <paramref name="prefix"/>. The identifier is generating by appending a number to the
    /// prefix so that the resulting string does not match a known identifier.
    /// </summary>
    /// <param name="prefix">The prefix to use for the identifier.</param>
    /// <returns>A unique identifier starting with <paramref name="prefix"/>.</returns>
    public string GetUniqueIdentifier (string prefix)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("prefix", prefix);

      string identifier;
      do
      {
        identifier = prefix + _identifierCounter;
        ++_identifierCounter;
      } while (IsKnownIdentifier (identifier));

      return identifier;
    }
  }
}