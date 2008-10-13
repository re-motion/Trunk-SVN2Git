// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using System.Globalization;
using System.Threading;

namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// Represents a scope with a specific culture and UI-culture (see <see cref="CultureInfo"/>). 
  /// </summary>
  /// <example>
  /// Set German culture and Russian user-interface-culture for
  /// for current thread within using-block, automatically restore previously set cultures
  /// after using-block:
  /// <code><![CDATA[
  /// using (new CultureScope ("de","ru"))
  /// {
  ///   // Do something with German Culture and Russian UI-Culture here
  /// }
  /// ]]></code></example>
  public struct CultureScope : IDisposable
  {
    private readonly CultureInfo _backupCulture;
    private readonly CultureInfo _backupUICulture;

    /// <summary>
    /// Intialize <see cref="CultureScope"/> with culture-names-strings, e.g. "de-AT", "en-GB".
    /// </summary>
    /// <param name="cultureName">Culture name string. null to not switch culture.</param>
    /// <param name="uiCultureName">User interface culture name string. null to not switch UI-culture.</param>
    public CultureScope (string cultureName, string uiCultureName)
    {
      _backupCulture = null;
      _backupUICulture = null;

      Thread currentThread = Thread.CurrentThread;

      if (cultureName != null)
      {
        _backupCulture = currentThread.CurrentCulture;
        currentThread.CurrentCulture = CultureInfo.GetCultureInfo (cultureName);
      }

      if (uiCultureName != null)
      {
        _backupUICulture = currentThread.CurrentUICulture;
        currentThread.CurrentUICulture = CultureInfo.GetCultureInfo (uiCultureName);
      }
    }

    /// <summary>
    /// Intialize <see cref="CultureScope"/> from <see cref="CultureInfo"/> instances.
    /// </summary>
    /// <param name="cultureInfo">Culture to use.</param>
    /// <param name="uiCultureInfo">User interface culture to use.</param>
    public CultureScope (CultureInfo cultureInfo, CultureInfo uiCultureInfo)
      : this (cultureInfo.Name, uiCultureInfo.Name)
    {}


    public void Dispose ()
    {
      Thread currentThread = Thread.CurrentThread;
      if (_backupCulture != null)
      {
        currentThread.CurrentCulture = _backupCulture;
      }
      if (_backupUICulture != null)
      {
        currentThread.CurrentUICulture = _backupUICulture;
      }
    }
  }
}