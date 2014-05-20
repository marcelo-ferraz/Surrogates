using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.SDILReader
{
    public class MethodBodyReader
    {
        static MethodBodyReader() 
        {
            Globals.LoadOpCodes();
        }

        public List<SDILReader.ILInstruction> Instructions { get; set; }
        protected byte[] Il = null;
        private MethodInfo _mi = null;

        #region Il read methods
        
        private int ReadInt16(byte[] _il, ref int position)
        {
            return ((Il[position++] | (Il[position++] << 8)));
        }

        private ushort ReadUInt16(byte[] _il, ref int position)
        {
            return (ushort)((Il[position++] | (Il[position++] << 8)));
        }
        
        private int ReadInt32(byte[] _il, ref int position)
        {
            return (((Il[position++] | (Il[position++] << 8)) | (Il[position++] << 0x10)) | (Il[position++] << 0x18));
        }
        
        private ulong ReadInt64(byte[] _il, ref int position)
        {
            return (ulong)(((Il[position++] | (Il[position++] << 8)) | (Il[position++] << 0x10)) | (Il[position++] << 0x18) | (Il[position++] << 0x20) | (Il[position++] << 0x28) | (Il[position++] << 0x30) | (Il[position++] << 0x38));
        }
        
        private double ReadDouble(byte[] _il, ref int position)
        {
            return (((Il[position++] | (Il[position++] << 8)) | (Il[position++] << 0x10)) | (Il[position++] << 0x18) | (Il[position++] << 0x20) | (Il[position++] << 0x28) | (Il[position++] << 0x30) | (Il[position++] << 0x38));
        }
        
        private sbyte ReadSByte(byte[] _il, ref int position)
        {
            return (sbyte)Il[position++];
        }
        
        private byte ReadByte(byte[] _il, ref int position)
        {
            return (byte)Il[position++];
        }
        
        private Single ReadSingle(byte[] _il, ref int position)
        {
            return (Single)(((Il[position++] | (Il[position++] << 8)) | (Il[position++] << 0x10)) | (Il[position++] << 0x18));
        }
        
        #endregion

        /// <summary>
        /// Constructs the array of ILInstructions according to the IL byte code.
        /// </summary>
        /// <param name="module"></param>
        private void ConstructInstructions(Module module)
        {
            byte[] il = this.Il;
            int position = 0;
            Instructions = new List<ILInstruction>();
            while (position < il.Length)
            {
                ILInstruction instruction = new ILInstruction();

                // get the operation code of the current instruction
                OpCode code = OpCodes.Nop;
                ushort value = il[position++];
                if (value != 0xfe)
                {
                    code = Globals.singleByteOpCodes[(int)value];
                }
                else
                {
                    value = il[position++];
                    code = Globals.multiByteOpCodes[(int)value];
                    value = (ushort)(value | 0xfe00);
                }

                instruction.Code = code;
                instruction.Offset = position - 1;
                int metadataToken = 0;
                // get the operand of the current operation
                switch (code.OperandType)
                {
                    case OperandType.InlineBrTarget:
                        metadataToken = ReadInt32(il, ref position);
                        metadataToken += position;
                        instruction.Operand = metadataToken;
                        break;
                    case OperandType.InlineField:
                        metadataToken = ReadInt32(il, ref position);
                        instruction.Operand = module.ResolveField(metadataToken);
                        break;
                    case OperandType.InlineMethod:
                        metadataToken = ReadInt32(il, ref position);
                        try
                        {
                            instruction.Operand = module.ResolveMethod(metadataToken);
                        }
                        catch
                        {
                            instruction.Operand = module.ResolveMember(metadataToken);
                        }
                        break;
                    case OperandType.InlineSig:
                        metadataToken = ReadInt32(il, ref position);
                        instruction.Operand = module.ResolveSignature(metadataToken);
                        break;
                    case OperandType.InlineTok:
                        metadataToken = ReadInt32(il, ref position);
                        try
                        {
                            instruction.Operand = module.ResolveType(metadataToken);
                        }
                        catch(Exception ex)
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                        // SSS : see what to do here
                        break;
                    case OperandType.InlineType:
                        metadataToken = ReadInt32(il, ref position);
                        // now we call the ResolveType always using the generic attributes type in order
                        // to support decompilation of generic methods and classes

                        // thanks to the guys from code project who commented on this missing feature

                        instruction.Operand = module.ResolveType(metadataToken, this._mi.DeclaringType.GetGenericArguments(), this._mi.GetGenericArguments());
                        break;
                    case OperandType.InlineI:
                        {
                            instruction.Operand = ReadInt32(il, ref position);
                            break;
                        }
                    case OperandType.InlineI8:
                        {
                            instruction.Operand = ReadInt64(il, ref position);
                            break;
                        }
                    case OperandType.InlineNone:
                        {
                            instruction.Operand = null;
                            break;
                        }
                    case OperandType.InlineR:
                        {
                            instruction.Operand = ReadDouble(il, ref position);
                            break;
                        }
                    case OperandType.InlineString:
                        {
                            metadataToken = ReadInt32(il, ref position);
                            instruction.Operand = module.ResolveString(metadataToken);
                            break;
                        }
                    case OperandType.InlineSwitch:
                        {
                            int count = ReadInt32(il, ref position);
                            int[] casesAddresses = new int[count];
                            for (int i = 0; i < count; i++)
                            {
                                casesAddresses[i] = ReadInt32(il, ref position);
                            }
                            int[] cases = new int[count];
                            for (int i = 0; i < count; i++)
                            {
                                cases[i] = position + casesAddresses[i];
                            }
                            break;
                        }
                    case OperandType.InlineVar:
                        {
                            instruction.Operand = ReadUInt16(il, ref position);
                            break;
                        }
                    case OperandType.ShortInlineBrTarget:
                        {
                            instruction.Operand = ReadSByte(il, ref position) + position;
                            break;
                        }
                    case OperandType.ShortInlineI:
                        {
                            instruction.Operand = ReadSByte(il, ref position);
                            break;
                        }
                    case OperandType.ShortInlineR:
                        {
                            instruction.Operand = ReadSingle(il, ref position);
                            break;
                        }
                    case OperandType.ShortInlineVar:
                        {
                            instruction.Operand = ReadByte(il, ref position);
                            break;
                        }
                    default:
                        {
                            throw new Exception("Unknown operand type.");
                        }
                }
                Instructions.Add(instruction);
            }
        }

        public object GetRefferencedOperand(Module module, int metadataToken)
        {
            AssemblyName[] assemblyNames = module.Assembly.GetReferencedAssemblies();
            for (int i = 0; i < assemblyNames.Length; i++)
            {
                Module[] modules = Assembly.Load(assemblyNames[i]).GetModules();
                for (int j = 0; j < modules.Length; j++)
                {
                    try
                    {
                        Type t = modules[j].ResolveType(metadataToken);
                        return t;
                    }
                    catch
                    {

                    }

                }
            }
            return null;
            //System.Reflection.Assembly.Load(module.Assembly.GetReferencedAssemblies()[3]).GetModules()[0].ResolveType(metadataToken)

        }
        /// <summary>
        /// Gets the IL code of the method
        /// </summary>
        /// <returns></returns>
        public string GetBodyCode()
        {
            string result = "";
            if (Instructions != null)
            {
                for (int i = 0; i < Instructions.Count; i++)
                {
                    result += Instructions[i].GetCode() + "\n";
                }
            }
            return result;

        }

        /// <summary>
        /// MethodBodyReader constructor
        /// </summary>
        /// <param name="_mi">
        /// The System.Reflection defined MethodInfo
        /// </param>
        public MethodBodyReader(MethodInfo mi)
        {
            this._mi = mi;
            if (mi.GetMethodBody() != null)
            {
                Il = mi.GetMethodBody().GetILAsByteArray();
                ConstructInstructions(mi.Module);
            }
        }
    }
}
