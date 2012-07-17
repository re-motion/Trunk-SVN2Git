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

namespace Remotion.Development.UnitTesting.ObjectMother
{
  /// <summary>
  /// Supplies factories to easily create <see cref="List{T}"/> instances.
  /// </summary>
  /// <example><code>
  /// <![CDATA[  
  /// var listList = ListMother.New( List.New(1,2), List.New(3,4) );
  /// ]]>
  /// </code></example>
  public class ListMother
  {
    public static System.Collections.Generic.List<T> New<T> (params T[] values)
    {
      var container = new System.Collections.Generic.List<T> (values);
      return container;
    }
  }
}
