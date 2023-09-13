// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

#pragma warning disable CS8618

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
        
        [MessagePackObject]
        public struct StartMatchPacket : IPacket
        {
        }
        
        [MessagePackObject]
        public struct CancelMatchPacket : IPacket
        {
        }

        public static class Room
        {
            [MessagePackObject]
            public struct MovePacket : IPacket
            {
                [Key(0)] public Position Position { get; set; }
                [Key(1)] public int Horizontal { get; set; }
            }

            [MessagePackObject]
            public struct AttackPacket : IPacket
            {
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
        
        [MessagePackObject]
        public struct MatchingStartedPacket : IPacket 
        {
        }
        
        [MessagePackObject]
        public struct MatchingStoppedPacket : IPacket 
        {
        }
        
        [MessagePackObject]
        public struct MatchingEndedPacket : IPacket 
        {
        }
        
        [MessagePackObject]
        public struct MatchInfoPacket : IPacket 
        {
            [Key(0)] public int MaxPlayers { get; set; }
            [Key(1)] public int CurrentPlayers { get; set; }
        }

        public static class Room
        {
            [MessagePackObject]
            public struct MovePacket : IPacket
            {
                [Key(0)] public int UserId { get; set; }
                [Key(1)] public Position Position { get; set; }
                [Key(2)] public int Horizontal { get; set; }
            }
            
            [MessagePackObject]
            public struct RoomWelcomePacket : IPacket
            {
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

            [MessagePackObject]
            public struct AttackPacket : IPacket
            {
                [Key(0)] public int UserId { get; set; }
            }
        }
    }
}