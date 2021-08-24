using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;

namespace AspNetCore.Reporting.Common.Services
{
   public class ExpandoObjectLoader
    {
        private readonly string _boName;

        public ExpandoObjectLoader(string businessObjectName)
        {
            _boName = businessObjectName;
        }
        
        public IEnumerable<ExpandoObject> GetData()
        {
            return new[]
            {
                new Dictionary<string, object>()
                {
                    {"Id", 30},
                    {"Nome", "abc"}
                }.ToExpando(),

                new Dictionary<string, object>()
                {
                    {"Id", 40},
                    {"Nome", "def"}
                }.ToExpando()
            };
        }
    }
    
    public class ExpandoObjectSchemaLoader : ITypedList
    {
        public string BusinessObject
        {
            get { return _boName; }
        }
        
        private readonly string _boName;

        public ExpandoObjectSchemaLoader()
        {
        }

        public ExpandoObjectSchemaLoader(string businessObjectName) : this()
        {
            _boName = businessObjectName;
        }

        public IEnumerable<ExpandoObject> GetData
        {
            get
            {
                return new[]
                {
                    new Dictionary<string, object>()
                    {
                        {"Id", 80},
                        {"Nome", "abc"}
                    }.ToExpando(),

                    new Dictionary<string, object>()
                    {
                        {"Id", 90},
                        {"Nome", "def"}
                    }.ToExpando()
                };
            }
        }
        
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            var defaultFromSchema = new Dictionary<string, object>()
            {
                {"Id", 0},
                {"Nome", "xxx"}
            }.ToExpando();

            var pdc = new ExpandoObjectTypeDescriptionProvider()
                .GetTypeDescriptor(typeof(ExpandoObject), defaultFromSchema)
                .GetProperties();

            return pdc;
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return _boName;
        }
    }

    public static class DictionaryExtensionMethods
    {
        /// <summary>
        /// Extension method that turns a dictionary of string and object to an ExpandoObject
        /// </summary>
        public static ExpandoObject ToExpando(this IDictionary<string, object> dictionary)
        {
            var expando = new ExpandoObject();
            var expandoDic = (IDictionary<string, object>) expando;

            // go through the items in the dictionary and copy over the key value pairs)
            foreach (var kvp in dictionary)
            {
                // if the value can also be turned into an ExpandoObject, then do it!
                if (kvp.Value is IDictionary<string, object>)
                {
                    var expandoValue = ((IDictionary<string, object>) kvp.Value).ToExpando();
                    expandoDic.Add(kvp.Key, expandoValue);
                }
                else if (kvp.Value is ICollection)
                {
                    // iterate through the collection and convert any strin-object dictionaries
                    // along the way into expando objects
                    var itemList = new List<object>();
                    foreach (var item in (ICollection) kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            var expandoItem = ((IDictionary<string, object>) item).ToExpando();
                            itemList.Add(expandoItem);
                        }
                        else
                        {
                            itemList.Add(item);
                        }
                    }

                    expandoDic.Add(kvp.Key, itemList);
                }
                else
                {
                    expandoDic.Add(kvp);
                }
            }

            return expando;
        }
    }

    public class ExpandoObjectTypeDescriptionProvider : TypeDescriptionProvider
    {
        private static readonly TypeDescriptionProvider _default = TypeDescriptor.GetProvider(typeof(ExpandoObject));

        public ExpandoObjectTypeDescriptionProvider()
            : base(_default)
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            var defaultDescriptor = base.GetTypeDescriptor(objectType, instance);

            return instance == null ? defaultDescriptor : new ExpandoObjectTypeDescriptor(instance as ExpandoObject);
        }
    }

    public class ExpandoObjectTypeDescriptor : ICustomTypeDescriptor
    {
        private readonly ExpandoObject _expando;

        public ExpandoObjectTypeDescriptor(ExpandoObject expando)
        {
            _expando = expando;
        }

        // Just use the default behavior from TypeDescriptor for most of these
        // This might need some tweaking to work correctly for ExpandoObjects though...

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _expando;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        // This is where the GetProperties() calls are
        // Ignore the Attribute for now, if it's needed support will have to be implemented
        // Should be enough for simple usage...

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor) this).GetProperties(new Attribute[0]);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            // This just casts the ExpandoObject to an IDictionary<string, object> to get the keys
            return new PropertyDescriptorCollection(
                ((IDictionary<string, object>) _expando).Keys
                .Select(x => new ExpandoPropertyDescriptor(((IDictionary<string, object>) _expando), x))
                .ToArray());
        }

        // A nested PropertyDescriptor class that can get and set properties of the
        // ExpandoObject dynamically at run time
        private class ExpandoPropertyDescriptor : PropertyDescriptor
        {
            private readonly IDictionary<string, object> _expando;
            private readonly string _name;

            public ExpandoPropertyDescriptor(IDictionary<string, object> expando, string name)
                : base(name, null)
            {
                _expando = expando;
                _name = name;
            }

            public override Type PropertyType
            {
                get { return _expando[_name].GetType(); }
            }

            public override void SetValue(object component, object value)
            {
                _expando[_name] = value;
            }

            public override object GetValue(object component)
            {
                return _expando[_name];
            }

            public override bool IsReadOnly
            {
                get
                {
                    // You might be able to implement some better logic here
                    return false;
                }
            }

            public override Type ComponentType
            {
                get { return null; }
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override void ResetValue(object component)
            {
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            public override string Category
            {
                get { return string.Empty; }
            }

            public override string Description
            {
                get { return string.Empty; }
            }
        }
    }
    
    
    
    
}