using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using UnityEngine;

public class GlbInterpreterSystem
{
    public class GlbFile
    {
        public struct Header
        {
            public UInt32 magic;
            public UInt32 version;
            public UInt32 length;

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

            public UInt32 chunkLength;
            public UInt32 chunkType;
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

            //public string GetDataAsText()
            //{
            //    chunkData.
            //}
        }

        public Header header;
        public Chunk[] chunks;
    }


    public async Task<GlbFile> ReadGlb(string path)
    {
        var stream = new BinaryReader(File.OpenRead(path)); // TODO async???
        var header = ReadHeader(stream);
        var chunks = new List<GlbFile.Chunk>();
        while (stream.BaseStream.Position < stream.BaseStream.Length)
        {
            chunks.Add(ReadChunk(stream));
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

        var fill = (4 - (chunkLength % 4)) % 4;
        stream.ReadBytes((int) fill);

        return new GlbFile.Chunk {chunkLength = chunkLength, chunkType = chunkType, chunkData = chunkData};
    }
}
