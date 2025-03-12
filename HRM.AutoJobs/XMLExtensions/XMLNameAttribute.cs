namespace HRM.AutoJobs.XMLExtensions
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class XMLNameAttribute(string name) : Attribute
    {
        public string Name => name;
    }
}