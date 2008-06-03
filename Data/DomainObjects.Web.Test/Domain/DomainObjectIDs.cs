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

namespace Remotion.Data.DomainObjects.Web.Test.Domain
{
  public static class DomainObjectIDs
  {
    public static ObjectID ObjectWithAllDataTypes1 = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));
    public static ObjectID ObjectWithUndefinedEnum = new ObjectID ("ClassWithUndefinedEnum", new Guid ("{4F85CEE5-A53A-4bc5-B9D3-448C48946498}"));
  }
}
