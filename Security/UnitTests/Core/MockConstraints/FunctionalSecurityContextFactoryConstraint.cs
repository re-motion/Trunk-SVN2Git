using System;
using Rhino.Mocks.Constraints;
using Remotion.Utilities;

namespace Remotion.Security.UnitTests.Core.MockConstraints
{
  public class FunctionalSecurityContextFactoryConstraint : AbstractConstraint
  {
    private string _typeName;
    private string _message;

    public FunctionalSecurityContextFactoryConstraint (string typeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      _typeName = typeName;
    }

    public override bool Eval (object obj)
    {
      if (obj == null)
      {
        _message = "The FunctionalSecurityContextFactory is null.";
        return false;
      }
      FunctionalSecurityContextFactory factory = (FunctionalSecurityContextFactory) obj;

      SecurityContext context = factory.CreateSecurityContext ();
      if (context == null)
      {
        _message = "The FunctionalSecurityContextFactory.GetSecurityContext() evaluated and returned null.";
        return false;
      }

      if (!string.Equals (_typeName, context.Class, StringComparison.Ordinal))
      {
        _message = string.Format ("Expected class {0}, but was {1}", _typeName, context.Class ?? "null");
        return false;
      }

      return true;
    }

    public override string Message
    {
      get { return _message; }
    }
  }
}