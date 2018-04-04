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
            //string a = DateTime.Now.ToString("yyyyMMdd HH.mm.ss");

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
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Test0_20180320\2) Täglicher Betrieb";
            int columnIndex = -1;
            IList<string> searchedValues = new List<string>() { "FileName" };
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

                        if (columnIndex < 0)
                        {
                            foreach (string split in splitedLine)
                            {
                                if (searchedValues.Contains(split))
                                {
                                    string fileName = Path.GetFileName(filePath);
                                    MessageBox.Show("Found it!" + Environment.NewLine + fileName + " >> Line nr = " + lineNr);
                                }
                            }
                        }
                        else if (searchedValues.Contains(splitedLine[columnIndex]))
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
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Test0_20180320\2) Täglicher Betrieb";
            string filePattern = "LOT";
            char spliter = '|';
            IDictionary<int, string> searched = new Dictionary<int, string>()
            {
                { 0, "20180102" },
                //{ 1, "10458" },
                //{ 2, "1003341" },
                //{ 3, "2129047" },
            };

            string[] allFilesPaths;
            if (filePattern == null)
            {
                allFilesPaths = Directory.GetFiles(sourcePath);
            }
            else
            {
                allFilesPaths = Directory.GetFiles(sourcePath, "*" + filePattern + "*");
            }
            

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

        private void B16_CheckColumnNamesInFiles_Click(object sender, EventArgs e)
        {
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Test0_20180320\2) Täglicher Betrieb";
            //char spliter = '|';

            string AKT = "DATUM|FONDSID|FINAL";
            string ALD = "FONDSID|TRANCHENID|DATUM|ANLEGERID|ANLEGER|ANTEILE_TRANCHE|QUOTE_TRANCHE|QUOTE_FONDS|NAHERSTEHENDE_PERSON|GLOBAL";
            string ASD = "ANLAGEID|DATUM|ISIN|SCD_SECID|WAEHRUNG|WKNTEXT";
            string DIV = "AUFTRAGSNUMMER|FONDSID|ANLAGEID|GESCHAEFTSNUMMER|STORNOSTATUS|EXTAG|DATUM|NOMINALSTUECK|AUSMACHENDERBETRAG|AUSMACHENDERBETRAG_WAEHRUNG|DEVISENKURS_ABR|DIVIDENDE100|DIVIDENDE_PRO_STUECK|DIVIDENDE_WAEHRUNG|KEST_BETRAG";
            string FSD = "FONDSID|TRANCHENID|DATUM|ISIN|SCD_FONDSID|SCD_TRANCHEID|FONDSNAME|RECHENSCHAFTSBERICHT|KAGID|VERWAHRSTELLENKUERZEL|KAG|HIERARCHIETYP|FONDSWAEHRUNG|AUFLEGUNG|AUFLOESUNG|FONDSKATEGORIE|SORTENREIN|TRANSPARENZOPTION|INVESTMENTFONDS|FONDSKURZNAME";
            string FTK = "FONDSID|TRANCHENID|DATUM|ANTEILEUMLAUF|TRANCHENVERMOEGEN|FONDSVERMOEGEN|TRANCHENRATIO|EURO_KURS";
            string LOT = "DATUM|GESCHAEFTSNR|FONDSID|ANLAGEID|STUECKE_GESAMT|STUECKE_KAUF|STUECKE_LOT|HANDELSTAG|HALTEDAUER";
            string MCH = "FONDSID|ANLAGEID|GESCHAEFTSNR|LIEFERDATUM|EXTAG|MICRO_HEDGE|GUELTIGBIS|HANDELSDATUM";
            string NAH = "QUELLE|DATUM|ISIN|FONDSID|TRANCHENID|ANTEILE|MARKTWERT|WAEHRUNG|QUOTE_TRANCHE|QUOTE_FONDS|ANLEGER|NAHESTEHENDE_PERSON|GLOBAL";
            string TAD = "V3BEWEGUNGSARTID|NAME_ELMGA|NAME_GA|DATUM|HANDELSDATUM|FONDSID|ANLAGEID|STORNOSTATUSID|NOMINALSTUECK|VERHAELTNIS|AUSWIRKUNG|AUFTRAGSNR|GESCHAEFTSNR";
            string WAER = "DATUM|FONDSID|ANLAGEID|MARKTWERT|EXPOSURE|WAER";

            Dictionary<FileTypeEnum, string> dictionary = new Dictionary<FileTypeEnum, string>()
            {
                {FileTypeEnum.AKT , AKT  },
                {FileTypeEnum.ALD , ALD  },
                {FileTypeEnum.ASD , ASD  },
                {FileTypeEnum.DIV , DIV  },
                {FileTypeEnum.FSD , FSD  },
                {FileTypeEnum.FTK , FTK  },
                {FileTypeEnum.LOT , LOT  },
                {FileTypeEnum.MCH , MCH  },
                {FileTypeEnum.NAH , NAH  },
                {FileTypeEnum.TAD , TAD  },
                {FileTypeEnum.WAER, WAER },
            };

            string[] allFilesPaths = Directory.GetFiles(sourcePath);

            foreach (string filePath in allFilesPaths)
            {
                string fileName = Path.GetFileName(filePath);
                FileTypeEnum fileType = GetFileTypeFromPathFile(filePath);
                string firstLineShouldBe = dictionary[fileType];

                using (var reader = new StreamReader(filePath, this.Encoding, true))
                {
                    while (!reader.EndOfStream)
                    {
                        string currentLine = reader.ReadLine();
                        if (!currentLine.Equals(firstLineShouldBe))
                        {
                            Console.WriteLine(fileName);
                        }

                        break;
                    }
                }
            }
        }

        private FileTypeEnum GetFileTypeFromPathFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            if (fileName.Contains(FileTypeEnum.AKT.ToString()))
            {
                return FileTypeEnum.AKT;
            }
            else if (fileName.Contains(FileTypeEnum.ALD.ToString()))
            {
                return FileTypeEnum.ALD;
            }
            else if (fileName.Contains(FileTypeEnum.ASD.ToString()))
            {
                return FileTypeEnum.ASD;
            }
            else if (fileName.Contains(FileTypeEnum.DIV.ToString()))
            {
                return FileTypeEnum.DIV;
            }
            else if (fileName.Contains(FileTypeEnum.FSD.ToString()))
            {
                return FileTypeEnum.FSD;
            }
            else if (fileName.Contains(FileTypeEnum.FTK.ToString()))
            {
                return FileTypeEnum.FTK;
            }
            else if (fileName.Contains(FileTypeEnum.LOT.ToString()))
            {
                return FileTypeEnum.LOT;
            }
            else if (fileName.Contains(FileTypeEnum.MCH.ToString()))
            {
                return FileTypeEnum.MCH;
            }
            else if (fileName.Contains(FileTypeEnum.NAH.ToString()))
            {
                return FileTypeEnum.NAH;
            }
            else if (fileName.Contains(FileTypeEnum.TAD.ToString()))
            {
                return FileTypeEnum.TAD;
            }
            else if (fileName.Contains(FileTypeEnum.WAER.ToString()))
            {
                return FileTypeEnum.WAER;
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        public enum FileTypeEnum
        {
            AKT,
            ALD,
            ASD,
            DIV,
            FSD,
            FTK,
            LOT,
            MCH,
            NAH,
            TAD,
            WAER
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

        private void B7_ReplaceInColumns_Click(object sender, EventArgs e)
        {
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Test0_20180320\Bad";
            string targetPath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Test0_20180320\Good";
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


        private void B15_SearchRueckwirkendeAenderungen_Click(object sender, EventArgs e)
        {
            //DateTime date1 = DateTime.ParseExact("20180119", "yyyyMMdd", CultureInfo.InvariantCulture);
            //DateTime date2 = Repo.GetWorkingDateAfter(date1);
            //DateTime date3 = Repo.GetWorkingDateBefore(date1);


            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Test0_20180320\2) Täglicher Betrieb";
            string searching = "DATUM";
            char spliter = '|';

            DateTime middleDay = DateTime.ParseExact("20180101", "yyyyMMdd", CultureInfo.InvariantCulture);
            List<Time> timesForWorkingDateBefore = Repo.GetWorkingDaysBetween(middleDay.AddDays(-400), middleDay.AddDays(400)).OrderByDescending(t => t.Date).ToList();


            string[] files = Directory.GetFiles(sourcePath);
            foreach (string filePath in files)
            {
                DateTime dateOfFile = GetDateFromDekaFilePath(filePath);
                //DateTime startWith = DateTime.ParseExact("20180122", "yyyyMMdd", CultureInfo.InvariantCulture);

                //if (dateOfFile < startWith)
                //{
                //    continue;
                //}

                if (dateOfFile.Year < 2018)
                {
                    continue;
                }

                using (var reader = new StreamReader(filePath, this.Encoding, true))
                {
                    int lineNr = -1;
                    int indexOfDate = -1;
                    while (!reader.EndOfStream)
                    {
                        lineNr++;
                        string currentLine = reader.ReadLine();
                        string[] splitedLine = currentLine.Split(spliter);

                        if (lineNr == 0)
                        {
                            for (int i = 0; i < splitedLine.Length; i++)
                            {
                                string spl = splitedLine[i];
                                if (spl.Equals(searching))
                                {
                                    indexOfDate = i;
                                    break;
                                }
                            }

                            if (indexOfDate == -1)
                            {
                                break;
                            }
                        }
                        else
                        {
                            string dateAsString = splitedLine[indexOfDate];
                            DateTime dateFound = DateTime.ParseExact(dateAsString, "yyyyMMdd", CultureInfo.InvariantCulture);

                            DateTime shouldBe = Repo.GetWorkingDateBefore(dateOfFile, timesForWorkingDateBefore);

                            if (dateFound != shouldBe)
                            {
                                string fileName = Path.GetFileName(filePath);


                                bool a = false;

                                break;
                                //MessageBox.Show("Found it!" + Environment.NewLine + fileName + " >> Line nr = " + lineNr);


                            }
                        }

                        
                        

                    }
                }
            }

        }


        private DateTime GetDateFromDekaFilePath(string file)
        {
            string fileName = Path.GetFileName(file);
            string firstSplit = fileName.Split(new string[] { ".txt" }, StringSplitOptions.None)[0];
            string[] secondSplitArray = firstSplit.Split('_');
            string secondSplit = secondSplitArray[secondSplitArray.Length - 1];
            string dateAsString = secondSplit.Substring(0, 8);
            DateTime date = DateTime.ParseExact(dateAsString, "yyyyMMdd", CultureInfo.InvariantCulture);
            return date;
        }

        private void B7Prime_DeleteColumnsAfterIndex_Click(object sender, EventArgs e)
        {
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Test0_20180320\Bad";
            string targetPath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Test0_20180320\Good";
            int maxIndex = 14;
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
                            string[] newLine = new string[maxIndex + 1];

                            for (int i = 0; i < maxIndex + 1; i++)
                            {
                                newLine[i] = splitedLine[i];
                            }

                            string correctedLine = string.Join(spliter.ToString(), newLine);
                            writer.WriteLine(correctedLine);
                        }
                    }
                }
            }
        }

        DateTime MaxDate;
        DateTime MinDate;

        private void B9Prime_SortFilesByDate_Click(object sender, EventArgs e)
        {
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Test0_20180320\2) Täglicher Betrieb";
            string targetPath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Test0_20180320\2) Täglicher Betrieb - Sorted";

            string[] allFilesPaths = Directory.GetFiles(sourcePath);

            SetMinDateAndMaxDate(allFilesPaths);
            List<Time> workingDates = Repo.GetWorkingDaysBetween(MinDate, MaxDate);

            foreach (Time day in workingDates)
            {
                string date = day.Date.Value.ToString("yyyyMMdd");
                string newFolder = targetPath + @"\" + date;
                DirectoryInfo folder = Directory.CreateDirectory(newFolder);

                foreach (FileInfo file in folder.GetFiles())
                {
                    file.Delete();
                }

                string[] filesToCopy = Directory.GetFiles(sourcePath, "*" + date + "*");

                foreach (string fileToCopy in filesToCopy)
                {
                    string existingPath = Path.GetFullPath(fileToCopy);
                    string newPath = targetPath + @"\" + date + @"\" + Path.GetFileName(fileToCopy);
                    File.Copy(existingPath, newPath);
                }
            }
        }

        private void B14_FindEmpthyFolders_Click(object sender, EventArgs e)
        {
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Test0_20180320\2) Täglicher Betrieb - Sorted";

            string[] folders = Directory.GetDirectories(sourcePath);
            foreach (string folder in folders)
            {
                string[] files = Directory.GetFiles(folder);

                if (files.Length == 0)
                {
                    MessageBox.Show("Empthy folder " + folder);
                }
            }

        }

        private void SetMinDateAndMaxDate(string[] allFilesPaths)
        {
            this.MaxDate = DateTime.MinValue;
            this.MinDate = DateTime.MaxValue;

            foreach (string file in allFilesPaths)
            {
                DateTime date = GetDateFromDekaFilePath(file);

                if (MaxDate < date)
                {
                    MaxDate = date;
                }
                if (MinDate > date)
                {
                    MinDate = date;
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
            string sourcePath = @"C:\Users\daniel.cojocaru\Desktop\Deka\Daten\Deka Produktiv\2) Täglicher Betrieb";
            List<string> names = new List<string>() {"ASD", "FSD", "FTK", "ALD", "LOT", "DIV", "MCH", "WAER", "NAH", "TAD" };

            List<string> files = Directory.GetFiles(sourcePath).ToList();
            List<DateTime> dates = new List<DateTime>();

            foreach (var file in files)
            {
                DateTime date = GetDateFromDekaFilePath(file);
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

            bool allFilesAreThere = missingFiles.Length != 0;

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

            DateTime start = DateTime.ParseExact("20180120", dateFormat, CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact("20180314", dateFormat, CultureInfo.InvariantCulture);

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
