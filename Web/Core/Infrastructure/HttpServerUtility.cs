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
using System.Web;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.Utilities;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="HttpServerUtility"/> type is the default implementation of the <see cref="IHttpServerUtility"/> interface.
  /// </summary>
  public class HttpServerUtility : IHttpServerUtility
  {
    private readonly HttpContext _context;

    public HttpServerUtility (HttpContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      _context = context;
    }

    public void Transfer (string path, bool preserveForm)
    {
      _context.Server.Transfer (path, preserveForm);
    }
  }
}