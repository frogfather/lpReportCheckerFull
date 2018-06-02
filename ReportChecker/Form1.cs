using System;
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
            tvResults.MouseDown += new MouseEventHandler(tvResults_MouseDown);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void tvResults_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode currentNode = tvResults.GetNodeAt(e.X, e.Y);
            if (currentNode != null)
            {
                tvResults.SelectedNode = currentNode;
            }
            
            if (currentNode != null && currentNode.Parent != null && currentNode.Parent.Parent != null)
            {
                if (currentNode.Nodes.Count == 0 && currentNode.Parent.Parent.Text.ToUpper() == "EMAIL")
                {                    
                    if (e.Button == System.Windows.Forms.MouseButtons.Right) 
                    {
                        //need to check if this error message is in the ignored list                                                

                        showAlertsToolStripMenuItem.Enabled = mainDashboard.InIgnoredList(currentNode.Text);
                        hideAlertsToolStripMenuItem.Enabled = !showAlertsToolStripMenuItem.Enabled;
                        Console.WriteLine("in mouseDown " + Cursor.Position.X + Cursor.Position.Y);
                        System.Drawing.Point mPos = new System.Drawing.Point();
                        mPos = Cursor.Position;                                            
                        mnuAlerts.Show(mPos);
                    }
                    
                    
                }
            }                            
        }

    
        private void hideAlertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.ToolStripMenuItem)
            {
                System.Windows.Forms.ToolStripMenuItem source = (System.Windows.Forms.ToolStripMenuItem)sender;
                if (source.Text == "Hide Alerts")
                {                    
                    mainDashboard.AddToIgnoredList(tvResults.SelectedNode.Text);
                    Console.WriteLine(tvResults.SelectedNode.Text + " add to ignored list");

                }
                else
                {
                    mainDashboard.RemoveFromIgnoredList(tvResults.SelectedNode.Text);
                    Console.WriteLine(tvResults.SelectedNode.Text + " removed from ignored list");
                }
            }
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
                for (int scriptNo = 0; scriptNo < scriptCount; scriptNo ++)
                {
                    //for each script we need a list of success email codes, success dash codes, fail email codes, fail dash codes
                    //we have a node for success and fail if there are any results                    
                    int dashSuccessCount = mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetDashSuccessCount();
                    int dashFailCount = mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetDashFailCount();
                    int emailSuccessCount = mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetEmailSuccessCount();
                    int emailFailCount = mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetEmailFailCount();

                    scriptArray[scriptNo] = new TreeNode(mainDashboard.GetFacility(facNo).GetScript(scriptNo).Name);
                    if (dashSuccessCount >0 || emailSuccessCount > 0)
                    {                       
                        TreeNode successCodes = new TreeNode("Success codes");
                        if(dashSuccessCount > 0)
                        {
                            TreeNode dashSuccessCodes = new TreeNode("Dashboard");
                            successCodes.Nodes.Add(dashSuccessCodes);
                            for(int i = 0; i< dashSuccessCount;i++)
                            {
                                dashSuccessCodes.Nodes.Add(mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetDashSuccess(i));
                            }
                        }
                        if(emailSuccessCount > 0)
                        {
                            TreeNode emailSuccessCodes = new TreeNode("Email");
                            successCodes.Nodes.Add(emailSuccessCodes);
                            for (int i = 0; i < emailSuccessCount; i++)
                            {
                                emailSuccessCodes.Nodes.Add(mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetEmailSuccess(i));
                            }

                        }

                        if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).SuccessCountMatch == false) { successCodes.ImageIndex = 1;}
                        successCodes.SelectedImageIndex = successCodes.ImageIndex;
                        scriptArray[scriptNo].Nodes.Add(successCodes);

                    }
                    if (dashFailCount > 0 || emailFailCount > 0)
                    {
                        TreeNode failCodes = new TreeNode("Fail codes");
                        if (dashFailCount>0)
                        {
                            TreeNode dashFailCodes = new TreeNode("Dashboard");
                            failCodes.Nodes.Add(dashFailCodes);
                            for (int i = 0; i < dashFailCount; i++)
                            {
                                dashFailCodes.Nodes.Add(mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetDashFail(i));
                            }

                        }
                        if (emailFailCount>0)
                        {
                            TreeNode emailFailCodes = new TreeNode("Email");
                            failCodes.Nodes.Add(emailFailCodes);
                            for (int i = 0; i < emailFailCount; i++)
                            {
                                string failCode = mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetEmailFailCode(i);
                                TreeNode emailFailCode = new TreeNode(failCode);
                                emailFailCode.Nodes.Add(mainDashboard.GetFacility(facNo).GetScript(scriptNo).GetEmailFailMessage(failCode));
                                emailFailCodes.Nodes.Add(emailFailCode);
                            }

                        }

                        if (mainDashboard.GetFacility(facNo).GetScript(scriptNo).FailCountMatch == false) { failCodes.ImageIndex = 1; }
                        failCodes.SelectedImageIndex = failCodes.ImageIndex;
                        scriptArray[scriptNo].Nodes.Add(failCodes);
                    }



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
            fr.ReadHTML(path + fileName);
            mainDashboard.ProcessEmails(fr.GetResults(), fr.FacilityName, fr.Success);            
        }

        Dashboard mainDashboard;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainDashboard.SaveIgnoredList();
        }
    }
}
