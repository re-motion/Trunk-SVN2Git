using System;

namespace Remotion.SecurityManager.UnitTests.TestDomain
{
  public class PremiumOrder : Order
  {
    public Delivery Delivery
    {
      get { return Delivery.Dhl; }
    }
  }
}
