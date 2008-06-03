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

namespace Remotion.Development.UnitTesting
{
  public class PEVerifyException : Exception
  {
    public PEVerifyException (string message) : base (message)
    {
    }

    public PEVerifyException (string message, Exception inner) : base (message, inner)
    {
    }

    public PEVerifyException (int resultCode, string output) : base (ConstructMessage (resultCode, output))
    {
    }

    private static string ConstructMessage (int code, string output)
    {
      return string.Format ("PEVerify returned {0}\n{1}", code, output);
    }
  }
}
