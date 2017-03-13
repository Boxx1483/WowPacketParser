using System;
using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;
using CoreParsers = WowPacketParser.Parsing.Parsers;

namespace WowPacketParserModule.V5_4_1_17538.Parsers
{
    public static class SessionHandler
    {

        [Parser(Opcode.CMSG_PLAYER_LOGIN)]
        public static void HandlePlayerLogin(Packet packet)
        {
            packet.ReadSingle("Unk Float");
            var guid = packet.StartBitStream(6, 7, 1, 5, 2, 4, 3, 0);
            packet.ParseBitStream(guid, 7, 6, 0, 1, 4, 3, 2, 5);
            CoreParsers.SessionHandler.LoginGuid = new WowGuid64(BitConverter.ToUInt64(guid, 0));
            packet.WriteGuid("Guid", guid);
        }

        [Parser(Opcode.SMSG_MOTD)]
        public static void HandleMessageOfTheDay(Packet packet)
        {
            var lineCount = packet.ReadBits("Line Count", 4);
            var lineLength = new int[lineCount];

            for (var i = 0; i < lineCount; i++)
                lineLength[i] = (int)packet.ReadBits(7);

            for (var i = 0; i < lineCount; i++)
                packet.ReadWoWString("Line", lineLength[i], i);
        }

        [Parser(Opcode.SMSG_AUTH_RESPONSE)]
        public static void HandleAuthResponse(Packet packet)
        {
            var bit4 = false;
            var bit32 = false;
            var bit68 = false;
            var bit6C = false;
            var bit70 = false;
            var bit7C = false;

            var bits14 = 0;
            var classCount = 0u;
            var raceCount = 0u;
            var bits58 = 0;
            uint[] bits0 = null;
            uint[] bits1 = null;
            uint[] bits45 = null;
            uint[] bitsEA = null;
            uint[] bits448 = null;

            packet.ReadByte("Byte84");
            var isQueued = packet.ReadBit("isQueued");

            if (bit7C)
                bit7C = packet.ReadBit();

            var hasAccountData = packet.ReadBit("Has Account Data");

            if (hasAccountData)
            {
                bit70 = packet.ReadBit("unk 1");
                bits14 = (int)packet.ReadBits(21);
                bits58 = (int)packet.ReadBits(21);
                classCount = packet.ReadBits(23);

                bits448 = new uint[bits58];
                bitsEA = new uint[bits58];
                bits45 = new uint[bits58];

                for (var i = 0; i < bits58; ++i)
                {
                    bits448[i] = packet.ReadBits(23);
                    bitsEA[i] =  packet.ReadBits(7);
                    bits45[i] = packet.ReadBits(10);
                }


                bit6C = packet.ReadBit();
                bit68 = packet.ReadBit();

                bits0 = new uint[bits14];
                bits1 = new uint[bits14];

                for (var i = 0; i < bits14; ++i)
                {
                    bits0[i] = packet.ReadBits(8);
                    bit4 = packet.ReadBit("unk bit", i);
                    bits1[i] = packet.ReadBits(8);
                }


                bit32 = packet.ReadBit();
                raceCount = packet.ReadBits("Race Activation Count", 23);
            }

            if (hasAccountData)
            {
                packet.ReadInt32("Int2C");
                packet.ReadInt32("Int28");
                for (var i = 0; i < bits58; ++i)
                {
                    for (var j = 0; j < bitsEA[i]; ++j)
                    {
                        packet.ReadByte("Byte5C", i, j);
                        packet.ReadByte("Byte5C", i, j);
                    }

                    packet.ReadInt32("Int5C");
                    packet.ReadWoWString("String5C", bits45[i], i);
                    packet.ReadWoWString("String5C", bits448[i], i);
                }

                packet.ReadByte("Byte31");

                for (var i = 0; i < raceCount; ++i)
                {
                    packet.ReadByteE<ClientType>("Race Expansion", i);
                    packet.ReadByteE<Race>("Race", i);
                }

                packet.ReadByte("Byte30");
                packet.ReadInt32("Int34");

                for (var i = 0; i < classCount; ++i)
                {
                    packet.ReadByteE<Class>("Class", i);
                    packet.ReadByteE<ClientType>("Class Expansion", i);
                }

                if (bit70)
                    packet.ReadInt16("Int6E");

                for (var i = 0; i < bits14; ++i)
                {
                    packet.ReadWoWString("Realm", bits0[i], i);
                    packet.ReadWoWString("Realm", bits1[i], i);
                    packet.ReadInt32("Realm Id", i);
                }

                packet.ReadInt32("Int10");
                packet.ReadInt32("Int24");

                if (bit6C)
                    packet.ReadInt16("Int6A");
            }

            if (isQueued)
                packet.ReadInt32("Int78");
        }
    }
}