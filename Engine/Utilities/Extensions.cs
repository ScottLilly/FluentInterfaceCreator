using System.Text;

namespace Engine.Utilities
{
    public static class Extensions
    {
        public static void AppendTabPrefixedLine(this StringBuilder builder, 
            int numberOfTabs, string text)
        {
            for(int i = 0; i < numberOfTabs; i++)
            {
                builder.Append("\t");
            }

            builder.AppendLine(text);
        }
    }
}