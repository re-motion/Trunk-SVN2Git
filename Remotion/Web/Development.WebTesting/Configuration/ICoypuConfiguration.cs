using System;
using Coypu;

namespace Remotion.Web.Development.WebTesting.Configuration
{
  /// <summary>
  /// Provides all the necessary information to initialize Coypu <see cref="Options"/>.
  /// </summary>
  public interface ICoypuConfiguration
  {
    /// <summary>
    /// Specifies how long the Coypu engine should maximally search for a web element or try to interact with a web element before it fails.
    /// </summary>
    TimeSpan SearchTimeout { get; }

    /// <summary>
    /// Whenever the element to be interacted with is not ready, visible or otherwise not present, the Coypu engine automatically retries the action
    /// after the given <see cref="RetryInterval"/> until the <see cref="SearchTimeout"/> has been reached.
    /// </summary>
    TimeSpan RetryInterval { get; }
  }
}