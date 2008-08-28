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
using System.Collections.Specialized;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="IHttpRequest"/> interface defines a wrapper for the <see cref="HttpRequest"/> type.
  /// </summary>
  public interface IHttpRequest
  {
    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="IHttpRequest"/> wrapper.
    /// </summary>
    /// <exception cref="NotSupportedException">This is a stub implementation which does not contain an <see cref="HttpRequest"/>. </exception>
    HttpRequest WrappedInstance { get; }

    /// <summary>
    /// Gets a value indicating whether the request is from the local computer.
    /// </summary>
    /// <returns>
    /// true if the request is from the local computer; otherwise, false.
    /// </returns>
    bool IsLocal { get; }

    /// <summary>
    /// Gets the HTTP data transfer method (such as GET, POST, or HEAD) used by the client.
    /// </summary>
    /// <returns>
    /// The HTTP data transfer method used by the client.
    /// </returns>
    string HttpMethod { get; }

    /// <summary>
    /// Gets or sets the HTTP data transfer method (GET or POST) used by the client.
    /// </summary>
    /// <returns>
    /// A string representing the HTTP invocation type sent by the client.
    /// </returns>
    string RequestType { get; set; }

    /// <summary>
    /// Gets or sets the MIME content type of the incoming request.
    /// </summary>
    /// <returns>
    /// A string representing the MIME content type of the incoming request, for example, "text/html".
    /// </returns>
    string ContentType { get; set; }

    /// <summary>
    /// Specifies the length, in bytes, of content sent by the client.
    /// </summary>
    /// <returns>
    /// The length, in bytes, of content sent by the client.
    /// </returns>
    int ContentLength { get; }

    /// <summary>
    /// Gets or sets the character set of the entity-body.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Text.Encoding" /> object representing the client's character set.
    /// </returns>
    Encoding ContentEncoding { get; set; }

    /// <summary>
    /// Gets a string array of client-supported MIME accept types.
    /// </summary>
    /// <returns>
    /// A string array of client-supported MIME accept types.
    /// </returns>
    string[] AcceptTypes { get; }

    /// <summary>
    /// Gets a value indicating whether the request has been authenticated.
    /// </summary>
    /// <returns>
    /// true if the request is authenticated; otherwise, false.
    /// </returns>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets a value indicting whether the HTTP connection uses secure sockets (that is, HTTPS).
    /// </summary>
    /// <returns>
    /// true if the connection is an SSL connection; otherwise, false.
    /// </returns>
    bool IsSecureConnection { get; }

    /// <summary>
    /// Gets the virtual path of the current request.
    /// </summary>
    /// <returns>
    /// The virtual path of the current request.
    /// </returns>
    string Path { get; }

    /// <summary>
    /// Gets the anonymous identifier for the user, if present.
    /// </summary>
    /// <returns>
    /// A string representing the current anonymous user identifier.
    /// </returns>
    string AnonymousID { get; }

    /// <summary>
    /// Gets the virtual path of the current request.
    /// </summary>
    /// <returns>
    /// The virtual path of the current request.
    /// </returns>
    string FilePath { get; }

    /// <summary>
    /// Gets the virtual path of the current request.
    /// </summary>
    /// <returns>
    /// The virtual path of the current request.
    /// </returns>
    string CurrentExecutionFilePath { get; }

    /// <summary>
    /// Gets the virtual path of the application root and makes it relative by using the tilde (~) notation for the application root (as in "~/page.aspx").
    /// </summary>
    /// <returns>
    /// The virtual path of the application root for the current request.
    /// </returns>
    string AppRelativeCurrentExecutionFilePath { get; }

    /// <summary>
    /// Gets additional path information for a resource with a URL extension.
    /// </summary>
    /// <returns>
    /// Additional path information for a resource.
    /// </returns>
    string PathInfo { get; }

    /// <summary>
    /// Gets the physical file system path corresponding to the requested URL.
    /// </summary>
    /// <returns>
    /// The file system path of the current request.
    /// </returns>
    string PhysicalPath { get; }

    /// <summary>
    /// Gets the ASP.NET application's virtual application root path on the server.
    /// </summary>
    /// <returns>
    /// The virtual path of the current application.
    /// </returns>
    string ApplicationPath { get; }

    /// <summary>
    /// Gets the physical file system path of the currently executing server application's root directory.
    /// </summary>
    /// <returns>
    /// The file system path of the current application's root directory.
    /// </returns>
    string PhysicalApplicationPath { get; }

    /// <summary>
    /// Gets the raw user agent string of the client browser.
    /// </summary>
    /// <returns>
    /// The raw user agent string of the client browser.
    /// </returns>
    string UserAgent { get; }

    /// <summary>
    /// Gets a sorted string array of client language preferences.
    /// </summary>
    /// <returns>
    /// A sorted string array of client language preferences, or <see langword="null" /> if empty.
    /// </returns>
    string[] UserLanguages { get; }

    /// <summary>
    /// Gets or sets information about the requesting client's browser capabilities.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.HttpBrowserCapabilities" /> object listing the capabilities of the client's browser.
    /// </returns>
    HttpBrowserCapabilities Browser { get; set; }

    /// <summary>
    /// Gets the DNS name of the remote client.
    /// </summary>
    /// <returns>
    /// The DNS name of the remote client.
    /// </returns>
    string UserHostName { get; }

    /// <summary>
    /// Gets the IP host address of the remote client.
    /// </summary>
    /// <returns>
    /// The IP address of the remote client.
    /// </returns>
    string UserHostAddress { get; }

    /// <summary>
    /// Gets the raw URL of the current request.
    /// </summary>
    /// <returns>
    /// The raw URL of the current request.
    /// </returns>
    string RawUrl { get; }

    /// <summary>
    /// Gets information about the URL of the current request.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Uri" /> object containing information regarding the URL of the current request.
    /// </returns>
    Uri Url { get; }

    /// <summary>
    /// Gets information about the URL of the client's previous request that linked to the current URL.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Uri" /> object.
    /// </returns>
    Uri UrlReferrer { get; }

    /// <summary>
    /// Gets a combined collection of <see cref="P:System.Web.HttpRequest.QueryString" />, <see cref="P:System.Web.HttpRequest.Form" />, <see cref="P:System.Web.HttpRequest.ServerVariables" />, and <see cref="P:System.Web.HttpRequest.Cookies" /> items.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> object. 
    /// </returns>
    NameValueCollection Params { get; }

    /// <summary>
    /// Gets the collection of HTTP query string variables.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> containing the collection of query string variables sent by the client. For example, If the request URL is <i>http://www.contoso.com/default.aspx?id=44"</i> then the value of <see cref="P:System.Web.HttpRequest.QueryString" /> is "<i>id=44</i>".
    /// </returns>
    NameValueCollection QueryString { get; }

    /// <summary>
    /// Gets a collection of form variables.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> representing a collection of form variables.
    /// </returns>
    NameValueCollection Form { get; }

    /// <summary>
    /// Gets a collection of HTTP headers.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> of headers.
    /// </returns>
    NameValueCollection Headers { get; }

    /// <summary>
    /// Gets a collection of Web server variables.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> of server variables.
    /// </returns>
    NameValueCollection ServerVariables { get; }

    /// <summary>
    /// Gets a collection of cookies sent by the client.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.HttpCookieCollection" /> object representing the client's cookie variables.
    /// </returns>
    HttpCookieCollection Cookies { get; }

    /// <summary>
    /// Gets the collection of files uploaded by the client, in multipart MIME format.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.HttpFileCollection" /> object representing a collection of files uploaded by the client. The items of the <see cref="T:System.Web.HttpFileCollection" /> object are of type <see cref="T:System.Web.HttpPostedFile" />.
    /// </returns>
    HttpFileCollection Files { get; }

    /// <summary>
    /// Gets the contents of the incoming HTTP entity body.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.IO.Stream" /> object representing the contents of the incoming HTTP content body.
    /// </returns>
    Stream InputStream { get; }

    /// <summary>
    /// Gets the number of bytes in the current input stream.
    /// </summary>
    /// <returns>
    /// The number of bytes in the input stream.
    /// </returns>
    int TotalBytes { get; }

    /// <summary>
    /// Gets or sets the filter to use when reading the current input stream.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.IO.Stream" /> object to be used as the filter.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// The specified <see cref="T:System.IO.Stream" /> is invalid.
    /// </exception>
    Stream Filter { get; set; }

    /// <summary>
    /// Gets the current request's client security certificate.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.HttpClientCertificate" /> object containing information about the client's security certificate settings.
    /// </returns>
    HttpClientCertificate ClientCertificate { get; }

    /// <summary>
    /// Gets the <see cref="T:System.Security.Principal.WindowsIdentity" /> type for the current user.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Security.Principal.WindowsIdentity" /> for the current Microsoft Internet Information Services (IIS) authentication settings.
    /// </returns>
    WindowsIdentity LogonUserIdentity { get; }

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
    byte[] BinaryRead (int count);

    /// <summary>
    /// Causes validation to occur for the collections accessed through the <see cref="P:System.Web.HttpRequest.Cookies" />, <see cref="P:System.Web.HttpRequest.Form" />, and <see cref="P:System.Web.HttpRequest.QueryString" /> properties.
    /// </summary>
    /// <exception cref="T:System.Web.HttpRequestValidationException">
    /// Potentially dangerous data was received from the client. 
    /// </exception>
    void ValidateInput ();

    /// <summary>
    /// Maps an incoming image-field form parameter to appropriate x-coordinate and y-coordinate values.
    /// </summary>
    /// <returns>
    /// A two-dimensional array of integers.
    /// </returns>
    /// <param name="imageFieldName">
    /// The name of the form image map. 
    /// </param>
    int[] MapImageCoordinates (string imageFieldName);

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
    void SaveAs (string filename, bool includeHeaders);

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
    string MapPath (string virtualPath);

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
    string MapPath (string virtualPath, string baseVirtualDir, bool allowCrossAppMapping);

    /// <summary>
    /// Gets the specified object from the <see cref="P:System.Web.HttpRequest.Cookies" />, <see cref="P:System.Web.HttpRequest.Form" />, <see cref="P:System.Web.HttpRequest.QueryString" /> or <see cref="P:System.Web.HttpRequest.ServerVariables" /> collections.
    /// </summary>
    /// <returns>
    /// The <see cref="P:System.Web.HttpRequest.QueryString" />, <see cref="P:System.Web.HttpRequest.Form" />, <see cref="P:System.Web.HttpRequest.Cookies" />, or <see cref="P:System.Web.HttpRequest.ServerVariables" /> collection member specified in the <paramref name="key" /> parameter. If the specified <paramref name="key" /> is not found, then <see langword="null" /> is returned.
    /// </returns>
    /// <param name="key">
    /// The name of the collection member to get. 
    /// </param>
    string this [string key] { get; }
  }
}