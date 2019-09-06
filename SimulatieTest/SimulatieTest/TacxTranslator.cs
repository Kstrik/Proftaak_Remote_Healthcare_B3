using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatieTest
{
    class TacxTranslator
    {
        public static Dictionary<string, int> Translate(string[] bits)
        {
            try
            {
                int dataPage = Convert.ToInt32(bits[4], 16);
                Console.WriteLine(dataPage);

                if (dataPage == 16)
                {
                    return TranslateDataPage16(bits);
                }
                else if (dataPage == 25)
                {
                    return TranslateDataPage25(bits);
                }

            }
            catch (IndexOutOfRangeException e) { }

            return new Dictionary<string, int>();
        }

        private static Dictionary<string, int> TranslateDataPage16(string[] bits)
        {
            Dictionary<string, int> translatedData = new Dictionary<string, int>();

            translatedData.Add("PageID", 16);
            translatedData.Add("EquipmentTypebitField ", Convert.ToInt32(bits[5], 16));
            translatedData.Add("time ", Convert.ToInt32(bits[6], 16));
            translatedData.Add("distance ", Convert.ToInt32(bits[7], 16));
            translatedData.Add("speed ", Convert.ToInt32(bits[8] + bits[9], 16));
            translatedData.Add("heartBeat ", Convert.ToInt32(bits[10], 16));
            translatedData.Add("capabilitiesBitField ", Convert.ToInt32(bits[11].First<char>().ToString(), 16));
            translatedData.Add("FEStateBitField ", Convert.ToInt32(bits[11].Last<char>().ToString(), 16));

            return translatedData;
        }

        private static Dictionary<string, int> TranslateDataPage25(string[] bits)
        {
            Dictionary<string, int> translatedData = new Dictionary<string, int>();

            translatedData.Add("PageID", 25);
            translatedData.Add("Eventcounter" , Convert.ToInt32(bits[5], 16));
            translatedData.Add("InstantaneousCadence " , Convert.ToInt32(bits[6], 16));
            translatedData.Add("AccumulatedPower " , Convert.ToInt32(bits[7] + bits[8], 16));
            translatedData.Add("InstantaneousPower " , Convert.ToInt32(bits[9] + bits[10].First<char>(), 16));
            translatedData.Add("TrainerStatusbitField " , Convert.ToInt32(bits[10].Last<char>().ToString(), 16));
            translatedData.Add("flagsBitField " , Convert.ToInt32(bits[11].First<char>().ToString(), 16));
            translatedData.Add("FEStateBitField " , Convert.ToInt32(bits[11].Last<char>().ToString(), 16));

            return translatedData;
        }
    }
}
