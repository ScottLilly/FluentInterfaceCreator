namespace Engine.Models
{
    public class ProjectVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }

        public string Complete => $"{Major}.{Minor}.{Revision}";

        public ProjectVersion(string completeVersion)
        {
            string[] values = completeVersion.Split('.');

            int major = 0;
            int minor = 0;
            int revision = 0;

            if(values.Length > 0)
            {
                int.TryParse(values[0], out major);
            }

            if(values.Length > 1)
            {
                int.TryParse(values[1], out minor);
            }

            if(values.Length > 2)
            {
                int.TryParse(values[2], out revision);
            }

            Major = major;
            Minor = minor;
            Revision = revision;
        }

        // For serialization
        public ProjectVersion()
        {
        }
    }
}