using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace crPakTool
{
    class ObjectName
    {
        static string[] preStrings;
        static string[,] strings;
        static int counter;
        string objectName;

        public void loadObjectFile(long hash)
        {
            string fileName = hash.ToString("X16");
            string appPath = Application.StartupPath;

            try
            {
                preStrings = File.ReadAllLines(appPath + "\\ObjectNames\\" + fileName);
            }
            catch
            {
                MessageBox.Show("Could not find objectDatabase for this file");

                preStrings = new string[] { "1234567812345678=default" };
            }
            

            strings = new string[preStrings.Length, 2];
            counter = preStrings.Length;

            for (int i = 0; i < preStrings.Length; i++)
            {
                string[] temp = preStrings[i].Split('=');

                if (temp[0].StartsWith("//") == false)
                {
                    strings[i, 0] = temp[0];
                    strings[i, 1] = temp[1];
                }
                else
                {
                    strings[i, 0] = "1234567812345678";
                    strings[i, 1] = "default";
                }
            }
        }

        public string giveObjectName(uint idPartone, uint idParttwo, string extension)
        {
            string idName = idPartone.ToString("X8") + idParttwo.ToString("X8");

            if (counter != 0)
            {
                for (int i = 0; i < preStrings.Length; i++)
                {
                    if (idName == strings[i, 0] && strings[i, 0].StartsWith("//") == false)
                    {
                        objectName = strings[i, 1];
                        counter -= 1;
                        break;
                    }
                    else
                    {
                        objectName = idName + "." + extension;
                    }
                }
            }
            else
            {
                objectName = idName + "." + extension;
            }

            return objectName;
        }
    }
}
