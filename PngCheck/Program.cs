using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Force.Crc32;
using Kermalis.EndianBinaryIO;
using PngCheck.PNG_CHUNK;
using PngCheck.PNG_CHUNK_ENUM;

//ushort=UInt16
//uint=UInt32

namespace PngCheck
{
    internal partial class Program
    {


  

   

 


        private static void Main(string[] args)
        {
            var bytes = File.ReadAllBytes("C:\\Users\\admin\\Desktop\\img101-fs8.png");

            var r = new EndianBinaryReader(new MemoryStream(bytes), Endianness.BigEndian);
            var signs = r.ReadChars(8, true);
            var current = CHUNK(r);
            current = CHUNK(r);
            current = CHUNK(r);
            current = CHUNK(r);
            current = CHUNK(r);
            current = CHUNK(r); current = CHUNK(r); current = CHUNK(r); current = CHUNK(r);
        }

        public static IPNG_CHUNK CHUNK_I(string cname, byte[] bytes)
        {
            IPNG_CHUNK it = null;
            switch (cname.ToUpper())
            {
                case "IHDR":
                {
                    using var reader = new EndianBinaryReader(new MemoryStream(bytes), Endianness.BigEndian);
                    var item = new PNG_CHUNK_IHDR();
                    item.width = reader.ReadUInt32();
                    item.height = reader.ReadUInt32();
                    item.bits = reader.ReadByte();
                    item.color_type = reader.ReadEnum<PNG_COLOR_SPACE_TYPE>();
                    item.compr_method = reader.ReadEnum<PNG_COMPR_METHOD>();
                    item.filter_method = reader.ReadEnum<PNG_FILTER_METHOD>();
                    item.interlace_method = reader.ReadEnum<PNG_INTERLACE_METHOD>();
                    it = item;
                }
                    break;
                case "CHRM":
                {
                    using var reader = new EndianBinaryReader(new MemoryStream(bytes), Endianness.BigEndian);
                    var item = new PNG_CHUNK_CHRM();
                    item.white.X=(int) reader.ReadUInt32();
                    item.white.Y=(int) reader.ReadUInt32(); 
                    item.red.X=(int) reader.ReadUInt32();
                    item.red.Y=(int) reader.ReadUInt32();    
                    item.green.X=(int) reader.ReadUInt32();
                    item.green.Y=(int) reader.ReadUInt32(); 
                    item.blue.X=(int) reader.ReadUInt32();
                    item.blue.Y=(int) reader.ReadUInt32();
                    it = item;
                }
                    break;
                case "PHYS":
                {
                    using var reader = new EndianBinaryReader(new MemoryStream(bytes), Endianness.BigEndian);
                    var item = new PNG_CHUNK_PHYS();
                    item.physPixelPerUnitX = reader.ReadUInt32();
                    item.physPixelPerUnitY = reader.ReadUInt32();
                    item.physUnitSpec = reader.ReadEnum<PNG_PHYS_UNITSPEC>();
                    it = item;
                }
                    break;
                case "IEND":
                {
                    var item = new PNG_CHUNK_IEND();
                    it = item;
                }
                    break;
                case "IDAT":
                {
                    var item = new PNG_CHUNK_IDAT();
                    item.bytes = bytes;
                    it = item;
                }
                    break;
                case "PLTE":
                {
                    using var reader = new EndianBinaryReader(new MemoryStream(bytes), Endianness.BigEndian);
                    var item = new PNG_CHUNK_PLTE();
                    item.plteChunkData = new PNG_PALETTE_PIXEL[bytes.Length / 3];
                    for (int i = 0; i < item.plteChunkData.Length; i++)
                    {
                        item.plteChunkData[i].btRed = reader.ReadByte();
                        item.plteChunkData[i].btGreen = reader.ReadByte();
                        item.plteChunkData[i].btBlue = reader.ReadByte();
                    }
                    it = item;
                }
                    break;
            }

            return it;
        }


        public static PNG_CHUNK.PNG_CHUNK? CHUNK(EndianBinaryReader r)
        {
            if (r.BaseStream.Position >= r.BaseStream.Length) return null;
            var chunk = new PNG_CHUNK.PNG_CHUNK();
            chunk.length = r.ReadUInt32();
            chunk.cname = r.ReadString(4, true);
            var dataBytes = r.ReadBytes((int) chunk.length);
            chunk.chunk = CHUNK_I(chunk.cname, dataBytes);
            r.BaseStream.Position -= chunk.length + 4;
            var crcData = r.ReadBytes((int) chunk.length + 4);
            var crcCompute = Crc32Algorithm.Compute(crcData);
            var crc = r.ReadUInt32();
            if (crcCompute != crc) throw new Exception("crc check error !");
            chunk.crc = crc;
            return chunk;
        }



 



 
   
   

   
    }
}