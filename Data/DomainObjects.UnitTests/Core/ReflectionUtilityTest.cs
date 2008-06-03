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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using File=System.IO.File;

namespace Remotion.Data.DomainObjects.UnitTests.Core
{
  [TestFixture]
  public class ReflectionUtilityTest : StandardMappingTest
  {
    [Test]
    public void GetPropertyName ()
    {
      PropertyInfo propertyInfo = typeof (DerivedClassWithMixedProperties).GetProperty ("Int32");

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.Int32",
          ReflectionUtility.GetPropertyName (propertyInfo));
    }
       
    private int TestProperty
    {
      get { return 0; }
      set { }
    }

    private static int StaticTestProperty
    {
      get { return 0; }
      set { }
    }

    public int TestPropertyMixedVisibility
    {
      get { return 12; }
      private set { }
    }

    [Test]
    public void GetPropertyFromMethod ()
    {
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName (""));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("bla"));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("MethodWithLongName"));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("get_"));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("set_"));

      Assert.AreEqual ("Prop", ReflectionUtility.GetPropertyNameForMethodName ("get_Prop"));
      Assert.AreEqual ("Prop", ReflectionUtility.GetPropertyNameForMethodName ("set_Prop"));

      Assert.IsNull (ReflectionUtility.GetPropertyForMethod (typeof (object).GetMethod ("ToString")));

      Assert.AreEqual (
          typeof (Order).GetProperty ("Number"),
          ReflectionUtility.GetPropertyForMethod (typeof (Order).GetMethod ("get_Number")));
      Assert.AreEqual (
          typeof (Order).GetProperty ("Number"),
          ReflectionUtility.GetPropertyForMethod (typeof (Order).GetMethod ("set_Number")));

      PropertyInfo privateProperty = typeof (ReflectionUtilityTest).GetProperty (
          "TestProperty",
          BindingFlags.NonPublic
              | BindingFlags.Instance);
      Assert.IsNotNull (privateProperty);
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod (privateProperty.GetGetMethod (true)));
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod (privateProperty.GetSetMethod (true)));

      privateProperty = typeof (ReflectionUtilityTest).GetProperty (
          "StaticTestProperty",
          BindingFlags.NonPublic
              | BindingFlags.Static);
      Assert.IsNotNull (privateProperty);
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod (privateProperty.GetGetMethod (true)));
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod (privateProperty.GetSetMethod (true)));

      PropertyInfo mixedVisibilityProperty = typeof (ReflectionUtilityTest).GetProperty (
          "TestPropertyMixedVisibility",
          BindingFlags.Public | BindingFlags.Instance);

      Assert.IsNotNull (mixedVisibilityProperty);
      Assert.AreEqual (mixedVisibilityProperty, ReflectionUtility.GetPropertyForMethod (mixedVisibilityProperty.GetGetMethod (true)));
      Assert.AreEqual (mixedVisibilityProperty, ReflectionUtility.GetPropertyForMethod (mixedVisibilityProperty.GetSetMethod (true)));
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
            }, directoryPath, newAssemblyPath);
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
      _Assembly assemblyMock = mockRepository.CreateMock<_Assembly>();

      SetupResult.For (assemblyMock.EscapedCodeBase).Return ("http://server/File.ext");
      mockRepository.ReplayAll();

      ReflectionUtility.GetAssemblyDirectory (assemblyMock);
    }

    [Test]
    public void GetDomainObjectAssemblyDirectory ()
    {
      Assert.AreEqual (Path.GetDirectoryName (new Uri (typeof (DomainObject).Assembly.EscapedCodeBase).AbsolutePath),
          ReflectionUtility.GetConfigFileDirectory());
    }
  }
}
