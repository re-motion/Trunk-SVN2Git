// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.ExtensibleEnums;
using System.Reflection;

namespace Remotion.UnitTests.ExtensibleEnums.TestDomain
{
  public static class EnumWithDifferentCtorsExtensions
  {
    public static EnumWithDifferentCtors ShortIDOnly (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors ("ShortID");
    }

    public static EnumWithDifferentCtors ShortIDAndPrefix (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors ("Prefix", "ShortID");
    }

    public static EnumWithDifferentCtors ShortIDAndNullPrefix (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors ((string) null, "ShortID");
    }

    public static EnumWithDifferentCtors ShortIDAndEmptyPrefix (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors ("", "ShortID");
    }

    public static EnumWithDifferentCtors ShortIDAndType (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors (typeof (EnumWithDifferentCtorsExtensions), "ShortID");
    }

    public static EnumWithDifferentCtors MethodAsID (this ExtensibleEnumDefinition<EnumWithDifferentCtors> enumWithDifferentCtors)
    {
      return new EnumWithDifferentCtors (MethodBase.GetCurrentMethod());
    }
  }
}
