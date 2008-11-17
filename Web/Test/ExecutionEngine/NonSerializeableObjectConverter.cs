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
using System.ComponentModel;
using Remotion.Utilities;

namespace Remotion.Web.Test.ExecutionEngine
{
  public class NonSerializeableObjectConverter : TypeConverter
  {
    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof (string))
        return true;
      return base.CanConvertFrom (context, sourceType);
    }

    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof (string))
        return true;
      return base.CanConvertTo (context, destinationType);
    }

    public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      if (value is string)
      {
        return new NonSerializeableObject ((string) value);
      }
      return base.ConvertFrom (context, culture, value);
    }

    public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      NonSerializeableObject obj = ArgumentUtility.CheckNotNullAndType<NonSerializeableObject> ("value", value);
      if (destinationType == typeof (string))
      {
        return obj.Value;
      }
      return base.ConvertTo (context, culture, value, destinationType);
    }

  }
}