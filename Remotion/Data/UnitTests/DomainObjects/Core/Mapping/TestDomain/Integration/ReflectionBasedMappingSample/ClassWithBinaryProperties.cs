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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithBinaryProperties : DomainObject
  {
    protected ClassWithBinaryProperties ()
    {
    }

    public abstract byte[] NoAttribute { get; set; }

    [BinaryProperty (IsNullable = true)]
    public abstract byte[] NullableFromAttribute { get; set; }

    [BinaryProperty (IsNullable = false)]
    public abstract byte[] NotNullable { get; set; }

    [BinaryProperty (MaximumLength = 100)]
    public abstract byte[] MaximumLength { get; set; }

    [BinaryProperty (IsNullable = false, MaximumLength = 100)]
    public abstract byte[] NotNullableAndMaximumLength { get; set; }
  }
}
