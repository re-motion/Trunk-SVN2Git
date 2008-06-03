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
using Remotion.ObjectBinding;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Web.Test.Domain
{
  [BindableObject]
  public class SearchObjectWithUndefinedEnum
  {
    // types

    // static members and constants

    // member fields

    private UndefinedEnum _undefinedEnum;

    // construction and disposing

    public SearchObjectWithUndefinedEnum ()
    {
      _undefinedEnum = UndefinedEnum.Undefined;
    }

    // methods and properties

    public UndefinedEnum UndefinedEnum
    {
      get { return _undefinedEnum; }
      set { _undefinedEnum = value; }
    }
  }
}
