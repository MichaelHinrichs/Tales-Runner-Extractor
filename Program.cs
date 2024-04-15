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

            int n = 0;
            Directory.CreateDirectory(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]));
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                br.ReadInt32();
                int sizeUncompressed = br.ReadInt32();
                int sizeCompressed = br.ReadInt32();
                br.ReadSingle();
                int isCompressed = br.ReadInt32();

                if (isCompressed == 1)
                {
                    MemoryStream ms = new();
                    br.ReadInt16();
                    using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes(sizeCompressed - 2)), CompressionMode.Decompress))
                        ds.CopyTo(File.Create(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + n));
                    n++;
                    continue;
                }
                BinaryWriter bw = new(File.Create(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + n));
                bw.Write(br.ReadBytes(sizeUncompressed));
                bw.Close();
                n++;
            }
        }
    }
}
