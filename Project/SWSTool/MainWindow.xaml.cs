using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using DbscanImplementation;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using TermRules;
using Application = Microsoft.Office.Interop.Word.Application;
using Style = System.Windows.Style;
using Window = System.Windows.Window;

//using System.Windows.Forms;

namespace SWStool
{
    public class tableTerm
    {
        [DisplayName("Термин")]
        public string term { get; set; }

        public tableTerm()
        {
            term = "";
        }
    }
    public class indexTerm : tableTerm
    {
        [DisplayName("Страницы")]
        public string pages { get; set; }

        public indexTerm()
        {
            pages = "";
            term = "";
        }
    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TmpPath = Path.GetTempPath();
            Directory.CreateDirectory(TmpPath + "\\" + FolderPath);
            ProgrammTmpPath = TmpPath + "\\" + FolderPath;
            ChangeState(curState);
            CoreTermDG.ItemsSource = coreTerms;
            ExtractedTermsDG.ItemsSource = extractedTerms;
            TermsDG.ItemsSource = indexTerms;
        }

        private int curState = 0;
        public string TmpPath = "";
        public string FolderPath = "SWStoolTmp";
        public string ProgrammTmpPath = "";
        private Terms mainTermsAr;
        private List<tableTerm> coreTerms = new List<tableTerm>();
        private List<tableTerm> extractedTerms = new List<tableTerm>();
        private List<indexTerm> indexTerms = new List<indexTerm>();
        // Открытие документа в DocumentView-----------------------------------------------------
        public string TextFileName = "";
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            var dlg = new OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".docx";
            dlg.Filter = "Word documents (.docx)|*.docx";

            // Display OpenFileDialog by calling ShowDialog method
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a DocumentView
            if (result == true)
            {
                if (dlg.FileName.Length > 0)
                {
                    TextFileName = dlg.FileName;
                    //SelectedFileTextBox.Text = dlg.FileName;
                    //string newXPSDocumentName = String.Concat(System.IO.Path.GetDirectoryName(dlg.FileName), "\\",
                    //               System.IO.Path.GetFileNameWithoutExtension(dlg.FileName), ".xps");
                    var newXpsDocumentName = String.Concat(ProgrammTmpPath, "\\", Path.GetFileNameWithoutExtension(dlg.FileName), ".xps");
                    // Set DocumentViewer.Document to XPS document
                    DocViewer.Document = ConvertWordDocToXpsDoc(dlg.FileName, newXpsDocumentName).GetFixedDocumentSequence();
                }
            }
            StartButton.IsEnabled = true;
        }
        private XpsDocument ConvertWordDocToXpsDoc(string wordDocName, string xpsDocName)
        {
            // Create a WordApplication and add Document to it
            var
                wordApplication = new Application();
            wordApplication.Documents.Add(wordDocName);


            var doc = wordApplication.ActiveDocument;
            // You must ensure you have Microsoft.Office.Interop.Word.Dll version 12.
            // Version 11 or previous versions do not have WdSaveFormat.wdFormatXPS option
            try
            {
                doc.SaveAs(xpsDocName, WdSaveFormat.wdFormatXPS);
                wordApplication.Quit();

                var xpsDoc = new XpsDocument(xpsDocName, FileAccess.Read);
                return xpsDoc;
            }
            catch (Exception exp)
            {
                var str = exp.Message;
            }
            return null;
        }
        //---------------------------------------------------------------------------------------

        // Постраничное извлечение текста из Docx документа -------------------------------------


        //---------------------------------------------------------------------------------------

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //TextFileName = "C:\\Users\\Kir\\Desktop\\Test\\Пособие Лисп финал.docx";
            ////SelectedFileTextBox.Text = dlg.FileName;
            ////string newXPSDocumentName = String.Concat(System.IO.Path.GetDirectoryName(dlg.FileName), "\\",
            ////               System.IO.Path.GetFileNameWithoutExtension(dlg.FileName), ".xps");
            var newXpsDocumentName = String.Concat(ProgrammTmpPath, "\\", Path.GetFileNameWithoutExtension("C:\\Users\\Kir\\Desktop\\Test\\Программир_Пролог_all_final.docx"), ".xps");
            // Set DocumentViewer.Document to XPS document
            DocViewer.Document = ConvertWordDocToXpsDoc("C:\\Users\\Kir\\Desktop\\Test\\Программир_Пролог_all_final.docx", newXpsDocumentName).GetFixedDocumentSequence();


            //Rules rules = new Rules(textFileName, DictionaryF.IT_TERM,0, 5, true);      
            //Rules rules = new Rules("C:\\Users\\Kir\\Desktop\\test.docx", DictionaryF.IT_TERM, 1, 5, true);
            //Terms mainTermsAr = rules.ApplyRules();Glava3N_3.docx
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Пособие Лисп финал.docx", DictionaryF.ItTerm, 2, 101, true);
            //mainTermsAr = getter.GetMainTermsAr();
            //foreach (var term in mainTermsAr.TermsAr)
            //{
            //    if (term.Kind == KindOfTerm.AuthTerm)
            //    {
            //        extractedTerms.Add(new tableTerm() {term = term.TermWord});
            //    }
            //    string pagesString = "";
            //    foreach (var cluster in term.pages)
            //    {
            //        if (cluster.Length == 1)
            //            pagesString += cluster[0].number.ToString() + ", ";
            //        else
            //        {
            //            List<MyCustomDatasetItem> pages = new List<MyCustomDatasetItem>();
            //            pages = cluster.ToList().OrderBy(item => item.number).ToList();
            //            if (pages[0].number == pages[pages.Count() - 1].number)
            //                pagesString += pages[0].number.ToString() + ", ";
            //            else
            //                pagesString += pages[0].number.ToString() + "-" + pages[pages.Count() - 1].number.ToString() + ", ";
            //        }
            //    }
            //    pagesString = pagesString.Trim().Substring(0, pagesString.Length - 2).Trim();
            //    indexTerms.Add(new indexTerm() {term = term.TermWord, pages = pagesString});
            //}
            //TermsDG.Items.Refresh();
            //ExtractedTermsDG.Items.Refresh();
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Пособие Лисп финал.docx", DictionaryF.ItTerm, 78, 101, true);
            var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Пособие Лисп финал.docx", DictionaryF.ItTerm, 2, 101, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Пособие Лисп финал.docx", DictionaryF.ItTerm, 2, 101, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Пособие Лисп финал.docx", DictionaryF.ItTerm, 2, 101, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Пособие Лисп финал.docx", DictionaryF.ItTerm, 2, 101, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Пособие Лисп финал.docx", DictionaryF.ItTerm, 2, 101, true);

            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Солопов Ю.И. Лекции по дискретной математике учебное пособие.docx", DictionaryF.ItTerm, 4, 117, true);

            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Пособие по Рефалу_Итог.docx", DictionaryF.ItTerm, 2, 60, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Написание-Оформление_7.docx", DictionaryF.ItTerm, 2, 52, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Программир_Пролог_all_final.docx", DictionaryF.ItTerm, 2, 85, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Системы_программир_учеб_пособие.docx", DictionaryF.ItTerm, 5, 81, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Форм_грамматики_Элем_трансляции (02_2010).docx", DictionaryF.ItTerm, 3, 90, true);

            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Hsearch_Final_Glava1N_4.docx", DictionaryF.ItTerm, 1, 15, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Hsearch_Final_Glava2N_3.docx", DictionaryF.ItTerm, 1, 34, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Hsearch_Final_Glava3N_3.docx", DictionaryF.ItTerm, 1, 15, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Hsearch_Final.docx", DictionaryF.ItTerm, 1, 63, true);

            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Refal\\1 Базисный Рефал.docx", DictionaryF.ItTerm, 1, 22, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Refal\\2 Язык Рефал-2.docx", DictionaryF.ItTerm, 1, 21, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Refal\\3 Язык Рефал-5.docx", DictionaryF.ItTerm, 1, 14, true);

            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Форм_грамматики_Элем_трансляции (02_2010)\\Элементы теории трансляции.docx", DictionaryF.ItTerm, 1, 71, true);
            //var getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Форм_грамматики_Элем_трансляции (02_2010)\\Элементы теории формальных языков и грамматик.docx", DictionaryF.ItTerm, 1, 18, true);

            //IndexAndGlossary getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Glava3N_3.docx", DictionaryF.F_TERM, 1, 15, true);
            var glossary = getter.GetGlossary();
            var coll = new ObservableCollection<GlossaryItem>(glossary);
            GlossaryGrid.ItemsSource = coll;
            GlossaryGrid.Items.Refresh();
            var index = getter.GetIndexAr();
            mainTermsAr = getter.GetMainTermsAr();
            var curLetterBlock = new TreeViewItem();
            curLetterBlock.Header = index[0].Term[0].ToString().ToUpper();
            //curLetterBlock.ItemsSource = new string[] { "Monitor", "CPU", "Mouse" };
            foreach (var item in index)
            {
                if (item.Term[0].ToString().ToUpper().CompareTo(curLetterBlock.Header) != 0)
                {
                    IndexTreeView.Items.Add(curLetterBlock);
                    curLetterBlock = new TreeViewItem();
                    curLetterBlock.Header = item.Term[0].ToString().ToUpper();
                }
                var newTerm = new TreeViewItem();
                newTerm.Header = item.Term;
                newTerm.ItemsSource = item.SupportTerms;
                curLetterBlock.Items.Add(newTerm);
            }
            IndexTreeView.Items.Add(curLetterBlock);
            IndexTreeView.Items.Refresh();
        }

        private void NextBTN_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as FrameworkElement).Tag as string;
            switch (tag)
            {
                case "Next":
                    {
                        ChangeState(++curState);
                        break;
                    }
            }
        }

        private void ChangeState(int state)
        {
            switch (state)
            {
                case 1:
                    {
                        StageOneLBI.IsSelected = true;
                        break;
                    }
                case 2:
                    {
                        StageTwoLBI.IsSelected = true;
                        break;
                    }
                case 3:
                    {
                        StageThreeLBI.IsSelected = true;
                        break;
                    }
                case 4:
                    {
                        StageFourLBI.IsSelected = true;
                        break;
                    }
                case 5:
                    {
                        FinalStageLBI.IsSelected = true;
                        break;
                    }
                default:
                    {
                        CreateProjectGrid.Visibility = Visibility.Hidden;
                        TermsExtractionSettingsGrid.Visibility = Visibility.Hidden;
                        ExtractedTermsGrid.Visibility = Visibility.Hidden;
                        PagesDetectionGrid.Visibility = Visibility.Hidden;
                        FinalGrid.Visibility = Visibility.Visible;
                        FinalGrid.IsEnabled = false;
                        break;
                    }
            }
        }

        private void NewProjectMI_Click(object sender, RoutedEventArgs e)
        {
            ChangeState(++curState);
        }

        private void LoadMI_Click(object sender, RoutedEventArgs e)
        {
            StartButton_Click(sender, e);
        }

        private void StagesLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = (sender as ListBox).SelectedIndex;
            switch (selectedIndex)
            {
                case 0:
                    {
                        curState = 1;
                        CreateProjectGrid.Visibility = Visibility.Visible;
                        TermsExtractionSettingsGrid.Visibility = Visibility.Hidden;
                        ExtractedTermsGrid.Visibility = Visibility.Hidden;
                        PagesDetectionGrid.Visibility = Visibility.Hidden;
                        FinalGrid.Visibility = Visibility.Hidden;
                        break;
                    }
                case 1:
                    {
                        curState = 2;
                        CreateProjectGrid.Visibility = Visibility.Hidden;
                        TermsExtractionSettingsGrid.Visibility = Visibility.Visible;
                        ExtractedTermsGrid.Visibility = Visibility.Hidden;
                        PagesDetectionGrid.Visibility = Visibility.Hidden;
                        FinalGrid.Visibility = Visibility.Hidden;
                        break;
                    }
                case 2:
                    {
                        curState = 3;
                        CreateProjectGrid.Visibility = Visibility.Hidden;
                        TermsExtractionSettingsGrid.Visibility = Visibility.Hidden;
                        ExtractedTermsGrid.Visibility = Visibility.Visible;
                        PagesDetectionGrid.Visibility = Visibility.Hidden;
                        FinalGrid.Visibility = Visibility.Hidden;
                        break;
                    }
                case 3:
                    {
                        curState = 4;
                        CreateProjectGrid.Visibility = Visibility.Hidden;
                        TermsExtractionSettingsGrid.Visibility = Visibility.Hidden;
                        ExtractedTermsGrid.Visibility = Visibility.Hidden;
                        PagesDetectionGrid.Visibility = Visibility.Visible;
                        FinalGrid.Visibility = Visibility.Hidden;
                        break;
                    }
                case 4:
                    {
                        curState = 5;
                        CreateProjectGrid.Visibility = Visibility.Hidden;
                        TermsExtractionSettingsGrid.Visibility = Visibility.Hidden;
                        ExtractedTermsGrid.Visibility = Visibility.Hidden;
                        PagesDetectionGrid.Visibility = Visibility.Hidden;
                        FinalGrid.Visibility = Visibility.Visible;
                        //TermsDG.Items.Add(new ListViewItem());
                        FinalGrid.IsEnabled = true;
                        break;
                    }
            }
        }

        private void TermsDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var element = e.EditingElement as TextBox;
            var text = element.Text.Trim();
            int ind = indexTerms.FindIndex(item => item.term == text);
            if (ind != -1)
            {
                MessageBox.Show("Такой термин уже есть в списке! Он будет удален.");
                e.Cancel = true;
            }
        }

        private void ExtractedTermsDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var element = e.EditingElement as TextBox;
            var text = element.Text.Trim();
            int ind = extractedTerms.FindIndex(item => item.term == text);
            if (ind != -1)
            {
                MessageBox.Show("Такой термин уже есть в списке! Он будет удален.");
                e.Cancel = true;
            }
        }

        private void CoreTermDG_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var element = e.EditingElement as TextBox;
            var text = element.Text.Trim();
            int ind = coreTerms.FindIndex(item => item.term == text);
            if (ind != -1)
            {
                MessageBox.Show("Такой термин уже есть в списке! Он будет удален.");
                e.Cancel = true;
            }
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var displayName = GetPropertyDisplayName(e.PropertyDescriptor);

            if (!string.IsNullOrEmpty(displayName))
            {
                e.Column.Header = displayName;
            }

        }

        public static string GetPropertyDisplayName(object descriptor)
        {
            var pd = descriptor as PropertyDescriptor;

            if (pd != null)
            {
                // Check for DisplayName attribute and set the column header accordingly
                var displayName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                if (displayName != null && displayName != DisplayNameAttribute.Default)
                {
                    return displayName.DisplayName;
                }

            }
            else
            {
                var pi = descriptor as PropertyInfo;

                if (pi != null)
                {
                    // Check for DisplayName attribute and set the column header accordingly
                    Object[] attributes = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    for (int i = 0; i < attributes.Length; ++i)
                    {
                        var displayName = attributes[i] as DisplayNameAttribute;
                        if (displayName != null && displayName != DisplayNameAttribute.Default)
                        {
                            return displayName.DisplayName;
                        }
                    }
                }
            }

            return null;
        }

        private void delCoreBTN_Click(object sender, RoutedEventArgs e)
        {
            var selected = CoreTermDG.SelectedItems;
            foreach (var element in selected)
            {
                coreTerms.Remove(element as tableTerm);
            }
            CoreTermDG.Items.Refresh();
        }

        private void delExtractedTermsBTN_Click(object sender, RoutedEventArgs e)
        {
            var selected = ExtractedTermsDG.SelectedItems;
            foreach (var element in selected)
            {
                extractedTerms.Remove(element as tableTerm);
            }
            ExtractedTermsDG.Items.Refresh();
        }

        private void delIndexTermsBTN_Click(object sender, RoutedEventArgs e)
        {
            var selected = TermsDG.SelectedItems;
            foreach (var element in selected)
            {
                indexTerms.Remove(element as indexTerm);
            }
            TermsDG.Items.Refresh();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }
    }
}



//StringBuilder text = new StringBuilder();  
//ReadTextFromDocxByPage(@"C:\Users\Kir\Desktop\Документы\Отчет Иванов.docx");
//string inputFile = ProgrammTmpPath + "\\TextA.txt";
//            try
//            {
//                using (StreamWriter sw = new StreamWriter(inputFile, false, System.Text.Encoding.GetEncoding("Windows-1251")))
//                {
//                    sw.WriteLine(text);
//                    sw.Close();
//                }
//            }
//            catch (Exception exp)
//            {
//                using (StreamWriter sw = new StreamWriter(ProgrammTmpPath + "\\ExeptionLog.txt", false, System.Text.Encoding.GetEncoding("Windows-1251")))
//                {
//                    sw.WriteLine(exp.Message);
//                    sw.Close();
//                }
//            }