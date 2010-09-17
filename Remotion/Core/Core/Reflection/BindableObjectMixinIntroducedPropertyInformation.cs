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
using Remotion.Utilities;

namespace Remotion.Reflection
{
  // Note that this class is only temporay, used as long as BindableObjectGlobalizationService requires the concrete type and property in order to 
  // work correctly

  public class BindableObjectMixinIntroducedPropertyInformation : MixinIntroducedPropertyInformation
  {
    private readonly Type _concreteType;
    private readonly PropertyInfo _concreteProperty;

    public BindableObjectMixinIntroducedPropertyInformation (IPropertyInformation mixinPropertyInfo, Type concreteType, PropertyInfo propertyInfo)
        : base(mixinPropertyInfo)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);

      _concreteType = concreteType;
      _concreteProperty = propertyInfo;
    }

    public Type ConcreteType
    {
      get { return _concreteType; }
    }

    public PropertyInfo ConcreteProperty
    {
      get { return _concreteProperty; }
    }
  }
}