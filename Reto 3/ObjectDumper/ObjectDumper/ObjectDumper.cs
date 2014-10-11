using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectDumper
{
    public class ObjectDumper<T>
    {
        private readonly Dictionary<string, Delegate> _templates;

        public ObjectDumper()
        {
            _templates = new Dictionary<string, Delegate>();
        }

        public IEnumerable<KeyValuePair<string, string>> Dump(T data)
        {
            if (data == null)
            {
                yield break;
            }

            var type = data.GetType();

            var properties = type.GetProperties()
                .Where(p => p.CanRead)
                .OrderBy(p => p.Name);

            foreach (var property in properties)
            {
                var value = property.GetValue(data);
                var template = GetTemplateFor(property);

                if (template != null)
                {
                    yield return PropertyDumpWithTemplate(property, template, value);
                }

                else yield return PropertyDump(property, value);
            }
        }

        public void AddTemplateFor<T2>(Expression<Func<T, T2>> expression, Func<T2, string> result)
        {
            var property = expression.GetPropertyInfo();

            _templates[property.Name] = result;
        }

        private static KeyValuePair<string, string> PropertyDumpWithTemplate(PropertyInfo property, Delegate template, object value)
        {
            return new KeyValuePair<string, string>(
                property.Name,
                template.DynamicInvoke(value) as string);
        }

        private static KeyValuePair<string, string> PropertyDump(PropertyInfo property, object value)
        {
            return new KeyValuePair<string, string>(
                property.Name,
                Convert.ToString(value));
        }

        private Delegate GetTemplateFor(PropertyInfo property)
        {
            if (_templates.ContainsKey(property.Name))
            {
                return _templates[property.Name];
            }

            return null;
        }
    }
}