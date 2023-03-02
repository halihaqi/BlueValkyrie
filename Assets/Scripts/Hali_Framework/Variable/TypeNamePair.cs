using System;

namespace Hali_Framework.Variable
{
    public class TypeNamePair : IEquatable<TypeNamePair>
    {
        private readonly Type _type;
        private readonly string _name;
        
        public TypeNamePair(Type type)
            : this(type, string.Empty)
        {
        }
        
        public TypeNamePair(Type type, string name)
        {
            _type = type ?? throw new Exception("Type is invalid.");
            _name = name ?? string.Empty;
        }

        /// <summary>
        /// 获取类型。
        /// </summary>
        public Type Type => _type;

        /// <summary>
        /// 获取名称。
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// 获取类型和名称的组合值字符串。
        /// </summary>
        /// <returns>类型和名称的组合值字符串。</returns>
        public override string ToString()
        {
            if (_type == null)
                throw new Exception("Type is invalid.");

            string typeName = _type.FullName;
            return string.IsNullOrEmpty(_name) ? typeName : $"{typeName}.{_name}";
        }

        /// <summary>
        /// 获取对象的哈希值。
        /// </summary>
        /// <returns>对象的哈希值。</returns>
        public override int GetHashCode()
        {
            return _type.GetHashCode() ^ _name.GetHashCode();
        }

        /// <summary>
        /// 比较对象是否与自身相等。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns>被比较的对象是否与自身相等。</returns>
        public override bool Equals(object obj)
        {
            return obj is TypeNamePair pair && Equals(pair);
        }

        /// <summary>
        /// 比较对象是否与自身相等。
        /// </summary>
        /// <param name="value">要比较的对象。</param>
        /// <returns>被比较的对象是否与自身相等。</returns>
        public bool Equals(TypeNamePair value)
        {
            return _type == value._type && _name == value._name;
        }

        /// <summary>
        /// 判断两个对象是否相等。
        /// </summary>
        /// <param name="a">值 a。</param>
        /// <param name="b">值 b。</param>
        /// <returns>两个对象是否相等。</returns>
        public static bool operator ==(TypeNamePair a, TypeNamePair b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// 判断两个对象是否不相等。
        /// </summary>
        /// <param name="a">值 a。</param>
        /// <param name="b">值 b。</param>
        /// <returns>两个对象是否不相等。</returns>
        public static bool operator !=(TypeNamePair a, TypeNamePair b)
        {
            return !(a == b);
        }
    }
}