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
using System.Collections;
using System.ComponentModel;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

/// <summary> Exposes non-public members of the <see cref="TypeConversionProvider"/> type. </summary>
public class StubTypeConversionProvider: TypeConversionProvider
{
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
    IDictionary cache = (IDictionary) PrivateInvoke.GetNonPublicStaticField (typeof (TypeConversionProvider), "s_typeConverters");
    cache.Clear();
  }
}

}
