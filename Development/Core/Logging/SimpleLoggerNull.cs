/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Development.Logging;

namespace Remotion.Development.Logging
{
  public class SimpleLoggerNull : ISimpleLogger
  {
    public void It(object obj)
    {
      // Does nothing on purpose
    }

    public void It(string s)
    {
      // Does nothing on purpose
    }

    public void It(string format, params object[] parameters)
    {
      // Does nothing on purpose
    }

    public void Item(object obj)
    {
      // Does nothing on purpose
    }

    public void Item(string s)
    {
      // Does nothing on purpose
    }

    public void Item(string format, params object[] parameters)
    {
      // Does nothing on purpose
    }

    public void Sequence (params object[] parameters)
    {
      // Does nothing on purpose
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}