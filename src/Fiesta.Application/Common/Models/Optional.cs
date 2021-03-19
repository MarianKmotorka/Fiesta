using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Fiesta.Application.Common.Models
{
    public interface IOptional
    {
        bool HasValue { get; }
        object Value { get; }
    }

    [JsonConverter(typeof(OptionalJsonConverter))]
    [DebuggerDisplay("HasValue = {HasValue}, Value = {Value}")]
    public struct Optional<T> : IOptional
    {
        public Optional(T value)
        {
            Value = value;
            HasValue = true;
        }

        public bool HasValue { get; }

        public T Value { get; }

        object IOptional.Value => Value;

        public static explicit operator T(Optional<T> optional)
        {
            if (optional.HasValue)
            {
                throw new InvalidCastException("Value is not set.");
            }

            return optional.Value;
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static bool operator !=(Optional<T> a, Optional<T> b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Optional<T> a, Optional<T> b)
        {
            return a.Equals(b);
        }

        public bool Equals(Optional<T> other)
        {
            return HasValue == other.HasValue && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is Optional<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HasValue && Value != null ? Value.GetHashCode() : 0;
        }

        public bool TryGetValue(out T value)
        {
            value = Value;
            return HasValue;
        }
    }

    public class OptionalJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Optional<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var innerValue = serializer.Deserialize(reader, objectType.GetGenericArguments().Single());
            return Activator.CreateInstance(objectType, innerValue);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var innerValue = value.GetType().GetProperty("Value").GetValue(value);
            serializer.Serialize(writer, innerValue);
        }
    }

    public class OptionalContractResolver : CamelCasePropertyNamesContractResolver
    {

        public static OptionalContractResolver Instance { get; } = new OptionalContractResolver();

        public static OptionalContractResolver CreateReplacement(IContractResolver original)
        {
            if (original is DefaultContractResolver defaultContractResolver)
                return new OptionalContractResolver()
                {
                    IgnoreIsSpecifiedMembers = defaultContractResolver.IgnoreIsSpecifiedMembers,
                    IgnoreSerializableAttribute = defaultContractResolver.IgnoreSerializableInterface,
                    IgnoreSerializableInterface = defaultContractResolver.IgnoreSerializableInterface,
                    IgnoreShouldSerializeMembers = defaultContractResolver.IgnoreShouldSerializeMembers,
                    NamingStrategy = defaultContractResolver.NamingStrategy,
                    SerializeCompilerGeneratedMembers = defaultContractResolver.SerializeCompilerGeneratedMembers
                };
            else
                return Instance;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Optional<>))
            {
                property.DefaultValue = Activator.CreateInstance(property.PropertyType);
                property.ShouldSerialize =
                    instance =>
                    {
                        var value = property.ValueProvider.GetValue(instance);
                        return !value.Equals(property.DefaultValue);
                    };
            }

            return property;
        }
    }
}
