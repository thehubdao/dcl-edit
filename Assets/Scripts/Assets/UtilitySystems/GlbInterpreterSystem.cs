using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class GlbInterpreterSystem
{
    public class GlbFile
    {
        public struct Header
        {
            public uint magic;
            public uint version;
            public uint length;

            public bool Validate()
            {
                return magic == 0x46546C67 && version is 2 or 1;
            }
        }

        public struct Chunk
        {
            public enum Type
            {
                Json,
                Bin,
                Unknown
            }

            public uint chunkLength;
            public uint chunkType;
            public byte[] chunkData;

            public Type GetChunkType()
            {
                return chunkType switch
                {
                    0x4E4F534A => Type.Json,
                    0x004E4942 => Type.Bin,
                    _ => Type.Unknown
                };
            }
        }

        public Header header;
        public Chunk[] chunks;

        public GltfJson GetGltfJson()
        {
            var fistChunk = chunks[0];
            Assert.IsTrue(fistChunk.GetChunkType() == Chunk.Type.Json, "Can not get json from a non json chunk");
            return new GltfJson(JObject.Parse(Encoding.UTF8.GetString(fistChunk.chunkData)));
        }

        public void SetGltfJson(GltfJson value)
        {
            var bytes = Encoding.UTF8.GetBytes(value.data.ToString(Formatting.None));
            byte[] alignedBytes;

            var fill = StaticUtils.FillToAlignment((uint) bytes.Length);
            if (fill > 0)
            {
                alignedBytes = new byte[bytes.Length + fill];
                bytes.CopyTo(alignedBytes, 0);
            }
            else
            {
                alignedBytes = bytes;
            }

            chunks[0].chunkData = alignedBytes;
            chunks[0].chunkLength = (uint) alignedBytes.Length;

            header.length = (uint) (12 + chunks.Sum(c => c.chunkData.Length + 8));
        }

        public byte[] GetAllBytes()
        {
            var bytes = new byte[header.length];
            var stream = new BinaryWriter(new MemoryStream(bytes));

            stream.Write(header.magic);
            stream.Write(header.version);
            stream.Write(header.length);

            foreach (var chunk in chunks)
            {
                stream.Write(chunk.chunkLength);
                stream.Write(chunk.chunkType);
                stream.Write(chunk.chunkData);
            }

            return bytes;
        }
    }

    public class GltfJson
    {
        public JObject data;

        public GltfJson(JObject data)
        {
            this.data = data;
        }

        public List<JToken> FindUriValues(JToken token = null)
        {
            var retVal = new List<JToken>();
            token ??= data;

            if (token.Type == JTokenType.Object)
            {
                foreach (var property in token.Children<JProperty>())
                {
                    if (property.Name == "uri" && property.Value.Type == JTokenType.String)
                    {
                        //string uriValue = property.Value.ToString();

                        retVal.Add(property.Value);
                    }
                    else
                    {
                        retVal.AddRange(FindUriValues(property.Value));
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var arrayItem in token.Children())
                {
                    retVal.AddRange(FindUriValues(arrayItem));
                }
            }
            else if (token.Type == JTokenType.Property)
            {
                retVal.AddRange(FindUriValues(((JProperty) token).Value));
            }

            return retVal;
        }
    }

    public GlbFile ReadGlb(string path)
    {
        return ReadGlb(new BinaryReader(File.OpenRead(path)));
    }

    public GlbFile ReadGlb(byte[] data)
    {
        return ReadGlb(new BinaryReader(new MemoryStream(data)));
    }

    public GlbFile ReadGlb(BinaryReader reader)
    {
        var header = ReadHeader(reader);
        var chunks = new List<GlbFile.Chunk>();
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            chunks.Add(ReadChunk(reader));
        }

        return new GlbFile {header = header, chunks = chunks.ToArray()};
    }

    private GlbFile.Header ReadHeader(BinaryReader stream)
    {
        var headerMagic = stream.ReadUInt32();
        var headerVersion = stream.ReadUInt32();
        var headerLength = stream.ReadUInt32();
        return new GlbFile.Header {magic = headerMagic, version = headerVersion, length = headerLength};
    }

    private GlbFile.Chunk ReadChunk(BinaryReader stream)
    {
        var chunkLength = stream.ReadUInt32();
        var chunkType = stream.ReadUInt32();
        var chunkData = stream.ReadBytes((int) chunkLength);

        var fill = StaticUtils.FillToAlignment(chunkLength);
        stream.ReadBytes((int) fill);

        return new GlbFile.Chunk {chunkLength = chunkLength, chunkType = chunkType, chunkData = chunkData};
    }
}
