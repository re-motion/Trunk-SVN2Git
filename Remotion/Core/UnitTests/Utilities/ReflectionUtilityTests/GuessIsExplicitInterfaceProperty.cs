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
using System.Reflection;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class GuessIsExplicitInterfaceProperty
  {
    public interface Interface
    {
      int Property01 { get; }
      int Property02 { get; }
      int Property03 { get; }
      int Property04 { get; }

      int Property05 { set; }
      int Property06 { set; }
      int Property07 { set; }
      int Property08 { set; }

      int Property09 { get; set; }
      int Property10 { get; set; }
      int Property11 { get; set; }
    }

    public class ClassWithInterfaceProperties : Interface
    {
      public int Property01
      {
        get { throw new NotImplementedException(); }
      }

      public int Property02
      {
        get { throw new NotImplementedException(); }
        private set { throw new NotImplementedException(); }
      }

      int Interface.Property03
      {
        get { throw new NotImplementedException(); }
      }

      public virtual int Property04
      {
        get { throw new NotImplementedException(); }
      }

      public int Property05
      {
        set { throw new NotImplementedException(); }
      }

      public int Property06
      {
        set { throw new NotImplementedException(); }
        private get { throw new NotImplementedException(); }
      }

      int Interface.Property07
      {
        set { throw new NotImplementedException(); }
      }

      public virtual int Property08
      {
        set { throw new NotImplementedException(); }
      }

      public int Property09
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      int Interface.Property10
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      public virtual int Property11
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }
    }

    public class ClassWithoutInterfaceProperties
    {
      public int Property01
      {
        get { throw new NotImplementedException(); }
      }

      public int Property02
      {
        get { throw new NotImplementedException(); }
        private set { throw new NotImplementedException(); }
      }

      private int Property03
      {
        get { throw new NotImplementedException(); }
      }

      public virtual int Property04
      {
        get { throw new NotImplementedException(); }
      }

      public int Property05
      {
        set { throw new NotImplementedException(); }
      }

      public int Property06
      {
        set { throw new NotImplementedException(); }
        private get { throw new NotImplementedException(); }
      }

      private int Property07
      {
        set { throw new NotImplementedException(); }
      }

      public virtual int Property08
      {
        set { throw new NotImplementedException(); }
      }

      public int Property09
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      private int Property10
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      public virtual int Property11
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }
    }

    [Test]
    public void NonInterfaceProperties ()
    {
      Type type = typeof (ClassWithoutInterfaceProperties);
      PropertyInfo[] properties = type.GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.AreEqual (11, properties.Length);

      foreach (PropertyInfo property in properties)
        Assert.IsFalse (ReflectionUtility.GuessIsExplicitInterfaceProperty (property));
    }

    [Test]
    public void InterfaceProperties ()
    {
      Type type = typeof (ClassWithInterfaceProperties);
      PropertyInfo[] properties = type.GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.AreEqual (11, properties.Length);

      Array.Sort (properties, delegate (PropertyInfo one, PropertyInfo two)
      {
        return GetShortName (one.Name).CompareTo (GetShortName (two.Name));
      });

      Assert.AreEqual ("Property01", properties[0].Name);
      Assert.AreEqual ("Property02", properties[1].Name);
      Assert.AreEqual ("Remotion.UnitTests.Utilities.ReflectionUtilityTests.GuessIsExplicitInterfaceProperty.Interface.Property03",
          properties[2].Name);

      Assert.IsFalse (ReflectionUtility.GuessIsExplicitInterfaceProperty (properties[0]));
      Assert.IsFalse (ReflectionUtility.GuessIsExplicitInterfaceProperty (properties[1]));
      Assert.IsTrue (ReflectionUtility.GuessIsExplicitInterfaceProperty (properties[2]));
      Assert.IsFalse (ReflectionUtility.GuessIsExplicitInterfaceProperty (properties[3]));
      Assert.IsFalse (ReflectionUtility.GuessIsExplicitInterfaceProperty (properties[4]));
      Assert.IsFalse (ReflectionUtility.GuessIsExplicitInterfaceProperty (properties[5]));
      Assert.IsTrue (ReflectionUtility.GuessIsExplicitInterfaceProperty (properties[6]));
      Assert.IsFalse (ReflectionUtility.GuessIsExplicitInterfaceProperty (properties[7]));
      Assert.IsFalse (ReflectionUtility.GuessIsExplicitInterfaceProperty (properties[8]));
      Assert.IsTrue (ReflectionUtility.GuessIsExplicitInterfaceProperty (properties[9]));
      Assert.IsFalse (ReflectionUtility.GuessIsExplicitInterfaceProperty (properties[10]));
    }

    private string GetShortName (string name)
    {
      return name.Substring (name.LastIndexOf ('.') + 1);
    }
  }
}
