using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTFLLab.Classes
{
    internal class CStruct : IDictionary<string, Type>
    {
        string _name;
        public string name { get => _name;}

        public CStruct(string name)
        {
            _name = name;
            _fields = new Dictionary<string, Type>();
        }

        public ICollection<string> Keys => ((IDictionary<string, Type>)_fields).Keys;

        public ICollection<Type> Values => ((IDictionary<string, Type>)_fields).Values;

        public int Count => ((ICollection<KeyValuePair<string, Type>>)_fields).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<string, Type>>)_fields).IsReadOnly;

        public Type this[string key] { get => ((IDictionary<string, Type>)_fields)[key]; set => ((IDictionary<string, Type>)_fields)[key] = value; }

        Dictionary<string, Type> _fields;

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, Type>)_fields).ContainsKey(key);
        }

        public void Add(string key, Type value)
        {
            ((IDictionary<string, Type>)_fields).Add(key, value);
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, Type>)_fields).Remove(key);
        }

        public bool TryGetValue(string key, out Type value)
        {
            return ((IDictionary<string, Type>)_fields).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, Type> item)
        {
            ((ICollection<KeyValuePair<string, Type>>)_fields).Add(item);
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<string, Type>>)_fields).Clear();
        }

        public bool Contains(KeyValuePair<string, Type> item)
        {
            return ((ICollection<KeyValuePair<string, Type>>)_fields).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, Type>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, Type>>)_fields).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, Type> item)
        {
            return ((ICollection<KeyValuePair<string, Type>>)_fields).Remove(item);
        }

        public IEnumerator<KeyValuePair<string, Type>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, Type>>)_fields).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_fields).GetEnumerator();
        }
    }
}
