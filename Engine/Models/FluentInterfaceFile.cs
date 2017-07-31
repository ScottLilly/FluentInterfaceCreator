using System.Collections.Generic;
using System.Text;
using Engine.FluentInterfaceCreators;

namespace Engine.Models
{
    public class FluentInterfaceFile
    {
        private readonly List<Line> _lines = new List<Line>();

        public string FileName { get; set; }

        public string Contents
        {
            get
            {
                StringBuilder outputString = new StringBuilder();

                foreach(Line outputLine in _lines)
                {
                    for(int i = 0; i < outputLine.IndentationLevels; i++)
                    {
                        outputString.Append("\t");
                    }

                    outputString.AppendLine(outputLine.Text);
                }

                return outputString.ToString();
            }
        }

        public FluentInterfaceFile(string fileName)
        {
            FileName = fileName;
        }

        public void AddLine(int indentationLevels, string text)
        {
            _lines.Add(new Line(indentationLevels, text));
        }

        public void AddBlankLine()
        {
            _lines.Add(new Line(0, ""));
        }
    }
}