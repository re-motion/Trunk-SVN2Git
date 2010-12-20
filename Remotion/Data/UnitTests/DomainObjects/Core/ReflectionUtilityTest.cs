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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using File = System.IO.File;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class ReflectionUtilityTest : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classDefinitionWithMixedproperty;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _classDefinitionWithMixedproperty = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassWithMixedProperty), typeof (MixinAddingProperty));
    }

    [Test]
    [Obsolete]
    public void GetPropertyName ()
    {
      PropertyInfo propertyInfo = typeof (DerivedClassWithDifferentProperties).GetProperty ("Int32");

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithDifferentProperties.Int32",
          ReflectionUtility.GetPropertyName (propertyInfo));
    }

    [Test]
    public void GetAssemblyPath ()
    {
      Assert.AreEqual (AppDomain.CurrentDomain.BaseDirectory, ReflectionUtility.GetAssemblyDirectory (typeof (ReflectionUtilityTest).Assembly));
    }

    [Test]
    public void GetAssemblyPath_WithHashInDirectoryName ()
    {
      string directoryPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "#HashTestPath");
      string originalAssemblyPath = typeof (ReflectionUtilityTest).Assembly.Location;
      string newAssemblyPath = Path.Combine (directoryPath, Path.GetFileName (originalAssemblyPath));

      if (Directory.Exists (directoryPath))
        Directory.Delete (directoryPath, true);

      Directory.CreateDirectory (directoryPath);
      try
      {
        File.Copy (originalAssemblyPath, newAssemblyPath);
        AppDomainRunner.Run (
            delegate (object[] args)
            {
              string directory = (string) args[0];
              string assemblyPath = (string) args[1];

              Assembly assembly = Assembly.LoadFile (assemblyPath);
              Assert.AreEqual (directory, Path.GetDirectoryName (assembly.Location));
              Assert.AreEqual (directory, ReflectionUtility.GetAssemblyDirectory (assembly));
            },
            directoryPath,
            newAssemblyPath);
      }
      finally
      {
        Directory.Delete (directoryPath, true);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The assembly's code base 'http://server/File.ext' is not a local path.")]
    public void GetAssemblyPath_FromNonLocalUri ()
    {
      MockRepository mockRepository = new MockRepository();
      _Assembly assemblyMock = mockRepository.StrictMock<_Assembly>();

      SetupResult.For (assemblyMock.EscapedCodeBase).Return ("http://server/File.ext");
      mockRepository.ReplayAll();

      ReflectionUtility.GetAssemblyDirectory (assemblyMock);
    }

    [Test]
    public void GetDomainObjectAssemblyDirectory ()
    {
      Assert.AreEqual (
          Path.GetDirectoryName (new Uri (typeof (DomainObject).Assembly.EscapedCodeBase).AbsolutePath),
          ReflectionUtility.GetConfigFileDirectory());
    }

    [Test]
    public void IsDomainObjectBase_DomainObjectBase ()
    {
      Assert.That (ReflectionUtility.IsDomainObjectBase (typeof (DomainObject)), Is.True);
    }

    [Test]
    public void IsDomainObjectBase_NoDomainObjectBase ()
    {
      Assert.That (ReflectionUtility.IsDomainObjectBase (typeof (string)), Is.False);
    }

    [Test]
    public void IsInheritanceRoot_BaseTypeIsDomainObjectBase ()
    {
      Assert.That (ReflectionUtility.IsInheritanceRoot (typeof (AbstractClass)), Is.True);
    }

    [Test]
    public void IsInheritanceRoot_BaseTypeIsNoDomainObjectBase_ClassHasStorageGroupAttributeApplied ()
    {
      Assert.That (ReflectionUtility.IsInheritanceRoot (typeof (Derived2ClassWithStorageGroupAttribute)), Is.True);
    }

    [Test]
    public void IsInheritanceRoot_BaseTypeIsNoDomainObjectBase_DomainObject ()
    {
      Assert.That (ReflectionUtility.IsInheritanceRoot (typeof (DomainObject)), Is.False);
    }

    [Test]
    public void IsRelationProperty_DomainObjectIsNotAssignableFromPropertyType ()
    {
      Assert.That (ReflectionUtility.IsDomainObject (typeof (string)), Is.False);
    }

    [Test]
    public void IsRelationProperty_DomainObjectIsAssignableFromPropertyType ()
    {
      Assert.That (ReflectionUtility.IsDomainObject (typeof (AbstractClass)), Is.True);
    }

    // TODO Review 3488: Add _classDefinitionWithMixedProperty, initialize in setup, use in the following tests

    [Test]
    public void GetDeclaringDomainObjectTypeForProperty_NoMixedProperty ()
    {
      var property = typeof (ClassWithMixedProperty).GetProperty ("PublicNonMixedProperty");

      var result = ReflectionUtility.GetDeclaringDomainObjectTypeForProperty (property, _classDefinitionWithMixedproperty);

      Assert.That (result, Is.SameAs (typeof (ClassWithMixedProperty)));
    }

    [Test]
    public void GetDeclaringDomainObjectTypeForProperty_MixedProperty ()
    {
      var property = typeof (MixinAddingProperty).GetProperty ("MixedProperty");

      var result = ReflectionUtility.GetDeclaringDomainObjectTypeForProperty (property, _classDefinitionWithMixedproperty);

      Assert.That (result, Is.SameAs (typeof (ClassWithMixedProperty)));
    }

    [Test]
    public void IsMixedProperty_NoMixedProperty_ReturnsFalse ()
    {
      var property = typeof (ClassWithMixedProperty).GetProperty ("PublicNonMixedProperty");

      var result = ReflectionUtility.IsMixedProperty (property, _classDefinitionWithMixedproperty);

      Assert.That (result, Is.False);
    }

    [Test]
    public void IsMixedProperty_MixedProperty_ReturnsTrue ()
    {
      var property = typeof (MixinAddingProperty).GetProperty ("MixedProperty");

      var result = ReflectionUtility.IsMixedProperty (property, _classDefinitionWithMixedproperty);

      Assert.That (result, Is.True);
    }
  }
}