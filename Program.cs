//Written for Tales Runner. https://store.steampowered.com/app/328060
using System.IO;
using System.IO.Compression;

namespace Tales_Runner_Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryReader br = new(File.OpenRead(args[0]));
            br.BaseStream.Position = 32;

            string path = Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//";
            int n = 0;
            Directory.CreateDirectory(path);
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                br.ReadInt32();
                int sizeUncompressed = br.ReadInt32();
                int sizeCompressed = br.ReadInt32();
                br.ReadSingle();
                int isCompressed = br.ReadInt32();

                MemoryStream ms = new();
                if (isCompressed == 1)
                {
                    br.ReadInt16();
                    using var ds = new DeflateStream(new MemoryStream(br.ReadBytes(sizeCompressed - 2)), CompressionMode.Decompress);
                    ds.CopyTo(ms);
                }
                else
                {
                    BinaryWriter bw = new(ms);
                    bw.Write(br.ReadBytes(sizeUncompressed));
                }

                StreamReader sr = new(ms);
                sr.BaseStream.Position = 0;
                char[] magicChars = new char[4];
                sr.ReadBlock(magicChars, 0, 4);
                string magic = new(magicChars);

                string extention = "";
                switch(magic)
                {
                    case "DDS ":
                        extention = ".dds";
                        break;
                }
                FileStream fs = File.Create(path + n + extention);
                ms.CopyTo(fs);
                fs.Close();
                sr.Close();
                ms.Close();
                n++;
            }
        }
    }
}
