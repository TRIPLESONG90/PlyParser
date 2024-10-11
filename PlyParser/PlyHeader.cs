using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlyParser
{
    public interface IPlyProperty
    {
        public string Name { get; }
        public Type Type { get; }
        public string TypeName { get; }
    }

    public enum ElementType
    {
        Vertex,
        Face,
        UserDefined,
    }

    public interface IPlyElement
    {
        public ElementType Type { get; }
        public string Name { get; }
        public int Count { get; }
        public List<IPlyProperty> Properties { get; }
    }

    public enum PlyFormat
    {
        Ascii,
        BinaryLittleEndian,
        BinaryBigEndian
    }

    public class PlyHeader
    {
        public required PlyFormat Format { get; init; }
        public required string Comment { get; init; }
        public required Version Version { get; init; }
        public required List<IPlyElement> Elements { get; init; }
    }
}
