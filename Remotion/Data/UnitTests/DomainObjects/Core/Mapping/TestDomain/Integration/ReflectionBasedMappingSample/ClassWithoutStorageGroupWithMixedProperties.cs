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
  //No StorageGroup
  [Instantiable]
  public abstract class ClassWithoutStorageGroupWithMixedProperties : DomainObject
  {
    protected ClassWithoutStorageGroupWithMixedProperties ()
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
        return Properties["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.String"]
            .GetValue<string> ();
      }
      set { Properties["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.String"]
            .SetValue (value); }
    }

    public abstract ClassWithVirtualRelationEndPoints UnidirectionalOneToOne { get; set; }

    private string PrivateString
    {
      get
      {
        return Properties["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.PrivateString"]
            .GetValue<string> ();
      }
      set
      {
        Properties["Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithMixedProperties.PrivateString"].SetValue (value);
      }
    }
  }
}
