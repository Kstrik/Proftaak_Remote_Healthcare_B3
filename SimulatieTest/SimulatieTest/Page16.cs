using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatieTest
{
    class Page16 : Page
    {
        byte equipmentTypeBitField;
        byte elapsedTime;
        byte distanceTraveled;
        byte speedLSB;
        byte speedMSB;
        byte heartrate;
        byte bitField; 
        public Page16(byte equipmentTypeBitField, byte elapsedTime, byte distanceTraveled, byte speedLSB, byte heartrate, byte capabillitiesBitField, byte feStateBitField) 
            : base(10)
        {
            this.equipmentTypeBitField = equipmentTypeBitField;
            this.elapsedTime = elapsedTime;
            this.distanceTraveled = distanceTraveled;
            this.speedLSB = speedLSB;
            this.speedMSB = ReverseByte(speedLSB);
            this.heartrate = heartrate;
            this.bitField = (byte)((capabillitiesBitField <<= 4) & feStateBitField);
        }

        public override byte[] GetBytes()
        {
            return new byte[8] {
                this.pageID,
                this.equipmentTypeBitField,
                this.elapsedTime,
                this.distanceTraveled,
                this.speedLSB,
                this.speedMSB,
                this.heartrate,
                this.bitField
            };
        }
    }
}
