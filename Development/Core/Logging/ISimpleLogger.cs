/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Development.Logging
{
  public interface ISimpleLogger : INullObject
  {
    void It (object obj);
    void It (string s);
    void It (string format, params object[] parameters);
    void Item (object obj);
    void Item (string s);
    void Item (string format, params object[] parameters);

    void Sequence(params object[] parameters);
  }
}