using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Opera
{
    // #############################################################################################
    /// Function:<summary>
    ///				Build Version class, for managing incremental build numbers and the like
    ///			</summary>
    // #############################################################################################
    [DataContract]
    [Serializable]
    public class BuildVersion : ICloneable, IComparable<BuildVersion>
    {
        /// <summary>Major Number</summary>
        [DataMember]
        [DefaultValue(-1)]
        public int major;
        /// <summary>Minor Number</summary>
        [DataMember]
        [DefaultValue(-1)]
        public int minor;
        /// <summary>Build Number</summary>
        [DataMember]
        [DefaultValue(-1)]
        public int build;
        /// <summary>Revision Number</summary>
        [DataMember]
        [DefaultValue(-1)]
        public int revision;

        /// <summary>Whether we only care of major.minor.build</summary>
        public bool majorminorbuild_only { get; set; }

        /// <summary>Whether we only care of the major part</summary>
        public bool major_only { get; set; }


        // #############################################################################################
        /// <summary>Set default to 1.0.0.0</summary>
        // #############################################################################################
        public BuildVersion()
        {
            major = 0;
            minor = 0;
            build = 0;
            revision = 0;
        }

        // #############################################################################################
        /// <summary>
        ///         Create and provide all values
        /// </summary>
        /// <param name="_major">major as an integer</param>
        /// <param name="_minor">minor as an integer</param>
        /// <param name="_build">build as an integer</param>
        /// <param name="_revision">revision as an integer</param>
        // #############################################################################################
        public BuildVersion(int _major, int _minor, int _build, int _revision)
        {
            major = _major;
            minor = _minor;
            build = _build;
            revision = _revision;
        }

        // #############################################################################################
        /// <summary>
        ///         Create using another one as a template
        /// </summary>
        /// <param name="_version">class to copy</param>
        // #############################################################################################
        public BuildVersion(BuildVersion _version)
        {
            major = _version.major;
            minor = _version.minor;
            build = _version.build;
            revision = _version.revision;

            majorminorbuild_only = _version.majorminorbuild_only;
            major_only = _version.major_only;
        }

        // #############################################################################################
        /// <summary>
        /// Create from a string in the format of "major.minor.build.revision"
        /// </summary>
        /// <param name="_version">String in format of "major.minor.build.revision"</param>
        // #############################################################################################
        public BuildVersion(string _version)
        {
            string[] sections = _version.Split('.');

            int _major = 1;
            int _minor = 0;
            int _build = 0;
            int _revision = 0;

            int.TryParse(sections[0], out _major);
            if (1 < sections.Length) int.TryParse(sections[1], out _minor);
            if (2 < sections.Length) int.TryParse(sections[2], out _build);
            if (3 < sections.Length) int.TryParse(sections[3], out _revision);

            major = _major;
            minor = _minor;
            build = _build;
            revision = _revision;
        }

        // #############################################################################################
        /// <summary>
        /// Helper to increment the version
        /// </summary>
        // #############################################################################################
        public void Increment()
        {
            if (true == majorminorbuild_only)
                ++build;
            else if (true == major_only)
                ++major;
            else
                ++revision;
        }

        // #############################################################################################
        /// <summary>
        ///     Clone this BuildRevision, and return a new one
        ///     Returns a plain object for the ICloneable interface
        /// </summary>
        // #############################################################################################
        public object Clone()
        {
            return new BuildVersion(this);
        }

        // #############################################################################################
        /// <summary>
        ///     Compares this BuildRevision to another and returns if the contents match
        /// </summary>
        // #############################################################################################
        public override bool Equals(object _compare)
        {
            BuildVersion compare = _compare as BuildVersion;
            return (compare != null) && (compare.major == major) && (compare.minor == minor) && (compare.build == build) && (compare.revision == revision);
        }

        // #############################################################################################
        /// <summary>
        ///     Convert the BuildRevision into a string
        /// </summary>
        // #############################################################################################
        public override string ToString()
        {
            if (true == majorminorbuild_only)
                return (major + "." + minor + "." + build);
            else if (true == major_only)
                return major.ToString();
            else
                return (major + "." + minor + "." + build + "." + revision);
        }

        // #############################################################################################
        /// <summary>
        ///     Return a HashCode
        /// </summary>
        // #############################################################################################
        public override int GetHashCode()
        {
            int hash = 19;
            hash = (hash * 7) + major.GetHashCode();
            hash = (hash * 7) + minor.GetHashCode();
            hash = (hash * 7) + build.GetHashCode();
            hash = (hash * 7) + revision.GetHashCode();
            return hash;
        }

        public int CompareTo(BuildVersion other)
        {
            if (major > other.major)
                return 1;
            if (major < other.major)
                return -1;

            if (minor > other.minor)
                return 1;
            if (minor < other.minor)
                return -1;

            if (build > other.build)
                return 1;
            if (build < other.build)
                return -1;

            if (revision > other.revision)
                return 1;
            if (revision < other.revision)
                return -1;

            return 0;
        }

        public static bool operator <(BuildVersion a, BuildVersion b) => a.CompareTo(b) < 0;
        public static bool operator >(BuildVersion a, BuildVersion b) => a.CompareTo(b) > 0;
    }
}
