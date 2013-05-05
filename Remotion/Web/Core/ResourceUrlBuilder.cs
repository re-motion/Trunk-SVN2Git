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
using System.Reflection;
using System.Text;
using System.Web;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Configuration;

namespace Remotion.Web
{
  public abstract class ResourceUrlBuilderBase
  {
    protected abstract string GetResourceRoot ();

    protected abstract string BuildPath (string[] completePath);

    public string BuildAbsoluteUrl (Assembly assembly, params string[] assemblyRelativeUrlParts)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("assemblyRelativeUrlParts", assemblyRelativeUrlParts);

      string root = GetResourceRoot();
      string assemblyName = assembly.FullName.Split (new[] { ',' }, 2)[0];

      string[] completePath = ArrayUtility.Combine (new[] { root, assemblyName }, assemblyRelativeUrlParts);
      return BuildPath (completePath);
    }
  }

  public class ResourceUrlBuilder : ResourceUrlBuilderBase
  {
    private readonly string _configuredResourceRoot;

    public ResourceUrlBuilder ()
    {
      _configuredResourceRoot = WebConfiguration.Current.Resources.Root;
      Assertion.IsFalse (_configuredResourceRoot.StartsWith ("/"));
      Assertion.IsFalse (_configuredResourceRoot.EndsWith ("/"));
    }

    protected override string GetResourceRoot ()
    {
      var stringBuilder = new StringBuilder();

      var applicationPath = GetApplicationPath();
      if (!applicationPath.StartsWith ("/"))
        stringBuilder.Append ("/");
      stringBuilder.Append (applicationPath);
      if (applicationPath.Length > 0 && !applicationPath.EndsWith ("/"))
        stringBuilder.Append ("/");

      stringBuilder.Append (_configuredResourceRoot);

      return stringBuilder.ToString();
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

    protected override string BuildPath (string[] completePath)
    {
      return StringUtility.ConcatWithSeparator (completePath, "/");
    }
  }

  public class DesignTimeResourceUrlBuilder : ResourceUrlBuilderBase
  {
    private const string c_designTimeRootDefault = "C:\\Remotion.Resources";
    private const string c_designTimeRootEnvironmentVaribaleName = "REMOTIONRESOURCES";

    protected override string GetResourceRoot ()
    {
      return GetDesignTimeRoot();
    }

    protected override string BuildPath (string[] completePath)
    {
      return StringUtility.ConcatWithSeparator (completePath, "\\");
    }


    private static string GetDesignTimeRoot ()
    {
      string root = Environment.GetEnvironmentVariable (c_designTimeRootEnvironmentVaribaleName);
      if (StringUtility.IsNullOrEmpty (root))
        root = c_designTimeRootDefault;
      return root;

      //EnvDTE._DTE environment = (EnvDTE._DTE) site.GetService (typeof (EnvDTE._DTE));
      //if(environment != null)
      //{
      //  EnvDTE.Project project = environment.ActiveDocument.ProjectItem.ContainingProject;          
      //  //  project.Properties uses a 1-based index
      //  for (int i = 1; i <= project.Properties.Count; i++)
      //  {
      //    if(project.Properties.Item (i).Name == "ActiveFileSharePath")
      //      return project.Properties.Item (i).Value.ToString();
      //  }
      //}
    }
  }
}