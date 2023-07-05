// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

#pragma warning disable CS8618

using System;
using MessagePack;

namespace ProjectUCS.Common.Data
{
    [MessagePackObject]
    public class RootPacket
    {
        [Key(0)] public int Id { get; set; }

        [Key(1)] public byte[] Data { get; set; }
    }


    [MessagePackObject]
    public class Position
    {
        [Key(0)] public int X { get; set; }
        [Key(1)] public int Y { get; set; }
    }

    public static class C2S
    {
        [MessagePackObject]
        public class ChatPacket : IPacket
        {
            [Key(0)] public string Message { get; set; }
        }

        public static class Room
        {
            [MessagePackObject]
            public class MovePacket : IPacket
            {
                [Key(0)] public Position Position { get; set; }
            }
        }
    }

    public static class S2C
    {
        [MessagePackObject]
        public class ChatPacket : IPacket
        {
            [Key(0)] public int UserId { get; set; }
            [Key(1)] public string Message { get; set; }
        }

        public static class Room
        {
            [MessagePackObject]
            public class MovePacket : IPacket
            {
                [Key(0)] public Guid UserId { get; set; }
                [Key(1)] public Position Position { get; set; }
            }

            [MessagePackObject]
            public class RoomInfoPacket : IPacket
            {
                [Key(0)] public int MaxPlayers { get; set; }
                [Key(1)] public int CurrentPlayers { get; set; }
            }

            [MessagePackObject]
            public class PlayerJoinedPacket : IPacket
            {
                [Key(0)] public Guid UserId { get; set; }
            }

            [MessagePackObject]
            public class PlayerLeftPacket : IPacket
            {
                [Key(0)] public Guid UserId { get; set; }
            }
        }
    }
}