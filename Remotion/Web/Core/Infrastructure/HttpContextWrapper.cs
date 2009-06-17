// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Profile;
using System.Web.SessionState;
using Remotion.Utilities;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="HttpContextWrapper"/> type is the default implementation of the <see cref="IHttpContext"/> interface.
  /// </summary>
  public class HttpContextWrapper : IHttpContext
  {
    private readonly HttpContext _httpContext;
    private readonly IHttpRequest _request;
    private readonly IHttpResponse _response;
    private IHttpServerUtility _server;
    private IHttpSessionState _session;
    private IHttpApplicationState _applicationState;

    public HttpContextWrapper (HttpContext httpContext)
    {
      ArgumentUtility.CheckNotNull ("httpContext", httpContext);

      _httpContext = httpContext;
      _request = new HttpRequestWrapper (httpContext.Request);
      _response = new HttpResponseWrapper (httpContext.Response);
    }

    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="IHttpContext"/> wrapper.
    /// </summary>
    /// <exception cref="NotSupportedException">This is a stub implementation which does not contain an <see cref="HttpContext"/>. </exception>
    public HttpContext WrappedInstance
    {
      get { return _httpContext; }
    }

    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <returns>
    /// A service object of type <paramref name="serviceType" />.
    /// <para>-or- </para>
    /// <para><see langword="null" /> if there is no service object of type <paramref name="serviceType" />.</para>
    /// </returns>
    /// <param name="serviceType">
    /// An object that specifies the type of service object to get. 
    /// </param><filterpriority>2</filterpriority>
    public object GetService (Type serviceType)
    {
      return ((IServiceProvider) _httpContext).GetService(serviceType);
    }

    ///// <summary>
    ///// Enables you to specify a handler for the request.
    ///// </summary>
    ///// <param name="handler">
    ///// The object that should process the request.
    ///// </param>
    ///// <exception cref="T:System.InvalidOperationException">
    ///// The <see cref="M:System.Web.HttpContext.RemapHandler(System.Web.IHttpHandler)" /> method was called after the <see cref="E:System.Web.HttpApplication.MapRequestHandler" /> event occurred.
    ///// </exception>
    //public void RemapHandler (IHttpHandler handler)
    //{
    //  _httpContext.RemapHandler(handler);
    //}

    /// <summary>
    /// Adds an exception to the exception collection for the current HTTP request.
    /// </summary>
    /// <param name="errorInfo">
    /// The <see cref="T:System.Exception" /> to add to the exception collection.
    /// </param>
    public void AddError (Exception errorInfo)
    {
      _httpContext.AddError(errorInfo);
    }

    /// <summary>
    /// Clears all errors for the current HTTP request.
    /// </summary>
    public void ClearError ()
    {
      _httpContext.ClearError();
    }

    /// <summary>
    /// Gets a specified configuration section for the current application's default configuration. 
    /// </summary>
    /// <returns>
    /// The specified <see cref="T:System.Configuration.ConfigurationSection" />, <see langword="null" /> if the section does not exist, or an internal object if the section is not accessible at run time.
    /// </returns>
    /// <param name="sectionName">
    /// The configuration section path (in XPath format) and the configuration element name.
    /// </param>
    public object GetSection (string sectionName)
    {
      return _httpContext.GetSection(sectionName);
    }

    /// <summary>
    /// Rewrites the URL using the given path.
    /// </summary>
    /// <param name="path">
    /// The internal rewrite path.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="path" /> parameter is <see langword="null" />.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// The <paramref name="path" /> parameter is not in the current application's root directory.
    /// </exception>
    public void RewritePath (string path)
    {
      _httpContext.RewritePath(path);
    }

    /// <summary>
    /// Rewrites the URL using the given path and a Boolean value that specifies whether the virtual path for server resources is modified.
    /// </summary>
    /// <param name="path">
    /// The internal rewrite path.
    /// </param>
    /// <param name="rebaseClientPath">true to reset the virtual path; false to keep the virtual path unchanged.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="path" /> parameter is <see langword="null" />.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// The <paramref name="path" /> parameter is not in the current application's root directory.
    /// </exception>
    public void RewritePath (string path, bool rebaseClientPath)
    {
      _httpContext.RewritePath(path, rebaseClientPath);
    }

    /// <summary>
    /// Rewrites the URL using the given path, path information, and a Boolean value that specifies whether the virtual path for server resources is modified.
    /// </summary>
    /// <param name="filePath">
    /// The internal rewrite path.
    /// </param>
    /// <param name="pathInfo">
    /// Additional path information for a resource.
    /// </param>
    /// <param name="queryString">
    /// The request query string.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="filePath" /> parameter is not in the current application's root directory.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// The <paramref name="filePath" /> parameter is not in the current application's root directory.
    /// </exception>
    public void RewritePath (string filePath, string pathInfo, string queryString)
    {
      _httpContext.RewritePath(filePath, pathInfo, queryString);
    }

    /// <summary>
    /// Rewrites the URL using the given virtual path, path information, query string information, and a Boolean value that specifies whether the client file path is set to the rewrite path. 
    /// </summary>
    /// <param name="filePath">
    /// The virtual path to the resource that services the request.
    /// </param>
    /// <param name="pathInfo">
    /// Additional path information to use for the URL redirect.
    /// </param>
    /// <param name="queryString">
    /// The request query string to use for the URL redirect.
    /// </param>
    /// <param name="setClientFilePath">true to set the file path used for client resources to the value of the <paramref name="filePath" /> parameter; otherwise false.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="filePath" /> parameter is not in the current application's root directory.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// The <paramref name="filePath" /> parameter is not in the current application's root directory.
    /// </exception>
    public void RewritePath (string filePath, string pathInfo, string queryString, bool setClientFilePath)
    {
      _httpContext.RewritePath(filePath, pathInfo, queryString, setClientFilePath);
    }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Web.HttpApplication" /> object for the current HTTP request.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.HttpApplication" /> for the current HTTP request.
    /// </returns>
    public HttpApplication ApplicationInstance
    {
      get { return _httpContext.ApplicationInstance; }
      set { _httpContext.ApplicationInstance = value; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.HttpApplicationState" /> object for the current HTTP request.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.HttpApplicationState" /> for the current HTTP request.
    /// </returns>
    public IHttpApplicationState Application
    {
      get
      {
        if (_applicationState == null)
          _applicationState = new HttpApplicationStateWrapper (_httpContext.Application);
        return _applicationState;
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Web.IHttpHandler" /> object responsible for processing the HTTP request.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.IHttpHandler" /> responsible for processing the HTTP request.
    /// </returns>
    public IHttpHandler Handler
    {
      get { return _httpContext.Handler; }
      set { _httpContext.Handler = value; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.IHttpHandler" /> object for the parent handler.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.IHttpHandler" /> for the parent handler; otherwise, <see langword="null" /> if no previous handler was found.
    /// </returns>
    public IHttpHandler PreviousHandler
    {
      get { return _httpContext.PreviousHandler; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.IHttpHandler" /> object that represents the currently executing handler.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.IHttpHandler" /> that represents the currently executing handler. 
    /// </returns>
    public IHttpHandler CurrentHandler
    {
      get { return _httpContext.CurrentHandler; }
    }

    /// <summary>
    /// Gets the <see cref="IHttpRequest" /> object for the current HTTP request.
    /// </summary>
    /// <returns>
    /// The <see cref="IHttpRequest" /> for the current HTTP request.
    /// </returns>
    public IHttpRequest Request
    {
      get { return _request; }
    }

    /// <summary>
    /// Gets the <see cref="IHttpResponse" /> object for the current HTTP response.
    /// </summary>
    /// <returns>
    /// The <see cref="IHttpResponse" /> for the current HTTP response.
    /// </returns>
    public IHttpResponse Response
    {
      get { return _response; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.TraceContext" /> object for the current HTTP response.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.TraceContext" /> for the current HTTP response.
    /// </returns>
    public TraceContext Trace
    {
      get { return _httpContext.Trace; }
    }

    /// <summary>
    /// Gets a key/value collection that can be used to organize and share data between an <see cref="T:System.Web.IHttpModule" /> interface and an <see cref="T:System.Web.IHttpHandler" /> interface during an HTTP request.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IDictionary" /> key/value collection that provides access to an individual value in the collection by a specified key.
    /// </returns>
    public IDictionary Items
    {
      get { return _httpContext.Items; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.SessionState.HttpSessionState" /> object for the current HTTP request.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.SessionState.HttpSessionState" /> object for the current HTTP request.
    /// </returns>
    public IHttpSessionState Session
    {
      get
      {
        if (_session == null && _httpContext.Session != null)
          _session = new HttpSessionStateWrapper (_httpContext.Session);
        return _session;
      }
    }

    /// <summary>
    /// Gets the <see cref="IHttpServerUtility" /> object that provides methods used in processing Web requests.
    /// </summary>
    /// <returns>
    /// The <see cref="IHttpServerUtility" /> for the current HTTP request.
    /// </returns>
    public IHttpServerUtility Server
    {
      get
      {
        if (_server == null)
          _server = new HttpServerUtilityWrapper (_httpContext.Server);
        return _server;
      }
    }

    /// <summary>
    /// Gets the first error (if any) accumulated during HTTP request processing.
    /// </summary>
    /// <returns>
    /// The first <see cref="T:System.Exception" /> for the current HTTP request/response process; otherwise, <see langword="null" /> if no errors were accumulated during the HTTP request processing. The default is <see langword="null" />.
    /// </returns>
    public Exception Error
    {
      get { return _httpContext.Error; }
    }

    /// <summary>
    /// Gets an array of errors accumulated while processing an HTTP request.
    /// </summary>
    /// <returns>
    /// An array of <see cref="T:System.Exception" /> objects for the current HTTP request.
    /// </returns>
    public Exception[] AllErrors
    {
      get { return _httpContext.AllErrors; }
    }

    /// <summary>
    /// Gets or sets security information for the current HTTP request.
    /// </summary>
    /// <returns>
    /// Security information for the current HTTP request.
    /// </returns>
    public IPrincipal User
    {
      get { return _httpContext.User; }
      set { _httpContext.User = value; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.Profile.ProfileBase" /> object for the current user profile.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Web.Profile.ProfileBase" /> if the application configuration file contains a definition for the profile's properties; otherwise, <see langword="null" />.
    /// </returns>
    public ProfileBase Profile
    {
      get { return _httpContext.Profile; }
    }

    /// <summary>
    /// Gets or sets a value that specifies whether the <see cref="T:System.Web.Security.UrlAuthorizationModule" /> object should skip the authorization check for the current request.
    /// </summary>
    /// <returns>
    /// true if <see cref="T:System.Web.Security.UrlAuthorizationModule" /> should skip the authorization check; otherwise, false. The default is false.
    /// </returns>
    public bool SkipAuthorization
    {
      get { return _httpContext.SkipAuthorization; }
      set { _httpContext.SkipAuthorization = value; }
    }

    /// <summary>
    /// Gets a value indicating whether the current HTTP request is in debug mode.
    /// </summary>
    /// <returns>
    /// true if the request is in debug mode; otherwise, false.
    /// </returns>
    public bool IsDebuggingEnabled
    {
      get { return _httpContext.IsDebuggingEnabled; }
    }

    /// <summary>
    /// Gets a value indicating whether custom errors are enabled for the current HTTP request.
    /// </summary>
    /// <returns>
    /// true if custom errors are enabled; otherwise, false.
    /// </returns>
    public bool IsCustomErrorEnabled
    {
      get { return _httpContext.IsCustomErrorEnabled; }
    }

    /// <summary>
    /// Gets the initial timestamp of the current HTTP request.
    /// </summary>
    /// <returns>
    /// The timestamp of the current HTTP request.
    /// </returns>
    public DateTime Timestamp
    {
      get { return _httpContext.Timestamp; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Web.Caching.Cache" /> object for the current application domain.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.Caching.Cache" /> for the current application domain.
    /// </returns>
    public Cache Cache
    {
      get { return _httpContext.Cache; }
    }

    /// <summary>
    /// Gets a <see cref="T:System.Web.RequestNotification" /> value that indicates the current <see cref="T:System.Web.HttpApplication" /> event that is processing. 
    /// </summary>
    /// <returns>
    /// One of the <see cref="T:System.Web.RequestNotification" /> values.
    /// </returns>
    /// <exception cref="T:System.PlatformNotSupportedException">
    /// The operation requires integrated pipeline mode in IIS 7.0 and at least the .NET Framework version 3.0.
    /// </exception>
    public RequestNotification CurrentNotification
    {
      get { return _httpContext.CurrentNotification; }
    }

    /// <summary>
    /// Gets a value that is the current processing point in the ASP.NET pipeline just after an <see cref="T:System.Web.HttpApplication" /> event has finished processing. 
    /// </summary>
    /// <returns>
    /// true if custom errors are enabled; otherwise, false.
    /// </returns>
    /// <exception cref="T:System.PlatformNotSupportedException">
    /// The operation requires the integrated pipeline mode in IIS 7.0 and at least the .NET Framework 3.0.
    /// </exception>
    public bool IsPostNotification
    {
      get { return _httpContext.IsPostNotification; }
    }
  }
}
