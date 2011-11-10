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
using System;

namespace Remotion.Linq.SqlBackend
{
  /// <summary>
  /// <see cref="StringExtensions"/> provides extension methods for strings.
  /// </summary>
  public static class StringExtensions
  {
    public static bool SqlLike (this string source, string pattern)
    {
      throw new NotImplementedException ("This method is only meant for translation to SQL.");
    }

    public static bool SqlContainsFulltext (this string source, string searchCondition)
    {
      throw new NotImplementedException ("This method is only meant for translation to SQL.");
    }

    public static bool SqlContainsFulltext (this string source, string searchCondition, string language)
    {
      throw new NotImplementedException ("This method is only meant for translation to SQL.");
    }

    public static bool SqlContainsFreetext (this string source, string searchCondition)
    {
      throw new NotImplementedException ("This method is only meant for translation to SQL.");
    }

    public static bool SqlContainsFreetext (this string source, string searchCondition, string language)
    {
      throw new NotImplementedException ("This method is only meant for translation to SQL.");
    }
  }
}