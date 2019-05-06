using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Random = UnityEngine.Random;

namespace Assets.Utils
{
    public abstract class Number<T> where T : struct
    {
        public abstract T GetNumber();

        public static implicit operator Number<T>(T exact)
        {
            return new ExactNumber<T>(exact);
        }

        public static implicit operator Number<T>(T[] range)
        {
            return new RangeNumber<T>(range[0], range[1]);
        }
    }

    public class ExactNumber<T> : Number<T> where T : struct
    {
        private T _number;

        public ExactNumber(T number)
        {
            _number = number;
        }

        public override T GetNumber()
        {
            return _number;
        }
    }

    public class RangeNumber<T> : Number<T> where T : struct
    {
        private T _min;
        private T _max;

        public RangeNumber(T min, T max)
        {
            _min = min;
            _max = max;
        }

        public override T GetNumber()
        {
            if (typeof(T) == typeof(float))
                return (T)(object)Random.Range((float)(object)_min, (float)(object)_max);

            if (typeof(T) == typeof(int))
                return (T)(object)Random.Range((int)(object)_min, (int)(object)_max);

            throw new Exception("Number must be of type int or float.");
        }
    }

    public class NumberConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Number<float>) ||
                   objectType == typeof(Number<int>);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var arr = JArray.Load(reader);

                if (objectType == typeof(Number<float>))
                {
                    return new RangeNumber<float>(arr[0].ToObject<float>(), arr[1].ToObject<float>());
                }

                if (objectType == typeof(Number<int>))
                {
                    return new RangeNumber<int>(arr[0].ToObject<int>(), arr[1].ToObject<int>());
                }

                throw new NotSupportedException("Value was an array but cannot be converted to desired type.");
            }
            else
            {
                var obj = reader.Value;
                var valueType = obj.GetType();

                if (objectType == typeof(Number<float>))
                {
                    return new ExactNumber<float>(Convert.ToSingle(obj));
                }

                if (objectType == typeof(Number<int>))
                {
                    return new ExactNumber<int>(Convert.ToInt32(obj));
                }

                throw new NotSupportedException("Value " + obj + " of type " + valueType + " cannot be converted to Number<T>");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
