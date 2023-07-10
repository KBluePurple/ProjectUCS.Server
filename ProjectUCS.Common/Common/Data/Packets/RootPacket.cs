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
    public struct RootPacket
    {
        [Key(0)] public int Id { get; set; }

        [Key(1)] public byte[] Data { get; set; }
    }


    [MessagePackObject]
    public struct Position
    {
        [Key(0)] public float X { get; set; }
        [Key(1)] public float Y { get; set; }
    }

    public static class C2S
    {
        [MessagePackObject]
        public struct ChatPacket : IPacket
        {
            [Key(0)] public string Message { get; set; }
        }

        public static class Room
        {
            [MessagePackObject]
            public struct MovePacket : IPacket
            {
                [Key(0)] public Position Position { get; set; }
            }
        }
    }

    public static class S2C
    {
        [MessagePackObject]
        public struct ChatPacket : IPacket
        {
            [Key(0)] public int UserId { get; set; }
            [Key(1)] public string Message { get; set; }
        }
        
        [MessagePackObject]
        public struct WelcomePacket : IPacket
        {
            [Key(0)] public int UserId { get; set; }
        }

        public static class Room
        {
            [MessagePackObject]
            public struct MovePacket : IPacket
            {
                [Key(0)] public int UserId { get; set; }
                [Key(1)] public Position Position { get; set; }
            }

            [MessagePackObject]
            public struct RoomInfoPacket : IPacket
            {
                [Key(0)] public int MaxPlayers { get; set; }
                [Key(1)] public int CurrentPlayers { get; set; }
            }

            [MessagePackObject]
            public struct PlayerJoinedPacket : IPacket
            {
                [Key(0)] public int UserId { get; set; }
            }

            [MessagePackObject]
            public struct PlayerLeftPacket : IPacket
            {
                [Key(0)] public int UserId { get; set; }
            }
        }
    }
}