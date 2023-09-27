using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATD_ID4P.Class;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Drawing.Drawing2D;


// load from ClientData.Jason file in *Project Name*/bin/Debug/Config
namespace ATD_ID4P.Model
{
    public class ClientDataModel
    {

        public int Selected_PalletWidth = 0;
        public int Selected_PalletLength = 0;
        public int Selected_PalletHeight = 0;
        public string Selected_PalletType = "";
        public string Selected_PalletText = "";

        public TSSize[] TSSizeOption = new TSSize[5];
        public int Selected_TSWidth = 0;
        public int Selected_TSLength = 0;
        public string Selected_TSText = "";

        public string[] Flute = new string[7];
        public int[] Thickness = new int[7];
        public int[] BoxPerBundle = new int[7];
        public int[] Ratio_a = new int[7];
        public int[] Ratio_b = new int[7];
        public int[] Ratio_c = new int[7];
        public int[] Ratio_d = new int[7];
        public int[] StackHeight = new int[7];
        public string[,] PatternData = new string[29, 5];

        private LogCls Log = new LogCls();

        public string[] LoadBoxStyleOption()
        {
            try
            {
                string SavePath = Application.StartupPath + @"\Config\BoxStyleOptions.json";
                if (System.IO.File.Exists(SavePath))
                {
                    string[] arrStyle = JArray.Parse(File.ReadAllText(SavePath)).Select(element => (string)element["Style"]).ToArray();
                    return arrStyle;
                }
                else
                {
                    string buf_Msg = "Cannot find Box Style Option json file.\n" +
                                        "Please check path: " + SavePath;
                    string buf_Topic = "Error! - Can not find json file.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Log.WriteLog("Error! - Can not find Box Style Option json file. BoxStyleOptions.json doesn't exist.", LogType.Fail);
                }
            }
            catch (Exception ex)
            {
                string buf_Msg = "Cannot read Box Style Option json file.\n" +
                                    "Please check log file for more detail.";
                string buf_Topic = "Error! - Can not read json file";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log.WriteLog("Error! - Can not read Box Style Option json file." + ex.Message, LogType.Fail);
            }
            return null;
        }

        public string[] LoadFluteDataOption()
        {
            try
            {
                string SavePath = Application.StartupPath + @"\Config\FluteDataOptions.json";
                if (System.IO.File.Exists(SavePath))
                {
                    string[] arrStyle = JArray.Parse(File.ReadAllText(SavePath)).Select(element => (string)element["Flute"]).ToArray();

                    int x = 0;
                    JArray conversionArray = JArray.Parse(File.ReadAllText(SavePath));
                    foreach (JObject conversion in conversionArray.Children<JObject>())
                    {
                        Flute[x] = (string)conversion["Flute"];
                        Thickness[x] = (int)conversion["Thickness"];
                        BoxPerBundle[x] = (int)conversion["BoxPerBundle"];
                        Ratio_a[x] = (int)conversion["a"];
                        Ratio_b[x] = (int)conversion["b"];
                        Ratio_c[x] = (int)conversion["c"];
                        Ratio_d[x] = (int)conversion["d"];
                        StackHeight[x] = (int)conversion["StackHeight"];

                        x++;
                    }

                    return arrStyle;

                }
                else
                {
                    string buf_Msg = "Cannot find Flute Data Option json file.\n" +
                                        "Please check path: " + SavePath;
                    string buf_Topic = "Error! - Can not find json file.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Log.WriteLog("Error! - Can not find Flute Data Option json file. FluteDataOptions.json doesn't exist. ", LogType.Fail);
                }
            }
            catch (Exception ex)
            {
                string buf_Msg = "Cannot read Flute Data Option json file.\n" +
                                    "Please check log file for more detail.";
                string buf_Topic = "Error! - Can not read json file";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log.WriteLog("Error! - Can not read Flute Data Option json file." + ex.Message, LogType.Fail);
            }
            return null;
        }

        public void LoadPatternOptions()
        {
            try
            {
                string SavePath = Application.StartupPath + @"\Config\PatternOptions.json";
                if (System.IO.File.Exists(SavePath))
                {
                    var json = File.ReadAllText(SavePath);
                    var pattArray = JArray.Parse(json);

                    var patt = pattArray.Children<JObject>()
                                  .Select(pattDB => pattDB.Properties().ToDictionary(z => z.Name, z => z.Value.ToString()))
                                  .ToArray();

                    var x = 0;
                    foreach (var pattDB in patt)
                    {
                        PatternData[x, 0] = pattDB["ID"];
                        PatternData[x, 1] = pattDB["Code"];
                        PatternData[x, 2] = pattDB["L_Side"];
                        PatternData[x, 3] = pattDB["W_Side"];
                        PatternData[x, 4] = pattDB["BundlePerLayer"];

                        x++;
                    }

                }
                else
                {
                    string buf_Msg = "Cannot find Pattern Option json file.\n" +
                                        "Please check path: " + SavePath;
                    string buf_Topic = "Error! - Can not find json file.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Log.WriteLog("Error! - Can not find Pattern Option json file. PatternOptions.json doesn't exist. ", LogType.Fail);
                }
            }
            catch (Exception ex)
            {
                string buf_Msg = "Cannot read Pattern Option json file.\n" +
                                    "Please check log file for more detail.";
                string buf_Topic = "Error! - Can not read json file";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log.WriteLog("Error! - Can not read Pattern Option json file." + ex.Message, LogType.Fail);
            }
        }

        public void LoadTSSizeOptions()
        {
            try
            {
                string SavePath = Application.StartupPath + @"\Config\TSSizeOptions.json";
                if (System.IO.File.Exists(SavePath))
                {
                    string RawData = System.IO.File.ReadAllText(SavePath);
                    TSSizeOption = JsonConvert.DeserializeObject<TSSize[]>(RawData);
                    Selected_TSWidth = TSSizeOption.Where(buf_option => buf_option.Selected == true).First().Width;
                    Selected_TSLength = TSSizeOption.Where(buf_option => buf_option.Selected == true).First().Length;
                    CreateTieSheetText();
                }
                else
                {
                    string buf_Msg = "Cannot find TieSheet Option json file.\n" + 
                                        "Please check path: " + SavePath;
                    string buf_Topic = "Error! - Can not find json file.";
                    MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Log.WriteLog("Error! - Can not find TS Option json file. TSSizeOptions.json doesn't exist. ", LogType.Fail);
                }
            }
            catch (Exception ex)
            {
                string buf_Msg = "Cannot read TieSheet Option json file.\n" +
                                    "Please check log file for more detail.";
                string buf_Topic = "Error! - Can not read json file";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log.WriteLog("Error! - Can not read TS Option json file." + ex.Message, LogType.Fail);
            }
        }

        public void SaveTieSheetOption()
        {
            try
            {
                string SavePath = Application.StartupPath + @"\Config\TSSizeOptions.json";

                if (System.IO.File.Exists(SavePath))
                    System.IO.File.Delete(SavePath);
                System.IO.File.WriteAllText(SavePath, JsonConvert.SerializeObject(TSSizeOption));
                CreateTieSheetText();
            }
            catch (Exception ex)
            {
                string buf_Msg = "Cannot write TieSheet Option json file.\n" +
                                    "Please check log file for more detail.";
                string buf_Topic = "Error! - Can not write json file";
                MessageBox.Show(buf_Msg, buf_Topic, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Log.WriteLog("Error! - Can not write TS Option json file." + ex.Message, LogType.Fail);
            }
        }
        private void CreateTieSheetText()
        {
            Selected_TSText = string.Format("{0}x{1}", Selected_TSWidth, Selected_TSLength);
        }
    }

    public class TSSize
    {
        public int ID { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public bool Selected { get; set; }
    }
}
