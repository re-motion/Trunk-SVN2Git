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

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="IHttpServerUtility"/> interface defines a wrapper for the <see cref="HttpServerUtility"/> type.
  /// </summary>
  public interface IHttpServerUtility
  {
    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="IHttpServerUtility"/> wrapper.
    /// </summary>
    /// <exception cref="NotSupportedException">This is a stub implementation which does not contain an <see cref="HttpServerUtility"/>. </exception>
    HttpServerUtility WrappedInstance { get; }

    /// <summary>
    /// Gets the server's computer name.
    /// </summary>
    /// <returns>
    /// The name of the local computer.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// The computer name cannot be found.
    /// </exception>
    string MachineName { get; }

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
    int ScriptTimeout { get; set; }

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
    object CreateObject (string progID);

    /// <summary>
    /// Creates a server instance of a COM object identified by the object's type.
    /// </summary>
    /// <returns>
    /// The new object.
    /// </returns>
    /// <param name="type">
    /// A <see cref="T:System.Type" /> representing the object to create.
    /// </param>
    object CreateObject (Type type);

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
    object CreateObjectFromClsid (string clsid);

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
    string MapPath (string path);

    /// <summary>
    /// Returns the previous exception.
    /// </summary>
    /// <returns>
    /// The previous exception that was thrown.
    /// </returns>
    Exception GetLastError ();

    /// <summary>
    /// Clears the previous exception.
    /// </summary>
    void ClearError ();

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
    void Execute (string path);

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
    void Execute (string path, TextWriter writer);

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
    void Execute (string path, bool preserveForm);

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
    void Execute (string path, TextWriter writer, bool preserveForm);

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
    void Execute (IHttpHandler handler, TextWriter writer, bool preserveForm);

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
    void Transfer (string path, bool preserveForm);

    /// <summary>
    /// For the current request, terminates execution of the current page and starts execution of a new page by using the specified URL path of the page.
    /// </summary>
    /// <param name="path">
    /// The URL path of the new page on the server to execute.
    /// </param>
    void Transfer (string path);

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
    void Transfer (IHttpHandler handler, bool preserveForm);

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
    void TransferRequest (string path);

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
    void TransferRequest (string path, bool preserveForm);

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
    void TransferRequest (string path, bool preserveForm, string method, NameValueCollection headers);

    /// <summary>
    /// Decodes an HTML-encoded string and returns the decoded string.
    /// </summary>
    /// <returns>
    /// The decoded text.
    /// </returns>
    /// <param name="s">
    /// The HTML string to decode.
    /// </param>
    string HtmlDecode (string s);

    /// <summary>
    /// Decodes an HTML-encoded string and sends the resulting output to a <see cref="T:System.IO.TextWriter" /> output stream.
    /// </summary>
    /// <param name="s">
    /// The HTML string to decode.
    /// </param>
    /// <param name="output">
    /// The <see cref="T:System.IO.TextWriter" /> output stream that contains the decoded string.
    /// </param>
    void HtmlDecode (string s, TextWriter output);

    /// <summary>
    /// HTML-encodes a string and returns the encoded string.
    /// </summary>
    /// <returns>
    /// The HTML-encoded text.
    /// </returns>
    /// <param name="s">
    /// The text string to encode.
    /// </param>
    string HtmlEncode (string s);

    /// <summary>
    /// HTML-encodes a string and sends the resulting output to a <see cref="T:System.IO.TextWriter" /> output stream.
    /// </summary>
    /// <param name="s">
    /// The string to encode. 
    /// </param>
    /// <param name="output">
    /// The <see cref="T:System.IO.TextWriter" /> output stream that contains the encoded string.
    /// </param>
    void HtmlEncode (string s, TextWriter output);

    /// <summary>
    /// URL-encodes a string and returns the encoded string.
    /// </summary>
    /// <returns>
    /// The URL-encoded text.
    /// </returns>
    /// <param name="s">
    /// The text to URL-encode.
    /// </param>
    string UrlEncode (string s);

    /// <summary>
    /// URL-encodes the path section of a URL string and returns the encoded string.
    /// </summary>
    /// <returns>
    /// The URL encoded text.
    /// </returns>
    /// <param name="s">
    /// The text to URL-encode.
    /// </param>
    string UrlPathEncode (string s);

    /// <summary>
    /// URL-encodes a string and sends the resulting output to a <see cref="T:System.IO.TextWriter" /> output stream.
    /// </summary>
    /// <param name="s">
    /// The text string to encode.
    /// </param>
    /// <param name="output">
    /// The <see cref="T:System.IO.TextWriter" /> output stream that contains the encoded string.
    /// </param>
    void UrlEncode (string s, TextWriter output);

    /// <summary>
    /// URL-decodes a string and returns the decoded string.
    /// </summary>
    /// <returns>
    /// The decoded text.
    /// </returns>
    /// <param name="s">
    /// The text string to decode.
    /// </param>
    string UrlDecode (string s);

    /// <summary>
    /// Decodes an HTML string received in a URL and sends the resulting output to a <see cref="T:System.IO.TextWriter" /> output stream.
    /// </summary>
    /// <param name="s">
    /// The HTML string to decode.
    /// </param>
    /// <param name="output">
    /// The <see cref="T:System.IO.TextWriter" /> output stream that contains the decoded string.
    /// </param>
    void UrlDecode (string s, TextWriter output);
  }
}
