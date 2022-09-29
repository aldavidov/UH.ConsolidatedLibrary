using System.Windows.Controls;

namespace UH.TraumaLink
{
    public class Column
    {
        public string Name { get; set; }
        public string Header { get; set; }
        public int ColumnWidth { get; set; }
        public FilterType Filter { get; set; }
        public Control FilterControl { get; set; }
        public string FilterLabel { get; set; }
        public int FilterLabelWidth { get; set; }
        public int FilterControlWidth { get; set; }
        public int FilterMarginLeft { get; set; }

        public Column(string name)
            : this(name, name, 0, 0, name, 0, 0, 0)
        {
        }

        /// <summary>
        /// Sets the column information for all lists
        /// </summary>
        /// <param name="name">Column name in the SQL recordset</param>
        /// <param name="header">Header label for the column, also used for the filter box label</param>
        /// <param name="columnWidth">Width of the column in px</param>
        /// <param name="filter">filter types: 0 = no filter, 1 = text filter with 'like x%' match logic, 2 = text filter with 'like %x%' match logic, 3=dropdown</param>
        /// <param name="filterLabel">Text of the label on the filter box</param>
        /// <param name="filterLabelWidth">Width of the label on the filter box. 0 = auto width, number (e.g., 60) is a fixed width in px</param>
        /// <param name="filterControlWidth">Width of the filter box. 0 = auto width, number (e.g., 60) is a fixed width in px</param>
        /// <param name="filterMarginLeft">Left margin on the filter label/control block... for spacing out the filter controls horizontally</param>

        public Column(string name, string header, int columnWidth, FilterType filter, string filterLabel,
            int filterLabelWidth, int filterControlWidth, int filterMarginLeft)
        {
            Name = name;
            Header = header;
            ColumnWidth = columnWidth;
            Filter = filter;
            FilterLabel = filterLabel;
            FilterLabelWidth = filterLabelWidth;
            FilterControlWidth = filterControlWidth;
            FilterMarginLeft = filterMarginLeft;
        }
    }
}
