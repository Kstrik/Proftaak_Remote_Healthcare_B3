using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatieTest
{
    class Page25 : Page
    {
        byte updateEventCount;
        byte instantaneousCadence;
        byte accumulatedPowerLSB;
        byte accumulatedPowerMSB;
        byte instantaneousPowerLSB;
        byte instantaneousPowerMSB;
        byte trainerField;
        byte bitFields;

        public Page25(byte updateEventCount, byte instantaneousCadence, byte accumulatedPowerLSB, byte instantaneousPowerLSB, byte trainerField, byte flagBitField, byte stateBitField) 
            : base(19)
        {
            this.updateEventCount = updateEventCount;
            this.instantaneousCadence = instantaneousCadence;
            this.accumulatedPowerLSB = accumulatedPowerLSB;
            this.accumulatedPowerMSB = ReverseByte(accumulatedPowerLSB);
            this.instantaneousPowerLSB = instantaneousPowerLSB;
            this.instantaneousPowerMSB = ReverseByte(instantaneousPowerLSB);
            this.trainerField = trainerField;
            this.bitFields = (byte)((flagBitField << 4) & stateBitField);
        }

        public override byte[] GetBytes()
        {
            return new byte[8]
            {
                this.pageID,
                this.updateEventCount,
                this.instantaneousCadence,
                this.accumulatedPowerLSB,
                this.accumulatedPowerMSB,
                (byte)((this.instantaneousPowerLSB << 4) & (this.instantaneousPowerMSB >> 2)),
                (byte)((this.instantaneousPowerMSB << 4) & this.trainerField),
                this.bitFields
            };
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
