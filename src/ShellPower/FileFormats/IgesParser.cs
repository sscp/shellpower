using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OpenTK;

namespace SSCP.ShellPower {
    /// <summary>
    /// Represents an IGES file. Loads all directory entry and parameter types, but only parses the ones it has support for.
    /// Does not support compressed IGES format.
    /// </summary>
    public class IgesFile {
        public string Start { get; private set; }
        public IgesSettings Global { get; private set; }
        public Dictionary<int, IgesDirectoryEntry> Directory { get; private set; }
        public List<IgesParameter> Parameters { get; private set; }

        /* parse state */
        string filename;
        StreamReader reader;
        char section = ' ';
        int lineNumber;
        string line;
        int lineTag;
        static readonly List<char> sections = new char[] { 'S', 'G', 'D', 'P', 'T' }.ToList();

        public IgesFile() {
            Global = null;
            Directory = new Dictionary<int, IgesDirectoryEntry>();
            Parameters = new List<IgesParameter>();
        }

        public void Load(string filename) {
            lock (this) {
                /* clear any existing state */
                Start = "";
                Directory.Clear();
                Parameters.Clear();
                Dictionary<char, int> sectionLineCounts = new Dictionary<char, int>();
                foreach (char ch in sections)
                    sectionLineCounts.Add(ch, 0);
                int lastLineTag = 0;

                /* iterate through file lines */
                this.filename = filename;
                reader = new StreamReader(filename);
                for (lineNumber = 1; (line = reader.ReadLine()) != null; lineNumber++) {
                    /* trailing empty lines are allowed */
                    if (line.Length == 0 && section == 'T')
                        continue;

                    /* all other lines are expected to be 80 chars long */
                    if (line.Length != 80)
                        FileReadError("Encountered line with != 80 chars");

                    /* parse section */
                    char newSection = line[72];
                    if (sections.IndexOf(newSection) < 0)
                        FileReadError("Encountered unrecognized section '" + section + "', expected " + string.Join(",", sections));
                    if (sections.IndexOf(newSection) != sections.IndexOf(section) && sections.IndexOf(newSection) != sections.IndexOf(section) + 1)
                        FileReadError("Encountered out-of-order section '" + newSection + "' following '" + section + "'");
                    int expectedLineTag;
                    if (section != newSection)
                        expectedLineTag = 1;
                    else
                        expectedLineTag = lineTag + 1;
                    section = newSection;
                    sectionLineCounts[section]++;

                    /* parse line tag */
                    if (!int.TryParse(line.Substring(73), out lineTag))
                        FileReadError("Couldn't parse line tag expected in cols 73-80.");
                    if (lineTag != expectedLineTag) {
                        FileReadError("Found unexpected line tag " + lineTag + ", expected " + expectedLineTag);
                    }
                    lastLineTag = lineTag;

                    /* parse section-specific info */
                    switch (section) {
                        case 'S':
                            Start += line.Substring(0, 72);
                            break;
                        case 'G':
                            /* TODO */
                            break;
                        case 'D':
                            if (lineTag % 2 == 1) {
                                var entry = new IgesDirectoryEntry();
                                entry.Fields = new string[18];
                                for (int i = 0; i < 9; i++)
                                    entry.Fields[i] = line.Substring(i * 8, 8);
                                entry.Tag = lineTag;
                                Directory.Add(lineTag, entry);
                            } else {
                                var entry = Directory[lineTag - 1];
                                for (int i = 0; i < 9; i++)
                                    entry.Fields[i + 9] = line.Substring(i * 8, 8);
                                ParseDirectoryEntry(entry);
                            }
                            break;
                        case 'P':
                            int dirTag;
                            if (!int.TryParse(line.Substring(65, 7), out dirTag))
                                FileReadError("Expected directory pointer in cols 66-72");
                            if (!Directory.ContainsKey(dirTag))
                                FileReadError("Couldn't find directory with tag " + dirTag + ".");
                            if (Directory[dirTag].ParameterData == null)
                                Directory[dirTag].ParameterData = "";
                            Directory[dirTag].ParameterData += line.Substring(0, 64);
                            break;
                        case 'T':
                            if (!Regex.IsMatch(line, "^S[ 0-9]{7}G[ 0-9]{7}D[ 0-9]{7}P[ 0-9]{7}.*"))
                                FileReadError("Incorrect format in terminate section.");
                            Dictionary<char, int> expectedLineCounts = new Dictionary<char, int>();
                            expectedLineCounts.Add('S', int.Parse(line.Substring(1, 7)));
                            expectedLineCounts.Add('G', int.Parse(line.Substring(9, 7)));
                            expectedLineCounts.Add('D', int.Parse(line.Substring(17, 7)));
                            expectedLineCounts.Add('P', int.Parse(line.Substring(25, 7)));
                            expectedLineCounts.Add('T', 1);
                            foreach (char ch in sections) {
                                if (sectionLineCounts[ch] != expectedLineCounts[ch]) {
                                    FileReadError("Read " + sectionLineCounts[ch] + "for section '" + ch + "', but there should be " + expectedLineCounts[ch] + ".");
                                }
                            }
                            break;
                    }
                }

                foreach (var dir in Directory.Values)
                    ParseParameterData(dir);
            }
        }
        void ParseDirectoryEntry(IgesDirectoryEntry dir) {
            int num;
            if (!int.TryParse(dir.Fields[0], out num))
                FileReadError("Could not parse entity type number expected in cols 1-8.");
            dir.EntityTypeNumber = num;

            /* TODO */
        }
        void ParseParameterData(IgesDirectoryEntry dir) {
            Debug.WriteLine("Parsing dir entry " + dir.Tag + ", entity type number " + dir.EntityTypeNumber);
            string[] parameters = dir.ParameterData.Split(',');
            parameters[parameters.Length - 1] = parameters[parameters.Length - 1].TrimEnd();
            if (parameters[parameters.Length - 1].EndsWith(";")) {
                parameters[parameters.Length - 1] =
                    parameters[parameters.Length - 1]
                    .Substring(0, parameters[parameters.Length - 1].Length - 1);
            }
            int entityTypeNum;
            if (!int.TryParse(parameters[0], out entityTypeNum) || entityTypeNum != dir.EntityTypeNumber) {
                FormatError("Error parsing entity type number of parameter data entry for dir entry " + dir.Tag + ", expected " + dir.EntityTypeNumber + ", got " + parameters[0] + ".");
            }
            switch (dir.EntityTypeNumber) {
                case 128:
                    /* rational b-spline surface TODO: validation */
                    var entity = new IgesRationalBSplineSurfaceEntity();
                    int
                        k1 = int.Parse(parameters[1]),
                        k2 = int.Parse(parameters[2]),
                        m1 = int.Parse(parameters[3]),
                        m2 = int.Parse(parameters[4]);
                    entity.Props = new bool[5]{
                        int.Parse(parameters[5]) > 0,
                        int.Parse(parameters[6]) > 0,
                        int.Parse(parameters[7]) > 0,
                        int.Parse(parameters[8]) > 0,
                        int.Parse(parameters[9]) > 0
                    };
                    entity.UControlPoints = k1 + 1;
                    entity.VControlPoints = k2 + 1;
                    entity.UDegree = m1;
                    entity.VDegree = m2;
                    int
                        a = k1 + 1 + m1,
                        b = k2 + 1 + m2,
                        c = (k1 + 1) * (k2 + 1);

                    int offset = 10;
                    entity.UKnots = new double[a + 1];
                    for (int i = 0; i < a + 1; i++)
                        entity.UKnots[i] = ParseDouble(parameters[offset + i]);

                    offset = 11 + a;
                    entity.VKnots = new double[b + 1];
                    for (int i = 0; i < b + 1; i++)
                        entity.VKnots[i] = ParseDouble(parameters[offset + i]);

                    offset = 12 + a + b;
                    entity.Weights = new double[k1 + 1, k2 + 1];
                    for (int i = 0; i <= k1; i++)
                        for (int j = 0; j <= k2; j++)
                            entity.Weights[i, j] = ParseDouble(
                                parameters[offset + i * (k2 + 1) + j]);

                    offset = 12 + a + b + c;
                    entity.ControlPoints = new Vector3d[entity.UControlPoints, entity.VControlPoints];
                    for (int i = 0; i < entity.UControlPoints; i++) {
                        for (int j = 0; j < entity.VControlPoints; j++) {
                            entity.ControlPoints[i, j] = new Vector3d(
                                ParseDouble(parameters[offset + 3 * (j * entity.UControlPoints + i)]),
                                ParseDouble(parameters[offset + 3 * (j * entity.UControlPoints + i) + 1]),
                                ParseDouble(parameters[offset + 3 * (j * entity.UControlPoints + i) + 2]));
                        }
                    }

                    offset = 12 + a + b + 4 * c;
                    entity.UMin = ParseDouble(parameters[offset]);
                    entity.UMax = ParseDouble(parameters[offset + 1]);
                    entity.VMin = ParseDouble(parameters[offset + 2]);
                    entity.VMax = ParseDouble(parameters[offset + 3]);

                    dir.Entity = entity;
                    break;
            }
        }
        void FormatError(string message) {
            throw new Exception(message);
        }
        void FileReadError(string message) {
            throw new Exception(message + " in " + filename + ", line" + lineNumber);
        }

        double ParseDouble(string str) {
            double val;
            if (!double.TryParse(str.Replace('D', 'E'), out val)) {
                FormatError("Couldn't parse real number: " + str);
            }
            return val;
        }
    }


    /// <summary>
    /// Represents global settings of an IGES file, spec'd in that file's G section.
    /// </summary>
    public class IgesSettings {

    }
    /// <summary>
    /// Each directory entry has 18 8-char-long fields, spanning two lines in the file's D section.
    /// Parameter data entries refer to it by the tag number of the first of these two lines.
    /// </summary>
    public class IgesDirectoryEntry {
        public int Tag { get; internal set; }
        public string[] Fields { get; internal set; }
        public string ParameterData { get; internal set; }
        public int EntityTypeNumber { get; internal set; }
        public IgesEntity Entity { get; internal set; }
    }
    /// <summary>
    /// Represents the data of an IGES file (points, widths, connections, etc--the rest of the file is just metadata).
    /// IGES parameter data entries are in the P section. Each one belongs to a specific directory entry.
    /// </summary>
    public class IgesParameter {
        public int DirectoryTag { get; internal set; }
        public string Data { get; internal set; }
    }
}
