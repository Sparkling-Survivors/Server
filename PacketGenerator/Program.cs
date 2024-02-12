using System.Diagnostics;
using System.Xml;

namespace PacketGenerator;

class Program
{
    static string clientRegister;
    static string serverRegister;
    static string dedicatedServerRegister;

    static void Main(string[] args)
    {
        string file = "../../../Common/protoc-3.12.3-win64/bin/Protocol.proto";
        if (args.Length >= 1)
            file = args[0];

        bool startParsing = false;
        foreach (string line in File.ReadAllLines(file))
        {
            if (!startParsing && line.Contains("enum MsgId"))
            {
                startParsing = true;
                continue;
            }

            if (!startParsing)
                continue;

            if (line.Contains("}"))
                break;

            string[] names = line.Trim().Split(" =");
            if (names.Length == 0)
                continue;

            string name = names[0];
            if (name.StartsWith("SC_")) //방 서버->클라이언트용
            {
                string[] words = name.Split("_");

                string msgName = "";
                foreach (string word in words)
                    msgName += FirstCharToUpper(word);

                string packetName = $"SC_{msgName.Substring(2)}";
                clientRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
            }
            else if (name.StartsWith("DSC_")) //데디케이티드 서버 -> 클라이언트 용
            {
                string[] words = name.Split("_");

                string msgName = "";
                foreach (string word in words)
                    msgName += FirstCharToUpper(word);

                string packetName = $"DSC_{msgName.Substring(3)}";
                clientRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
            }
            else if (name.StartsWith("CS_")) //클라이언트 -> 방 서버 용
            {
                string[] words = name.Split("_");

                string msgName = "";
                foreach (string word in words)
                    msgName += FirstCharToUpper(word);

                string packetName = $"CS_{msgName.Substring(2)}";
                serverRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
            }
            else if (name.StartsWith("CDS_")) //클라이언트 -> 데디케이티드 서버 용
            {
                string[] words = name.Split("_");

                string msgName = "";
                foreach (string word in words)
                    msgName += FirstCharToUpper(word);

                string packetName = $"CDS_{msgName.Substring(3)}";
                dedicatedServerRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
            }
            else if (name.StartsWith("DSS_")) //데디케이티드 서버 -> 방 서버 용
            {
                string[] words = name.Split("_");

                string msgName = "";
                foreach (string word in words)
                    msgName += FirstCharToUpper(word);

                string packetName = $"DSS_{msgName.Substring(3)}";
                serverRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
            }
            else if (name.StartsWith("SDS_")) //방 서버 -> 데디케이티드 서버 용
            {
                string[] words = name.Split("_");

                string msgName = "";
                foreach (string word in words)
                    msgName += FirstCharToUpper(word);

                string packetName = $"SDS_{msgName.Substring(3)}";
                dedicatedServerRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
            }
        }

        string clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
        File.WriteAllText("ClientPacketManager.cs", clientManagerText);
        string serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
        File.WriteAllText("ServerPacketManager.cs", serverManagerText);
        string dedicatedServerManagerText = string.Format(PacketFormat.managerFormat, dedicatedServerRegister);
        File.WriteAllText("DedicatedServerPacketManager.cs", dedicatedServerManagerText);
    }

    public static string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";
        return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
    }
}