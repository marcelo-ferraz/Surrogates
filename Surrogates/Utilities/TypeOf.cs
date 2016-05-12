using Surrogates.Model.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace Surrogates.Utilities
{
    public class TypeOf
    {
        public static Type Object = null;
        public static Type ObjectArray = null;
        public static Type Short = null;
        public static Type Int = null;
        public static Type Long =null;
        public static Type Float = null;
        public static Type Double = null;
        public static Type Decimal = null;
        public static Type Byte = null;
        public static Type Char = null;
        public static Type String = null;
        public static Type DateTime = null;
        public static Type TimeSpan = null;
        
        public static Type Boolean = null;
        
        public static Type Sbyte  = null;
        public static Type Ushort = null;
        public static Type Uint  = null;
        public static Type Ulong  = null;
        
        public static Type Void = null;
        public static Type Attribute = null;
        public static Type Type = null;
        public static Type Action = null;
        public static Type Func = null;
        public static Type Func2 = null;
        public static Type Delegate = null;
        public static Type MulticastDelegate = null;
        public static Type Activator = null;
        
        public static Type Dictionary = null;
        public static Type Infer = null;
        
        public static Type IDynamicMetaObjectProvider;
        public static Type DynamicAttribute;

        public static Type IContainsStateBag;
        public static Type InterferenceKind;

        public static Type OpCode = null;
        public static Type OpCodes = null;

        static TypeOf()
        {
            Object = typeof(Object);
            ObjectArray = typeof(Object[]);
            Short = typeof(short);
            Int = typeof(int);
            Long = typeof(long);
            Float = typeof(float);
            Double = typeof(Double);
            Decimal = typeof(decimal);
            Byte = typeof(byte);
            Char = typeof(Char);
            String = typeof(String);
            DateTime = typeof(DateTime);
            TimeSpan = typeof(TimeSpan);
            Boolean = typeof(Boolean);
            
            Sbyte = typeof(sbyte);  
            Ushort = typeof(ushort);
            Uint = typeof(uint);
            Ulong = typeof(ulong);  

            Void = typeof(void);
            Attribute = typeof(Attribute);
            Type = typeof(Type);
            Action = typeof(Action);
            Func = typeof(Func<>);
            Func2 = typeof(Func<,>);
            Delegate = typeof(Delegate);
            MulticastDelegate = typeof(MulticastDelegate);
            Activator = typeof(Activator);

            Dictionary = typeof(Dictionary<,>);

            IDynamicMetaObjectProvider = typeof(IDynamicMetaObjectProvider);
            DynamicAttribute = typeof(DynamicAttribute);

            Infer = typeof(Infer);
            IContainsStateBag = typeof(IContainsStateBag);
            InterferenceKind = typeof(InterferenceKind);

            OpCode = typeof(OpCode);
            OpCodes = typeof(OpCodes);
        }
    }
}
