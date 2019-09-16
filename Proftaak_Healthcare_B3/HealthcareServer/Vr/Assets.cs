using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr
{
    public class Assets
    {
        public enum AssetType
        {
            MODELS,
            DIFFUSETEXTURES,
            NORMALMAPS,
            SPECULATMAPS,
            SKYBOXTEXTURES
        }

        private static Dictionary<AssetType, List<string>> AssetsDictionaryFilePath;

        public static List<string> GetAllDiffuseMaps()
        {
            return null;
        }
    }
}
