namespace Remotion.ObjectBinding
{
  /// <summary>
  /// The <see cref="IBusinessObjectProviderWithIdentity"/> interface defines a getter and a set-method to associate a 
  /// <see cref="BusinessObjectProviderAttribute"/> with the provider. Through this attribute it is possible to identify the relationship
  /// between a provider and its object model.
  /// </summary>
  public interface IBusinessObjectProviderWithIdentity : IBusinessObjectProvider
  {
    BusinessObjectProviderAttribute ProviderAttribute { get; }
  }
}