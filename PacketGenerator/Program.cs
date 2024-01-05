using System.Xml;

namespace PacketGenerator;

class Program
{
    static void Main(string[] args)
    {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
            IgnoreComments = true,
            IgnoreWhitespace = true
        };

        //using을 사용하면 이 범위를 벗어날때 알아서 r.dispose를 호출해줌
        using (XmlReader r = XmlReader.Create("PDL.xml", settings))
        {
            r.MoveToContent(); //헤더 같은거 건너뛰고 바로 <packet ~>로 감

            while (r.Read())
            {
                if (r.Depth == 1 && r.NodeType==XmlNodeType.Element) // element는 정보 시작하는 부분
                    ParsePacket(r);
                //Console.WriteLine(r.Name+" "+r["name"]);                
            }
        }
    }

    public static void ParsePacket(XmlReader r)
    {
        if (r.NodeType == XmlNodeType.EndElement)
            return;

        if (r.Name.ToLower() != "packet")
        {
            Console.WriteLine("Invalid packet node");
            return;
        }

        string packetName = r["name"];
        if (string.IsNullOrEmpty(packetName))
        {
            Console.WriteLine("Packet without name");
            return;
        }
        
        ParseMembers(r);
    }

    public static void ParseMembers(XmlReader r)
    {
        string packetName = r["name"];

        int depth = r.Depth + 1; //<packet>다음 부분의 뎁스
        while (r.Read())
        {
            if (r.Depth != depth)
                break;

            string memberName = r["name"];
            if (string.IsNullOrEmpty(memberName))
            {
                Console.WriteLine("Member without name");
                return;
            }

            string memberType = r.Name.ToLower();
            switch (memberType)
            {
                case "bool":
                case "byte":
                case "short":
                case "ushort":
                case "int":
                case "long":
                case "float":
                case "double":
                case "string":
                case "list":
                    break;
                default:
                    break;
            }
            
        }
    }
}

