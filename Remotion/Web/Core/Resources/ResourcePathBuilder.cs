// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Linq;
using System.Web;
using Remotion.Utilities;
using Remotion.Web.Configuration;

namespace Remotion.Web.Resources
{
  public class ResourcePathBuilder : ResourcePathBuilderBase
  {
    private readonly string _configuredResourceRoot;

    public ResourcePathBuilder ()
    {
      _configuredResourceRoot = WebConfiguration.Current.Resources.Root;
    }

    protected override string BuildPath (string[] completePath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("completePath", completePath);

      return completePath.Aggregate (VirtualPathUtility.Combine);
    }

    protected override string GetResourceRoot ()
    {
      var applicationPath = GetApplicationPath();
      Assertion.IsTrue (VirtualPathUtility.IsAbsolute (applicationPath));
      return VirtualPathUtility.Combine (applicationPath, _configuredResourceRoot);
    }

    private string GetApplicationPath ()
    {
      var context = HttpContext.Current;
      if (context != null)
        return GetApplicationPathFromHttpContext (context);

      return HttpRuntime.AppDomainAppVirtualPath ?? "/";
    }

    private string GetApplicationPathFromHttpContext (HttpContext context)
    {
      var applicationPath = context.Request.ApplicationPath ?? "/";

      var requestUrlAbsolutePath = context.Request.Url.AbsolutePath;
      if (requestUrlAbsolutePath.StartsWith (applicationPath, StringComparison.OrdinalIgnoreCase))
        return requestUrlAbsolutePath.Remove (applicationPath.Length);

      //Note: context.Request.Url.LocalPath would provide the unescaped Path. 
      //      Unfortunately, cookie-paths are matched against the case and the escape sequences, 
      //      so this would only result in silent failures when using cookie paths.

      throw new InvalidOperationException (
          string.Format (
              "Cannot calculate the application path when the request URL does not start with the application path. "
              + "Possible reasons include the use of escape sequences in the path, e.g. when the application path contains whitespace.\r\n"
              + "  Absolute path from request: {0}\r\n"
              + "  Application path: {1}\r\n",
              requestUrlAbsolutePath,
              applicationPath));
    }
  }
}