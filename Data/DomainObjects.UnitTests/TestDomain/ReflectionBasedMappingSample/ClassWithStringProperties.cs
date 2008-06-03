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

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithStringProperties : DomainObject
  {
    protected ClassWithStringProperties ()
    {
    }

    public abstract string NoAttribute { get; set; }

    [StringProperty (IsNullable = true)]
    public abstract string NullableFromAttribute { get; set; }

    [StringProperty (IsNullable = false)]
    public abstract string NotNullable { get; set; }

    [StringProperty (MaximumLength = 100)]
    public abstract string MaximumLength { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string NotNullableAndMaximumLength { get; set; }
  }
}
