using System;

namespace Application.Common.Decorators
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class LoggingAttribute : Attribute { }
}