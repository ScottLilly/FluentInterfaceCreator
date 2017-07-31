namespace Engine.FluentInterfaceCreators
{
    public class Line
    {
        public int IndentationLevels { get; }
        public string Text { get; }

        public Line(int indentationLevels, string text)
        {
            IndentationLevels = indentationLevels;
            Text = text;
        }
    }
}