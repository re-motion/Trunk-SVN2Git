// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.ExtensibleEnums;

namespace Remotion.UnitTests.ExtensibleEnums.TestDomain
{
  public static class WrongColorValues
  {
    public static int WrongReturnType (this ExtensibleEnumValues<Color> values)
    {
      throw new NotImplementedException ();
    }

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
    private static Color WrongVisibility1 (this ExtensibleEnumValues<Color> values)
// ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local
    {
      throw new NotImplementedException ();
    }

    internal static Color WrongVisibility2 (this ExtensibleEnumValues<Color> values)
    {
      throw new NotImplementedException ();
    }

    public static Color NonExtensionMethod (ExtensibleEnumValues<Color> values)
    {
      throw new NotImplementedException ();
    }

    public static Color WrongParameterCount (this ExtensibleEnumValues<Color> values, int index)
    {
      throw new NotImplementedException ();
    }

    public static Color NotDerivedFromValuesClass (this Color values)
    {
      throw new NotImplementedException ();
    }

    public static Color DerivedFromDerivedValuesClass (this ExtensibleEnumValues<MetallicColor> values)
    {
      throw new NotImplementedException ();
    }

// ReSharper disable UnusedTypeParameter
    public static Color Generic<T> (this ExtensibleEnumValues<MetallicColor> values)
// ReSharper restore UnusedTypeParameter
    {
      throw new NotImplementedException ();
    }
  }
}