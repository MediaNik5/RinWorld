using System;

namespace RimCopy.Attribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DontSaveAttribute : System.Attribute
    {
    }
}