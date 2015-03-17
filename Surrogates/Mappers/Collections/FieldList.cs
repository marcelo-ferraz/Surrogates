
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Mappers.Entities;
using Surrogates.Utilities;

namespace Surrogates.Mappers.Collections
{
    using TypeFieldList = List<FieldList.TypeNField>;

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
        private Dictionary<string, TypeFieldList> _innerTable;
        private MappingState _owner;

        internal FieldList(MappingState owner)
        {
            _owner = owner;
            _fields = new List<FieldBuilder>();
            _innerTable = new Dictionary<string, TypeFieldList>();
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

        internal FieldBuilder Get(Type type, string name = null)
        {
            if (string.IsNullOrEmpty(name))
            { name = "interference"; }

            if (name.CanBeFieldName())
            { throw new ArgumentException("The name cannot contain any special characters and can only start with a letter."); }

            if (!_innerTable.ContainsKey(name))
            {
                _innerTable.Add(
                    name,
                    new TypeFieldList() { 
                        new TypeNField(type) 
                    });
            }

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
            var field = _owner.TypeBuilder.DefineField(
                string.Format("_{0}{1}_{2}", Char.ToLower(name[0]), name.Substring(1), index.ToString()),
                type,
                FieldAttributes.Private);

            fields.Insert(index++, new TypeNField(type, field));
            _fields.Add(field);

            return field;
        }

    }
}