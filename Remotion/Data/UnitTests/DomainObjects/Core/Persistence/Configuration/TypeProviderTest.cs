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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.ExtensibleEnums;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Configuration
{
  [TestFixture]
  public class TypeProviderTest
  {
    private TypeProvider _typeProvider;

    private enum TestEnum { }

    [SetUp]
    public void SetUp ()
    {
      _typeProvider = new TypeProvider ();
    }

    [Test]
    public void AddSupportedType ()
    {
      Assert.That (_typeProvider.IsTypeSupported (typeof (ClassDefinition)), Is.False);

      _typeProvider.AddSupportedType (typeof (ClassDefinition));

      Assert.That (_typeProvider.IsTypeSupported (typeof (ClassDefinition)), Is.True);
    }

    [Test]
    public void AddSupportedBaseType_BaseClass ()
    {
      Assert.That (_typeProvider.IsTypeSupported (typeof (ClassDefinition)), Is.False);
      Assert.That (_typeProvider.IsTypeSupported (typeof (ReflectionBasedClassDefinition)), Is.False);

      _typeProvider.AddSupportedBaseType (typeof (ClassDefinition));

      Assert.That (_typeProvider.IsTypeSupported (typeof (ClassDefinition)), Is.False);
      Assert.That (_typeProvider.IsTypeSupported (typeof (ReflectionBasedClassDefinition)), Is.True);
    }

    [Test]
    public void AddSupportedBaseType_Interface ()
    {
      Assert.That (_typeProvider.IsTypeSupported (typeof (IRelationEndPointDefinition)), Is.False);
      Assert.That (_typeProvider.IsTypeSupported (typeof (RelationEndPointDefinition)), Is.False);

      _typeProvider.AddSupportedBaseType (typeof (IRelationEndPointDefinition));

      Assert.That (_typeProvider.IsTypeSupported (typeof (IRelationEndPointDefinition)), Is.False);
      Assert.That (_typeProvider.IsTypeSupported (typeof (RelationEndPointDefinition)), Is.True);
    }

    [Test]
    public void Initialization_StandardTypes ()
    {
      Assert.That (_typeProvider.IsTypeSupported (typeof (bool)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (byte)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (DateTime)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (decimal)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (double)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (TestEnum)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (Color)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (Guid)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (short)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (int)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (long)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (float)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (string)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (ObjectID)), Is.True);
      Assert.That (_typeProvider.IsTypeSupported (typeof (byte[])), Is.True);

      Assert.That (_typeProvider.IsTypeSupported (typeof (Enum)), Is.False);
      Assert.That (_typeProvider.IsTypeSupported (typeof (IExtensibleEnum)), Is.False);

      Assert.That (_typeProvider.IsTypeSupported (typeof (object)), Is.False);
      Assert.That (_typeProvider.IsTypeSupported (typeof (char)), Is.False);
      Assert.That (_typeProvider.IsTypeSupported (typeof (char[])), Is.False);
      Assert.That (_typeProvider.IsTypeSupported (typeof (DomainObject)), Is.False);
      Assert.That (_typeProvider.IsTypeSupported (typeof (DomainObjectCollection)), Is.False);
    }
  }
}
