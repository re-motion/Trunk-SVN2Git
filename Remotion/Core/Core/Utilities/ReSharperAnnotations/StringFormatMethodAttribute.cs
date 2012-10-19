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

namespace JetBrains.Annotations
{
  /// <summary>
  /// Indicates that marked method builds string by format pattern and (optional) arguments. 
  /// Parameter, which contains format string, should be given in constructor.
  /// The format string should be in <see cref="string.Format(IFormatProvider,string,object[])"/> -like form
  /// </summary>
  [AttributeUsage (AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class StringFormatMethodAttribute : Attribute
  {
    /// <summary>
    /// Initializes new instance of StringFormatMethodAttribute
    /// </summary>
    /// <param name="formatParameterName">Specifies which parameter of an annotated method should be treated as format-string</param>
    public StringFormatMethodAttribute (string formatParameterName)
    {
      FormatParameterName = formatParameterName;
    }

    /// <summary>
    /// Gets format parameter name
    /// </summary>
    [UsedImplicitly]
    public string FormatParameterName { get; private set; }
  }
}