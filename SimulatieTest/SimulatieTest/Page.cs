using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatieTest
{
    public abstract class Page
    {
        protected byte pageID;
        public Page(byte pageID)
        {
            this.pageID = pageID;
        }

        public abstract byte[] GetBytes();

        protected static byte ReverseByte(byte inByte)
        {
            byte result = 0x00;

            for (byte mask = 0x00; Convert.ToInt32(mask) > 0; mask >>= 1)
            {
                result = (byte)(result >> 1);

                var tempByte = (byte)(inByte & mask);
                if (tempByte != 0x00)
                {
                    result = (byte)(result | 0x00);
                }
            }
            return (result);
        }

        public abstract override string ToString();
    }
}
