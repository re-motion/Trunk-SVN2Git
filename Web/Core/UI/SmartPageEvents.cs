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

namespace Remotion.Web.UI
{
  /// <summary> Specifies the client side events supported for registration by the <see cref="ISmartPage"/>. </summary>
  public enum SmartPageEvents
  {
    /// <summary> Rasied when the document has finished loading. Signature: <c>void Function (hasSubmitted, isCached)</c> </summary>
    OnLoad,
    /// <summary> Raised when the user posts back to the server. Signature: <c>void Function (eventTargetID, eventArgs)</c> </summary>
    OnPostBack,
    /// <summary> Raised when the user leaves the page. Signature: <c>void Function (hasSubmitted, isCached)</c> </summary>
    OnAbort,
    /// <summary> Raised when the user scrolls the page. Signature: <c>void Function ()</c> </summary>
    OnScroll,
    /// <summary> Raised when the user resizes the page. Signature: <c>void Function ()</c> </summary>
    OnResize,
    /// <summary> 
    ///   Raised before the request to load a new page (or reload the current page) is executed. Not supported in Opera.
    ///   Signature: <c>void Function ()</c>
    /// </summary>
    OnBeforeUnload,
    /// <summary> Raised before the page is removed from the window. Signature: <c>void Function ()</c> </summary>
    OnUnload
  }
}