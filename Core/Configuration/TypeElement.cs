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
using System.Configuration;

namespace Remotion.Configuration
{
  public class TypeElement<TBase> : ConfigurationElement
      where TBase : class
  {
    //TODO: test
    public static ConfigurationProperty CreateTypeProperty (Type defaultValue)
    {
      return new ConfigurationProperty (
          "type",
          typeof (Type),
          defaultValue,
          new Remotion.Utilities.TypeNameConverter (),
          new SubclassTypeValidator (typeof (TBase)),
          ConfigurationPropertyOptions.IsRequired);
    }

    private readonly ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _typeProperty;

    public TypeElement ()
      : this (null)
    {
    }

    protected TypeElement (Type defaultValue)
    {
      _typeProperty = CreateTypeProperty (defaultValue);

      _properties = new ConfigurationPropertyCollection ();
      _properties.Add (_typeProperty);
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public Type Type
    {
      get { return (Type) base[_typeProperty]; }
      set { base[_typeProperty] = value; }
    }

    public TBase CreateInstance ()
    {
      if (Type == null)
        return null;
        
      return (TBase) Activator.CreateInstance (Type);
    }
  }

  public class TypeElement<TBase, TDefault> : TypeElement<TBase>
    where TBase : class
    where TDefault : TBase
  {
    public TypeElement ()
      : base (typeof (TDefault))
    {
    }
  }
}
