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
using System.Web;
using Remotion.Utilities;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="HttpServerUtilityWrapper"/> type is the default implementation of the <see cref="IHttpServerUtility"/> interface.
  /// </summary>
  public class HttpServerUtilityWrapper : IHttpServerUtility
  {
    private readonly HttpServerUtility _serverUtility;

    public HttpServerUtilityWrapper (HttpServerUtility serverUtility)
    {
      ArgumentUtility.CheckNotNull ("serverUtility", serverUtility);
      _serverUtility = serverUtility;
    }

    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="IHttpServerUtility"/> wrapper.
    /// </summary>
    /// <exception cref="NotSupportedException">This is a stub implementation which does not contain an <see cref="HttpServerUtility"/>. </exception>
    public HttpServerUtility WrappedInstance
    {
      get { return _serverUtility; }
    }

    /// <summary>
    /// Creates a server instance of a COM object identified by the object's programmatic identifier (ProgID).
    /// </summary>
    /// <returns>
    /// The new object.
    /// </returns>
    /// <param name="progID">
    /// The class or type of object to create an instance of.
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// An instance of the object could not be created.
    /// </exception>
    public object CreateObject (string progID)
    {
      return _serverUtility.CreateObject(progID);
    }

    /// <summary>
    /// Creates a server instance of a COM object identified by the object's type.
    /// </summary>
    /// <returns>
    /// The new object.
    /// </returns>
    /// <param name="type">
    /// A <see cref="T:System.Type" /> representing the object to create.
    /// </param>
    public object CreateObject (Type type)
    {
      return _serverUtility.CreateObject(type);
    }

    /// <summary>
    /// Creates a server instance of a COM object identified by the object's class identifier (CLSID).
    /// </summary>
    /// <returns>
    /// The new object.
    /// </returns>
    /// <param name="clsid">
    /// The class identifier of the object to create an instance of.
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// An instance of the object could not be created.
    /// </exception>
    public object CreateObjectFromClsid (string clsid)
    {
      return _serverUtility.CreateObjectFromClsid(clsid);
    }

    /// <summary>
    /// Returns the physical file path that corresponds to the specified virtual path on the Web server.
    /// </summary>
    /// <returns>
    /// The physical file path that corresponds to <paramref name="path" />.
    /// </returns>
    /// <param name="path">
    /// The virtual path of the Web server.
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// The current <see cref="T:System.Web.HttpContext" /> is <see langword="null" />.
    /// </exception>
    public string MapPath (string path)
    {
      return _serverUtility.MapPath(path);
    }

    /// <summary>
    /// Returns the previous exception.
    /// </summary>
    /// <returns>
    /// The previous exception that was thrown.
    /// </returns>
    public Exception GetLastError ()
    {
      return _serverUtility.GetLastError();
    }

    /// <summary>
    /// Clears the previous exception.
    /// </summary>
    public void ClearError ()
    {
      _serverUtility.ClearError();
    }

    /// <summary>
    /// Executes the handler for the specified virtual path in the context of the current request. 
    /// </summary>
    /// <param name="path">
    /// The URL path to execute.
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// The current <see cref="T:System.Web.HttpContext" /> is <see langword="null" />.
    /// <para>- or -</para>
    /// An error occurred while executing the handler specified by <paramref name="path" />.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="path" /> is <see langword="null" />. 
    /// <para>- or -</para>
    /// <paramref name="path" /> is not a virtual path.
    /// </exception>
    public void Execute (string path)
    {
      _serverUtility.Execute(path);
    }

    /// <summary>
    /// Executes the handler for the specified virtual path in the context of the current request. A <see cref="T:System.IO.TextWriter" /> captures output from the executed handler.
    /// </summary>
    /// <param name="path">
    /// The URL path to execute. 
    /// </param>
    /// <param name="writer">
    /// The <see cref="T:System.IO.TextWriter" /> to capture the output. 
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// The current <see cref="T:System.Web.HttpContext" /> is <see langword="null" />. 
    /// <para>- or -</para>
    /// An error occurred while executing the handler specified by <paramref name="path" />.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="path" /> is <see langword="null" />. 
    /// <para>- or -</para>
    /// <paramref name="path" /> is not a virtual path. 
    /// </exception>
    public void Execute (string path, TextWriter writer)
    {
      _serverUtility.Execute(path, writer);
    }

    /// <summary>
    /// Executes the handler for the specified virtual path in the context of the current request and specifies whether to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </summary>
    /// <param name="path">
    /// The URL path to execute. 
    /// </param>
    /// <param name="preserveForm">true to preserve the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections; false to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// The current <see cref="T:System.Web.HttpContext" /> is <see langword="null" />.
    /// <para>- or -</para>
    /// An error occurred while executing the handler specified by <paramref name="path" />.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="path" /> is <see langword="null" />. 
    /// <para>- or -</para>
    /// <paramref name="path" /> is not a virtual path. 
    /// </exception>
    public void Execute (string path, bool preserveForm)
    {
      _serverUtility.Execute(path, preserveForm);
    }

    /// <summary>
    /// Executes the handler for the specified virtual path in the context of the current request. A <see cref="T:System.IO.TextWriter" /> captures output from the page and a Boolean parameter specifies whether to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </summary>
    /// <param name="path">
    /// The URL path to execute.
    /// </param>
    /// <param name="writer">
    /// The <see cref="T:System.IO.TextWriter" /> to capture the output.
    /// </param>
    /// <param name="preserveForm">true to preserve the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections; false to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// The current <see cref="T:System.Web.HttpContext" /> is a <see langword="null" /> reference (Nothing in Visual Basic).
    ///  <para>- or -</para>
    /// <paramref name="path" /> ends with a period (.).
    /// <para>- or -</para>
    /// An error occurred while executing the handler specified by <paramref name="path" />.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="path" /> is <see langword="null" />. 
    /// </exception>
    /// <exception cref="T:System.ArgumentException"><paramref name="path" /> is not a virtual path.
    /// </exception>
    public void Execute (string path, TextWriter writer, bool preserveForm)
    {
      _serverUtility.Execute(path, writer, preserveForm);
    }

    /// <summary>
    /// Executes the handler for the specified virtual path in the context of the current request. A <see cref="T:System.IO.TextWriter" /> captures output from the executed handler and a Boolean parameter specifies whether to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </summary>
    /// <param name="handler">
    /// The HTTP handler that implements the <see cref="T:System.Web.IHttpHandler" /> to transfer the current request to.
    /// </param>
    /// <param name="writer">
    /// The <see cref="T:System.IO.TextWriter" /> to capture the output.
    /// </param>
    /// <param name="preserveForm">true to preserve the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections; false to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// An error occurred while executing the handler specified by <paramref name="handler" />.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="handler" /> parameter is <see langword="null" />.
    /// </exception>
    public void Execute (IHttpHandler handler, TextWriter writer, bool preserveForm)
    {
      _serverUtility.Execute(handler, writer, preserveForm);
    }

    /// <summary>
    /// Terminates execution of the current page and starts execution of a new page by using the specified URL path of the page. Specifies whether to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </summary>
    /// <param name="path">
    /// The URL path of the new page on the server to execute.
    /// </param>
    /// <param name="preserveForm">true to preserve the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections; false to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </param>
    /// <exception cref="T:System.ApplicationException">
    /// The current page request is a callback.
    /// </exception>
    public void Transfer (string path, bool preserveForm)
    {
      _serverUtility.Transfer(path, preserveForm);
    }

    /// <summary>
    /// For the current request, terminates execution of the current page and starts execution of a new page by using the specified URL path of the page.
    /// </summary>
    /// <param name="path">
    /// The URL path of the new page on the server to execute.
    /// </param>
    public void Transfer (string path)
    {
      _serverUtility.Transfer(path);
    }

    /// <summary>
    /// Terminates execution of the current page and starts execution of a new request by using a custom HTTP handler that implements the <see cref="T:System.Web.IHttpHandler" /> interface and specifies whether to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </summary>
    /// <param name="handler">
    /// The HTTP handler that implements the <see cref="T:System.Web.IHttpHandler" /> to transfer the current request to.
    /// </param>
    /// <param name="preserveForm">true to preserve the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections; false to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </param>
    /// <exception cref="T:System.ApplicationException">
    /// The current page request is a callback.
    /// </exception>
    public void Transfer (IHttpHandler handler, bool preserveForm)
    {
      _serverUtility.Transfer(handler, preserveForm);
    }

    /// <summary>
    /// Performs an asynchronous execution of the specified URL.
    /// </summary>
    /// <param name="path">
    /// The URL path of the new page on the server to execute.
    /// </param>
    /// <exception cref="T:System.PlatformNotSupportedException">
    /// The request requires the integrated pipeline mode of IIS 7.0.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// The server is not available to handle the request.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="path" /> parameter is <see langword="null" />.
    /// </exception>
    public void TransferRequest (string path)
    {
      _serverUtility.TransferRequest(path);
    }

    /// <summary>
    /// Performs an asynchronous execution of the specified URL and preserves query string parameters.
    /// </summary>
    /// <param name="path">
    /// The URL path of the new page on the server to execute.
    /// </param>
    /// <param name="preserveForm">true to preserve the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections; false to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </param>
    /// <exception cref="T:System.PlatformNotSupportedException">
    /// The request requires the integrated pipeline mode of IIS 7.0.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// The server is not available to handle the request.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="path" /> parameter is <see langword="null" />.
    /// </exception>
    public void TransferRequest (string path, bool preserveForm)
    {
      _serverUtility.TransferRequest(path, preserveForm);
    }

    /// <summary>
    /// Performs an asynchronous execution of the specified URL using the specified HTTP method and headers.
    /// </summary>
    /// <param name="path">
    /// The URL path of the new page on the server to execute.
    /// </param>
    /// <param name="preserveForm">true to preserve the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections; false to clear the <see cref="P:System.Web.HttpRequest.QueryString" /> and <see cref="P:System.Web.HttpRequest.Form" /> collections.
    /// </param>
    /// <param name="method">
    /// The HTTP method to use in the execution of the new request.
    /// </param>
    /// <param name="headers">
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> of request headers for the new request.
    /// </param>
    /// <exception cref="T:System.PlatformNotSupportedException">
    /// The request requires IIS 7.0 running in integrated mode.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// The server is not available to handle the request.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="path" /> parameter is <see langword="null" />.
    /// </exception>
    public void TransferRequest (string path, bool preserveForm, string method, NameValueCollection headers)
    {
      _serverUtility.TransferRequest(path, preserveForm, method, headers);
    }

    /// <summary>
    /// Decodes an HTML-encoded string and returns the decoded string.
    /// </summary>
    /// <returns>
    /// The decoded text.
    /// </returns>
    /// <param name="s">
    /// The HTML string to decode.
    /// </param>
    public string HtmlDecode (string s)
    {
      return _serverUtility.HtmlDecode(s);
    }

    /// <summary>
    /// Decodes an HTML-encoded string and sends the resulting output to a <see cref="T:System.IO.TextWriter" /> output stream.
    /// </summary>
    /// <param name="s">
    /// The HTML string to decode.
    /// </param>
    /// <param name="output">
    /// The <see cref="T:System.IO.TextWriter" /> output stream that contains the decoded string.
    /// </param>
    public void HtmlDecode (string s, TextWriter output)
    {
      _serverUtility.HtmlDecode(s, output);
    }

    /// <summary>
    /// HTML-encodes a string and returns the encoded string.
    /// </summary>
    /// <returns>
    /// The HTML-encoded text.
    /// </returns>
    /// <param name="s">
    /// The text string to encode.
    /// </param>
    public string HtmlEncode (string s)
    {
      return _serverUtility.HtmlEncode(s);
    }

    /// <summary>
    /// HTML-encodes a string and sends the resulting output to a <see cref="T:System.IO.TextWriter" /> output stream.
    /// </summary>
    /// <param name="s">
    /// The string to encode. 
    /// </param>
    /// <param name="output">
    /// The <see cref="T:System.IO.TextWriter" /> output stream that contains the encoded string.
    /// </param>
    public void HtmlEncode (string s, TextWriter output)
    {
      _serverUtility.HtmlEncode(s, output);
    }

    /// <summary>
    /// URL-encodes a string and returns the encoded string.
    /// </summary>
    /// <returns>
    /// The URL-encoded text.
    /// </returns>
    /// <param name="s">
    /// The text to URL-encode.
    /// </param>
    public string UrlEncode (string s)
    {
      return _serverUtility.UrlEncode(s);
    }

    /// <summary>
    /// URL-encodes the path section of a URL string and returns the encoded string.
    /// </summary>
    /// <returns>
    /// The URL encoded text.
    /// </returns>
    /// <param name="s">
    /// The text to URL-encode.
    /// </param>
    public string UrlPathEncode (string s)
    {
      return _serverUtility.UrlPathEncode(s);
    }

    /// <summary>
    /// URL-encodes a string and sends the resulting output to a <see cref="T:System.IO.TextWriter" /> output stream.
    /// </summary>
    /// <param name="s">
    /// The text string to encode.
    /// </param>
    /// <param name="output">
    /// The <see cref="T:System.IO.TextWriter" /> output stream that contains the encoded string.
    /// </param>
    public void UrlEncode (string s, TextWriter output)
    {
      _serverUtility.UrlEncode(s, output);
    }

    /// <summary>
    /// URL-decodes a string and returns the decoded string.
    /// </summary>
    /// <returns>
    /// The decoded text.
    /// </returns>
    /// <param name="s">
    /// The text string to decode.
    /// </param>
    public string UrlDecode (string s)
    {
      return _serverUtility.UrlDecode(s);
    }

    /// <summary>
    /// Decodes an HTML string received in a URL and sends the resulting output to a <see cref="T:System.IO.TextWriter" /> output stream.
    /// </summary>
    /// <param name="s">
    /// The HTML string to decode.
    /// </param>
    /// <param name="output">
    /// The <see cref="T:System.IO.TextWriter" /> output stream that contains the decoded string.
    /// </param>
    public void UrlDecode (string s, TextWriter output)
    {
      _serverUtility.UrlDecode(s, output);
    }

    /// <summary>
    /// Gets the server's computer name.
    /// </summary>
    /// <returns>
    /// The name of the local computer.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// The computer name cannot be found.
    /// </exception>
    public string MachineName
    {
      get { return _serverUtility.MachineName; }
    }

    /// <summary>
    /// Gets and sets the request time-out value in seconds.
    /// </summary>
    /// <returns>
    /// The time-out value setting for requests.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// The current <see cref="T:System.Web.HttpContext" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// The time-out period is <see langword="null" /> or otherwise could not be set.
    /// </exception>
    public int ScriptTimeout
    {
      get { return _serverUtility.ScriptTimeout; }
      set { _serverUtility.ScriptTimeout = value; }
    }
  }
}
