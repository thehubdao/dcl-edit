using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
        public string path;
        public string lineText;

        public bool HasLineInfo()
        {
            return line >= 0;
        }

        public static implicit operator JToken(JTokenWithLineInfo tokenWithLineInfo)
        {
            return tokenWithLineInfo.token;
        }
    }

    private string[] completeJsonTextLines;
    private string filePath;

    public JsonLineNumbers(string completeJsonText, string filePath)
    {
        completeJsonTextLines = Regex.Split(completeJsonText, "\r\n|\r|\n", RegexOptions.None);
        this.filePath = filePath;
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
            token.line = textReader.LineNumber;
            token.column = textReader.LinePosition;
            token.path = filePath;
            token.lineText = completeJsonTextLines[textReader.LineNumber - 1];
        }

        token.token = JToken.ReadFrom(reader);

        return token;
    }
}
