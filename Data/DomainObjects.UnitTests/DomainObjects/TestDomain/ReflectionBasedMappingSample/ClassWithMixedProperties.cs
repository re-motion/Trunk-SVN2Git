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
  public abstract class ClassWithMixedProperties: ClassWithMixedPropertiesNotInMapping
  {
    protected ClassWithMixedProperties ()
    {
    }

    [StorageClassNone]
    public object Unmanaged
    {
      get { return null; }
      set { }
    }

    public abstract int Int32 { get; set; }

    public virtual string String
    {
      get
      {
        return CurrentProperty.GetValue<string> (); // CurrentProperty used on purpose here - tests whether shadowed properties work correctly
      }
      set
      {
        CurrentProperty.SetValue (value); // CurrentProperty used on purpose here - tests whether shadowed properties work correctly
      }
    }

    public abstract ClassWithOneSideRelationProperties UnidirectionalOneToOne { get; set; }

    private string PrivateString
    {
      get
      {
        return Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.PrivateString"]
            .GetValue<string> ();
      }
      set
      {
        Properties["Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.PrivateString"]
            .SetValue (value);
      }
    }
  }
}
