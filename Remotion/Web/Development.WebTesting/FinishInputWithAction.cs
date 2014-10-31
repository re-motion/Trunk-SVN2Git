using System;
using Coypu;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Defines an action which finishes a textual input actopm (most notably: finish using the tab character to trigger the ASP.NET WebForms postback).
  /// </summary>
  public delegate void FinishInputWithAction (ElementScope scope);
}