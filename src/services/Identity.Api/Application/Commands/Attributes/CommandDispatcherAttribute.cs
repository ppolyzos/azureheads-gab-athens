using System;

namespace Identity.Api.Application.Commands.Attributes
{
    public class CommandDispatcherAttribute : Attribute
    {
        public string Name { get; }

        public CommandDispatcherAttribute(string name)
        {
            Name = name;
        }
    }
}