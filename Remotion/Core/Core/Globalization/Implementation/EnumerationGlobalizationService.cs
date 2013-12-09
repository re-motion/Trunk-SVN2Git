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
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Retrieving the human-readable localized representation of enumeration objects.
  /// </summary>
  public class EnumerationGlobalizationService : IEnumerationGlobalizationService
  {
    public string GetEnumerationValueDisplayName (Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      // TODO RM-5831: change this to have the implementation in here. Get IGlobalizationService injected into ctor.
      // cache ResourceManager for EnumType
      // if has ResourceManager, get globalization from ResourceManager and fallback to value.ToString()
      // else use EnumDescriptionAttriute as used in EnumDescription-class, but only cache the Enum as key and string as value, also cache the fallback to value.ToString()
      // Dpublicate existing tests for EnumDescription for GetDescription-API
      // Inject IMemberInformationNameResolver and add GetEnumerationValueName (Enum value) to API for resolving the identifier.
      return EnumDescription.GetDescription (value);
    }
  }
}