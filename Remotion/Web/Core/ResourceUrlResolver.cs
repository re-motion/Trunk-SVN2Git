// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Configuration;
using Remotion.Web.Design;
using Remotion.Web.UI.Controls;

namespace Remotion.Web
{

/// <summary> Utility methods for URL resolving. </summary>
public sealed class ResourceUrlResolver
{
  private const string c_designTimeRootDefault = "C:\\Remotion.Resources";
  private const string c_designTimeRootEnvironmentVaribaleName = "REMOTIONRESOURCES";

  /// <summary>
  ///   Returns the physical URL of a resource item.
  ///   <seealso cref="IResourceUrlResolver"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     If the current ASP.NET application object implements <see cref="IResourceUrlResolver"/>, the application 
  ///     object creates the URL string. 
  ///     Otherwise, or if <paramref name="context"/> is <see langword="null"/>, the URL 
  ///     <c>&lt;resource root&gt;/&lt;definingType.Assembly&gt;/&lt;ResourceType&gt;/fileName</c> is used.
  ///   </para><para>
  ///     The <b>resource root</b> is loaded from the application configuration,
  ///     <see cref="Remotion.Web.Configuration.WebConfiguration.Resources">WebConfiguration.Resources</see>, and 
  ///     defaults to <c>/&lt;AppDir&gt;/res</c>, e.g. <c>/WebApplication/res/Remotion.Web/Image/Help.gif</c>.
  ///   </para><para>
  ///     During design time, the <b>resource root</b> is mapped to the environment variable
  ///     <c>REMOTIONRESOURCES</c>, or if the variable does not exist, <c>C:\Remotion.Resources</c>.
  ///   </para>
  /// </remarks>
  /// <param name="control"> 
  ///   The current <see cref="Control"/>. Currently, this parameter is only used to detect design time.
  /// </param>
  /// <param name="context"> The current <see cref="HttpContext"/>. </param>
  /// <param name="definingType"> The type that this resource item is associated with. </param>
  /// <param name="resourceType"> The resource type (image, static html, etc.) </param>
  /// <param name="relativeUrl"> The relative URL of the item. </param>
  public static string GetResourceUrl (IControl control, HttpContextBase context, Type definingType, ResourceType resourceType, string relativeUrl)
  {
    IResourceUrlResolver resolver = null;
    if (context != null)
      resolver = context.ApplicationInstance as IResourceUrlResolver;
    if (resolver != null)
      return resolver.GetResourceUrl (control, definingType, resourceType, relativeUrl);
    else
      return GetResourceUrl (control, definingType, resourceType, relativeUrl);
  }

  /// <summary>
  ///   Returns the physical URL of a resource item.
  /// </summary>
  /// <seealso cref="IResourceUrlResolver"/>.
  /// <remarks>
  ///   <para>
  ///     Uses the URL &lt;resource root&gt;/&lt;definingType.Assembly&gt;/&lt;ResourceType&gt;/relativeUrl.
  ///   </para><para>
  ///     The <b>resource root</b> is loaded from the application configuration,
  ///     <see cref="Remotion.Web.Configuration.WebConfiguration.Resources">WebConfiguration.Resources</see>, and 
  ///     defaults to <c>/&lt;AppDir&gt;/res</c>, e.g. <c>/WebApplication/res/Remotion.Web/Image/Help.gif</c>.
  ///   </para><para>
  ///     During design time, the <b>resource root</b> is mapped to the environment variable
  ///     <c>REMOTIONRESOURCES</c>, or if the variable does not exist, <c>C:\Remotion.Resources</c>.
  ///   </para>
  /// </remarks>
  /// <param name="control"> 
  ///   The current <see cref="Control"/>. This parameter is only used to detect design time.
  /// </param>
  /// <param name="definingType"> 
  ///   The type that this resource item is associated with. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="resourceType"> The resource type (image, static html, etc.) Must not be <see langword="null"/>. </param>
  /// <param name="relativeUrl"> The resource file name. Must not be <see langword="null"/> or empty.</param>
  public static string GetResourceUrl (IControl control, Type definingType, ResourceType resourceType, string relativeUrl)
  {
    return GetResourceUrl (control, definingType, resourceType, null, relativeUrl);
  }

  /// <summary>
  ///   Returns the physical URL of a resource item.
  /// </summary>
  /// <seealso cref="IResourceUrlResolver"/>.
  /// <remarks>
  ///   <para>
  ///     Uses the URL &lt;resource root&gt;/&lt;definingType.Assembly&gt;/&lt;ResourceType&gt;/relativeUrl.
  ///   </para><para>
  ///     The <b>resource root</b> is loaded from the application configuration,
  ///     <see cref="Remotion.Web.Configuration.WebConfiguration.Resources">WebConfiguration.Resources</see>, and 
  ///     defaults to <c>/&lt;AppDir&gt;/res</c>, e.g. <c>/WebApplication/res/Remotion.Web/Image/Help.gif</c>.
  ///   </para><para>
  ///     During design time, the <b>resource root</b> is mapped to the environment variable
  ///     <c>REMOTIONRESOURCES</c>, or if the variable does not exist, <c>C:\Remotion.Resources</c>.
  ///   </para>
  /// </remarks>
  /// <param name="control"> 
  ///   The current <see cref="Control"/>. This parameter is only used to detect design time.
  /// </param>
  /// <param name="definingType"> 
  ///   The type that this resource item is associated with. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="resourceType"> The resource type (image, static html, etc.) Must not be <see langword="null"/>. </param>
  /// <param name="theme">The <see cref="ResourceTheme"/> to which the resource belongs.</param>
  /// <param name="relativeUrl"> The resource file name. Must not be <see langword="null"/> or empty.</param>
  public static string GetResourceUrl (IControl control, Type definingType, ResourceType resourceType, ResourceTheme theme, string relativeUrl)
  {
    ArgumentUtility.CheckNotNull ("definingType", definingType);
    ArgumentUtility.CheckNotNull ("resourceType", resourceType);
    ArgumentUtility.CheckNotNullOrEmpty ("relativeUrl", relativeUrl);

    IResourceUrl resourceUrl;

    bool isDesignMode = (control == null) ? false : Remotion.Web.Utilities.ControlHelper.IsDesignMode (control);
    if (isDesignMode)
    {
      if (theme == null)

        resourceUrl = new DesignTimeResourceUrl (definingType, resourceType, relativeUrl);
      else
        resourceUrl = new DesignTimeThemedResourceUrl (definingType, resourceType, theme, relativeUrl);
    }
    else
    {
      if (theme == null)
        resourceUrl = new ResourceUrl (definingType, resourceType, relativeUrl);
      else
        resourceUrl = new ThemedResourceUrl (definingType, resourceType, theme, relativeUrl);
    }

    return resourceUrl.GetUrl();
  }

  /// <summary> Returns the root folder for all resources belonging to the <paramref name="assembly"/>. </summary>
  /// <param name="isDesignMode"> <see langword="true"/> if the application is in design mode. </param>
  /// <param name="assembly">The <paramref name="assembly"/> for which a ressource is being resolved.</param>
  /// <returns> 
  ///   The folder where the resources are expected to be for the <paramref name="assembly"/>. 
  ///   Always ends on a slash.
  /// </returns>
  public static string GetAssemblyRoot (bool isDesignMode, Assembly assembly)
  {
    ArgumentUtility.CheckNotNull ("assembly", assembly);
    
    string root = GetRoot (isDesignMode);
    string assemblyName = assembly.FullName.Split (new char[]{','}, 2)[0];
    string separator = isDesignMode ? @"\" : "/";
    return root + assemblyName + separator;
  }

  /// <summary> Returns the root folder for all resources. </summary>
  /// <param name="isDesignMode"> <see langword="true"/> if the application is in design mode. </param>
  /// <returns> 
  ///   The folder where the resources are expected to be. Ends on a slash unless the root folder is an 
  ///   empty string.
  /// </returns>
  public static string GetRoot (bool isDesignMode)
  {
    string root;
    
    if (isDesignMode)
      root = GetDesignTimeRoot();
    else 
      root = GetRunTimeRoot();

    string separator = isDesignMode ? @"\" : "/";
    if (root.Length > 0 && ! root.EndsWith (separator))
      root += separator;
    return root;
  }

  private static string GetRunTimeRoot()
  {
    string root = WebConfiguration.Current.Resources.Root;
    if (HttpRuntime.AppDomainAppVirtualPath != "/")
      root = HttpRuntime.AppDomainAppVirtualPath + "/" + root;
    if (! root.StartsWith ("/"))
      root = "/" + root;
    return root;
  }

  private static string GetDesignTimeRoot()
  { 
    string root = System.Environment.GetEnvironmentVariable (c_designTimeRootEnvironmentVaribaleName);
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
