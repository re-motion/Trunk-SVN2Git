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
  ///   The <see cref="IWxeCallArguments"/> interface is used to to collect the parameters for executing a <see cref="WxeFunction"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  ///   The usage pattern is to pass the <see cref="IWxeCallArguments"/> and the <see cref="WxeFunction"/> to the <see cref="IWxePage.ExecuteFunction"/>
  ///   method defined by the <see cref="IWxePage"/>. This methood then invokes the <see cref="Dispatch"/> method, passing the <see cref="IWxeExecutor"/>
  ///   for the <see cref="IWxePage"/> and the <see cref="WxeFunction"/>. It is the <see cref="Dispatch"/> method's responsibility to correctly 
  ///   execute the <see cref="WxeFunction"/> with the help of the <see cref="IWxeExecutor"/> and using the state of this <see cref="IWxeCallArguments"/> object.
  /// </para>
  /// <para>
  ///   Use the <see cref="WxeCallArguments.Default"/> instance exposed on the <see cref="WxeCallArguments"/> type if your usecase is to simply
  ///   invoke a sub-function on your page. If you whish to execute the function with more advanced <see cref="WxeCallOptions"/>, isntantiate an 
  ///   instance of the <see cref="WxeCallArguments"/> type. Finally, the <see cref="WxePermaUrlCallArguments"/> type is used if simply wish to
  ///   display a perma-URL in the browser's location-bar.
  /// </para>
  /// <para>
  ///   The <b>WxeGen</b> also allows for a simplified syntax by providing static <b>Call</b> methods on each page that will accept all required 
  ///   parameters (the <see cref="IWxePage"/>, the <see cref="IWxeCallArguments"/>, and additional arguments required by the specific function).
  /// </para>
  /// </remarks>
  /// <example>
  /// <code escaped="true" lang="C#">
  /// internal class Sample
  /// {
  ///   void OnClick (object sender, EventArgs e)
  ///   {
  ///     try
  ///     {
  ///       IWxeCallArguments callArguments;
  ///       callArguments = WxeCallArguments.Default;                                    
  ///       callArguments = new WxePermaUrlCallArguments ();                                  
  ///       callArguments = new WxePermaUrlCallArguments (true);
  ///       callArguments = new WxeCallArguments ((Control) sender, new WxeCallOptionsExternal ("_blank"));
  ///       callArguments = new WxeCallArguments ((Control) sender, new WxeCallOptionsNoRepost ());
  ///       callArguments = new WxeCallArguments ((Control) sender, new WxeCallOptions ());
  ///       // MyPage.Call (this, callArguments, arg1, arg2, ...);
  ///      }
  ///     catch (WxeIgnorableException) { }
  ///   }
  /// }
  /// </code>
  /// </example>
  public interface IWxeCallArguments
  {
    void Dispatch (IWxeExecutor executor, WxeFunction function);
  }
}