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
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="IHttpResponse"/> interface defines a wrapper for the <see cref="HttpResponse"/> type.
  /// </summary>
  public interface IHttpResponse
  {
    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="IHttpResponse"/> wrapper.
    /// </summary>
    /// <exception cref="NotSupportedException">This is a stub implementation which does not contain an <see cref="HttpResponse"/>. </exception>
    HttpResponse WrappedInstance { get; }

    /// <summary>
    /// Gets the response cookie collection.
    /// </summary>
    /// <returns>
    /// The response cookie collection.
    /// </returns>
    HttpCookieCollection Cookies { get; }

    /// <summary>
    /// Gets the collection of response headers.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> of response headers.
    /// </returns>
    /// <exception cref="T:System.PlatformNotSupportedException">
    /// The operation requires the integrated pipeline mode in IIS 7.0 and at least the .NET Framework version 3.0.
    /// </exception>
    NameValueCollection Headers { get; }

    /// <summary>
    /// Gets or sets the HTTP status code of the output returned to the client.
    /// </summary>
    /// <returns>
    /// An Integer representing the status of the HTTP output returned to the client. The default value is 200 (OK). For a listing of valid status codes, see Http Status Codes.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException"><see cref="P:System.Web.HttpResponse.StatusCode" /> is set after the HTTP headers have been sent.
    /// </exception>
    int StatusCode { get; set; }

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
    int SubStatusCode { get; set; }

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
    string StatusDescription { get; set; }

    /// <summary>
    /// Gets or sets a value that specifies whether IIS 7.0 custom errors are disabled.
    /// </summary>
    /// <returns>
    /// true to disable IIS custom errors; otherwise, false.
    /// </returns>
    bool TrySkipIisCustomErrors { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to buffer output and send it after the complete page is finished processing.
    /// </summary>
    /// <returns>
    /// true if the output to client is buffered; otherwise false. The default is true.
    /// </returns>
    bool BufferOutput { get; set; }

    /// <summary>
    /// Gets or sets the HTTP MIME type of the output stream.
    /// </summary>
    /// <returns>
    /// The HTTP MIME type of the output stream. The default value is "text/html".
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// The <see cref="P:System.Web.HttpResponse.ContentType" /> property is set to null.
    /// </exception>
    string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the HTTP character set of the output stream.
    /// </summary>
    /// <returns>
    /// The HTTP character set of the output stream.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// The Charset property was set after headers were sent.
    /// </exception>
    string Charset { get; set; }

    /// <summary>
    /// Gets or sets the HTTP character set of the output stream.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Text.Encoding" /> object that contains information about the character set of the current response.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// Attempted to set <see cref="P:System.Web.HttpResponse.ContentEncoding" /> to null.
    /// </exception>
    Encoding ContentEncoding { get; set; }

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
    Encoding HeaderEncoding { get; set; }

    /// <summary>
    /// Gets the caching policy (such as expiration time, privacy settings, and vary clauses) of a Web page.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.HttpCachePolicy" /> object that contains information about the caching policy of the current response.
    /// </returns>
    HttpCachePolicy Cache { get; }

    /// <summary>
    /// Gets a value indicating whether the client is still connected to the server.
    /// </summary>
    /// <returns>
    /// true if the client is currently connected; otherwise, false.
    /// </returns>
    bool IsClientConnected { get; }

    /// <summary>
    /// Gets a Boolean value indicating whether the client is being transferred to a new location.
    /// </summary>
    /// <returns>
    /// true if the value of the location response header is different than the current location; otherwise, false.
    /// </returns>
    bool IsRequestBeingRedirected { get; }

    /// <summary>
    /// Gets or sets the value of the Http Location header.
    /// </summary>
    /// <returns>
    /// The absolute URI that is transmitted to the client in the HTTP Location header.
    /// </returns>
    string RedirectLocation { get; set; }

    /// <summary>
    /// Enables output of text to the outgoing HTTP response stream.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.IO.TextWriter" /> object that enables custom output to the client.
    /// </returns>
    TextWriter Output { get; }

    /// <summary>
    /// Enables binary output to the outgoing HTTP content body.
    /// </summary>
    /// <returns>
    /// An IO <see cref="T:System.IO.Stream" /> representing the raw contents of the outgoing HTTP content body.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException"><cref see="OutputStream" /> is not available.
    /// </exception>
    Stream OutputStream { get; }

    /// <summary>
    /// Gets or sets a wrapping filter object that is used to modify the HTTP entity body before transmission.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.IO.Stream" /> object that acts as the output filter.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// Filtering is not allowed with the entity.
    /// </exception>
    Stream Filter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to send HTTP content to the client.
    /// </summary>
    /// <returns>
    /// true to suppress output; otherwise, false.
    /// </returns>
    bool SuppressContent { get; set; }

    /// <summary>
    /// Sets the Status line that is returned to the client.
    /// </summary>
    /// <returns>
    /// Setting the status code causes a string describing the status of the HTTP output to be returned to the client. The default value is 200 (OK).
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// Status is set to an invalid status code.
    /// </exception>
    string Status { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to buffer output and send it after the complete response is finished processing.
    /// </summary>
    /// <returns>
    /// true if the output to client is buffered; otherwise, false.
    /// </returns>
    bool Buffer { get; set; }

    /// <summary>
    /// Gets or sets the number of minutes before a page cached on a browser expires. If the user returns to the same page before it expires, the cached version is displayed. <see cref="P:System.Web.HttpResponse.Expires" /> is provided for compatibility with earlier versions of ASP.
    /// </summary>
    /// <returns>
    /// The number of minutes before the page expires.
    /// </returns>
    int Expires { get; set; }

    /// <summary>
    /// Gets or sets the absolute date and time at which to remove cached information from the cache. <see cref="P:System.Web.HttpResponse.ExpiresAbsolute" /> is provided for compatibility with earlier versions of ASP.
    /// </summary>
    /// <returns>
    /// The date and time at which the page expires.
    /// </returns>
    DateTime ExpiresAbsolute { get; set; }

    /// <summary>
    /// Gets or sets the Cache-Control HTTP header that matches one of the <see cref="T:System.Web.HttpCacheability" /> enumeration values.
    /// </summary>
    /// <returns>
    /// A string representation of the <see cref="T:System.Web.HttpCacheability" /> enumeration value.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    /// The string value set does not match one of the <see cref="T:System.Web.HttpCacheability" /> enumeration values.
    /// </exception>
    string CacheControl { get; set; }

    /// <summary>
    /// Disables kernel caching for the current response.
    /// </summary>
    void DisableKernelCache ();

    /// <summary>
    /// Adds a single file name to the collection of file names on which the current response is dependent.
    /// </summary>
    /// <param name="filename">
    /// The name of the file to add.
    /// </param>
    void AddFileDependency (string filename);

    /// <summary>
    /// Adds a group of file names to the collection of file names on which the current response is dependent.
    /// </summary>
    /// <param name="filenames">
    /// The collection of files to add.
    /// </param>
    void AddFileDependencies (ArrayList filenames);

    /// <summary>
    /// Adds an array of file names to the collection of file names on which the current response is dependent.
    /// </summary>
    /// <param name="filenames">
    /// An array of files to add.
    /// </param>
    void AddFileDependencies (string[] filenames);

    /// <summary>
    /// Makes the validity of a cached response dependent on another item in the cache.
    /// </summary>
    /// <param name="cacheKey">
    /// The key of the item that the cached response is dependent upon.
    /// </param>
    void AddCacheItemDependency (string cacheKey);

    /// <summary>
    /// Makes the validity of a cached response dependent on other items in the cache.
    /// </summary>
    /// <param name="cacheKeys">
    /// The <see cref="T:System.Collections.ArrayList" /> that contains the keys of the items that the current cached response is dependent upon.
    /// </param>
    void AddCacheItemDependencies (ArrayList cacheKeys);

    /// <summary>
    /// Makes the validity of a cached item dependent on another item in the cache.
    /// </summary>
    /// <param name="cacheKeys">
    /// An array of item keys that the cached response is dependent upon.
    /// </param>
    void AddCacheItemDependencies (string[] cacheKeys);

    /// <summary>
    /// Associates a set of cache dependencies with the response to facilitate invalidation of the response if it is stored in the output cache and the specified dependencies change.
    /// </summary>
    /// <param name="dependencies">
    /// A file, cache key, or <see cref="T:System.Web.Caching.CacheDependency" /> to add to the list of application dependencies.
    /// </param>
    void AddCacheDependency (params CacheDependency[] dependencies);

    /// <summary>
    /// Closes the socket connection to a client.
    /// </summary>
    void Close ();

    /// <summary>
    /// Writes a string of binary characters to the HTTP output stream.
    /// </summary>
    /// <param name="buffer">
    /// The bytes to write to the output stream.
    /// </param>
    void BinaryWrite (byte[] buffer);

    /// <summary>
    /// Appends a HTTP PICS-Label header to the output stream.
    /// </summary>
    /// <param name="value">
    /// The string to add to the PICS-Label header.
    /// </param>
    void Pics (string value);

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
    void AppendHeader (string name, string value);

    /// <summary>
    /// Adds an HTTP cookie to the intrinsic cookie collection.
    /// </summary>
    /// <param name="cookie">
    /// The <see cref="T:System.Web.HttpCookie" /> to add to the output stream.
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// A cookie is appended after the HTTP headers have been sent.
    /// </exception>
    void AppendCookie (HttpCookie cookie);

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
    void SetCookie (HttpCookie cookie);

    /// <summary>
    /// Clears all headers from the buffer stream.
    /// </summary>
    /// <exception cref="T:System.Web.HttpException">
    /// Headers are cleared after the HTTP headers have been sent.
    /// </exception>
    void ClearHeaders ();

    /// <summary>
    /// Clears all content output from the buffer stream.
    /// </summary>
    void ClearContent ();

    /// <summary>
    /// Clears all content output from the buffer stream.
    /// </summary>
    void Clear ();

    /// <summary>
    /// Sends all currently buffered output to the client.
    /// </summary>
    /// <exception cref="T:System.Web.HttpException">
    /// The cache is flushed after the response has been sent.
    /// </exception>
    void Flush ();

    /// <summary>
    /// Adds custom log information to the Internet Information Services (IIS) log file.
    /// </summary>
    /// <param name="param">
    /// The text to add to the log file.
    /// </param>
    void AppendToLog (string param);

    /// <summary>
    /// Redirects a client to a new URL and specifies the new URL.
    /// </summary>
    /// <param name="url">
    /// The target location. 
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// A redirection is attempted after the HTTP headers have been sent.
    /// </exception>
    void Redirect (string url);

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
    void Redirect (string url, bool endResponse);

    /// <summary>
    /// Writes a string to an HTTP response output stream.
    /// </summary>
    /// <param name="s">
    /// The string to write to the HTTP output stream.
    /// </param>
    void Write (string s);

    /// <summary>
    /// Writes an <see cref="T:System.Object" /> to an HTTP response stream.
    /// </summary>
    /// <param name="obj">
    /// The <see cref="T:System.Object" /> to write to the HTTP output stream.
    /// </param>
    void Write (object obj);

    /// <summary>
    /// Writes a character to an HTTP response output stream.
    /// </summary>
    /// <param name="ch">
    /// The character to write to the HTTP output stream.
    /// </param>
    void Write (char ch);

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
    void Write (char[] buffer, int index, int count);

    /// <summary>
    /// Allows insertion of response substitution blocks into the response, which allows dynamic generation of specified response regions for output cached responses.
    /// </summary>
    /// <param name="callback">
    /// The method, user control, or object to substitute.
    /// </param>
    /// <exception cref="T:System.ArgumentException">
    /// The target of the <paramref name="callback" /> parameter is of type <see cref="T:System.Web.UI.Control" />.
    /// </exception>
    void WriteSubstitution (HttpResponseSubstitutionCallback callback);

    /// <summary>
    /// Writes the contents of the specified file directly to an HTTP response output stream as a file block.
    /// </summary>
    /// <param name="filename">
    /// The name of the file to write to the HTTP output.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="filename" /> parameter is null.
    /// </exception>
    void WriteFile (string filename);

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
    void WriteFile (string filename, bool readIntoMemory);

    /// <summary>
    /// Writes the specified file directly to an HTTP response output stream, without buffering it in memory.
    /// </summary>
    /// <param name="filename">
    /// The name of the file to write to the HTTP output.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="filename" /> parameter is null</exception>
    void TransmitFile (string filename);

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
    void TransmitFile (string filename, long offset, long length);

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
    void WriteFile (string filename, long offset, long size);

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
    void WriteFile (IntPtr fileHandle, long offset, long size);

    /// <summary>
    /// Adds an HTTP header to the output stream. <see cref="M:System.Web.HttpResponse.AddHeader(System.String,System.String)" /> is provided for compatibility with earlier versions of ASP.
    /// </summary>
    /// <param name="name">
    /// The name of the HTTP header to add <paramref name="value" /> to.
    /// </param>
    /// <param name="value">
    /// The string to add to the header.
    /// </param>
    void AddHeader (string name, string value);

    /// <summary>
    /// Sends all currently buffered output to the client, stops execution of the page, and raises the <see cref="E:System.Web.HttpApplication.EndRequest" /> event.
    /// </summary>
    /// <exception cref="T:System.Threading.ThreadAbortException">
    /// The call to <see cref="M:System.Web.HttpResponse.End" /> has terminated the current request.
    /// </exception>
    void End ();

    /// <summary>
    /// Adds a session ID to the virtual path if the session is using <see cref="P:System.Web.Configuration.SessionStateSection.Cookieless" /> session state and returns the combined path. If <see cref="P:System.Web.Configuration.SessionStateSection.Cookieless" /> session state is not used, <see cref="M:System.Web.HttpResponse.ApplyAppPathModifier(System.String)" /> returns the original virtual path.
    /// </summary>
    /// <returns>
    /// The <paramref name="virtualPath" /> with the session ID inserted.
    /// </returns>
    /// <param name="virtualPath">
    /// The virtual path to a resource. 
    /// </param>
    string ApplyAppPathModifier (string virtualPath);
  }
}