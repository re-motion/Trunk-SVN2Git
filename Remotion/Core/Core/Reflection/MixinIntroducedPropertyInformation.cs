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
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Represents a mixin property that is introduced to its target classes.
  /// </summary>
  /// <remarks>
  /// This is mainly just a wrapper around a <see cref="IPropertyInformation"/>
  /// describing the property as it is implemented on the mixin type itself. The <see cref="GetGetMethod"/> and <see cref="GetSetMethod"/> methods, 
  /// however, return instances of <see cref="MixinIntroducedMethodInformation"/>. This means that a <see cref="MixinIntroducedPropertyInformation"/>'s
  /// getter and setter can be used to get and set the value both via the mixin instance and via an instance of the target class. 
  /// <see cref="GetValue"/> and <see cref="SetValue"/> also work this way.
  /// </remarks>
  /// <seealso cref="MixinIntroducedMethodInformation"/>
  public class MixinIntroducedPropertyInformation : IPropertyInformation
  {
    private readonly InterfaceImplementationPropertyInformation _mixinPropertyInfo;

    public MixinIntroducedPropertyInformation (InterfaceImplementationPropertyInformation mixinPropertyInfo)
    {
      ArgumentUtility.CheckNotNull ("mixinPropertyInfo", mixinPropertyInfo);

      _mixinPropertyInfo = mixinPropertyInfo;
    }

    public string Name
    {
      get { return _mixinPropertyInfo.Name;  }
    }

    public Type DeclaringType
    {
      get { return _mixinPropertyInfo.DeclaringType;  }
    }

    public Type GetOriginalDeclaringType ()
    {
      return _mixinPropertyInfo.GetOriginalDeclaringType();
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return _mixinPropertyInfo.GetCustomAttribute<T>(inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return _mixinPropertyInfo.GetCustomAttributes<T> (inherited);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return _mixinPropertyInfo.IsDefined<T> (inherited);
    }

    public IPropertyInformation FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);

      return _mixinPropertyInfo.FindInterfaceImplementation (implementationType);
    }

    public IPropertyInformation FindInterfaceDeclaration ()
    {
      return _mixinPropertyInfo.FindInterfaceDeclaration();
    }

    public ParameterInfo[] GetIndexParameters ()
    {
      return _mixinPropertyInfo.GetIndexParameters();
    }

    public Type PropertyType
    {
      get { return _mixinPropertyInfo.PropertyType; }
    }

    public bool CanBeSetFromOutside
    {
      get { return _mixinPropertyInfo.CanBeSetFromOutside; }
    }

    public object GetValue (object instance, object[] indexParameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      return _mixinPropertyInfo.GetValue (instance, indexParameters);
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      _mixinPropertyInfo.SetValue (instance, value, indexParameters);
    }

    public IMethodInformation GetGetMethod (bool nonPublic)
    {
      return Maybe
          .ForValue (_mixinPropertyInfo.GetGetMethod (nonPublic))
          .Select (mi => mi as InterfaceImplementationMethodInformation)
          .Select (mi => new MixinIntroducedMethodInformation (mi))
          .ValueOrDefault ();
    }

    public IMethodInformation GetSetMethod (bool nonPublic)
    {
      return Maybe
          .ForValue (_mixinPropertyInfo.GetSetMethod (nonPublic))
          .Select (mi => mi as InterfaceImplementationMethodInformation)
          .Select (mi => new MixinIntroducedMethodInformation (mi))
          .ValueOrDefault ();
    }

    IMemberInformation IMemberInformation.FindInterfaceImplementation (Type implementationType)
    {
      return FindInterfaceImplementation (implementationType);
    }

    IMemberInformation IMemberInformation.FindInterfaceDeclaration ()
    {
      return FindInterfaceDeclaration();
    }

    public override string ToString ()
    {
      // TODO Review 3325: Should be _mixinPropertyInfo + " (Mixin)"
      return Name + "(added by mixin)";
    }
  }
}