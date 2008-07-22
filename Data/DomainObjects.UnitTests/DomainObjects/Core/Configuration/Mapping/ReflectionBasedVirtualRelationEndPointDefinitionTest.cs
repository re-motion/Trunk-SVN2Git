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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class ReflectionBasedVirtualRelationEndPointDefinitionTest : StandardMappingTest
  {
    [Test]
    public void PropertyInfo ()
    {
      ClassDefinition employeeClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Employee));
      ReflectionBasedVirtualRelationEndPointDefinition relationEndPointDefinition =
          (ReflectionBasedVirtualRelationEndPointDefinition) employeeClassDefinition.GetRelationEndPointDefinition (typeof (Employee) + ".Computer");
      Assert.AreEqual (typeof (Employee).GetProperty ("Computer"), relationEndPointDefinition.PropertyInfo);
    }
  }
}
