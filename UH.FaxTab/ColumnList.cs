using System.Collections.Generic;

namespace UH.FaxTab
{
    class ColumnList : List<Column>
    {
        public void Add(string name, string header, int columnWidth, FilterType filter,
            string filterLabel, int filterLabelWidth, int filterControlWidth, int filterMarginLeft)
        {
            Add(new Column(name, header, columnWidth, filter, filterLabel, filterLabelWidth, filterControlWidth, filterMarginLeft));
        }
    }
}
