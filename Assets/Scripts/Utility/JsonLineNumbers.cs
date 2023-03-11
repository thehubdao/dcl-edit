using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class JsonLineNumbers : JsonConverter<JsonLineNumbers.JTokenWithLineInfo>
{
    public class JTokenWithLineInfo
    {
        public JToken token;
        public int line = -1;
        public int column = -1;

        public bool HasLineInfo()
        {
            return line >= 0;
        }

        public static implicit operator JToken(JTokenWithLineInfo tokenWithLineInfo)
        {
            return tokenWithLineInfo.token;
        }
    }

    private int lineOffset;

    public JsonLineNumbers(int lineOffset = 0)
    {
        this.lineOffset = lineOffset;
    }


    public override void WriteJson(JsonWriter writer, JTokenWithLineInfo value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override JTokenWithLineInfo ReadJson(JsonReader reader, Type objectType, JTokenWithLineInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = new JTokenWithLineInfo();

        if (reader is JsonTextReader textReader)
        {
            token.line = textReader.LineNumber + lineOffset;
            token.column = textReader.LinePosition;
        }

        token.token = JToken.ReadFrom(reader);

        return token;
    }
}
