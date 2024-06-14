using System;

namespace Brawl.State
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class AdjustableAttribute : Attribute
    {
    }
}