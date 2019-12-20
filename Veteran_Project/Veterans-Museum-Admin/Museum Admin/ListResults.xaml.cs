using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Museum_Admin
{
    /// <summary>
    /// Interaction logic for ListResults.xaml
    /// </summary>
    public partial class ListResults : UserControl
    {
        // This class recieves a query string and a results list of strings
        // (The results list may be empty if the answer to the query is in the query string)
        // It shows the results to the user via the UI and allows formatted printing of the information

        private List<string> results;
        public string Query { get; }

        public ListResults(string queryString, List<string> resultsList)
        {
            InitializeComponent();

            DataContext = this;

            Query = queryString;
            results = resultsList;

            BuildResultsList();
        }

        // Turns the list of results into a flow document and prints it
        // Sources:
        // https://www.c-sharpcorner.com/uploadfile/mahesh/printing-in-wpf/
        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.documents.flowdocument?view=netframework-4.8
        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.documents.list.listitems?view=netframework-4.8#System_Windows_Documents_List_ListItems
        // https://stackoverflow.com/questions/38213327/wpf-flowdocument-printing-only-to-small-area
        private void PrintListReport()
        {
            List flowList = new List();

            foreach (string result in results)
            {
                flowList.ListItems.Add(new ListItem(new Paragraph(new Run(result))));
            }

            FlowDocument doc = new FlowDocument();

            // Bold the query and add it to the document
            Run title = new Run(Query);
            title.FontFamily = new FontFamily("Segoe UI, Tahoma");
            title.FontSize = 20;
            Bold bd = new Bold();
            bd.Inlines.Add(title);
            doc.Blocks.Add(new Paragraph(bd));

            // Then add the results list
            flowList.FontFamily = new FontFamily("Segoe UI, Tahoma");
            flowList.FontSize = 14;
            doc.Blocks.Add(flowList);

            PrintDialog printDlg = new PrintDialog();
            printDlg.PageRangeSelection = PageRangeSelection.AllPages;
            printDlg.UserPageRangeEnabled = true;

            // Set column width to printable area width to force 1 column printing
            doc.ColumnWidth = printDlg.PrintableAreaWidth;

            IDocumentPaginatorSource idpSource = doc as IDocumentPaginatorSource;

            bool? dlgResult; // Nullable boolean

            dlgResult = printDlg.ShowDialog();

            if (dlgResult == true)
            {
                printDlg.PrintDocument(idpSource.DocumentPaginator, "List of Results");
            }
        }

        private void BuildResultsList()
        {
            foreach (string result in results)
            {
                TxtBlk_Results.Text += result;
                TxtBlk_Results.Text += Environment.NewLine;
            }
        }

        private void Btn_Print_Click(object sender, RoutedEventArgs e)
        {
            PrintListReport();
        }
    }
}
