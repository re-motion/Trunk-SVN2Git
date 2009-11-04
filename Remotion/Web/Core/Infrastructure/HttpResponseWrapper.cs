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
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using Remotion.Utilities;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="HttpResponseWrapper"/> type is the default implementation of the <see cref="IHttpResponse"/> interface.
  /// </summary>
  public class HttpResponseWrapper : IHttpResponse
  {
    private readonly HttpResponse _response;

    public HttpResponseWrapper (HttpResponse response)
    {
      ArgumentUtility.CheckNotNull ("response", response);
      _response = response;
    }

    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="IHttpResponse"/> wrapper.
    /// </summary>
    public HttpResponse WrappedInstance
    {
      get { return _response; }
    }

    /// <summary>
    /// Disables kernel caching for the current response.
    /// </summary>
    public void DisableKernelCache ()
    {
      _response.DisableKernelCache();
    }

    /// <summary>
    /// Adds a single file name to the collection of file names on which the current response is dependent.
    /// </summary>
    /// <param name="filename">
    /// The name of the file to add.
    /// </param>
    public void AddFileDependency (string filename)
    {
      _response.AddFileDependency(filename);
    }

    /// <summary>
    /// Adds a group of file names to the collection of file names on which the current response is dependent.
    /// </summary>
    /// <param name="filenames">
    /// The collection of files to add.
    /// </param>
    public void AddFileDependencies (ArrayList filenames)
    {
      _response.AddFileDependencies(filenames);
    }

    /// <summary>
    /// Adds an array of file names to the collection of file names on which the current response is dependent.
    /// </summary>
    /// <param name="filenames">
    /// An array of files to add.
    /// </param>
    public void AddFileDependencies (string[] filenames)
    {
      _response.AddFileDependencies(filenames);
    }

    /// <summary>
    /// Makes the validity of a cached response dependent on another item in the cache.
    /// </summary>
    /// <param name="cacheKey">
    /// The key of the item that the cached response is dependent upon.
    /// </param>
    public void AddCacheItemDependency (string cacheKey)
    {
      _response.AddCacheItemDependency(cacheKey);
    }

    /// <summary>
    /// Makes the validity of a cached response dependent on other items in the cache.
    /// </summary>
    /// <param name="cacheKeys">
    /// The <see cref="T:System.Collections.ArrayList" /> that contains the keys of the items that the current cached response is dependent upon.
    /// </param>
    public void AddCacheItemDependencies (ArrayList cacheKeys)
    {
      _response.AddCacheItemDependencies(cacheKeys);
    }

    /// <summary>
    /// Makes the validity of a cached item dependent on another item in the cache.
    /// </summary>
    /// <param name="cacheKeys">
    /// An array of item keys that the cached response is dependent upon.
    /// </param>
    public void AddCacheItemDependencies (string[] cacheKeys)
    {
      _response.AddCacheItemDependencies(cacheKeys);
    }

    /// <summary>
    /// Associates a set of cache dependencies with the response to facilitate invalidation of the response if it is stored in the output cache and the specified dependencies change.
    /// </summary>
    /// <param name="dependencies">
    /// A file, cache key, or <see cref="T:System.Web.Caching.CacheDependency" /> to add to the list of application dependencies.
    /// </param>
    public void AddCacheDependency (params CacheDependency[] dependencies)
    {
      _response.AddCacheDependency(dependencies);
    }

    /// <summary>
    /// Closes the socket connection to a client.
    /// </summary>
    public void Close ()
    {
      _response.Close();
    }

    /// <summary>
    /// Writes a string of binary characters to the HTTP output stream.
    /// </summary>
    /// <param name="buffer">
    /// The bytes to write to the output stream.
    /// </param>
    public void BinaryWrite (byte[] buffer)
    {
      _response.BinaryWrite(buffer);
    }

    /// <summary>
    /// Appends a HTTP PICS-Label header to the output stream.
    /// </summary>
    /// <param name="value">
    /// The string to add to the PICS-Label header.
    /// </param>
    public void Pics (string value)
    {
      _response.Pics(value);
    }

    /// <summary>
    /// Adds an HTTP header to the output stream.
    /// </summary>
    /// <param name="name">
    /// The name of the HTTP header to add to the output stream.
    /// </param>
    /// <param name="value">
    /// The string to append to the header.
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// The header is appended after the HTTP headers have been sent.
    /// </exception>
    public void AppendHeader (string name, string value)
    {
      _response.AppendHeader(name, value);
    }

    /// <summary>
    /// Adds an HTTP cookie to the intrinsic cookie collection.
    /// </summary>
    /// <param name="cookie">
    /// The <see cref="T:System.Web.HttpCookie" /> to add to the output stream.
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// A cookie is appended after the HTTP headers have been sent.
    /// </exception>
    public void AppendCookie (HttpCookie cookie)
    {
      _response.AppendCookie(cookie);
    }

    /// <summary>
    /// Updates an existing cookie in the cookie collection.
    /// </summary>
    /// <param name="cookie">
    /// The cookie in the collection to be updated.
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// The cookie is set after the HTTP headers have been sent.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// Attempted to set the cookie after the HTTP headers were sent.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// The cookie is set after the HTTP headers have been sent.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// Attempted to set the cookie after the HTTP headers were sent.
    /// </exception>
    public void SetCookie (HttpCookie cookie)
    {
      _response.SetCookie(cookie);
    }

    /// <summary>
    /// Clears all headers from the buffer stream.
    /// </summary>
    /// <exception cref="T:System.Web.HttpException">
    /// Headers are cleared after the HTTP headers have been sent.
    /// </exception>
    public void ClearHeaders ()
    {
      _response.ClearHeaders();
    }

    /// <summary>
    /// Clears all content output from the buffer stream.
    /// </summary>
    public void ClearContent ()
    {
      _response.ClearContent();
    }

    /// <summary>
    /// Clears all content output from the buffer stream.
    /// </summary>
    public void Clear ()
    {
      _response.Clear();
    }

    /// <summary>
    /// Sends all currently buffered output to the client.
    /// </summary>
    /// <exception cref="T:System.Web.HttpException">
    /// The cache is flushed after the response has been sent.
    /// </exception>
    public void Flush ()
    {
      _response.Flush();
    }

    /// <summary>
    /// Adds custom log information to the Internet Information Services (IIS) log file.
    /// </summary>
    /// <param name="param">
    /// The text to add to the log file.
    /// </param>
    public void AppendToLog (string param)
    {
      _response.AppendToLog(param);
    }

    /// <summary>
    /// Redirects a client to a new URL and specifies the new URL.
    /// </summary>
    /// <param name="url">
    /// The target location. 
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// A redirection is attempted after the HTTP headers have been sent.
    /// </exception>
    public void Redirect (string url)
    {
      _response.Redirect(url);
    }

    /// <summary>
    /// Redirects a client to a new URL. Specifies the new URL and whether execution of the current page should terminate.
    /// </summary>
    /// <param name="url">
    /// The location of the target. 
    /// </param>
    /// <param name="endResponse">
    /// Indicates whether execution of the current page should terminate. 
    /// </param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="url" /> is null.
    /// </exception>
    /// <exception cref="T:System.ArgumentException"><paramref name="url" /> contains a newline character.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// A redirection is attempted after the HTTP headers have been sent.
    /// </exception>
    /// <exception cref="T:System.ApplicationException">
    /// The page request is the result of a callback.
    /// </exception>
    public void Redirect (string url, bool endResponse)
    {
      _response.Redirect(url, endResponse);
    }

    /// <summary>
    /// Writes a string to an HTTP response output stream.
    /// </summary>
    /// <param name="s">
    /// The string to write to the HTTP output stream.
    /// </param>
    public void Write (string s)
    {
      _response.Write(s);
    }

    /// <summary>
    /// Writes an <see cref="T:System.Object" /> to an HTTP response stream.
    /// </summary>
    /// <param name="obj">
    /// The <see cref="T:System.Object" /> to write to the HTTP output stream.
    /// </param>
    public void Write (object obj)
    {
      _response.Write(obj);
    }

    /// <summary>
    /// Writes a character to an HTTP response output stream.
    /// </summary>
    /// <param name="ch">
    /// The character to write to the HTTP output stream.
    /// </param>
    public void Write (char ch)
    {
      _response.Write(ch);
    }

    /// <summary>
    /// Writes an array of characters to an HTTP response output stream.
    /// </summary>
    /// <param name="buffer">
    /// The character array to write.
    /// </param>
    /// <param name="index">
    /// The position in the character array where writing starts.
    /// </param>
    /// <param name="count">
    /// The number of characters to write, beginning at <paramref name="index" />.
    /// </param>
    public void Write (char[] buffer, int index, int count)
    {
      _response.Write(buffer, index, count);
    }

    /// <summary>
    /// Allows insertion of response substitution blocks into the response, which allows dynamic generation of specified response regions for output cached responses.
    /// </summary>
    /// <param name="callback">
    /// The method, user control, or object to substitute.
    /// </param>
    /// <exception cref="T:System.ArgumentException">
    /// The target of the <paramref name="callback" /> parameter is of type <see cref="T:System.Web.UI.Control" />.
    /// </exception>
    public void WriteSubstitution (HttpResponseSubstitutionCallback callback)
    {
      _response.WriteSubstitution(callback);
    }

    /// <summary>
    /// Writes the contents of the specified file directly to an HTTP response output stream as a file block.
    /// </summary>
    /// <param name="filename">
    /// The name of the file to write to the HTTP output.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="filename" /> parameter is null.
    /// </exception>
    public void WriteFile (string filename)
    {
      _response.WriteFile(filename);
    }

    /// <summary>
    /// Writes the contents of the specified file directly to an HTTP response output stream as a memory block.
    /// </summary>
    /// <param name="filename">
    /// The name of the file to write into a memory block.
    /// </param>
    /// <param name="readIntoMemory">
    /// Indicates whether the file will be written into a memory block.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="filename" /> parameter is null.
    /// </exception>
    public void WriteFile (string filename, bool readIntoMemory)
    {
      _response.WriteFile(filename, readIntoMemory);
    }

    /// <summary>
    /// Writes the specified file directly to an HTTP response output stream, without buffering it in memory.
    /// </summary>
    /// <param name="filename">
    /// The name of the file to write to the HTTP output.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="filename" /> parameter is null</exception>
    public void TransmitFile (string filename)
    {
      _response.TransmitFile(filename);
    }

    /// <summary>
    /// Writes the specified part of a file directly to an HTTP response output stream without buffering it in memory.
    /// </summary>
    /// <param name="filename">
    /// The name of the file to write to the HTTP output.
    /// </param>
    /// <param name="offset">
    /// The position in the file to begin to write to the HTTP output.
    /// </param>
    /// <param name="length">
    /// The number of bytes to be transmitted.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="offset" /> parameter is less than zero.
    /// <para>- or -</para>
    /// The <paramref name="length" /> parameter is less than -1.
    /// <para>- or -</para> 
    /// The <paramref name="length" /> parameter specifies a number of bytes that is greater than the number of bytes the file contains minus the offset.
    /// </exception>
    /// <exception cref="T:System.PlatformNotSupportedException">
    /// The out-of-process worker request is not supported.
    /// <para>- or -</para>
    /// The response is not using an <see cref="T:System.Web.HttpWriter" /> object.
    /// </exception>
    public void TransmitFile (string filename, long offset, long length)
    {
      _response.TransmitFile(filename, offset, length);
    }

    /// <summary>
    /// Writes the specified file directly to an HTTP response output stream.
    /// </summary>
    /// <param name="filename">
    /// The name of the file to write to the HTTP output stream.
    /// </param>
    /// <param name="offset">
    /// The byte position in the file where writing will start.
    /// </param>
    /// <param name="size">
    /// The number of bytes to write to the output stream.
    /// </param>
    /// <exception cref="T:System.Web.HttpException"><paramref name="offset" /> is less than 0.
    /// <para>- or -</para>
    /// <paramref name="size" /> is greater than the file size minus <paramref name="offset" />.
    /// </exception>
    public void WriteFile (string filename, long offset, long size)
    {
      _response.WriteFile(filename, offset, size);
    }

    /// <summary>
    /// Writes the specified file directly to an HTTP response output stream.
    /// </summary>
    /// <param name="fileHandle">
    /// The file handle of the file to write to the HTTP output stream.
    /// </param>
    /// <param name="offset">
    /// The byte position in the file where writing will start.
    /// </param>
    /// <param name="size">
    /// The number of bytes to write to the output stream.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="fileHandle" /> is null.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException"><paramref name="offset" /> is less than 0.
    /// <para>- or -</para>
    /// <paramref name="size" /> is greater than the file size minus <paramref name="offset" />.
    /// </exception>
    public void WriteFile (IntPtr fileHandle, long offset, long size)
    {
      _response.WriteFile(fileHandle, offset, size);
    }

    /// <summary>
    /// Adds an HTTP header to the output stream. <see cref="M:System.Web.HttpResponse.AddHeader(System.String,System.String)" /> is provided for compatibility with earlier versions of ASP.
    /// </summary>
    /// <param name="name">
    /// The name of the HTTP header to add <paramref name="value" /> to.
    /// </param>
    /// <param name="value">
    /// The string to add to the header.
    /// </param>
    public void AddHeader (string name, string value)
    {
      _response.AddHeader(name, value);
    }

    /// <summary>
    /// Sends all currently buffered output to the client, stops execution of the page, and raises the <see cref="E:System.Web.HttpApplication.EndRequest" /> event.
    /// </summary>
    /// <exception cref="T:System.Threading.ThreadAbortException">
    /// The call to <see cref="M:System.Web.HttpResponse.End" /> has terminated the current request.
    /// </exception>
    public void End ()
    {
      _response.End();
    }

    /// <summary>
    /// Adds a session ID to the virtual path if the session is using <see cref="P:System.Web.Configuration.SessionStateSection.Cookieless" /> session state and returns the combined path. If <see cref="P:System.Web.Configuration.SessionStateSection.Cookieless" /> session state is not used, <see cref="M:System.Web.HttpResponse.ApplyAppPathModifier(System.String)" /> returns the original virtual path.
    /// </summary>
    /// <returns>
    /// The <paramref name="virtualPath" /> with the session ID inserted.
    /// </returns>
    /// <param name="virtualPath">
    /// The virtual path to a resource. 
    /// </param>
    public string ApplyAppPathModifier (string virtualPath)
    {
      return _response.ApplyAppPathModifier(virtualPath);
    }

    /// <summary>
    /// Gets the response cookie collection.
    /// </summary>
    /// <returns>
    /// The response cookie collection.
    /// </returns>
    public HttpCookieCollection Cookies
    {
      get { return _response.Cookies; }
    }

    /// <summary>
    /// Gets the collection of response headers.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> of response headers.
    /// </returns>
    /// <exception cref="T:System.PlatformNotSupportedException">
    /// The operation requires the integrated pipeline mode in IIS 7.0 and at least the .NET Framework version 3.0.
    /// </exception>
    public NameValueCollection Headers
    {
      get { return _response.Headers; }
    }

    /// <summary>
    /// Gets or sets the HTTP status code of the output returned to the client.
    /// </summary>
    /// <returns>
    /// An Integer representing the status of the HTTP output returned to the client. The default value is 200 (OK). For a listing of valid status codes, see Http Status Codes.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException"><see cref="P:System.Web.HttpResponse.StatusCode" /> is set after the HTTP headers have been sent.
    /// </exception>
    public int StatusCode
    {
      get { return _response.StatusCode; }
      set { _response.StatusCode = value; }
    }

    /// <summary>
    /// Gets or sets a value qualifying the status code of the response.
    /// </summary>
    /// <returns>
    /// An integer value that represents the IIS 7.0 substatus code.
    /// </returns>
    /// <exception cref="T:System.PlatformNotSupportedException">
    /// The operation requires the integrated pipeline mode in IIS 7.0 and at least the .NET Framework version 3.0.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// The status code is set after all HTTP headers have been sent.
    /// </exception>
    public int SubStatusCode
    {
      get { return _response.SubStatusCode; }
      set { _response.SubStatusCode = value; }
    }

    /// <summary>
    /// Gets or sets the HTTP status string of the output returned to the client.
    /// </summary>
    /// <returns>
    /// A string that describes the status of the HTTP output returned to the client. The default value is "OK". For a listing of valid status codes, see Http Status Codes.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException"><cref see="StatusDescription" /> is set after the HTTP headers have been sent.
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// The selected value has a length greater than 512.
    /// </exception>
    public string StatusDescription
    {
      get { return _response.StatusDescription; }
      set { _response.StatusDescription = value; }
    }

    /// <summary>
    /// Gets or sets a value that specifies whether IIS 7.0 custom errors are disabled.
    /// </summary>
    /// <returns>
    /// true to disable IIS custom errors; otherwise, false.
    /// </returns>
    public bool TrySkipIisCustomErrors
    {
      get { return _response.TrySkipIisCustomErrors; }
      set { _response.TrySkipIisCustomErrors = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to buffer output and send it after the complete page is finished processing.
    /// </summary>
    /// <returns>
    /// true if the output to client is buffered; otherwise false. The default is true.
    /// </returns>
    public bool BufferOutput
    {
      get { return _response.BufferOutput; }
      set { _response.BufferOutput = value; }
    }

    /// <summary>
    /// Gets or sets the HTTP MIME type of the output stream.
    /// </summary>
    /// <returns>
    /// The HTTP MIME type of the output stream. The default value is "text/html".
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// The <see cref="P:System.Web.HttpResponse.ContentType" /> property is set to null.
    /// </exception>
    public string ContentType
    {
      get { return _response.ContentType; }
      set { _response.ContentType = value; }
    }

    /// <summary>
    /// Gets or sets the HTTP character set of the output stream.
    /// </summary>
    /// <returns>
    /// The HTTP character set of the output stream.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// The Charset property was set after headers were sent.
    /// </exception>
    public string Charset
    {
      get { return _response.Charset; }
      set { _response.Charset = value; }
    }

    /// <summary>
    /// Gets or sets the HTTP character set of the output stream.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Text.Encoding" /> object that contains information about the character set of the current response.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// Attempted to set <see cref="P:System.Web.HttpResponse.ContentEncoding" /> to null.
    /// </exception>
    public Encoding ContentEncoding
    {
      get { return _response.ContentEncoding; }
      set { _response.ContentEncoding = value; }
    }

    /// <summary>
    /// Gets or sets an <see cref="T:System.Text.Encoding" /> object that represents the encoding for the current header output stream.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Text.Encoding" /> that contains information about the character set for the current header.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// The encoding value is null.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    /// The encoding value is <see cref="P:System.Text.Encoding.Unicode" />.
    /// <para>- or -</para>
    /// The headers have already been sent.
    /// </exception>
    public Encoding HeaderEncoding
    {
      get { return _response.HeaderEncoding; }
      set { _response.HeaderEncoding = value; }
    }

    /// <summary>
    /// Gets the caching policy (such as expiration time, privacy settings, and vary clauses) of a Web page.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.HttpCachePolicy" /> object that contains information about the caching policy of the current response.
    /// </returns>
    public HttpCachePolicy Cache
    {
      get { return _response.Cache; }
    }

    /// <summary>
    /// Gets a value indicating whether the client is still connected to the server.
    /// </summary>
    /// <returns>
    /// true if the client is currently connected; otherwise, false.
    /// </returns>
    public bool IsClientConnected
    {
      get { return _response.IsClientConnected; }
    }

    /// <summary>
    /// Gets a Boolean value indicating whether the client is being transferred to a new location.
    /// </summary>
    /// <returns>
    /// true if the value of the location response header is different than the current location; otherwise, false.
    /// </returns>
    public bool IsRequestBeingRedirected
    {
      get { return _response.IsRequestBeingRedirected; }
    }

    /// <summary>
    /// Gets or sets the value of the Http Location header.
    /// </summary>
    /// <returns>
    /// The absolute URI that is transmitted to the client in the HTTP Location header.
    /// </returns>
    public string RedirectLocation
    {
      get { return _response.RedirectLocation; }
      set { _response.RedirectLocation = value; }
    }

    /// <summary>
    /// Enables output of text to the outgoing HTTP response stream.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.IO.TextWriter" /> object that enables custom output to the client.
    /// </returns>
    public TextWriter Output
    {
      get { return _response.Output; }
    }

    /// <summary>
    /// Enables binary output to the outgoing HTTP content body.
    /// </summary>
    /// <returns>
    /// An IO <see cref="T:System.IO.Stream" /> representing the raw contents of the outgoing HTTP content body.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException"><cref see="OutputStream" /> is not available.
    /// </exception>
    public Stream OutputStream
    {
      get { return _response.OutputStream; }
    }

    /// <summary>
    /// Gets or sets a wrapping filter object that is used to modify the HTTP entity body before transmission.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.IO.Stream" /> object that acts as the output filter.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// Filtering is not allowed with the entity.
    /// </exception>
    public Stream Filter
    {
      get { return _response.Filter; }
      set { _response.Filter = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to send HTTP content to the client.
    /// </summary>
    /// <returns>
    /// true to suppress output; otherwise, false.
    /// </returns>
    public bool SuppressContent
    {
      get { return _response.SuppressContent; }
      set { _response.SuppressContent = value; }
    }

    /// <summary>
    /// Sets the Status line that is returned to the client.
    /// </summary>
    /// <returns>
    /// Setting the status code causes a string describing the status of the HTTP output to be returned to the client. The default value is 200 (OK).
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// Status is set to an invalid status code.
    /// </exception>
    public string Status
    {
      get { return _response.Status; }
      set { _response.Status = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to buffer output and send it after the complete response is finished processing.
    /// </summary>
    /// <returns>
    /// true if the output to client is buffered; otherwise, false.
    /// </returns>
    public bool Buffer
    {
      get { return _response.Buffer; }
      set { _response.Buffer = value; }
    }

    /// <summary>
    /// Gets or sets the number of minutes before a page cached on a browser expires. If the user returns to the same page before it expires, the cached version is displayed. <see cref="P:System.Web.HttpResponse.Expires" /> is provided for compatibility with earlier versions of ASP.
    /// </summary>
    /// <returns>
    /// The number of minutes before the page expires.
    /// </returns>
    public int Expires
    {
      get { return _response.Expires; }
      set { _response.Expires = value; }
    }

    /// <summary>
    /// Gets or sets the absolute date and time at which to remove cached information from the cache. <see cref="P:System.Web.HttpResponse.ExpiresAbsolute" /> is provided for compatibility with earlier versions of ASP.
    /// </summary>
    /// <returns>
    /// The date and time at which the page expires.
    /// </returns>
    public DateTime ExpiresAbsolute
    {
      get { return _response.ExpiresAbsolute; }
      set { _response.ExpiresAbsolute = value; }
    }

    /// <summary>
    /// Gets or sets the Cache-Control HTTP header that matches one of the <see cref="T:System.Web.HttpCacheability" /> enumeration values.
    /// </summary>
    /// <returns>
    /// A string representation of the <see cref="T:System.Web.HttpCacheability" /> enumeration value.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    /// The string value set does not match one of the <see cref="T:System.Web.HttpCacheability" /> enumeration values.
    /// </exception>
    public string CacheControl
    {
      get { return _response.CacheControl; }
      set { _response.CacheControl = value; }
    }
  }
}
