using System;

namespace System.Text.Json.Serialization;

[AttributeUsage(AttributeTargets.Class)]
public class JsonSchemaNameAttribute : Attribute
{
    public string Name { get; }

    public JsonSchemaNameAttribute(string name)
    {
        Name = name;
    }
} 