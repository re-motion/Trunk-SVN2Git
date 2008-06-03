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
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Web.Domain
{
  [BindableObject]
  public class TypeWithDateTime
  {
    public static TypeWithDateTime Create ()
    {
      return ObjectFactory.Create<TypeWithDateTime> (true).With ();
    }

    private DateTime _dateTimeValue;
    private DateTime? _nullableDateTimeValue;

    protected TypeWithDateTime ()
    {
    }

    public DateTime DateTimeValue
    {
      get { return _dateTimeValue; }
      set { _dateTimeValue = value; }
    }

    public DateTime? NullableDateTimeValue
    {
      get { return _nullableDateTimeValue; }
      set { _nullableDateTimeValue = value; }
    }
  }
}
