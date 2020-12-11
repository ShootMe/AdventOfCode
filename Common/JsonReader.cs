using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace AdventOfCode.Common {
    public static class JsonReader {
        public static JsonObject Read(string json) {
            for (int i = 0; i < json.Length; i++) {
                char c = json[i];
                if (c == '[') {
                    return new JsonArray(null, json, ref i);
                } else if (c == '{') {
                    return new JsonClass(null, json, ref i);
                }
            }
            return null;
        }
    }
    public class JsonArray : JsonObject {
        public List<JsonObject> Objects { get; private set; }

        internal JsonArray(JsonObject parent, string json, ref int index) {
            Parent = parent;
            Objects = new List<JsonObject>();
            for (int i = index + 1; i < json.Length; i++) {
                char c = json[i];
                if (char.IsWhiteSpace(c)) { continue; }

                if (c == '[') {
                    Objects.Add(new JsonArray(this, json, ref i));
                } else if (c == '{') {
                    Objects.Add(new JsonClass(this, json, ref i));
                } else if (c == ']') {
                    index = i;
                    break;
                } else if (c != ',') {
                    Objects.Add(new JsonItem(this, json, ref i));
                }
            }
        }
        public override string Value() { return ToString(); }
        public override string ToString() { return ToString(0); }
        internal override string ToString(int level) {
            string padding = string.Empty.PadLeft(level * 4);
            StringBuilder values = new StringBuilder();
            values.AppendLine("[");
            for (int i = 0; i < Objects.Count; i++) {
                values.Append(padding).Append("    ").Append($"{Objects[i].ToString(level + 1)}, ");
                if (i + 1 < Objects.Count) {
                    values.AppendLine();
                }
            }
            if (values.Length > 1) {
                values.Length -= 2;
            }
            values.AppendLine().Append(padding).Append(']');
            return values.ToString();
        }
        internal override IEnumerator<JsonObject> Enumerate() {
            for (int i = 0; i < Objects.Count; i++) {
                IEnumerator<JsonObject> e = Objects[i].Enumerate();
                while (e.MoveNext()) {
                    yield return e.Current;
                }
            }
        }
    }
    public class JsonClass : JsonObject {
        public List<string> Fields { get; private set; }
        public List<JsonObject> Values { get; private set; }

        internal JsonClass(JsonObject parent, string json, ref int index) {
            Parent = parent;
            Fields = new List<string>();
            Values = new List<JsonObject>();
            for (int i = index + 1; i < json.Length; i++) {
                char c = json[i];
                if (c == '"') {
                    Fields.Add(ReadField(json, ref i));
                    bool foundColon = false;
                    do {
                        c = json[++i];
                        if (c == ':') {
                            if (foundColon) { break; }
                            foundColon = true;
                        }
                    } while (char.IsWhiteSpace(c) || c == ':');

                    if (c == '{') {
                        Values.Add(new JsonClass(this, json, ref i));
                    } else if (c == '[') {
                        Values.Add(new JsonArray(this, json, ref i));
                    } else {
                        Values.Add(new JsonItem(this, json, ref i));
                    }
                } else if (c == '}') {
                    index = i;
                    break;
                }
            }
        }
        private string ReadField(string json, ref int index) {
            int start = index;
            char c = json[index], last;
            do {
                last = c;
                c = json[++index];
            } while (c != '"' || last == '\\');
            return json.Substring(start + 1, index - start - 1);
        }
        public override string Value() { return ToString(); }
        public override string ToString() { return ToString(0); }
        internal override string ToString(int level) {
            string padding = string.Empty.PadLeft(level * 4);
            StringBuilder values = new StringBuilder();
            values.AppendLine("{");
            for (int i = 0; i < Fields.Count; i++) {
                values.Append(padding).Append("    ").Append($"\"{Fields[i]}\": {Values[i].ToString(level + 1)}, ");
                if (i + 1 < Fields.Count) {
                    values.AppendLine();
                }
            }
            if (values.Length > 1) {
                values.Length -= 2;
            }
            values.AppendLine().Append(padding).Append('}');
            return values.ToString();
        }
        internal override IEnumerator<JsonObject> Enumerate() {
            for (int i = 0; i < Values.Count; i++) {
                IEnumerator<JsonObject> e = Values[i].Enumerate();
                while (e.MoveNext()) {
                    yield return e.Current;
                }
            }
        }
    }
    public class JsonItem : JsonObject {
        public string Item { get; set; }

        internal JsonItem(JsonObject parent, string json, ref int index) {
            Parent = parent;
            char last = '\0';
            for (int i = index + 1; i < json.Length; i++) {
                char c = json[i];
                if (c == ']' || c == '}' || (c == '"' && last != '\\') || c == ',') {
                    Item = c == '"' ? json.Substring(index + 1, i - index - 1) : json.Substring(index, i - index).Trim();
                    if (c == '"') {
                        do {
                            c = json[++i];
                        } while (c != ']' && c != '}' && c != ',');
                    }
                    index = c == ']' || c == '}' ? i - 1 : i;
                    break;
                }
                last = c;
            }
        }
        public override string Value() { return Item; }
        public override string ToString() { return Item; }
        internal override string ToString(int level) { return Item; }
    }
    public abstract class JsonObject : IEnumerable<JsonObject> {
        public JsonObject Parent { get; set; }
        public virtual string Value() { return string.Empty; }
        internal virtual string ToString(int level) { return string.Empty; }
        public override string ToString() { return string.Empty; }
        internal virtual IEnumerator<JsonObject> Enumerate() { yield return this; }
        public IEnumerator<JsonObject> GetEnumerator() { return Enumerate(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}