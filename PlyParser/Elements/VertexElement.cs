using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlyParser.Elements
{
    enum VertexPropertyName
    {
        x,
        y,
        z,
        nx,
        ny,
        nz,
        s,
        t,
        u,
        v,
        red,
        blue,
        green,
        alpha
    }
    class VertexProperty : IPlyProperty
    {
        public string Name => vertexPropertyName.ToString().ToLower();
        public required VertexPropertyName vertexPropertyName { get; init; }
        public required string TypeName { get; init; }
        public required Type Type { get; init; }

        public override string ToString()
        {
            return $"property {TypeName} {Name}";
        }
    }

    //element vertex 2332800
    //property float x
    //property float y
    //property float z
    //property uchar red
    //property uchar green
    //property uchar blue
    class VertexElement : IPlyElement
    {
        public ElementType Type => ElementType.Vertex;
        public string Name => "vertex";
        public int Count { get; private set; }
        public List<IPlyProperty> Properties { get; private set; }

        public VertexElement(string elemString, List<string> propertyStrings)
        {
            if (elemString.Split(' ')[0] != "element")
                throw new Exception("Invalid element string");
            if (elemString.Split(' ')[1] != "vertex")
                throw new Exception("Invalid element string");
            if (!int.TryParse(elemString.Split(' ')[2], out int count))
                throw new Exception("Invalid element string");

            Count = count;

            Properties = new List<IPlyProperty>();
            foreach (var propertyString in propertyStrings)
            {
                var property = GetProperty(propertyString);
                Properties.Add(property);
            }
        }

        private VertexProperty GetProperty(string propertyString)
        {
            var strType = propertyString.Split(' ')[1];
            var strName = propertyString.Split(' ')[2];

            return new VertexProperty
            {
                vertexPropertyName = Enum.Parse<VertexPropertyName>(strName.ToLower()),
                Type = strType switch
                {
                    "float" => typeof(float),
                    "uchar" => typeof(byte),
                    _ => throw new Exception($"Invalid property type {strType}")
                },
                TypeName = strType
            };
        }

        public override string ToString()
        {
            return $"element {Name} {Count}";
        }
    }
}
