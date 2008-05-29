using System;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public abstract class ClassWithIdentityFromOtherBusinessObjectImplementation : ClassFromOtherBusinessObjectImplementation, IBusinessObjectWithIdentity
  {
    public abstract string UniqueIdentifier { get; }
  }
}