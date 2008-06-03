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

namespace Remotion.Context
{
//  Irgendwos CurrentIrgendwos 
//{
//  get { return (Irgendwos) SafeContextStorage.GetValue ("Irgendwos"); }

//  get { return SafeContextStorage.GetValue<Irgendwos> ("Irgendwos"); }
//  get { return SafeContextStorage.GetValue<Irgendwos> (); }


//  get { return SafeContextStorage.GetContextSingleton<Irgendwos> (); }


//  get 
//  {
//    return SafeContextStorage.GetOrCreateValue<Irgendwos> (
//      "Irgendwos",
//      delegate() { return new Irgendwos(...); });
//  }

  internal class SafeContext
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public SafeContext ()
    {
    }

    // methods and properties
  }
}
