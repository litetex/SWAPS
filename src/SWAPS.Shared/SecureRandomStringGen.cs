using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SWAPS.Shared
{
   public static class SecureRandomStringGen
   {
      private const string NUMBERS = "0123456789";
      private const string UPPER_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      private const string LOWER_CHARS = "abcdefghijklmnopqrstuvwxyz";

      public static string RandomString(int length, bool includeLowerCase = false)
      {
         return
            new string(
               Enumerable
                  .Repeat($"{NUMBERS}{UPPER_CHARS}{(includeLowerCase ? LOWER_CHARS : "")}", length)
                  .Select(s => s[Next(s.Length)])
                  .ToArray());
      }

      private static int Next(int maxExclusiveValue) => Next(0, maxExclusiveValue);
      private static int Next(int minValue, int maxExclusiveValue)
      {
         if (minValue >= maxExclusiveValue)
            throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} must be lower than {nameof(maxExclusiveValue)}");

         long diff = (long)maxExclusiveValue - minValue;
         long upperBound = uint.MaxValue / diff * diff;

         uint ui;
         do
         {
            ui = GetRandomUInt();
         } while (ui >= upperBound);
         return (int)(minValue + (ui % diff));
      }

      private static uint GetRandomUInt()
      {
         var randomBytes = GenerateRandomBytes(sizeof(uint));
         return BitConverter.ToUInt32(randomBytes, 0);
      }

      private static byte[] GenerateRandomBytes(int bytesNumber)
      {
         return RandomNumberGenerator.GetBytes(bytesNumber);
      }
   }
}
