// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.ComponentModel;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

/// <summary> Exposes non-public members of the <see cref="TypeConversionProvider"/> type. </summary>
public class StubTypeConversionProvider: TypeConversionProvider
{
  public StubTypeConversionProvider ()
      : base (SafeServiceLocator.Current.GetAllInstances<ITypeConverterFactory>())
  {
  }

  public new TypeConverter GetTypeConverterByAttribute (Type type)
  {
    return base.GetTypeConverterByAttribute (type);
  }

  public new TypeConverter GetBasicTypeConverter (Type type)
  {
    return base.GetBasicTypeConverter (type);
  }

  public new void AddTypeConverterToCache (Type key, TypeConverter converter)
  {
    base.AddTypeConverterToCache (key, converter);
  }

  public new TypeConverter GetTypeConverterFromCache (Type key)
  {
    return base.GetTypeConverterFromCache (key);
  }

  public new bool HasTypeInCache (Type type)
  {
    return base.HasTypeInCache (type);
  }

  public static void ClearCache()
  {
    var cache =
        (LockingDataStoreDecorator<Type, TypeConverter>) PrivateInvoke.GetNonPublicStaticField (typeof (TypeConversionProvider), "s_typeConverters");
    cache.Clear();
  }
}

}
