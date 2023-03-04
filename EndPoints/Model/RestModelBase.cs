using System;
using System.Collections.Generic;

namespace EndPoints.Model
{
    public abstract class RestModelBase : IRestModelBase
    {
        public List<Link> Links { get; set; } = new List<Link>();
    }
}
