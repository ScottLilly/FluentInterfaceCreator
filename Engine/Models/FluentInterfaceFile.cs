using System.Collections.Generic;
using System.Text;
using Engine.Shared;

namespace Engine.Models
{
    public class FluentInterfaceFile
    {
        private const string TAB_CHARACTER = "\t";

        private readonly List<Line> _lines = new List<Line>();

        public string Name { get; }

        public FluentInterfaceFile(string name)
        {
            Name = name;
        }

        public void AddLine(int indentationLevels, string text)
        {
            _lines.Add(new Line(indentationLevels, text));
        }

        public void AddBlankLine()
        {
            _lines.Add(new Line(0, ""));
        }

        public void AddLineAfterBlankLine(int indentationLevel, string text)
        {
            AddBlankLine();
            AddLine(indentationLevel, text);
        }

        public string FormattedText()
        {
            StringBuilder outputString = new StringBuilder();

            foreach (Line outputLine in _lines)
            {
                outputString.Append(TAB_CHARACTER.Repeated(outputLine.IndentationDepth));
                outputString.AppendLine(outputLine.Text);
            }

            return outputString.ToString();
        }

        private sealed class Line
        {
            public int IndentationDepth { get; }
            public string Text { get; }

            internal Line(int indentationDepth, string text)
            {
                IndentationDepth = indentationDepth;
                Text = text;
            }
        }
    }
}