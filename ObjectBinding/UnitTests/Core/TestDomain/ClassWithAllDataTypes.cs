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

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  public class ClassWithAllDataTypes
  {
    public ClassWithAllDataTypes ()
    {
    }

    public bool Boolean { get; set; }

    public byte Byte { get; set; }

    [DateProperty]
    public DateTime Date { get; set; }

    public DateTime DateTime { get; set; }

    public decimal Decimal { get; set; }

    public double Double { get; set; }

    public TestEnum Enum { get; set; }

    public TestFlags Flags { get; set; }

    public Guid Guid { get; set; }

    public short Int16 { get; set; }

    public int Int32 { get; set; }

    public long Int64 { get; set; }

    public SimpleBusinessObjectClass BusinessObject { get; set; }

    public float Single { get; set; }

    public string String { get; set; }
  }
}