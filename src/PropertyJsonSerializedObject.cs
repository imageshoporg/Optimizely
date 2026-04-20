using EPiServer.Core;
using System;
using System.Text.Json;

namespace Imageshop.Optimizely.Plugin
{
    /// <summary>
    /// Abstract class to serialize objects
    /// </summary>
#if NET10_0_OR_GREATER
    public abstract class PropertyJsonSerializedObject<T> : PropertyJsonString where T : class
    {
        protected T _value;

        public override Type PropertyValueType => typeof(T);

        public override object Value
        {
            get
            {
                try
                {
                    var json = Json;

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return null!;
                    }

                    _value = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions() { AllowTrailingCommas = true });

                    return _value!;
                }
                catch (Exception)
                {
                    return null!;
                }
            }
            set
            {
                if (value is T)
                {
                    _value = null;
                    Json = JsonSerializer.Serialize(value);
                    return;
                }

                _value = null;
                base.Value = value;
            }
        }
    }
#else
    public abstract class PropertyJsonSerializedObject<T> : PropertyLongString where T : class
    {
        protected T _value;

        public override Type PropertyValueType => typeof(T);

        public override object Value
        {
            get
            {
                try
                {
                    var value = LongString;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return null!;
                    }

                    _value = JsonSerializer.Deserialize<T>(value, new JsonSerializerOptions() { AllowTrailingCommas = true });

                    return _value!;
                }
                catch (Exception)
                {
                    return null!;
                }
            }
            set
            {
                if (value is T)
                {
                    _value = null;
                    base.Value = JsonSerializer.Serialize(value);
                    return;
                }

                base.Value = value;
            }
        }

        public override object SaveData(PropertyDataCollection properties)
        {
            return LongString;
        }
    }
#endif
}