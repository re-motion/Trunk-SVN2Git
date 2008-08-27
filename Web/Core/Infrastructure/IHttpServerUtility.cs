/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// This interface is a facade for methods exposed the <see cref="System.Web.HttpServerUtility"/> and related features (i.e. transfer, redirect,...).
  /// </summary>
  public interface IHttpServerUtility
  {
    void Transfer (string path, bool preserveForm);
  }
}