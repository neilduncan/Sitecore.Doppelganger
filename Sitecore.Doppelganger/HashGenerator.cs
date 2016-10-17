using System;
using System.Security.Cryptography;
using Sitecore.Data.Items;

namespace Sitecore.Doppelganger
{
    public static class HashGenerator
    {
        public static string GetHash(MediaItem media)
        {
            using (var md5 = MD5.Create())
            using (var content = media.GetMediaStream())
            using (new LogPerformance($"Generating hash for Item {media.MediaPath}"))
            {
                var hash = md5.ComputeHash(content);
                return BitConverter.ToString(hash);
            }
        }

        public static string GetHash(Item item)
        {
            return GetHash(new MediaItem(item));
        }
    }
}