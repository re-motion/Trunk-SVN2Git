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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample
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
