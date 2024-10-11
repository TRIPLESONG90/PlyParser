using PlyParser.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace PlyParser
{
    public class PlyFile : IDisposable
    {
        byte[] bytes;
        StreamReader reader;
        MemoryStream stream;
        PlyHeader header;
        List<string> headers;
        public PlyHeader Header { get; init; }
        public PlyFile(string filePath)
        {
            bytes = File.ReadAllBytes(filePath);
            stream = new MemoryStream(bytes);
            reader = new StreamReader(stream);
            var headerLines = GetHeaderLines();
            Header = GetHeader(headerLines);
        }


        private PlyHeader GetHeader(IEnumerable<string> headerLines)
        {
            PlyFormat? format = null;
            string? comment = null;
            Version? version = null;
            var elementStrings = new Dictionary<string, List<string>>();
            var elementKey = string.Empty;
            foreach (var lineWithIndex in headerLines.Select((x, i) => new { line = x, Index = i }))
            {
                var line = lineWithIndex.line;
                var index = lineWithIndex.Index;

                var key = line.Split(' ')[0];
                if (index == 0 && key != "ply")
                    throw new Exception("Invalid ply file");

                switch (key)
                {
                    case "ply":
                        break;
                    case "format":
                        var formatString = line.Split(' ')[1];
                        format = GetPlyFormat(formatString);

                        var versionString = line.Split(' ')[2];
                        version = new Version(versionString);
                        break;
                    case "comment":
                        if (comment != null)
                            comment += Environment.NewLine;
                        comment += line.Substring(8);
                        break;
                    case "element":
                        elementStrings.Add(line, new());
                        elementKey = line;
                        break;
                    case "property":
                        elementStrings[elementKey].Add(line);
                        break;
                }
            }

            var elements = new List<IPlyElement>();
            foreach (var elementString in elementStrings)
            {
                var elementName = elementString.Key.Split(' ')[1];
                switch (elementName)
                {
                    case "vertex":
                        elements.Add(new VertexElement(elementString.Key, elementString.Value));
                        break;
                        //case "face":
                        //    elements.Add(new FaceElement(elementName, elementString.Value));
                        //    break;
                        //default:
                        //    elements.Add(new UserDefinedElement(elementName, elementString.Value));
                        //    break;
                }
            }
            if (format == null || comment == null || version == null)
                throw new Exception("Invalid ply file");

            return new PlyHeader()
            {
                Format = format.Value,
                Comment = comment,
                Version = version,
                Elements = elements
            };
        }

        private PlyFormat GetPlyFormat(string formatString)
        {
            PlyFormat result;
            if (formatString == "ascii")
                result = PlyFormat.Ascii;
            else if (formatString == "binary_little_endian")
                result = PlyFormat.BinaryLittleEndian;
            else if (formatString == "binary_big_endian")
                result = PlyFormat.BinaryBigEndian;
            else
                throw new Exception("Invalid ply file");
            return result;
        }

        private IEnumerable<string> GetHeaderLines()
        {
            var headerLines = new List<string>();
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                    break;
                yield return line;
                if (line == "end_header")
                    break;
            }
        }
        private void ResetStream()
        {
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
        }
        public void Dispose()
        {
            if (reader != null)
                reader.Dispose();
            if (stream != null)
                stream.Dispose();
            if (bytes != null)
                bytes = null;
        }
    }
}
