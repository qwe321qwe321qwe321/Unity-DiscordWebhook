using System;

using Snowflake = System.UInt64;

namespace DiscordWebhook {
    /// <summary>
    /// Discord utilizes Twitter's snowflake format for uniquely identifiable descriptors (IDs). These IDs are guaranteed to be unique across all of Discord, except in some unique scenarios in which child objects share their parent's ID. Because Snowflake IDs are up to 64 bits in size (e.g. a uint64), they are always returned as strings in the HTTP API to prevent integer overflows in some languages. See Gateway ETF/JSON for more information regarding Gateway encoding.
    /// https://discord.com/developers/docs/reference#snowflakes
    /// </summary>
    public static class SnowflakeExtensions {
        public static DateTimeOffset Timestamp(this Snowflake snowflake) {
            // 42 bits timestamp
            ulong unixTimestamp = (snowflake >> 22) + 1420070400000;
            return DateTimeOffset.FromUnixTimeMilliseconds((long)unixTimestamp);
        }
        
        public static byte InternalWorkerId(this Snowflake snowflake) {
            // 5 bits worker ID
            return (byte)((snowflake & 0x3E0000) >> 17);
        }
        
        public static byte InternalProcessId(this Snowflake snowflake) {
            // 5 bits process ID
            return (byte)((snowflake & 0x1F000) >> 12);
        }
        
        public static ushort Increment(this Snowflake snowflake) {
            // 12 bits increment
            return (ushort)(snowflake & 0xFFF);
        }
        
        public static string DumpSnowflake(this Snowflake snowflake) {
            return $"Timestamp: {snowflake.Timestamp()}\n" +
                   $"InternalWorkerId: {snowflake.InternalWorkerId()}\n" +
                   $"InternalProcessId: {snowflake.InternalProcessId()}\n" +
                   $"Increment: {snowflake.Increment()}";
        }
    }
}