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
using MySql.Data.MySqlClient;

namespace FileDB
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        string strConn = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (fileTreeView.SelectedItem == null)
            {
                return;
            }

        }

        private void File_Delete(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not Implement sry;;");
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            if (fileTreeView.SelectedItem == null)
            {
                return;
            }
            DB_Upload(((RecipeParam)((TreeViewItem)fileTreeView.SelectedItem).Tag));
        }

        private void DB_Upload(RecipeParam param)
        {
            string queryForm =
                @"INSERT INTO recipe.spec
                    (cluster_recipe,
                    frontside_recipe,
                    inspection_dies,
                    inspection_columns,
                    inspection_rows)
                VALUES(
                    '{0}', '{1}', '{2}', '{3}', '{4}')
                ON
                    duplicate
                KEY UPDATE
                    modify_date = CURDATE();";
            strConn = string.Format("server={0};uid={1};pwd={2};database={3};charset=utf8 ;",
                                    "10.21.11.210", "aoi", "abc123**", "recipe");
            string query = string.Format(queryForm,
                                        param.Ppid,
                                        param.FrontsideRecipe,
                                        param.InsepctionDies,
                                        param.InspectionColumns,
                                        param.InspectionRows);
            MySqlConnection conn = new MySqlConnection(strConn);
            MySqlCommand cmd = new MySqlCommand(query, conn);
            conn.Open();
            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show($"{param.Ppid} Upload Success!!");
            }
            conn.Close();
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
                item.Tag = GetReciepInfo(file);
                item.Expanded += new RoutedEventHandler((object _sender, RoutedEventArgs _e)=> ShowRecipeInfo((RecipeParam)item.Tag));
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

        private RecipeParam GetReciepInfo(FileInfo file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);

            XmlNode _ppid = doc.SelectSingleNode("Root/PPID");
            XmlNode _recipe = doc.SelectSingleNode("Root/FrontsideRecipe/RecipeName");
            XmlNode _inspectionDies = doc.SelectSingleNode("Root/FrontsideRecipe/TestableDies");
            XmlNode _inspectionColumns = doc.SelectSingleNode("Root/FrontsideRecipe/ColumnNumber");
            XmlNode _inspectionRows = doc.SelectSingleNode("Root/FrontsideRecipe/RowNumber");

            return new RecipeParam
            {
                Ppid = _ppid.InnerText,
                FrontsideRecipe = _recipe.InnerText,
                InsepctionDies = _inspectionDies.InnerText,
                InspectionColumns = _inspectionColumns.InnerText,
                InspectionRows = _inspectionRows.InnerText
            };
        }

        private void ShowRecipeInfo(RecipeParam param)
        {
            string recipeStr = string.Empty;
            recipeStr += string.Format("ClusterRecipe : {0}{1}", param.Ppid, Environment.NewLine);
            recipeStr += string.Format("FrontsideRecipe : {0}{1}", param.FrontsideRecipe, Environment.NewLine);
            recipeStr += string.Format("InspectionDies : {0}{1}", param.InsepctionDies, Environment.NewLine);
            recipeStr += string.Format("InspectionColumns : {0}{1}", param.InspectionColumns, Environment.NewLine);
            recipeStr += string.Format("InspectionRows : {0}{1}", param.InspectionRows, Environment.NewLine);
            recipeInfo.Text = recipeStr;
        }

        internal class RecipeParam
        {
            string ppid;
            public string Ppid
            {
                set
                {
                    this.ppid = InputReplace(value);
                }
                get
                {
                    return this.ppid;
                }
            }

            string frontsideRecipe;
            public string FrontsideRecipe
            {
                set
                {
                    this.frontsideRecipe = InputReplace(value);
                }
                get
                {
                    return this.frontsideRecipe;
                }
            }

            string inspectionDies;
            public string InsepctionDies
            {
                set
                {
                    this.inspectionDies = InputReplace(value);
                }
                get
                {
                    return this.inspectionDies;
                }
            }

            string inspectionColumns;
            public string InspectionColumns
            {
                set
                {
                    this.inspectionColumns = InputReplace(value);
                }
                get
                {
                    return this.inspectionColumns;
                }
            }

            string inspectionRows;
            public string InspectionRows
            {
                set
                {
                    this.inspectionRows = InputReplace(value);
                }
                get
                {
                    return this.inspectionRows;
                }
            }


            private string InputReplace(string str)
            {
                return str.Replace('\\', '/');
            }
        }
    }
}
