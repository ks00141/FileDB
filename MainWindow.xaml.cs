using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Xml;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileDB
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Show_FileDialog(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog codf = new CommonOpenFileDialog()
            {
                IsFolderPicker = true
            };
            if(codf.ShowDialog() == CommonFileDialogResult.Ok)
            {
                folderPath.Text = codf.FileName;
                Set_FileTreeView(codf.FileName);
            }
            
        }

        private void Set_FileTreeView(string filePath)
        {
           fileTreeView.Items.Clear();
            DirectoryInfo di = new DirectoryInfo(filePath);
            FileInfo[] files = di.GetFiles("*.xml");
            foreach(FileInfo file in files)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = TranslateFileName(file);
                item.Expanded += new RoutedEventHandler((object _sender, RoutedEventArgs _e)=>ShowRecipeInfo(file));
                fileTreeView.Items.Add(item);
            }
        }

        private string TranslateFileName(FileInfo File)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(File.FullName);

                XmlNode _ppid = doc.SelectSingleNode("Root/PPID");
                return _ppid.InnerText;
            }
            catch
            {
                return File.Name;
            }
        }

        private void ShowRecipeInfo(FileInfo file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);

            XmlNode _ppid = doc.SelectSingleNode("Root/PPID");
            XmlNode _recipe = doc.SelectSingleNode("Root/FrontsideRecipe/RecipeName");
            XmlNode _inspectionDies = doc.SelectSingleNode("Root/FrontsideRecipe/TestableDies");
            XmlNode _inspectionColumns = doc.SelectSingleNode("Root/FrontsideRecipe/ColumnNumber");
            XmlNode _inspectionRows = doc.SelectSingleNode("Root/FrontsideRecipe/RowNumber");

            string recipeStr = string.Empty;
            recipeStr += string.Format("ClusterRecipe : {0}{1}", _ppid.InnerText, Environment.NewLine);
            recipeStr += string.Format("FrontsideRecipe : {0}{1}", _recipe.InnerText, Environment.NewLine);
            recipeStr += string.Format("InspectionDies : {0}{1}", _inspectionDies.InnerText, Environment.NewLine);
            recipeStr += string.Format("InspectionColumns : {0}{1}", _inspectionColumns.InnerText, Environment.NewLine);
            recipeStr += string.Format("InspectionRows : {0}{1}", _inspectionRows.InnerText, Environment.NewLine);
            recipeInfo.Text = recipeStr;
        }
    }
}
