/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.ObjectBinding.Sample
{
  public class ClassWithEnums:BindableXmlObject
  {
    public enum Enum
    {
      First,
      Second
    }

    [UndefinedEnumValue(Undefined)]
    public enum UndefinedEnum
    {
      Undefined,
      First,
      Second
    }

    public static ClassWithEnums CreateObject ()
    {
      return CreateObject<ClassWithEnums> ();
    }

    protected ClassWithEnums ()
    {
    }

    public Enum EnumNotNullable { get; set; }
    public Enum? EnumNullable { get; set; }
    public UndefinedEnum EnumUndefined { get; set; }
  }
}