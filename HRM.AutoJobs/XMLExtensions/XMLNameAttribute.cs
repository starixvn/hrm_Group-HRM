namespace HRM.AutoJobs.XMLExtensions
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class XMLNameAttribute : Attribute
    {
        private readonly string name;

        public XMLNameAttribute(string name)
        {
            this.name = name;
        }

        public string Name => name;
    }
}