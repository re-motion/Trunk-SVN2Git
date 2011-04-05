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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MixinTestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class PropertyFinderBaseMixinsIntegrationTest : PropertyFinderBaseTestBase
  {
    [Test]
    public void FindPropertyInfos_ForInheritanceRoot ()
    {
      var classDefinition = CreateReflectionBasedClassDefinition (typeof (TargetClassA));
      var propertyFinder = new StubPropertyFinderBase (typeof (TargetClassA), classDefinition, true, true, classDefinition.PersistentMixinFinder);
      
      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EquivalentTo (
              new []
                  {
                      GetProperty (typeof (TargetClassBase), "P0"),
                      GetProperty (typeof (MixinBase), "P0a"),
                      GetProperty (typeof (TargetClassA), "P1"),
                      GetProperty (typeof (TargetClassA), "P2"),
                      GetProperty (typeof (MixinA), "P5"),
                      GetProperty (typeof (MixinC), "P7"),
                      GetProperty (typeof (MixinD), "P8"),
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForNonInheritanceRoot ()
    {
      var classDefinition = CreateReflectionBasedClassDefinition (typeof (TargetClassA));
      var propertyFinder = new StubPropertyFinderBase (typeof (TargetClassA), classDefinition, false, true, classDefinition.PersistentMixinFinder);
      
      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EquivalentTo (
              new []
                  {
                      GetProperty (typeof (TargetClassA), "P1"),
                      GetProperty (typeof (TargetClassA), "P2"),
                      GetProperty (typeof (MixinA), "P5"),
                      GetProperty (typeof (MixinC), "P7"),
                      GetProperty (typeof (MixinD), "P8"),
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForDerived ()
    {
      var classDefinition = CreateReflectionBasedClassDefinition (typeof (TargetClassB));
      var propertyFinder = new StubPropertyFinderBase (typeof (TargetClassB), classDefinition, false, true, classDefinition.PersistentMixinFinder);

      Assert.That (
          propertyFinder.FindPropertyInfos (),
          Is.EquivalentTo (
              new[]
                  {
                      GetProperty (typeof (TargetClassB), "P3"),
                      GetProperty (typeof (TargetClassB), "P4"),
                      GetProperty (typeof (MixinB), "P6"),
                      GetProperty (typeof (MixinE), "P9"),
                  }));
    }
  }
}
