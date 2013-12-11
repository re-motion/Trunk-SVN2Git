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
using System.Linq;
using JetBrains.Annotations;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  /// Use this class to get the clear text representations of enumeration values.
  /// </summary>
  /// <remarks>
  /// Use the <see cref="EnumDescriptionResourceAttribute"/> to provide globalization support for the enum type
  /// or the <see cref="EnumDescriptionAttribute"/> to only provide friendly names for the individual enum values.
  /// </remarks>
  public static class EnumDescription
  {
    private static readonly DoubleCheckedLockingContainer<IEnumerationGlobalizationService> s_globalizationService =
      new DoubleCheckedLockingContainer<IEnumerationGlobalizationService> (() => SafeServiceLocator.Current.GetInstance<IEnumerationGlobalizationService>());

    [NotNull]
    //[Obsolete("(Version 1.13.222.0)")]
    public static EnumValue[] GetAllValues ([NotNull] Type enumType)
    {
      ArgumentUtility.CheckNotNull ("enumType", enumType);

      return Enum.GetValues (enumType).Cast<Enum>()
          .Select (e => new EnumValue (e, s_globalizationService.Value.GetEnumerationValueDisplayName (e)))
          .ToArray();
    }

    [NotNull]
    //[Obsolete("(Version 1.13.222.0)")]
    public static EnumValue[] GetAllValues ([NotNull] Type enumType, [CanBeNull] CultureInfo culture)
    {
      ArgumentUtility.CheckNotNull ("enumType", enumType);

      using (new CultureScope (CultureInfo.CurrentCulture, culture ?? CultureInfo.CurrentUICulture))
      {
        return GetAllValues (enumType);
      }
    }

    [NotNull]
    //[Obsolete("Use IEnumerationGlobalizationService.GetEnumerationValueDisplayName. (Version 1.13.222.0)")]
    public static string GetDescription ([NotNull] Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      return s_globalizationService.Value.GetEnumerationValueDisplayName (value);
    }

    [NotNull]
    //[Obsolete("Use IEnumerationGlobalizationService.GetEnumerationValueDisplayName. (Version 1.13.222.0)")]
    public static string GetDescription ([NotNull] Enum value, [CanBeNull] CultureInfo culture)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      using (new CultureScope (CultureInfo.CurrentCulture, culture ?? CultureInfo.CurrentUICulture))
      {
        return GetDescription (value);
      }
    }
  }
}