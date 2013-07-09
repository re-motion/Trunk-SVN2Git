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
using System.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation
{
  /// <summary>
  /// Utility class for formatting date/time values in a consistent way.
  /// <seealso cref="BocDateTimeValue"/>
  /// <seealso cref="BocDateTimeValueRenderer"/>
  /// </summary>
  public class DateTimeFormatter
  {
    public DateTimeFormatter ()
    {
    }

    /// <summary> Formats the <see cref="DateTime"/> value's date component according to the current culture. </summary>
    /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
    /// <returns> A formatted string representing the <see cref="DateTime"/> value's date component. </returns>
    public virtual string FormatDateValue (DateTime dateValue)
    {
      return dateValue.ToString ("d");
    }

    /// <summary> Formats the <see cref="DateTime"/> value's time component according to the current culture. </summary>
    /// <param name="timeValue"> The <see cref="DateTime"/> value to be formatted. </param>
    /// <param name="showSeconds"> <see langword="true"/> if the time format includes seconds. </param>
    /// <returns>  A formatted string representing the <see cref="DateTime"/> value's time component. </returns>
    public virtual string FormatTimeValue (DateTime timeValue, bool showSeconds)
    {
      //  ignore Read-Only

      if (showSeconds)
      {
        //  T: hh, mm, ss
        return timeValue.ToString ("T");
      }
      else
      {
        //  T: hh, mm
        return timeValue.ToString ("t");
      }
    }

    /// <summary> Formats the <see cref="DateTime"/> value's time component according to the current culture. </summary>
    /// <param name="timeValue"> The <see cref="DateTime"/> value to be formatted. </param>
    /// <returns> A formatted string representing the <see cref="DateTime"/> value's time component.  </returns>
    public virtual string FormatTimeValue (DateTime timeValue)
    {
      return FormatTimeValue (timeValue, false);
    }

    public bool Is12HourTimeFormat ()
    {
      return !string.IsNullOrEmpty(CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator);
    }
  }
}