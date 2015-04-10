
using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Model.Collections
{
    public class FieldList
    {
        internal class TypeNField
        {
            public Type Type { get; set; }
            public FieldBuilder Field { get; set; }

            public TypeNField(Type type)
            {
                this.Type = type;
            }

            public TypeNField(System.Type type, FieldBuilder field)
            {
                this.Type = type;
                this.Field = field;
            }
        }

        private List<FieldBuilder> _fields;
        private Dictionary<string, List<TypeNField>> _innerTable;
        private Strategies _owner;

        internal FieldList(Strategies owner)
        {
            _owner = owner;
            _fields = new List<FieldBuilder>();
            _innerTable = new Dictionary<string, List<TypeNField>>();
        }

        internal int Count { get { return _fields.Count; } }

        internal FieldBuilder this[int i]
        {
            get { return _fields[i]; }
        }

        internal FieldBuilder Get<T>(string name = null)
        {
            return Get(typeof(T), name);
        }

        internal bool TryAdd<T>(ref string name)
        {
            return TryAdd(typeof(T),ref name);
        }

        internal bool TryAdd(Type type, ref string name)
        {
            if (string.IsNullOrEmpty(name))
            { name = "interceptor"; }

            if (name.CanBeFieldName())
            { throw new ArgumentException("The name cannot contain any special characters and can only start with a letter."); }

            if (_innerTable.ContainsKey(name))
            { return false; }

            _innerTable.Add(
                name,
                new List<TypeNField>() { 
                        new TypeNField(type) 
                    });

            return true;
        }

        internal FieldBuilder this[string name]
        {
            get
            {
                for (int i = 0; i < _fields.Count; i++)
                {
                    if (_fields[i].Name == name)
                    { return _fields[i]; }
                }
                return null;
            }
        }

        internal FieldBuilder Get(Type type, string name = null)
        {
            TryAdd(type, ref name);

            var found = false;
            var fields = _innerTable[name];
            int index = 0;

            for (; index < fields.Count; index++)
            {
                if (found = fields[index].Type == type)
                { break; }
            }

            if (found && fields[index].Field != null)
            { return fields[index].Field; }

            //else
            var field = _owner.Builder.DefineField(
                string.Format("_{0}{1}{2}", Char.ToLower(name[0]), name.Length > 1 ? name.Substring(1) : string.Empty, index > 0 ? string.Concat('_', index.ToString()) : string.Empty),
                type,
                FieldAttributes.Private);

            fields.Insert(index++, new TypeNField(type, field));
            _fields.Add(field);

            return field;
        }

    }
}