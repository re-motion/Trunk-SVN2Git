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

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// 
  /// </summary>
  /// <example>
  /// internal class Sample
  /// {
  ///   void OnClick (object sender, EventArgs e)
  ///   {
  ///     try
  ///     {
  ///       IWxeCallArguments args;
  ///       args = WxeCallArguments.Default;                                    
  ///       args = new WxePermaUrlOptions ();                                  
  ///       args = new WxePermaUrlOptions (true);
  ///       args = new WxeCallArguments ((Control) sender, new WxeCallOptionsExternal ("_blank"));
  ///       args = new WxeCallArguments ((Control) sender, new WxeCallOptionsNoRepost ());
  ///       args = new WxeCallArguments ((Control) sender, new WxeCallOptions ());
  ///       // MyPage.Call (this, handler, arg1);</example>
  ///      }
  ///     catch (WxeIgnorableException) { }
  ///   }
  /// }
  /// </example>
  public interface IWxeCallArguments
  {
    void Call (IWxePage page, WxeFunction function);
  }
}