using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
namespace AdventOfCode.Common {
    public static class JsonReader {
        public static JsonObject Read(string json) {
            JsonStream stream = new JsonStream(json);
            while (!stream.EOF) {
                switch (stream.Current) {
                    case '[': return new JsonArray(stream);
                    case '{': return new JsonClass(stream);
                }
                stream.Advance();
            }
            return JsonObject.EMPTY;
        }
        public static JsonObject Read(byte[] json) {
            return Read(Encoding.UTF8.GetString(json));
        }
    }
    internal class JsonStream {
        private string Json;
        public int Position { get; set; }
        public bool EOF { get { return Position >= Json.Length; } }
        public char Current { get { return Position >= Json.Length ? '\0' : Json[Position]; } }
        public char Advance() { return Position >= Json.Length ? '\0' : Json[Position++]; }
        public JsonStream(string json) {
            Json = json;
        }
        public string ReadString() {
            char c = Current, last;
            int start = Position;
            do {
                last = c;
                Position++;
                c = Current;
            } while (!EOF && (c != '"' || last == '\\'));

            return Json.Substring(start + 1, Position - start - 1);
        }
        public string ReadItem(bool isString) {
            int start = Position;
            char c = isString ? Advance() : Current, last;
            do {
                last = c;
                c = Advance();
            } while (!EOF && (c != '"' || last == '\\') && (isString || (c != ']' && c != '}' && c != ',')));

            Position--;
            int extra = isString ? 1 : 0;
            return Json.Substring(start + extra, Position - start - extra).TrimEnd();
        }
        public char SkipToValue() {
            bool foundColon = false;
            char c;
            do {
                Position++;
                c = Current;
                if (c == ':') {
                    if (foundColon) { break; }
                    foundColon = true;
                } else if (!foundColon && !char.IsWhiteSpace(c)) {
                    throw new Exception($"Invalid JSON encountered at position ({Position})");
                }
            } while (!EOF && (char.IsWhiteSpace(c) || c == ':'));

            return c;
        }
        public void SkipToNext() {
            char c;
            do {
                c = Advance();
            } while (!EOF && c != ']' && c != '}' && c != ',');
            Position -= c == ']' || c == '}' ? 2 : 1;
        }
    }
    public class JsonArray : JsonObject {
        private List<JsonObject> Objects { get; set; }
        public int Count { get { return Objects.Count; } }
        public JsonArray() {
            Objects = new List<JsonObject>();
        }
        internal JsonArray(JsonStream stream) {
            Objects = new List<JsonObject>();
            stream.Position++;
            while (!stream.EOF) {
                char c = stream.Current;
                if (char.IsWhiteSpace(c)) { stream.Advance(); continue; }

                switch (c) {
                    case '[': Objects.Add(new JsonArray(stream)); break;
                    case '{': Objects.Add(new JsonClass(stream)); break;
                    case ',': break;
                    case ']': return;
                    default: Objects.Add(new JsonItem(stream)); break;
                }

                stream.Advance();
            }
        }
        public JsonObject this[int index] {
            get { return Objects[index]; }
            set { Objects[index] = value; }
        }
        public void Add(object value, bool isString = false) {
            if (value is JsonObject jObj) {
                Objects.Add(jObj);
            } else {
                Objects.Add(new JsonItem(value == null ? null : $"{value}", isString));
            }
        }
        public override string ToString() { return Serialize(); }
        internal override string Serialize(int indentation = 0) {
            string padding = indentation > 0 ? string.Empty.PadLeft(indentation * 2) : string.Empty;
            StringBuilder values = new StringBuilder();
            values.Append(indentation >= 0 ? "[\n" : "[");

            for (int i = 0; i < Objects.Count; i++) {
                if (indentation >= 0) {
                    values.Append($"{padding}  {Objects[i].Serialize(indentation + 1)}, \n");
                } else {
                    values.Append($"{Objects[i].Serialize(-1)},");
                }
            }

            if (Objects.Count > 0) {
                values.Length -= indentation >= 0 ? 3 : 1;
            }

            values.Append(indentation >= 0 ? $"\n{padding}]" : "]");

            return values.ToString();
        }
        internal override IEnumerator<JsonObject> Enumerate() {
            return Objects.GetEnumerator();
        }
        public T[] Deserialize<T>() {
            T[] array = new T[Count];
            for (int j = 0; j < array.Length; j++) {
                if (this[j] is JsonClass aryClass) {
                    array[j] = aryClass.Deserialize<T>();
                } else if (this[j] is JsonArray aryArray) {
                    array[j] = JsonClass.DeserializeArray<T>(aryArray);
                }
            }
            return array;
        }
    }
    public class JsonClass : JsonObject {
        private Dictionary<string, JsonObject> Fields { get; set; }

        public JsonClass() {
            Fields = new Dictionary<string, JsonObject>();
        }
        internal JsonClass(JsonStream stream) {
            Fields = new Dictionary<string, JsonObject>();
            stream.Position++;
            while (!stream.EOF) {
                char c = stream.Current;
                if (c == '}') { break; }

                if (c == '"') {
                    string field = stream.ReadString();
                    switch (stream.SkipToValue()) {
                        case '{': AddField(field, new JsonClass(stream)); break;
                        case '[': AddField(field, new JsonArray(stream)); break;
                        default: AddField(field, new JsonItem(stream)); break;
                    }
                } else if (c != ',' && !char.IsWhiteSpace(c)) {
                    throw new Exception($"Invalid JSON encountered at position ({stream.Position}).");
                }

                stream.Advance();
            }
        }
        private void AddField(string field, JsonObject value) {
            if (!Fields.ContainsKey(field)) {
                Fields[field] = value;
            }
        }
        public void Add(string field, object value, bool isString = false) {
            if (!Fields.ContainsKey(field)) {
                if (value is JsonObject jObj) {
                    Fields[field] = jObj;
                } else {
                    Fields[field] = new JsonItem(value == null ? null : $"{value}", isString);
                }
            }
        }
        public JsonObject this[string key] {
            get { return Fields.TryGetValue(key, out JsonObject value) ? value : JsonObject.EMPTY; }
        }
        public override string ToString() { return Serialize(); }
        internal override string Serialize(int indentation = 0) {
            StringBuilder values = new StringBuilder();
            if (Fields.Count > 0) {
                string padding = indentation >= 0 ? string.Empty.PadLeft(indentation * 2) : string.Empty;
                values.Append(indentation >= 0 ? "{\n" : "{");

                foreach (KeyValuePair<string, JsonObject> pair in Fields) {
                    if (indentation >= 0) {
                        values.Append($"{padding}  \"{pair.Key}\": {pair.Value.Serialize(indentation + 1)}, \n");
                    } else {
                        values.Append($"\"{pair.Key}\":{pair.Value.Serialize(-1)},");
                    }
                }

                if (Fields.Count > 0) {
                    values.Length -= indentation >= 0 ? 3 : 1;
                }

                values.Append(indentation >= 0 ? $"\n{padding}}}" : "}");
            } else {
                values.Append("{}");
            }
            return values.ToString();
        }
        internal override IEnumerator<JsonObject> Enumerate() {
            return Fields.Values.GetEnumerator();
        }
        public T Deserialize<T>() {
            object boxed = default(T);
            PropertyInfo[] properties = AttributeCache<T>.Properties;
            JsonFieldAttribute[] names = AttributeCache<T>.FieldNames;
            MethodInfo[] deserialize = AttributeCache<T>.Deserialize;
            MethodInfo[] deserializeArray = AttributeCache<T>.DeserializeArray;
            for (int i = 0; i < properties.Length; i++) {
                JsonFieldAttribute jsonField = names[i];
                PropertyInfo property = properties[i];
                string name = jsonField != null ? jsonField.Name : property.Name;
                DeserializeObject(boxed, this[name], property, deserialize[i], deserializeArray[i]);
            }
            return (T)boxed;
        }
        internal static T DeserializeArray<T>(JsonArray array) {
            object boxed = default(T);
            PropertyInfo[] properties = AttributeCache<T>.Properties;
            JsonFieldAttribute[] names = AttributeCache<T>.FieldNames;
            MethodInfo[] deserialize = AttributeCache<T>.Deserialize;
            MethodInfo[] deserializeArray = AttributeCache<T>.DeserializeArray;
            for (int i = 0; i < properties.Length; i++) {
                JsonFieldAttribute jsonField = names[i];
                if (jsonField != null && int.TryParse(jsonField.Name, out int index)) {
                    DeserializeObject(boxed, array[index], properties[i], deserialize[i], deserializeArray[i]);
                }
            }
            return (T)boxed;
        }
        private static void DeserializeObject(object boxed, JsonObject obj, PropertyInfo property, MethodInfo deserialize, MethodInfo deserializeArray) {
            if (obj is JsonItem) {
                property.SetValue(boxed, JsonObject.GetValue(obj, property.PropertyType));
            } else if (obj is JsonClass objClass) {
                property.SetValue(boxed, deserialize.Invoke(objClass, null));
            } else if (obj is JsonArray objArray) {
                Array subArray = Array.CreateInstance(property.PropertyType.GetElementType(), objArray.Count);
                for (int j = 0; j < subArray.Length; j++) {
                    if (objArray[j] is JsonClass aryClass) {
                        subArray.SetValue(deserialize.Invoke(aryClass, null), j);
                    } else if (objArray[j] is JsonArray aryArray) {
                        subArray.SetValue(deserializeArray.Invoke(null, new object[] { aryArray }), j);
                    }
                }
                property.SetValue(boxed, subArray);
            }
        }
        private static class AttributeCache<T> {
            public static readonly PropertyInfo[] Properties;
            public static readonly JsonFieldAttribute[] FieldNames;
            public static readonly MethodInfo[] Deserialize;
            public static readonly MethodInfo[] DeserializeArray;

            static AttributeCache() {
                Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                FieldNames = new JsonFieldAttribute[Properties.Length];
                Deserialize = new MethodInfo[Properties.Length];
                DeserializeArray = new MethodInfo[Properties.Length];
                for (int i = 0; i < Properties.Length; i++) {
                    PropertyInfo property = Properties[i];
                    FieldNames[i] = (JsonFieldAttribute)property.GetCustomAttribute(typeof(JsonFieldAttribute), false);
                    if (property.PropertyType.IsArray) {
                        Deserialize[i] = typeof(JsonClass).GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(property.PropertyType.GetElementType());
                        DeserializeArray[i] = typeof(JsonClass).GetMethod("DeserializeArray", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(property.PropertyType.GetElementType());
                    } else {
                        Deserialize[i] = typeof(JsonClass).GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(property.PropertyType);
                        DeserializeArray[i] = typeof(JsonClass).GetMethod("DeserializeArray", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(property.PropertyType);
                    }
                }
            }
        }
    }
    public class JsonItem : JsonObject {
        public string Item { get; set; }
        public bool IsString { get; set; }

        public JsonItem(string value, bool isString = false) {
            Item = value;
            IsString = isString;
        }
        internal JsonItem(JsonStream stream) {
            IsString = stream.Current == '"';
            Item = stream.ReadItem(IsString);
            if (Item == "null") { Item = null; }
            stream.SkipToNext();
        }
        public override string ToString() { return Item; }
        internal override string Serialize(int level) { return Item == null ? "null" : IsString ? $"\"{Item}\"" : Item; }
    }
    public abstract class JsonObject : IEnumerable<JsonObject> {
        public static JsonObject EMPTY = new JsonItem(null, false);
        internal virtual string Serialize(int indentation = 0) { return string.Empty; }
        public override string ToString() { return string.Empty; }
        internal virtual IEnumerator<JsonObject> Enumerate() { yield return this; }
        public IEnumerator<JsonObject> GetEnumerator() { return Enumerate(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public static implicit operator string(JsonObject obj) {
            return obj.ToString();
        }
        public int AsInt(int defaultValue = default) {
            string value = ToString();
            return !string.IsNullOrEmpty(value) && int.TryParse(value, out int intValue) ? intValue : defaultValue;
        }
        internal static object GetValue(JsonObject obj, Type type) {
            string value = obj.ToString();
            if (string.IsNullOrEmpty(value)) { return null; }

            return TypeDescriptor.GetConverter(type).ConvertFromString(value);
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonFieldAttribute : Attribute {
        public string Name { get; set; }
        public JsonFieldAttribute(string name) {
            Name = name;
        }
    }
}