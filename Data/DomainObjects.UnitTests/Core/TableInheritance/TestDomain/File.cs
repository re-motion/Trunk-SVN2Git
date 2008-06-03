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

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_File")]
  [DBTable ("TableInheritance_File")]
  [Instantiable]
  public abstract class File: FileSystemItem
  {
    public static File NewObject()
    {
      return NewObject<File>().With();
    }

    public static File GetObject (ObjectID id)
    {
      return GetObject<File> (id);
    }

    protected File ()
    {
    }

    public abstract int Size { get; set; }

    [DBColumn ("FileCreatedAt")]
    public abstract DateTime CreatedAt { get; set; }
  }
}
