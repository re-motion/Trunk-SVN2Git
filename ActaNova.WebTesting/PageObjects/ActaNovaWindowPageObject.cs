﻿using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  public class ActaNovaWindowPageObject : ActaNovaPageObject
  {
    public ActaNovaWindowPageObject ([NotNull] TestObjectContext context)
        : base(context)
    {
    }
  }
}