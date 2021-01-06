using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Sigil;

namespace PngCheck
{
    public class Common
    {
        internal static int SizeOf(Type type)
        {
            DynamicMethod dm = new DynamicMethod("$", typeof(int), Type.EmptyTypes);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Sizeof, type);
            il.Emit(OpCodes.Ret);
            return (int)dm.Invoke(null, null);
        }

        internal static MethodInfo GetPropertySetter(PropertyInfo propertyInfo, Type type)
        {
            if (propertyInfo.DeclaringType == type) return propertyInfo.GetSetMethod(true);

            return propertyInfo.DeclaringType.GetProperty(
                propertyInfo.Name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                Type.DefaultBinder,
                propertyInfo.PropertyType,
                propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray(),
                null).GetSetMethod(true);
        }
        internal static List<PropertyInfo> GetSettableProps(Type t)
        {
            return t
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => GetPropertySetter(p, t) != null)
                .ToList();
        }
        [AttributeUsage(AttributeTargets.Field)]
        public class EndianAttribute : Attribute
        {
            public Endianness Endianness { get; private set; }

            public EndianAttribute(Endianness endianness)
            {
                this.Endianness = endianness;
            }
        }

        public enum Endianness
        {
            BigEndian,
            LittleEndian
        }

        private static void RespectEndianness(Type type, byte[] data)
        {
            // var fields = type.GetFields().Where(f => f.IsDefined(typeof(EndianAttribute), false))
            //     .Select(f => new
            //     {
            //         Field = f,
            //         Attribute = (EndianAttribute)f.GetCustomAttributes(typeof(EndianAttribute), false)[0],
            //         Offset = Marshal.OffsetOf(type, f.Name).ToInt32()
            //     }).ToList();
            var fields = type.GetFields()
                .Select(f => new
                {
                    Field = f,
                    Attribute = (EndianAttribute)f.GetCustomAttributes(typeof(EndianAttribute), false)[0],
                    Offset = Marshal.OffsetOf(type, f.Name).ToInt32()
                }).ToList();
            int offset = 0;
            int size = 0;
            foreach (var field in fields)
            {
                if ((field.Attribute.Endianness == Endianness.BigEndian && BitConverter.IsLittleEndian) ||
                    (field.Attribute.Endianness == Endianness.LittleEndian && !BitConverter.IsLittleEndian))
                {
                    if (field.Field.FieldType.BaseType==typeof(Array))
                    {
                        string typeName = field.Field.FieldType.FullName.Replace("[]", string.Empty);
                        type = field.Field.FieldType.Assembly.GetType(typeName);
                        size = SizeOf(type);
                        Array.Reverse(data, offset, size);
                    }
                    else
                    {
                        size = SizeOf(field.Field.FieldType);
                        Array.Reverse(data, offset, size);
                    }
                }
            }
        }

        public static T BytesToStruct<T>(byte[] rawData) where T : struct
        {
            T result = default(T);

            RespectEndianness(typeof(T), rawData);

            // GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            var size = SizeOf(typeof(T));
            IntPtr handle = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(rawData, 0, handle, size);
                // IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                // result = (T)Marshal.PtrToStructure(rawDataPtr, typeof(T));
                result=(T) Marshal.PtrToStructure(handle, typeof(T));
            }
            finally
            {
                // handle.Free();
                Marshal.FreeHGlobal(handle);
            }

            return result;
        }

        public static byte[] StructToBytes<T>(T data) where T : struct
        {
            byte[] rawData = new byte[Marshal.SizeOf(data)];
            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                Marshal.StructureToPtr(data, rawDataPtr, false);
            }
            finally
            {
                handle.Free();
            }

            RespectEndianness(typeof(T), rawData);

            return rawData;
        }
    }
}