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
using System.Collections.Specialized;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Web;
using Remotion.Utilities;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="HttpRequestWrapper"/> type is the default implementation of the <see cref="IHttpRequest"/> interface.
  /// </summary>
  public class HttpRequestWrapper : IHttpRequest
  {
    private readonly HttpRequest _request;

    public HttpRequestWrapper (HttpRequest request)
    {
      ArgumentUtility.CheckNotNull ("request", request);

      _request = request;
    }

    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="IHttpRequest"/> wrapper.
    /// </summary>
    public HttpRequest WrappedInstance
    {
      get { return _request; }
    }

    /// <summary>
    /// Performs a binary read of a specified number of bytes from the current input stream.
    /// </summary>
    /// <returns>
    /// A byte array.
    /// </returns>
    /// <param name="count">
    /// The number of bytes to read. 
    /// </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="count" /> is 0.
    /// <para>- or -</para>
    /// <paramref name="count" /> is greater than the number of bytes available. 
    /// </exception>
    public byte[] BinaryRead (int count)
    {
      return _request.BinaryRead (count);
    }

    /// <summary>
    /// Causes validation to occur for the collections accessed through the <see cref="P:System.Web.HttpRequest.Cookies" />, <see cref="P:System.Web.HttpRequest.Form" />, and <see cref="P:System.Web.HttpRequest.QueryString" /> properties.
    /// </summary>
    /// <exception cref="T:System.Web.HttpRequestValidationException">
    /// Potentially dangerous data was received from the client. 
    /// </exception>
    public void ValidateInput ()
    {
      _request.ValidateInput();
    }

    /// <summary>
    /// Maps an incoming image-field form parameter to appropriate x-coordinate and y-coordinate values.
    /// </summary>
    /// <returns>
    /// A two-dimensional array of integers.
    /// </returns>
    /// <param name="imageFieldName">
    /// The name of the form image map. 
    /// </param>
    public int[] MapImageCoordinates (string imageFieldName)
    {
      return _request.MapImageCoordinates (imageFieldName);
    }

    /// <summary>
    /// Saves an HTTP request to disk.
    /// </summary>
    /// <param name="filename">
    /// The physical drive path. 
    /// </param>
    /// <param name="includeHeaders">
    /// A Boolean value specifying whether an HTTP header should be saved to disk. 
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// The <see cref="P:System.Web.Configuration.HttpRuntimeSection.RequireRootedSaveAsPath" /> property of the <see cref="T:System.Web.Configuration.HttpRuntimeSection" /> is set to true but <paramref name="filename" /> is not an absolute path.
    /// </exception>
    public void SaveAs (string filename, bool includeHeaders)
    {
      _request.SaveAs (filename, includeHeaders);
    }

    /// <summary>
    /// Maps the specified virtual path to a physical path.
    /// </summary>
    /// <returns>
    /// The physical path on the server specified by <paramref name="virtualPath" />.
    /// </returns>
    /// <param name="virtualPath">
    /// The virtual path (absolute or relative) for the current request. 
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// No <see cref="T:System.Web.HttpContext" /> object is defined for the request. 
    /// </exception>
    public string MapPath (string virtualPath)
    {
      return _request.MapPath (virtualPath);
    }

    /// <summary>
    /// Maps the specified virtual path to a physical path.
    /// </summary>
    /// <returns>
    /// The physical path on the server.
    /// </returns>
    /// <param name="virtualPath">
    /// The virtual path (absolute or relative) for the current request. 
    /// </param>
    /// <param name="baseVirtualDir">
    /// The virtual base directory path used for relative resolution. 
    /// </param>
    /// <param name="allowCrossAppMapping">true to indicate that <paramref name="virtualPath" /> may belong to another application; otherwise, false. 
    /// </param>
    /// <exception cref="T:System.Web.HttpException"><paramref name="allowCrossAppMapping" /> is false and <paramref name="virtualPath" /> belongs to another application. 
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// No <see cref="T:System.Web.HttpContext" /> object is defined for the request. 
    /// </exception>
    public string MapPath (string virtualPath, string baseVirtualDir, bool allowCrossAppMapping)
    {
      return _request.MapPath (virtualPath, baseVirtualDir, allowCrossAppMapping);
    }

    /// <summary>
    /// Gets a value indicating whether the request is from the local computer.
    /// </summary>
    /// <returns>
    /// true if the request is from the local computer; otherwise, false.
    /// </returns>
    public bool IsLocal
    {
      get { return _request.IsLocal; }
    }

    /// <summary>
    /// Gets the HTTP data transfer method (such as GET, POST, or HEAD) used by the client.
    /// </summary>
    /// <returns>
    /// The HTTP data transfer method used by the client.
    /// </returns>
    public string HttpMethod
    {
      get { return _request.HttpMethod; }
    }

    /// <summary>
    /// Gets or sets the HTTP data transfer method (GET or POST) used by the client.
    /// </summary>
    /// <returns>
    /// A string representing the HTTP invocation type sent by the client.
    /// </returns>
    public string RequestType
    {
      get { return _request.RequestType; }
      set { _request.RequestType = value; }
    }

    /// <summary>
    /// Gets or sets the MIME content type of the incoming request.
    /// </summary>
    /// <returns>
    /// A string representing the MIME content type of the incoming request, for example, "text/html".
    /// </returns>
    public string ContentType
    {
      get { return _request.ContentType; }
      set { _request.ContentType = value; }
    }

    /// <summary>
    /// Specifies the length, in bytes, of content sent by the client.
    /// </summary>
    /// <returns>
    /// The length, in bytes, of content sent by the client.
    /// </returns>
    public int ContentLength
    {
      get { return _request.ContentLength; }
    }

    /// <summary>
    /// Gets or sets the character set of the entity-body.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Text.Encoding" /> object representing the client's character set.
    /// </returns>
    public Encoding ContentEncoding
    {
      get { return _request.ContentEncoding; }
      set { _request.ContentEncoding = value; }
    }

    /// <summary>
    /// Gets a string array of client-supported MIME accept types.
    /// </summary>
    /// <returns>
    /// A string array of client-supported MIME accept types.
    /// </returns>
    public string[] AcceptTypes
    {
      get { return _request.AcceptTypes; }
    }

    /// <summary>
    /// Gets a value indicating whether the request has been authenticated.
    /// </summary>
    /// <returns>
    /// true if the request is authenticated; otherwise, false.
    /// </returns>
    public bool IsAuthenticated
    {
      get { return _request.IsAuthenticated; }
    }

    /// <summary>
    /// Gets a value indicting whether the HTTP connection uses secure sockets (that is, HTTPS).
    /// </summary>
    /// <returns>
    /// true if the connection is an SSL connection; otherwise, false.
    /// </returns>
    public bool IsSecureConnection
    {
      get { return _request.IsSecureConnection; }
    }

    /// <summary>
    /// Gets the virtual path of the current request.
    /// </summary>
    /// <returns>
    /// The virtual path of the current request.
    /// </returns>
    public string Path
    {
      get { return _request.Path; }
    }

    /// <summary>
    /// Gets the anonymous identifier for the user, if present.
    /// </summary>
    /// <returns>
    /// A string representing the current anonymous user identifier.
    /// </returns>
    public string AnonymousID
    {
      get { return _request.AnonymousID; }
    }

    /// <summary>
    /// Gets the virtual path of the current request.
    /// </summary>
    /// <returns>
    /// The virtual path of the current request.
    /// </returns>
    public string FilePath
    {
      get { return _request.FilePath; }
    }

    /// <summary>
    /// Gets the virtual path of the current request.
    /// </summary>
    /// <returns>
    /// The virtual path of the current request.
    /// </returns>
    public string CurrentExecutionFilePath
    {
      get { return _request.CurrentExecutionFilePath; }
    }

    /// <summary>
    /// Gets the virtual path of the application root and makes it relative by using the tilde (~) notation for the application root (as in "~/page.aspx").
    /// </summary>
    /// <returns>
    /// The virtual path of the application root for the current request.
    /// </returns>
    public string AppRelativeCurrentExecutionFilePath
    {
      get { return _request.AppRelativeCurrentExecutionFilePath; }
    }

    /// <summary>
    /// Gets additional path information for a resource with a URL extension.
    /// </summary>
    /// <returns>
    /// Additional path information for a resource.
    /// </returns>
    public string PathInfo
    {
      get { return _request.PathInfo; }
    }

    /// <summary>
    /// Gets the physical file system path corresponding to the requested URL.
    /// </summary>
    /// <returns>
    /// The file system path of the current request.
    /// </returns>
    public string PhysicalPath
    {
      get { return _request.PhysicalPath; }
    }

    /// <summary>
    /// Gets the ASP.NET application's virtual application root path on the server.
    /// </summary>
    /// <returns>
    /// The virtual path of the current application.
    /// </returns>
    public string ApplicationPath
    {
      get { return _request.ApplicationPath; }
    }

    /// <summary>
    /// Gets the physical file system path of the currently executing server application's root directory.
    /// </summary>
    /// <returns>
    /// The file system path of the current application's root directory.
    /// </returns>
    public string PhysicalApplicationPath
    {
      get { return _request.PhysicalApplicationPath; }
    }

    /// <summary>
    /// Gets the raw user agent string of the client browser.
    /// </summary>
    /// <returns>
    /// The raw user agent string of the client browser.
    /// </returns>
    public string UserAgent
    {
      get { return _request.UserAgent; }
    }

    /// <summary>
    /// Gets a sorted string array of client language preferences.
    /// </summary>
    /// <returns>
    /// A sorted string array of client language preferences, or <see langword="null" /> if empty.
    /// </returns>
    public string[] UserLanguages
    {
      get { return _request.UserLanguages; }
    }

    /// <summary>
    /// Gets or sets information about the requesting client's browser capabilities.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.HttpBrowserCapabilities" /> object listing the capabilities of the client's browser.
    /// </returns>
    public HttpBrowserCapabilities Browser
    {
      get { return _request.Browser; }
      set { _request.Browser = value; }
    }

    /// <summary>
    /// Gets the DNS name of the remote client.
    /// </summary>
    /// <returns>
    /// The DNS name of the remote client.
    /// </returns>
    public string UserHostName
    {
      get { return _request.UserHostName; }
    }

    /// <summary>
    /// Gets the IP host address of the remote client.
    /// </summary>
    /// <returns>
    /// The IP address of the remote client.
    /// </returns>
    public string UserHostAddress
    {
      get { return _request.UserHostAddress; }
    }

    /// <summary>
    /// Gets the raw URL of the current request.
    /// </summary>
    /// <returns>
    /// The raw URL of the current request.
    /// </returns>
    public string RawUrl
    {
      get { return _request.RawUrl; }
    }

    /// <summary>
    /// Gets information about the URL of the current request.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Uri" /> object containing information regarding the URL of the current request.
    /// </returns>
    public Uri Url
    {
      get { return _request.Url; }
    }

    /// <summary>
    /// Gets information about the URL of the client's previous request that linked to the current URL.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Uri" /> object.
    /// </returns>
    public Uri UrlReferrer
    {
      get { return _request.UrlReferrer; }
    }

    /// <summary>
    /// Gets a combined collection of <see cref="P:System.Web.HttpRequest.QueryString" />, <see cref="P:System.Web.HttpRequest.Form" />, <see cref="P:System.Web.HttpRequest.ServerVariables" />, and <see cref="P:System.Web.HttpRequest.Cookies" /> items.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> object. 
    /// </returns>
    public NameValueCollection Params
    {
      get { return _request.Params; }
    }

    /// <summary>
    /// Gets the specified object from the <see cref="P:System.Web.HttpRequest.Cookies" />, <see cref="P:System.Web.HttpRequest.Form" />, <see cref="P:System.Web.HttpRequest.QueryString" /> or <see cref="P:System.Web.HttpRequest.ServerVariables" /> collections.
    /// </summary>
    /// <returns>
    /// The <see cref="P:System.Web.HttpRequest.QueryString" />, <see cref="P:System.Web.HttpRequest.Form" />, <see cref="P:System.Web.HttpRequest.Cookies" />, or <see cref="P:System.Web.HttpRequest.ServerVariables" /> collection member specified in the <paramref name="key" /> parameter. If the specified <paramref name="key" /> is not found, then <see langword="null" /> is returned.
    /// </returns>
    /// <param name="key">
    /// The name of the collection member to get. 
    /// </param>
    public string this [string key]
    {
      get { return _request[key]; }
    }

    /// <summary>
    /// Gets the collection of HTTP query string variables.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> containing the collection of query string variables sent by the client. For example, If the request URL is <i>http://www.contoso.com/default.aspx?id=44"</i> then the value of <see cref="P:System.Web.HttpRequest.QueryString" /> is "<i>id=44</i>".
    /// </returns>
    public NameValueCollection QueryString
    {
      get { return _request.QueryString; }
    }

    /// <summary>
    /// Gets a collection of form variables.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> representing a collection of form variables.
    /// </returns>
    public NameValueCollection Form
    {
      get { return _request.Form; }
    }

    /// <summary>
    /// Gets a collection of HTTP headers.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> of headers.
    /// </returns>
    public NameValueCollection Headers
    {
      get { return _request.Headers; }
    }

    /// <summary>
    /// Gets a collection of Web server variables.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> of server variables.
    /// </returns>
    public NameValueCollection ServerVariables
    {
      get { return _request.ServerVariables; }
    }

    /// <summary>
    /// Gets a collection of cookies sent by the client.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.HttpCookieCollection" /> object representing the client's cookie variables.
    /// </returns>
    public HttpCookieCollection Cookies
    {
      get { return _request.Cookies; }
    }

    /// <summary>
    /// Gets the collection of files uploaded by the client, in multipart MIME format.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.HttpFileCollection" /> object representing a collection of files uploaded by the client. The items of the <see cref="T:System.Web.HttpFileCollection" /> object are of type <see cref="T:System.Web.HttpPostedFile" />.
    /// </returns>
    public HttpFileCollection Files
    {
      get { return _request.Files; }
    }

    /// <summary>
    /// Gets the contents of the incoming HTTP entity body.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.IO.Stream" /> object representing the contents of the incoming HTTP content body.
    /// </returns>
    public Stream InputStream
    {
      get { return _request.InputStream; }
    }

    /// <summary>
    /// Gets the number of bytes in the current input stream.
    /// </summary>
    /// <returns>
    /// The number of bytes in the input stream.
    /// </returns>
    public int TotalBytes
    {
      get { return _request.TotalBytes; }
    }

    /// <summary>
    /// Gets or sets the filter to use when reading the current input stream.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.IO.Stream" /> object to be used as the filter.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// The specified <see cref="T:System.IO.Stream" /> is invalid.
    /// </exception>
    public Stream Filter
    {
      get { return _request.Filter; }
      set { _request.Filter = value; }
    }

    /// <summary>
    /// Gets the current request's client security certificate.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.HttpClientCertificate" /> object containing information about the client's security certificate settings.
    /// </returns>
    public HttpClientCertificate ClientCertificate
    {
      get { return _request.ClientCertificate; }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Security.Principal.WindowsIdentity" /> type for the current user.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Security.Principal.WindowsIdentity" /> for the current Microsoft Internet Information Services (IIS) authentication settings.
    /// </returns>
    public WindowsIdentity LogonUserIdentity
    {
      get { return _request.LogonUserIdentity; }
    }
  }
}
