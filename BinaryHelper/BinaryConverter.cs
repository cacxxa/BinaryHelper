using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BinaryHelper
{
    public class BinaryConverter
    {
        private static readonly BinaryConverter Instance = new BinaryConverter();
        private readonly object _lock = new object();

        private readonly Dictionary<Type, CoreType> _types = new Dictionary<Type, CoreType>
        {
            [typeof(bool)] = CoreType.Boolean,
            [typeof(string)] = CoreType.String,
            [typeof(byte[])] = CoreType.ByteArray,
            [typeof(byte)] = CoreType.Byte,
            [typeof(sbyte)] = CoreType.SByte,
            [typeof(char)] = CoreType.Char,
            [typeof(float)] = CoreType.Float,
            [typeof(double)] = CoreType.Double,
            [typeof(short)] = CoreType.Short,
            [typeof(int)] = CoreType.Int,
            [typeof(long)] = CoreType.Long,
            [typeof(ushort)] = CoreType.UShort,
            [typeof(uint)] = CoreType.UInt,
            [typeof(ulong)] = CoreType.ULong
        };

        private readonly Dictionary<Type, IEnumerable<MemberInfo>> _cache =
            new Dictionary<Type, IEnumerable<MemberInfo>>();

        private bool _isBigEndian, _toBytes;
        private IEnumerator<byte> _data;
        private List<byte> _result;

        private BinaryConverter()
        {
        }

        public static T FromBytes<T>(IEnumerable<byte> data, bool isBigEndian = false) where T : new()
        {
            T obj = new T();
            if (data == null) return obj;

            lock (Instance._lock)
            {
                Instance._isBigEndian = isBigEndian;
                Instance._toBytes = false;
                using (Instance._data = data.GetEnumerator())
                {
                    Instance.Process(obj, typeof(T));
                }

                Instance._data = null;

                return obj;
            }
        }

        public static byte[] ToBytes(object obj, bool isBigEndian = false)
        {
            lock (Instance._lock)
            {
                Instance._isBigEndian = isBigEndian;
                Instance._toBytes = true;
                Instance._result = new List<byte>();
                Instance.Process(obj, obj.GetType());
                return Instance._result.ToArray();
            }
        }

        private void Process(object obj, Type type)
        {
            foreach (MemberInfo member in GetMembers(type))
            {
                BinaryAttribute attr = member.GetCustomAttribute<BinaryAttribute>();

                bool ignoreRead = !_toBytes && attr.IgnoreOnRead;
                bool ignoreWrite = _toBytes && attr.IgnoreOnWrite;
                if (ignoreRead || ignoreWrite) continue;

                object value;

                switch (member)
                {
                    case FieldInfo info:
                        value = Convert(info.FieldType, info.Name, attr, _toBytes ? info.GetValue(obj) : null);
                        if (!_toBytes)
                        {
                            info.SetValue(obj, value);
                        }

                        break;
                    case PropertyInfo info:
                        if (_toBytes && !info.CanRead) continue;
                        value = Convert(info.PropertyType, info.Name, attr, _toBytes ? info.GetValue(obj) : null);
                        if (!_toBytes && info.CanWrite)
                        {
                            info.SetValue(obj, value);
                        }

                        break;
                }
            }
        }

        private IEnumerable<MemberInfo> GetMembers(Type type)
        {
            if (_cache.TryGetValue(type, out IEnumerable<MemberInfo> value)) return value;
            List<MemberInfo> list = type.GetMembers()
                .Where(prop => Attribute.IsDefined(prop, typeof(BinaryAttribute)))
                .OrderBy(prop => prop.GetCustomAttribute<BinaryAttribute>().Order).ToList();
            _cache.Add(type, list);
            return list;
        }

        private object Convert(Type type, string name, BinaryAttribute attr, object ret)
        {
            IEnumerable<byte> bytes;

            int len = (int) attr.Count;

            bool reverse = _isBigEndian;
            if (attr.Reverse)
            {
                reverse = !reverse;
            }

            if (!_toBytes && attr.Skip > 0)
            {
                _data.Skip((int) attr.Skip);
            }

            switch (attr.CustomType)
            {
                case CustomType.Hex:
                    if (_types[type] != CoreType.String)
                        throw new ArgumentException($"Тип поля {name} не является типом string");

                    if (!_toBytes)
                        return BitConverter.ToString(_data.ToArray(len, attr.Reverse)).Replace("-", string.Empty);

                    string hex = (string) ret ?? string.Empty;
                    int hexLen = Math.Max(hex.Length, len * 2);
                    if (hexLen % 2 != 0) hexLen++;
                    if (hex.Length < hexLen) hex = hex.PadLeft(hexLen, '0');
                    bytes = Enumerable.Range(0, hex.Length)
                        .Where(x => x % 2 == 0)
                        .Select(x => System.Convert.ToByte(hex.Substring(x, 2), 16));
                    using (IEnumerator<byte> en = bytes.GetEnumerator())
                        _result.AddRange(en.ToArray(len, attr.Reverse));
                    return null;
            }

            if (type.IsEnum)
            {
                int value = 0;
                if (_toBytes)
                {
                    value = (int) ret;
                }

                switch (len)
                {
                    case 0:
                    case 1:
                        if (_toBytes)
                        {
                            _result.Add((byte) value);
                            return null;
                        }

                        value = _data.Take(1).Single();
                        break;
                    case 2:
                        if (_toBytes)
                        {
                            bytes = BitConverter.GetBytes((short) value);
                            _result.AddRange(reverse ? bytes.Reverse() : bytes);
                            return null;
                        }

                        value = BitConverter.ToInt16(_data.ToArray(2, reverse), 0);
                        break;
                    case 4:
                        if (_toBytes)
                        {
                            bytes = BitConverter.GetBytes(value);
                            _result.AddRange(reverse ? bytes.Reverse() : bytes);
                            return null;
                        }

                        value = BitConverter.ToInt32(_data.ToArray(4, reverse), 0);
                        break;
                    default:
                        throw new ArgumentException(
                            $"Атрибут Count у поля {name} типа {type.Name} может принимать только значения 1, 2 и 4");
                }

                return Enum.IsDefined(type, value) ? value : 0;
            }

            if (!_types.TryGetValue(type, out CoreType coreType))
                throw new ArgumentException($"Неподдерживаемый тип {type.Name} у поля {name}");

            switch (coreType)
            {
                case CoreType.Boolean:
                    if (!_toBytes) return BitConverter.ToBoolean(_data.ToArray(1), 0);

                    _result.AddRange(BitConverter.GetBytes((bool) ret));
                    return null;
                case CoreType.String:
                    Encoding encoding;
                    switch (attr.Encoding)
                    {
                        case TextEncoding.Ascii:
                            encoding = Encoding.ASCII;
                            break;
                        case TextEncoding.WinRus:
                            encoding = Encoding.GetEncoding(1251);
                            break;
                        case TextEncoding.DosRus:
                            encoding = Encoding.GetEncoding(866);
                            break;
                        case TextEncoding.Koi8Rus:
                            encoding = Encoding.GetEncoding(20866);
                            break;
                        case TextEncoding.Utf8:
                            encoding = Encoding.UTF8;
                            break;
                        case TextEncoding.Unicode:
                            encoding = Encoding.Unicode;
                            break;
                        case TextEncoding.UnicodeBigEndian:
                            encoding = Encoding.BigEndianUnicode;
                            break;
                        default:
                            encoding = Encoding.UTF8;
                            break;
                    }

                    byte[] val;

                    if (!_toBytes)
                    {
                        val = _data.ToArray(len, attr.Reverse);
                        if (val.All(b => b == 0))
                        {
                            val = Array.Empty<byte>();
                        }

                        return encoding.GetString(val).TrimEnd('\0');
                    }

                    if (ret == null) ret = string.Empty;
                    val = encoding.GetBytes((string) ret);
                    using (IEnumerator<byte> en = val.Length < len
                        ? val.Concat(Enumerable.Repeat<byte>(0x20, len - val.Length)).GetEnumerator()
                        : ((IEnumerable<byte>) val).GetEnumerator())
                        _result.AddRange(en.ToArray(len > 0 ? len : val.Length, attr.Reverse));
                    return null;
                case CoreType.ByteArray:
                    // TODO Есть ли смысл поменять на полноценную работу с массивами? Type.IsArray, Type.GetElementType()
                    if (!_toBytes) return _data.ToArray(len, attr.Reverse);

                    if (ret == null) ret = Array.Empty<byte>();
                    using (IEnumerator<byte> en = ((IEnumerable<byte>) ret).GetEnumerator())
                        _result.AddRange(en.ToArray(len, attr.Reverse));
                    return null;
                case CoreType.Byte:
                    if (!_toBytes) return _data.Take(1).Single();

                    _result.Add((byte) ret);
                    return null;
                case CoreType.SByte:
                    if (!_toBytes) return (sbyte) _data.Take(1).Single();

                    _result.Add((byte) (sbyte) ret);
                    return null;
                case CoreType.Char:
                    if (len == 0) len = 2;
                    if (!_toBytes) return BitConverter.ToChar(_data.ToArray(len, reverse, 2), 0);

                    bytes = BitConverter.GetBytes((char) ret).Take(len);
                    _result.AddRange(reverse ? bytes.Reverse() : bytes);
                    return null;
                case CoreType.Float:
                    if (len < 4) len = 4;
                    if (!_toBytes) return BitConverter.ToSingle(_data.ToArray(len, reverse, 4), 0);

                    bytes = BitConverter.GetBytes((float) ret);
                    _result.AddRange(reverse ? bytes.Reverse() : bytes);
                    return null;
                case CoreType.Double:
                    if (len < 8) len = 8;
                    if (!_toBytes) return BitConverter.ToDouble(_data.ToArray(len, reverse, 8), 0);

                    bytes = BitConverter.GetBytes((double) ret);
                    _result.AddRange(reverse ? bytes.Reverse() : bytes);
                    return null;
                case CoreType.Short:
                    if (len == 0) len = 2;
                    if (!_toBytes) return BitConverter.ToInt16(_data.ToArray(len, reverse, 2), 0);

                    bytes = BitConverter.GetBytes((short) ret).Take(len);
                    _result.AddRange(reverse ? bytes.Reverse() : bytes);
                    return null;
                case CoreType.Int:
                    if (len == 0) len = 4;
                    if (!_toBytes) return BitConverter.ToInt32(_data.ToArray(len, reverse, 4), 0);

                    bytes = BitConverter.GetBytes((int) ret).Take(len);
                    _result.AddRange(reverse ? bytes.Reverse() : bytes);
                    return null;
                case CoreType.Long:
                    if (len == 0) len = 8;
                    if (!_toBytes) return BitConverter.ToInt64(_data.ToArray(len, reverse, 8), 0);

                    bytes = BitConverter.GetBytes((long) ret).Take(len);
                    _result.AddRange(reverse ? bytes.Reverse() : bytes);
                    return null;
                case CoreType.UShort:
                    if (len == 0) len = 2;
                    if (!_toBytes) return BitConverter.ToUInt16(_data.ToArray(len, reverse, 2), 0);

                    bytes = BitConverter.GetBytes((ushort) ret).Take(len);
                    _result.AddRange(reverse ? bytes.Reverse() : bytes);
                    return null;
                case CoreType.UInt:
                    if (len == 0) len = 4;
                    if (!_toBytes) return BitConverter.ToUInt32(_data.ToArray(len, reverse, 4), 0);

                    bytes = BitConverter.GetBytes((uint) ret).Take(len);
                    _result.AddRange(reverse ? bytes.Reverse() : bytes);
                    return null;
                case CoreType.ULong:
                    if (len == 0) len = 8;
                    if (!_toBytes) return BitConverter.ToUInt64(_data.ToArray(len, reverse, 8), 0);

                    bytes = BitConverter.GetBytes((ulong) ret).Take(len);
                    _result.AddRange(reverse ? bytes.Reverse() : bytes);
                    return null;
                default:
                    return default(object);
            }
        }
    }
}