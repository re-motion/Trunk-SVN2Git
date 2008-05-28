using System;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Instances exposing the <see cref="IClassReflector"/> interface can be used to create a <see cref="BindableObjectClass"/> for a <see cref="Type"/>.
  /// </summary>
  public interface IClassReflector
  {
    /// <summary>
    /// The <see cref="Type"/> for which to create the <see cref="BindableObjectClass"/>.
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// The <see cref="BindableObjectProvider"/> associated with the <see cref="TargetType"/>.
    /// </summary>
    BindableObjectProvider BusinessObjectProvider { get; }

    /// <summary>
    /// Creates an instance of type <see cref="BindableObjectClass"/> for the <see cref="TargetType"/>.
    /// </summary>
    BindableObjectClass GetMetadata ();
  }
}