using System;
using Coypu;
using OpenQA.Selenium;

namespace Remotion.Web.Development.WebTesting
{
  public static class Then
  {
    public static readonly ThenAction DoNothing = s => { };
    public static readonly ThenAction TabAway = s => s.SendKeysFixed (Keys.Tab);
    public static readonly ThenAction PressEnter = s => s.SendKeysFixed (Keys.Enter);
  }

  public delegate void ThenAction (ElementScope scope);
}