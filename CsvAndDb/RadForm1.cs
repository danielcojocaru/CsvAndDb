using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Renci.SshNet;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CsvAndDb
{
    public partial class RadForm1 : Telerik.WinControls.UI.RadForm
    {
        public RadForm1()
        {
            InitializeComponent();
        }

        public Encoding Encoding { get; set; } = Encoding.GetEncoding(1252);

        private void B1_CopyAllLinesFromFileToNewFile_Click(object sender, EventArgs e)
        {

            using (var reader = new StreamReader(@"C:\Users\daniel.cojocaru\Desktop\Deka\Errors\WAER\1.txt", this.Encoding, true))
            {
                using (StreamWriter writer = new StreamWriter(@"C:\Users\daniel.cojocaru\Desktop\Deka\Errors\WAER\10.txt", true, this.Encoding))
                {
                    while (!reader.EndOfStream)
                    {
                        writer.WriteLine(reader.ReadLine());
                    }
                }
            }
        }

        private void B2_CompleteFileWhereNull_Click(object sender, EventArgs e)
        {
            string defaultValueOnNull = "20171231";
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Errors\FSD\Bad";
            string targetPath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Errors\FSD\Good";
            int columnIndex = 7;
            char spliter = '|';

            IList<string> badFiles = new List<string>();

            string[] allFilesPaths = Directory.GetFiles(sourcePath);
            foreach (string filePath in allFilesPaths)
            {
                using (var reader = new StreamReader(filePath, this.Encoding, true))
                {
                    bool fileIsValid = true;
                    string fileName = Path.GetFileName(filePath);

                    string targetFile = targetPath + "\\" + fileName;
                    using (StreamWriter writer = new StreamWriter(targetFile, true, this.Encoding))
                    {
                        while (!reader.EndOfStream)
                        {
                            string currentLine = reader.ReadLine();
                            string[] splitedLine = currentLine.Split(spliter);

                            if (string.IsNullOrEmpty(splitedLine[columnIndex]))
                            {
                                fileIsValid = false;
                                splitedLine[columnIndex] = defaultValueOnNull;
                            }

                            string correctedLine = string.Join(spliter.ToString(), splitedLine);
                            writer.WriteLine(correctedLine);
                        }
                    }

                    if (!fileIsValid)
                    {
                        badFiles.Add(fileName);
                    }
                }
            }
        }

        private void B3_FindValueInFiles_Click(object sender, EventArgs e)
        {
            // input
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Errors\ASD";
            int columnIndex = 0;
            IList<string> searchedValues = new List<string>() { "1030781" };
            char spliter = '|';

            string[] allFilesPaths = Directory.GetFiles(sourcePath);

            foreach (string filePath in allFilesPaths)
            {
                double lineNr = 0;
                using (var reader = new StreamReader(filePath, this.Encoding, true))
                {
                    while (!reader.EndOfStream)
                    {
                        lineNr++;

                        string currentLine = reader.ReadLine();
                        string[] splitedLine = currentLine.Split(spliter);

                        if (searchedValues.Contains(splitedLine[columnIndex]))
                        {
                            string fileName = Path.GetFileName(filePath);
                            MessageBox.Show("Found it!" + Environment.NewLine + fileName + " >> Line nr = " + lineNr);
                        }
                    }
                }
            }
        }

        private static void MessageBoxFoundIt(string filePath, double lineNr)
        {
            string fileName = Path.GetFileName(filePath);
            MessageBox.Show("Found it!" + Environment.NewLine + fileName + " >> Line nr = " + lineNr);
        }

        private void B4_FindValuesInMultipleColumns_Click(object sender, EventArgs e)
        {
            //input ALD 
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Deka Produktiv\TAD";
            char spliter = '|';
            IDictionary<int, string> searched = new Dictionary<int, string>()
            {
                { 12, "15695961" },
                //{ 3, "1020601" },
                //{ 2, "150" },
                //{ 3, "2129047" },
            };

            string[] allFilesPaths = Directory.GetFiles(sourcePath);
            foreach (string filePath in allFilesPaths)
            {
                double lineNr = 0;
                using (var reader = new StreamReader(filePath, this.Encoding, true))
                {
                    while (!reader.EndOfStream)
                    {
                        lineNr++;

                        string currentLine = reader.ReadLine();
                        string[] splitedLine = currentLine.Split(spliter);

                        if (MatchesValues(searched, splitedLine))
                        {
                            string fileName = Path.GetFileName(filePath);
                            MessageBox.Show("Found it!" + Environment.NewLine + fileName + " >> Line nr = " + lineNr);
                        }
                        
                    }
                }
            }
        }

        private bool MatchesValues(IDictionary<int, string> searched, string[] splitedLine)
        {
            bool toReturn = true;
            foreach (KeyValuePair<int, string> entry in searched)
            {
                if (splitedLine.Length <= entry.Key)
                {
                    toReturn = false;
                    break;
                }

                string searching = entry.Value;
                string existing = splitedLine[entry.Key];

                if (!searching.Equals(existing))
                {
                    toReturn = false;
                    break;
                }
            }
            return toReturn;
        }

        private void B5_CompresFilesToOne_Click(object sender, EventArgs e)
        {
            string defaultValueOnNull = "20171231";
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Errors\LOT\More";
            string targetPath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Errors\LOT\One";
            char spliter = '|';

            string[] allFilesPaths = Directory.GetFiles(sourcePath);

            string targetFile = targetPath + "\\" + "NewFile.txt";

            using (StreamWriter writer = new StreamWriter(targetFile, true, this.Encoding))
            {
                foreach (string filePath in allFilesPaths)
                {
                    bool isFirstLineInWriter = true;
                    using (var reader = new StreamReader(filePath, this.Encoding, true))
                    {
                        bool isFirstLineInReader = true;
                        while (!reader.EndOfStream)
                        {
                            // copy the first row only once
                            if (isFirstLineInWriter)
                            {
                                isFirstLineInWriter = false;
                                isFirstLineInReader = false;
                            }
                            else if (isFirstLineInReader)
                            {
                                isFirstLineInReader = false;
                                continue;
                            }

                            writer.WriteLine(reader.ReadLine());
                        }
                    }
                }
            }
        }

        private void B6_FindValuesAndDelete_Click(object sender, EventArgs e)
        {
            string defaultValueOnNull = "20171231";
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Errors\LOT\Bad";
            string targetPath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Errors\LOT\Good";
            int columnIndex = 1;
            string searched = "0";
            char spliter = '|';

            IList<string> badFiles = new List<string>();

            string[] allFilesPaths = Directory.GetFiles(sourcePath);
            foreach (string filePath in allFilesPaths)
            {
                using (var reader = new StreamReader(filePath, this.Encoding, true))
                {
                    bool fileIsValid = true;
                    string fileName = Path.GetFileName(filePath);

                    string targetFile = targetPath + "\\" + fileName;
                    using (StreamWriter writer = new StreamWriter(targetFile, true, this.Encoding))
                    {
                        while (!reader.EndOfStream)
                        {
                            string currentLine = reader.ReadLine();
                            string[] splitedLine = currentLine.Split(spliter);

                            if (string.IsNullOrEmpty(splitedLine[columnIndex]) || splitedLine[columnIndex].Equals(searched))
                            {
                                fileIsValid = false;
                                splitedLine[columnIndex] = defaultValueOnNull;
                            }

                            string correctedLine = string.Join(spliter.ToString(), splitedLine);
                            writer.WriteLine(correctedLine);
                        }
                    }

                    if (!fileIsValid)
                    {
                        badFiles.Add(fileName);
                    }
                }
            }
        }

        private void B7_ReplaceInColumns_Click(object sender, EventArgs e)
        {
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Errors\WAER\Bad";
            string targetPath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Errors\WAER\Good";
            IList<int> columnIndexes = new List<int>() { 3, 4, 5 };
            string searched = ",";
            string replace = ".";
            char spliter = '|';

            string[] allFilesPaths = Directory.GetFiles(sourcePath);
            foreach (string filePath in allFilesPaths)
            {
                using (var reader = new StreamReader(filePath, this.Encoding, true))
                {
                    string fileName = Path.GetFileName(filePath);

                    string targetFile = targetPath + "\\" + fileName;
                    using (StreamWriter writer = new StreamWriter(targetFile, true, this.Encoding))
                    {
                        while (!reader.EndOfStream)
                        {
                            string currentLine = reader.ReadLine();
                            string[] splitedLine = currentLine.Split(spliter);

                            foreach (int colIndex in columnIndexes)
                            {
                                splitedLine[colIndex] = splitedLine[colIndex].Replace(searched, replace);
                            }

                            string correctedLine = string.Join(spliter.ToString(), splitedLine);
                            writer.WriteLine(correctedLine);
                        }
                    }
                }
            }
        }

        private void B8_DownloadFromFstp_Click(object sender, EventArgs e)
        {
            bool filesLieDirectInDeka2lupusalphaFolder = true;

            var targetPath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Keys\tar\";
            //var targetPath = @"\\DEV-FLOW-10\WorkingFolder\CurrentlyProcessing\";
            var srcPathWithFolders = "/cumcum/deka2lupusalpha/";
            var keyFilePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Keys\DEKA-ex_lupusalpha_openssh.ppk";
            //var keyFilePath = @"C:\WorkingFolder\Access\DEKA-ex_lupusalpha_openssh.ppk";
            var keyFilePassPhrase = @"\dRT77dtA;nUP!u";
            var username = "ex_lupusalpha";
            var ipAddress = "192.166.111.159";
            var port = 8322;

            var keyFile = new PrivateKeyFile(keyFilePath, keyFilePassPhrase);
            var keyFiles = new[] { keyFile };

            var methods = new List<AuthenticationMethod>();
            methods.Add(new PrivateKeyAuthenticationMethod(username, keyFiles));

            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            var srcPath = srcPathWithFolders + currentDate + "/";

            // wait for the todays folder 
            bool filesHaveArrived = false;
            do
            {
                var con1 = new ConnectionInfo(ipAddress, port, username, methods.ToArray());
                using (var client = new SftpClient(con1))
                {
                    client.Connect();
                    var files = client.ListDirectory(srcPathWithFolders);

                    if (filesLieDirectInDeka2lupusalphaFolder)
                    {
                        filesHaveArrived = files.Any(file => file.Name.Contains(".txt"));
                    }
                    else
                    {
                        filesHaveArrived = files.Any(file => file.Name.Equals(currentDate));
                    }

                    client.Disconnect();

                    if (!filesHaveArrived)
                    {
                        System.Threading.Thread.Sleep(60000); //wait 1 minute before checking again
                    }
                }

            } while (!filesHaveArrived);

            // wait 20 minutes for all the files to be loaded on the ftp server
            System.Threading.Thread.Sleep(1200000);

            // download all files from the ftp server
            var con2 = new ConnectionInfo(ipAddress, port, username, methods.ToArray());
            using (var client = new SftpClient(con2))
            {
                client.Connect();

                var files = filesLieDirectInDeka2lupusalphaFolder 
                    ? client.ListDirectory(srcPathWithFolders).Where(file => file.Name.Contains(".txt"))
                    : client.ListDirectory(srcPath).Where(file => file.Name.Contains(".txt"));

                foreach (var file in files)
                {
                    using (var fs = new FileStream(targetPath + file.Name, FileMode.Create))
                    {
                        client.DownloadFile(file.FullName, fs);
                        fs.Close();
                    }
                }
                client.Disconnect();
            }
        }

        private void B9_CheckIfAllFileAreThere_Click(object sender, EventArgs e)
        {
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Prod";
            List<string> names = new List<string>() {"ASD", "FSD", "FTK", "ALD", "LOT", "DIV", "MCH", "WAER", "NAH", "TAD" };

            List<string> files = Directory.GetFiles(sourcePath).ToList();
            List<DateTime> dates = new List<DateTime>();

            foreach (var file in files)
            {
                string firstSplit = file.Split(new string[] { ".txt" }, StringSplitOptions.None)[0];
                string[] secondSplitArray = firstSplit.Split('_');
                string secondSplit = secondSplitArray[secondSplitArray.Length - 1];
                string dateAsString = secondSplit.Substring(0, 8);
                DateTime date = DateTime.ParseExact(dateAsString, "yyyyMMdd", CultureInfo.InvariantCulture);

                dates.Add(date);
            }

            dates = dates.Distinct().ToList();

            string missingFiles = "";
            List<string> missingFilesList = new List<string>();

            foreach (var date in dates)
            {
                string dateAsString = date.ToString("yyyyMMdd");
                IEnumerable<string> filesToDate = files.Where(file => file.Contains(dateAsString));

                foreach (var name in names)
                {
                    if (filesToDate.FirstOrDefault(file => file.Contains(name)) == null)
                    {
                        string missing = name + "_" + dateAsString;
                        missingFiles += (missing + ", ");
                        missingFilesList.Add(missing);
                    }
                }

            }

            bool allFilesAreThere = missingFiles.Length == 0;

        }

        private static string dateFormat = "yyyyMMdd";
        private static string edgeDate = "20170427";
        private static string oneDayEarlier = DateTime.ParseExact(edgeDate, dateFormat, CultureInfo.InvariantCulture).AddDays(-1).ToString(dateFormat);

        private void B10_RunProgram_Click(object sender, EventArgs e)
        {
            

            Process p = new Process();
            p.StartInfo.FileName = @"C:\Projects\Git\Deka\Src\DEKADWH\labs.DEKA.CumCum.Application\bin\Debug\labs.DEKA.CumCum.Application.exe";
            p.StartInfo.Arguments = string.Format("CALCULATIONDATE={0},LASTCALCULATIONDATE={1},LASTCHECKDATETO={1},CHECKDATEFROM=20000101,CHECKDATETO={0},PARALLEL=false,PERSISTLOTRESULTS=true,PERSISTTRANSACTIONS=true,CHECKTADFORSALES=false,TESTMODE=true,CANCELLATIONMODE=false,SILENTMODE=true", edgeDate, oneDayEarlier);
            p.Start();


        }

        public RepositoryDb Repo { get; set; } = new RepositoryDb();

        private void B11_RunForeachProgram_Click(object sender, EventArgs e)
        {
            string filePath = @"C:\Projects\Git\Deka\Src\DEKADWH\labs.DEKA.CumCum.Application\bin\Debug\labs.DEKA.CumCum.Application.exe";

            DateTime start = DateTime.ParseExact("20180221", dateFormat, CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact("20180226", dateFormat, CultureInfo.InvariantCulture);

            List<Time> days = Repo.GetWorkingDaysBetween(start, end);

            foreach (Time todayTime in days)
            {
                string today = todayTime.Date.Value.ToString(dateFormat);

                // Run program and wait to end
                Process p = new Process();
                p.StartInfo.FileName = filePath;
                string arguments = string.Format("CALCULATIONDATE={0},CHECKDATEFROM={0},CHECKDATETO={0},PARALLEL=true,PERSISTLOTRESULTS=true,PERSISTTRANSACTIONS=false,CHECKTADFORSALES=true,TESTMODE=false,CANCELLATIONMODE=false,SILENTMODE=true", today);
                p.StartInfo.Arguments = arguments;
                p.Start();
                p.WaitForExit();
            }
        }

        //private void B11_RunForeachProgram_Click(object sender, EventArgs e)
        //{
        //    string startDateAsString = DateTime.ParseExact(edgeDate, dateFormat, CultureInfo.InvariantCulture).AddDays(1).ToString(dateFormat);
        //    string endDateAsString = "20170706";
        //    string filePath = @"C:\Projects\Git\Deka\Src\DEKADWH\labs.DEKA.CumCum.Application\bin\Debug\labs.DEKA.CumCum.Application.exe";

        //    DateTime start = DateTime.ParseExact(startDateAsString, dateFormat, CultureInfo.InvariantCulture);
        //    DateTime end = DateTime.ParseExact(endDateAsString, dateFormat, CultureInfo.InvariantCulture);

        //    while (start <= end)
        //    {
        //        string currentDate = start.ToString(dateFormat);
        //        string yesterday = start.AddDays(-1).ToString(dateFormat);

        //        {   // run the program
        //            Process p = new Process();
        //            p.StartInfo.FileName = filePath;
        //            string arguments = string.Format("CALCULATIONDATE={0},LASTCALCULATIONDATE={1},LASTCHECKDATETO={1},CHECKDATEFROM={0},CHECKDATETO={0},PARALLEL=false,PERSISTLOTRESULTS=true,PERSISTTRANSACTIONS=true,CHECKTADFORSALES=false,TESTMODE=true,CANCELLATIONMODE=false,SILENTMODE=true", currentDate, yesterday);
        //            p.StartInfo.Arguments = arguments;
        //            p.Start();

        //            p.WaitForExit();
        //        }
        //        start = start.AddDays(1);
        //    }
        //}

        private void B12_CompleteWithHeader_Click(object sender, EventArgs e)
        {
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Deka Produktiv\Bad";
            string targetPath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Deka Produktiv\Good";

            string badFiles = "";
            //string header = "ANLAGEID|DATUM|ISIN|SCD_SECID|WAEHRUNG|WKNTEXT";

            char spliter = '|';

            string[] allFilesPaths = Directory.GetFiles(sourcePath);
            foreach (string filePath in allFilesPaths)
            {
                using (var reader = new StreamReader(filePath, this.Encoding, true))
                {
                    string fileName = Path.GetFileName(filePath);

                    string header = GetHeader(fileName);

                    string targetFile = targetPath + "\\" + fileName;
                    using (StreamWriter writer = new StreamWriter(targetFile, true, this.Encoding))
                    {
                        bool isFirstLine = true;

                        while (!reader.EndOfStream)
                        {
                            string currentLine = reader.ReadLine();

                            if (isFirstLine)
                            {
                                isFirstLine = false;
                                if (!currentLine.Equals(header))
                                {
                                    badFiles += fileName + ", ";
                                    writer.WriteLine(header);
                                }
                            }
                            writer.WriteLine(currentLine);
                        }
                    }
                }
            }
        }

        private Dictionary<string, string> Headers = new Dictionary<string, string>()
        {
            {"ASD", "ANLAGEID|DATUM|ISIN|SCD_SECID|WAEHRUNG|WKNTEXT"},
            {"FSD", "FONDSID|TRANCHENID|DATUM|ISIN|SCD_FONDSID|SCD_TRANCHEID|FONDSNAME|RECHENSCHAFTSBERICHT|KAGID|VERWAHRSTELLENKUERZEL|KAG|HIERARCHIETYP|FONDSWAEHRUNG|AUFLEGUNG|AUFLOESUNG|FONDSKATEGORIE|SORTENREIN|TRANSPARENZOPTION|INVESTMENTFONDS|FONDSKURZNAME"},
            {"FTK", "FONDSID|TRANCHENID|DATUM|ANTEILEUMLAUF|TRANCHENVERMOEGEN|FONDSVERMOEGEN|TRANCHENRATIO|EURO_KURS"},
            {"ALD", "FONDSID|TRANCHENID|DATUM|ANLEGERID|ANLEGER|ANTEILE_TRANCHE|QUOTE_TRANCHE|QUOTE_FONDS|NAHERSTEHENDE_PERSON|GLOBAL"},
            {"LOT", "DATUM|GESCHAEFTSNR|FONDSID|ANLAGEID|STUECKE_GESAMT|STUECKE_KAUF|STUECKE_LOT|HANDELSTAG|HALTEDAUER"},
            {"DIV", "AUFTRAGSNUMMER|FONDSID|ANLAGEID|GESCHAEFTSNUMMER|STORNOSTATUS|EXTAG|DATUM|NOMINALSTUECK|AUSMACHENDERBETRAG|AUSMACHENDERBETRAG_WAEHRUNG|DEVISENKURS_ABR|DIVIDENDE100|DIVIDENDE_PRO_STUECK|DIVIDENDE_WAEHRUNG|KEST_BETRAG"},
            {"MCH", "FONDSID|ANLAGEID|GESCHAEFTSNR|LIEFERDATUM|EXTAG|MICRO_HEDGE|GUELTIGBIS"},
            {"WAER", "DATUM|FONDSID|ANLAGEID|MARKTWERT|EXPOSURE|WAER"},
            {"NAH", "QUELLE|DATUM|ISIN|FONDSID|TRANCHENID|ANTEILE|MARKTWERT|WAEHRUNG|QUOTE_TRANCHE|QUOTE_FONDS|ANLEGER|NAHESTEHENDE_PERSON|GLOBAL"},
            {"TAD", "V3BEWEGUNGSARTID|NAME_ELMGA|NAME_GA|DATUM|HANDELSDATUM|FONDSID|ANLAGEID|STORNOSTATUSID|NOMINALSTUECK|VERHAELTNIS|AUSWIRKUNG"},
            {"AKT", "DATUM|FONDSID|FINAL"}
        };

        private string GetHeader(string fileName)
        {
            foreach (KeyValuePair<string, string> header in Headers)
            {
                if (fileName.Contains(header.Key))
                {
                    return header.Value;
                }
            }
            return "";

        }

            
        private void radButton13_Click(object sender, EventArgs e)
        {
            List<DateTime> list = new List<DateTime>();
            DateTime first = DateTime.Now.AddYears(-1000);

            int max     = 30000000;
            int middle  = 20000000;

            for (int i = 0; i < max; i++)
            {
                first.AddDays(1);
                list.Add(first);
            }

            DateTime searched = list[middle];



            var watch1 = System.Diagnostics.Stopwatch.StartNew();
            DateTime found1;
            for (int i = 0; i < 1000000; i++)
            {
                found1 = list.FirstOrDefault(x => x == searched);
            }
            watch1.Stop();
            var elapsedMs1 = watch1.ElapsedMilliseconds;

            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            DateTime found2;
            for (int i = 0; i < 1000000; i++)
            {
                found2 = list[list.BinarySearch(searched)];
            }
            watch2.Stop();
            var elapsedMs2 = watch2.ElapsedMilliseconds;


        }
    }
}
