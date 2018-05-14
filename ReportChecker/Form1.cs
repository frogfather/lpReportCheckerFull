using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace ReportChecker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            mainDashboard = new Dashboard();
            mainDashboard.DashFlagsChanged += ReportDashFlags;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ReportDashFlags(object sender, MatchEventArgs args)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName;
            string directoryPath;
            DateTime fileDate;
            listBox1.Items.Clear();
            fileName = OpenFile();
            if (fileName != "")
            {
                directoryPath = GetDirectoryFromFileName(fileName);
                //check the date of the selected file
                fileDate = GetFileDate(fileName);
                if (fileDate != Convert.ToDateTime("11/11/2222"))
                {
                    listBox1.Items.Add("Selected date "+ Convert.ToString(fileDate));
                    listBox1.Items.Add(directoryPath);
                    //now iterate through the files in the directory ignoring ones that are different days to the selected file
                    DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
                    System.IO.FileInfo[] fileInfoList = dirInfo.GetFiles("*.*");
                    
                    foreach(System.IO.FileInfo fileInfo in fileInfoList)
                    {
                        //compare its date to the selected file date. Ignore files that aren't either .html or .xlsx
                        if (fileInfo.LastWriteTime.Date != fileDate.Date || fileInfo.Attributes.HasFlag(FileAttributes.Hidden)) 
                        {
                            //listBox1.Items.Add("File " + fileInfo.Name + " out of date: " + fileInfo.LastWriteTime); maybe move it to an 'old files folder?
                        }
                        else
                        {
                            if (fileInfo.Extension == ".xlsx") 
                            {
                                ReadExcel(directoryPath,fileInfo.Name);
                            }
                            else if (fileInfo.Extension == ".html")
                            {
                                ReadHTML(directoryPath, fileInfo.Name);
                            }
                            else
                            {
                                //listBox1.Items.Add("File " + fileInfo.Name + " invalid extension");
                            }
                        }
                        
                    }

                    CheckAllFlags();
                    GenerateTreeResults();

                }                

            }
            
        }

        private void CheckAllFlags()
        {
            //should poll all facilities here
            int facilityCount = mainDashboard.GetFacilityCount();
            for (int i = 0; i < facilityCount; i++)
            {
                if (mainDashboard.GetFacility(i).FacilitySuccessMatch == false)
                {
                    listBox1.Items.Add("Facility " + mainDashboard.GetFacility(i).Name + " Success emails do not match dashboard");
                }
                if (mainDashboard.GetFacility(i).FacilityFailMatch == false)
                {
                    listBox1.Items.Add("Facility " + mainDashboard.GetFacility(i).Name + " Failure emails do not match dashboard");
                }
                if (mainDashboard.GetFacility(i).FacilityISMError == true)
                {
                    listBox1.Items.Add("Facility " + mainDashboard.GetFacility(i).Name + " ISM error at this facility");
                }
                if (mainDashboard.GetFacility(i).FacilityScriptError == true)
                {
                    listBox1.Items.Add("Facility " + mainDashboard.GetFacility(i).Name + " Script error at this facility");
                }
                

            }
        }

        private void GenerateTreeResults()
        {
            tvResults.Nodes.Clear();
            int facilityCount = mainDashboard.GetFacilityCount();
            for (int facNo = 0; facNo < facilityCount; facNo ++)
            {
                //generate array of scripts
                int scriptCount = mainDashboard.GetFacility(facNo).GetScriptCount();
                TreeNode[] scriptArray = new TreeNode[scriptCount];
                TreeNode[] resultArray;
                for (int scriptNo = 0; scriptNo < scriptCount; scriptNo ++)
                {
                    //for each script we need a list of success email codes, success dash codes, fail email codes, fail dash codes
                    //we have a node for success and fail if there are any results
                    int resultTypeCount = 0; 
                    if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetDashSuccessCount() > 0 || mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetEmailSuccessCount() > 0) { resultTypeCount += 1; }
                    if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetDashFailCount() > 0 || mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetEmailFailCount() > 0) { resultTypeCount += 1; }
                    
                    if (resultTypeCount > 0)
                    {
                        resultArray = new TreeNode[resultTypeCount];
                        if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetEmailSuccessCount() > 0 || mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetDashSuccessCount() > 0)
                        {
                            resultArray[0] = new TreeNode("Success codes");
                            if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).SuccessCountMatch == false) { resultArray[0].ImageIndex = 1; }
                            resultArray[0].SelectedImageIndex = resultArray[0].ImageIndex;
                        }
                        if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetEmailFailCount() > 0 || mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetDashFailCount() > 0)
                        {
                            if (resultTypeCount == 2)
                            {
                                resultArray[1] = new TreeNode("Fail codes");
                                if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).FailCountMatch == false) { resultArray[1].ImageIndex = 1; }
                                resultArray[1].SelectedImageIndex = resultArray[1].ImageIndex;
                            }
                            else
                            {
                                resultArray[0] = new TreeNode("Fail codes");
                                if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).FailCountMatch == false) { resultArray[0].ImageIndex = 1; }
                            }
                        }

                    }
                    else
                    {
                        resultArray = new TreeNode[1];
                        resultArray[0] = new TreeNode("No charge codes posted");
                    }

                    scriptArray[scriptNo] = new TreeNode(mainDashboard.GetFacility(facNo).GetScript(scriptNo).Name,resultArray);                    
                    if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).ISMError == true) { scriptArray[scriptNo].ImageIndex = 1; }
                    if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).ScriptError == true) { scriptArray[scriptNo].ImageIndex = 1; }
                    if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).FailCountMatch == false) { scriptArray[scriptNo].ImageIndex = 1; }
                    if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).SuccessCountMatch == false) { scriptArray[scriptNo].ImageIndex = 1; }
                    scriptArray[scriptNo].SelectedImageIndex = scriptArray[scriptNo].ImageIndex;
                }

                TreeNode treeNode = new TreeNode(mainDashboard.GetFacility(facNo).Name, scriptArray); 
                if (mainDashboard.GetFacility(facNo).FacilityISMError == true) { treeNode.ImageIndex = 1; }
                if (mainDashboard.GetFacility(facNo).FacilityScriptError == true) { treeNode.ImageIndex = 1; }
                if (mainDashboard.GetFacility(facNo).FacilityFailMatch == false) { treeNode.ImageIndex = 1; }
                if (mainDashboard.GetFacility(facNo).FacilitySuccessMatch == false) { treeNode.ImageIndex = 1; }
                treeNode.SelectedImageIndex = treeNode.ImageIndex;
                tvResults.Nodes.Add(treeNode);

            }
        }

        private string OpenFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = "C:\\";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                return fileDialog.FileName;
            }
            else
            {
                return "";
            }

        }

        private String GetDirectoryFromFileName(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf("\\") + 1);
        }
        private DateTime GetFileDate(string fileName)
        {
            if (File.Exists(fileName))
            {
                return File.GetLastWriteTime(fileName);
            }
            else
            {
                return Convert.ToDateTime("11/11/2222");
            }
        }
        private void ReadExcel(string path, string excelName)
        {
            //this should return a collection of strings that other methods can process

            ExcelReader exReader = new ExcelReader(path, excelName);
            exReader.ReadExcel();
            mainDashboard.ProcessDashboard(exReader.GetAllResults());            
            //destroy the excel reader here.
        }

        private void ReadHTML(string path, string fileName)
        {
            FileReader fr = new FileReader();
            fr.ReadFile(path + fileName);
            mainDashboard.ProcessEmails(fr.GetResults(), fr.FacilityName, fr.Success);            
        }

        Dashboard mainDashboard;

        private void tvResults_MouseClick(object sender, MouseEventArgs e)
        {
            
        }
    }
}
