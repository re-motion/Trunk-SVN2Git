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
using System.Web.UI;
using Remotion.Globalization;

namespace Remotion.Web.UI.Globalization
{

/// <summary>
///   Interface for controls who wish to use automatic resource dispatching
///   but implement the dispatching logic themselves.
/// </summary>
public interface IResourceDispatchTarget
{
  /// <summary>
  ///   <b>Dispatch</b> is called by the parent control
  ///   and receives the resources as an <b>IDictonary</b>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The implementation of <b>IResourceDispatchTarget</b> is responsible for interpreting
  ///     the resources provided through <b>ByElementName</b>.
  ///   </para><para>
  ///     The key of the <b>IDictonaryEntry</b> can be a simple property name
  ///     or a more complex string. It can be freely defined by the <c>IResourceDispatchTarget</c>
  ///     implementation. Inside the resource container, this key is prepended by the control
  ///     instance's ID and a prefix.For details, please refer to 
  ///     <see cref="ResourceDispatcher.Dispatch(Control, IResourceManager)" />
  ///   </para>
  /// </remarks>
  /// <param name="values">
  ///   An <b>IDictonary</b>: &lt;string key, string value&gt;.
  /// </param>
  void Dispatch (IDictionary values);
}

}
