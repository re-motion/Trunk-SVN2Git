// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
