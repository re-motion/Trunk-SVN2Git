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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.IntegrationTests
{
  [TestFixture]
  public class AttributesOnUnsupportedTypesIntegrationTests : ValidationIntegrationTestBase
  {
    [Test]
    [ExpectedException(typeof(MappingException), 
      ExpectedMessage = "The 'Remotion.Data.DomainObjects.StringPropertyAttribute' may be only applied to properties of type 'System.String'.\r\n"
        +"The 'Remotion.Data.DomainObjects.BinaryPropertyAttribute' may be only applied to properties of type 'System.Byte[]'.\r\n"
        +"The 'Remotion.Data.DomainObjects.ExtensibleEnumPropertyAttribute' may be only applied to properties of type "
        +"'Remotion.ExtensibleEnums.IExtensibleEnum'.\r\n"
        +"The 'Remotion.Data.DomainObjects.MandatoryAttribute' may be only applied to properties assignable to types "
        +"'Remotion.Data.DomainObjects.DomainObject' or 'Remotion.Data.DomainObjects.ObjectList`1[T]'.\r\n"
        + "The 'Remotion.Data.DomainObjects.DBBidirectionalRelationAttribute' may be only applied to properties assignable to types "
        +"'Remotion.Data.DomainObjects.DomainObject' or 'Remotion.Data.DomainObjects.ObjectList`1[T]'.")]
    public void AttributesOnUnsupportedTypes ()
    {
      ValidateMapping ("AttributesOnUnsupportedTypes");
    }
  }
}