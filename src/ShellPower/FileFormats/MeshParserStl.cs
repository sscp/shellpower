using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using OpenTK;

namespace SSCP.ShellPower {
    public class MeshParserStl : IMeshParser {
        private Mesh mesh;
        public void Parse(string filename) {
            var verts = new List<Vector3>();
            var tris = new List<Mesh.Triangle>();
            var norms = new List<Vector3>();
            //deduplicate the verts...
            var vertIxs = new Dictionary<Vector3, int>();

            // parse the file with a simple state machine
            const int INIT = 1, SOLID = 2, FACET = 3, LOOP = 4, ENDFACET=5;
            var triIxs = new List<int>();
            var norm = new Vector3();
            int state = INIT;
            StreamReader reader = new StreamReader(filename);
            String line;
            for (int lineNum = 1; (line = reader.ReadLine()) != null; lineNum++) {
                string[] parts = line.Trim().ToLower().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) {
                    // skip empty lines
                    continue;
                }
                if (parts[0] == "solid") {
                    Assert(state == INIT, filename, lineNum);
                    Logger.info("reading stl solid " + String.Join(" ", parts, 1, parts.Length - 1));
                    state = SOLID;
                } else if (parts[0] == "facet") {
                    Assert(state == SOLID, filename, lineNum);
                    Assert(parts[1] == "normal", filename, lineNum);
                    norm = ReadVector(parts, 2, filename, lineNum);
                    state = FACET;
                } else if (parts[0] == "outer") {
                    Assert(state == FACET, filename, lineNum);
                    Assert(parts[1] == "loop", filename, lineNum);
                    triIxs.Clear();
                    state = LOOP;
                } else if (parts[0] == "vertex") {
                    Assert(state == LOOP, filename, lineNum);
                    var vertex = ReadVector(parts, 1, filename, lineNum);
                    if (vertIxs.ContainsKey(vertex)) {
                        int vertIx = vertIxs[vertex];
                        triIxs.Add(vertIx);
                        norms[vertIx] += norm;
                    } else {
                        int vertIx = verts.Count;
                        vertIxs.Add(vertex, vertIx);
                        triIxs.Add(vertIx);
                        verts.Add(vertex);
                        norms.Add(norm);
                    }
                } else if (parts[0] == "endloop") {
                    Assert(state == LOOP, filename, lineNum);
                    //every face must be a triangle
                    Assert(triIxs.Count == 3, filename, lineNum);
                    var tri = new Mesh.Triangle(triIxs[0], triIxs[1], triIxs[2]);
                    tri.normal = norm;
                    tris.Add(tri);
                    state = ENDFACET;
                } else if (parts[0] == "endfacet") {
                    Assert(state == ENDFACET, filename, lineNum);
                    state = SOLID;
                } else if (parts[0] == "endsolid") {
                    Assert(state == SOLID, filename, lineNum);
                    state = INIT;
                }
            }

            // get all the averaged vertex normals and set them to unit length
            for(int i = 0; i < norms.Count; i++){
                float len = norms[i].Length;
                if (len == 0) {
                    norms[i] = new Vector3(1, 0, 0);
                } else {
                    norms[i] /= len;
                }
            }

            // finally, return a mesh
            mesh = new Mesh(verts.ToArray(), norms.ToArray(), tris.ToArray());
        }
        private void Assert(bool cond, String filename, int lineNum) {
            if (!cond) {
                throw new FormatException("couldn't parse stl file " + filename + " ... error on line " + lineNum);
            }
        }
        private Vector3 ReadVector(String[] parts, int ix, String filename, int lineNum) {
            Assert(parts.Length == ix + 3, filename, lineNum);
            float x, y, z;
            Assert(float.TryParse(parts[ix], out x), filename, lineNum);
            Assert(float.TryParse(parts[ix+1], out y), filename, lineNum);
            Assert(float.TryParse(parts[ix+2], out z), filename, lineNum);
            return new Vector3(x, y, z);
        }
        public Mesh GetMesh() {
            return mesh;
        }
    }
}
