using System;

namespace Remotion.Web.Development.WebTesting
{
  public class DriverOptions
  {
    /// <summary>
    /// Specifies a command timeout for the communication between the Selenium language bindings and the <see cref="OpenQA.Selenium.IWebDriver"/>.
    /// </summary>
    public TimeSpan? CommandTimeout { get; set; }
  }
}