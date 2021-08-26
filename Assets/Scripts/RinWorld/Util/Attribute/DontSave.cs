using System;

namespace RinWorld.Util.Attribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DontSaveAttribute : System.Attribute
    {
    }
}