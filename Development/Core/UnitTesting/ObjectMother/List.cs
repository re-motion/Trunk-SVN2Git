/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;

namespace Remotion.Development.UnitTesting.ObjectMother
{
  /// <summary>
  /// Supplies factories to easily create <see cref="List{T}"/> instances.
  /// </summary>
  /// <example><code>
  /// <![CDATA[  
  /// var listList = List.New( List.New(1,2), List.New(3,4) );
  /// ]]>
  /// </code></example>
  public class List
  {
    public static System.Collections.Generic.List<T> New<T> (params T[] values)
    {
      var container = new System.Collections.Generic.List<T> (values);
      return container;
    }
  }
}