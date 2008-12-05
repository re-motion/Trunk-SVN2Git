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
