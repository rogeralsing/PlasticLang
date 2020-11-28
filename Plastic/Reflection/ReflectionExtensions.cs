using System;
using System.Reflection;

namespace PlasticLang.Reflection
{
    public static class ReflectionExtensions
    {
        public static PropertyInfo GetProperty(this object self, string name)
        {
            var prop = self.GetType().GetProperty(name);
            if (prop is null) throw new NotSupportedException();
            return prop;
        }
        
        public static object? GetPropertyValue(this object self, string name)
        {
            var prop = self.GetType().GetProperty(name);
            if (prop is null) throw new NotSupportedException();
            return prop.GetValue(self);
        }
    }
}