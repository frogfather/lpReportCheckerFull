﻿using System;
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
        }

        private void Form1_Load(object sender, EventArgs e)
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
                            listBox1.Items.Add("File " + fileInfo.Name + " out of date: " + fileInfo.LastWriteTime);
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



                }                

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

    }
}
