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
using System;
using Remotion.ExtensibleEnums;
using Remotion.Globalization;

namespace Remotion.UnitTests.ExtensibleEnums.TestDomain
{
  [MultiLingualResources ("Remotion.UnitTests.Resources.ExtensibleEnumWithDuplicateAttributeExtensions")]
  [MultiLingualResources ("Remotion.UnitTests.Resources.ExtensibleEnumWithDuplicateAttributeExtensions")]
  public static class ExtensibleEnumWithDuplicateAttributeExtensions
  {
    public static ExtensibleEnumWithDuplicateAttribute Value1 (this ExtensibleEnumDefinition<ExtensibleEnumWithDuplicateAttribute> definition)
    {
      return new ExtensibleEnumWithDuplicateAttribute ("Value1");
    }

    public static ExtensibleEnumWithDuplicateAttribute Value2 (this ExtensibleEnumDefinition<ExtensibleEnumWithDuplicateAttribute> definition)
    {
      return new ExtensibleEnumWithDuplicateAttribute ("Value2");
    }

    public static ExtensibleEnumWithDuplicateAttribute Value3 (this ExtensibleEnumDefinition<ExtensibleEnumWithDuplicateAttribute> definition)
    {
      return new ExtensibleEnumWithDuplicateAttribute ("Value3");
    }
  }
}