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
using Remotion.Utilities;

namespace Remotion.Implementation
{
  internal class ArgumentUtility
  {
    public static T CheckNotNull<T> (string argumentName, T actualValue)
    {
      if (actualValue == null)
        throw new ArgumentNullException (argumentName);

      return actualValue;
    }

    public static string CheckNotNullOrEmpty (string argumentName, string actualValue)
    {
      CheckNotNull (argumentName, actualValue);
      if (actualValue.Length == 0)
        throw new ArgumentEmptyException (argumentName);

      return actualValue;
    }
  }
}
