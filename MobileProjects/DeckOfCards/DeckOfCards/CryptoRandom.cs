using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLCrypto;

namespace DeckOfCards
{

    ///<summary>
    /// Represents a pseudo-random number generator, a device that produces random data.
    ///</summary>
    static class CryptoRandom 
    {

        ///<summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        ///</summary>
        ///<param name=”buffer”>An array of bytes to contain random numbers.</param>
        public static void GetBytes(byte[] buffer)
        {
            PCLCrypto.NetFxCrypto.RandomNumberGenerator.GetBytes(buffer);
        }


        ///<summary>
        /// Returns a random number between 0.0 and 1.0.
        ///</summary>
        public static double NextDouble()
        {
            byte[] b = new byte[4];
            PCLCrypto.NetFxCrypto.RandomNumberGenerator.GetBytes(b);
            return (double)BitConverter.ToUInt32(b, 0) / UInt32.MaxValue;
        }

        ///<summary>
        /// Returns a random number within the specified range.
        ///</summary>
        ///<param name=”minValue”>The inclusive lower bound of the random number returned.</param>
        ///<param name=”maxValue”>The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        public static int Next(int minValue, int maxValue)
        {
            var multiplier = (maxValue - minValue - 1);
            return (int)Math.Round(NextDouble() * multiplier) +minValue;
        }

        ///<summary>
        /// Returns a nonnegative random number.
        ///</summary>
        public static int Next()
        {
            return Next(0, Int32.MaxValue);
        }

        ///<summary>
        /// Returns a nonnegative random number less than the specified maximum
        ///</summary>
        ///<param name=”maxValue”>The inclusive upper bound of the random number returned. maxValue must be greater than or equal 0</param>
        public static int Next(int maxValue)
        {
            return Next(0, maxValue);
        }
    }
}