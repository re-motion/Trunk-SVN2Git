using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaControlObject : ControlObject
  {
    public ActaNovaControlObject ([NotNull] TestObjectContext context)
        : base (context)
    {
    }
  }
}