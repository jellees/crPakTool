using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crPakTool
{
    class PakFileName
    {
        public string FakeName(uint idOne, uint idTwo, ulong hash)
        {
            string fileId = idOne.ToString("X2") + idTwo.ToString("X2");
            string fakeName = "";

            if (hash == 0x882FB3767C72A431) // L02_Jungle_Cling.pak
            {
                switch (fileId)
                {
                    case "739D5B3541982233":
                        fakeName = " (Wiggler Plant)";
                        break;
                    case "33A82B74D5C38B21":
                        fakeName = " (Riding Jump Platform)";
                        break;
                    case "1115B8B121A9DA7":
                        fakeName = " (Palm Leaves 1)";
                        break;
                    case "F3EC13DB199AE05E":
                        fakeName = " (Bouncing Plant little)";
                        break;
                    case "759F903115D51A7A":
                        fakeName = " (Tree leaves)";
                        break;
                    case "77B5D4EA3FEA3B26":
                        fakeName = " (Bouncing Plant Big)";
                        break;
                    case "962B6AA9F2F31D4F":
                        fakeName = " (Background jungle)";
                        break;
                }
            }
            else if (hash == 0x2717894DAA3CD7EF) // Frontend.pak
            {
                switch (fileId)
                {
                    case "F3014D218ACFA4FE":
                        fakeName = " (A Button Texture)";
                        break;
                    case "F6E84AD6BF8B457":
                        fakeName = " (B Button Texture)";
                        break;
                    case "E7E2A9509C5308EC":
                        fakeName = " (Green Arrow Right Texture)";
                        break;
                }
            }

            return fakeName;
        }
    }
}
