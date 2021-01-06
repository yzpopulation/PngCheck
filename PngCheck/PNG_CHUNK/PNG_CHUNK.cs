namespace PngCheck.PNG_CHUNK
{
    public struct PNG_CHUNK
    {
        public uint length;
        public string cname;
        public IPNG_CHUNK chunk;
        public uint crc;
    }
}