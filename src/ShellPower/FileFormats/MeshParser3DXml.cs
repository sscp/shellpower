using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using OpenTK;

namespace SSCP.ShellPower {
    public class MeshParser3DXml : IMeshParser {
        MeshSprite sprite;

        public void Parse(string filename) {
            /* load file */
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            /* parse file */
            var points = new List<Vector3>();
            var normals = new List<Vector3>();
            var triangles = new List<MeshSprite.Triangle>();

            //TODO: disgusting hack.
            //XmlNodeList xmlSurfaces;   = doc.FirstChild.SelectNodes(
            //"//GeometricRepresentationSet/Representation/AssociatedXML/Rep");
            XmlNode rep;
            if (doc["XMLRepresentation"] != null &&
              doc["XMLRepresentation"]["Root"] != null
              && doc["XMLRepresentation"]["Root"]["Rep"] != null) {
                rep = doc["XMLRepresentation"]["Root"]["Rep"];
            } else if (doc["Model_3dxml"] != null
                && doc["Model_3dxml"]["GeometricRepresentationSet"] != null
                && doc["Model_3dxml"]["GeometricRepresentationSet"]["Representation"] != null
                && doc["Model_3dxml"]["GeometricRepresentationSet"]["Representation"]["AssociatedXML"] != null) {
                rep = doc["Model_3dxml"]["GeometricRepresentationSet"]["Representation"]["AssociatedXML"];
            } else {
                throw new ArgumentException("cannot parse " + filename);
            }

            foreach (XmlNode node in rep.ChildNodes) {
                if (node["VertexBuffer"] == null ||
                    node["VertexBuffer"]["Positions"] == null ||
                    node["VertexBuffer"]["Normals"] == null ||
                        node["Faces"]["Face"] == null) {
                    Logger.warn("skipping rep...");
                    continue;
                }


                //read verts
                var units = 0.001f; /* units in mm */
                var newPoints = Parse3DXMLVectors(node["VertexBuffer"]["Positions"].InnerText);
                for (int i = 0; i < newPoints.Length; i++)
                    newPoints[i] = newPoints[i] * units;
                var newNormals = Parse3DXMLVectors(node["VertexBuffer"]["Normals"].InnerText);
                for (int i = 0; i < newNormals.Length; i++)
                    newNormals[i] = -newNormals[i];
                int offset = points.Count;
                points.AddRange(newPoints);
                normals.AddRange(newNormals);

                //read triangles
                XmlAttributeCollection faceAttrs = node["Faces"]["Face"].Attributes;
                Logger.info("parsing face " + faceAttrs["id"].Value);
                if (faceAttrs["triangles"] != null) {
                    int[] vertIxs = Parse3DXMLInts(faceAttrs["triangles"].Value);
                    var triangleIxs = new HashSet<int>(vertIxs);
                    /*for (int i = 0; i < newPoints.Length - 2; i++)
                    {
                        if (triangleIxs.Contains(i))
                        {
                            triangles.Add(new MeshSprite.Triangle()
                            {
                                VertexA = offset + i,
                                VertexB = offset + i + 1,
                                VertexC = offset + i + 2
                            });
                        }
                    }*/
                    for (int i = 0; i < vertIxs.Length; i += 3) {
                        triangles.Add(new MeshSprite.Triangle() {
                            VertexA = offset + vertIxs[i],
                            VertexB = offset + vertIxs[i + 1],
                            VertexC = offset + vertIxs[i + 2]
                        });
                    }
                } else if (faceAttrs["strips"] != null) {
                    String[] strips = faceAttrs["strips"].Value.Split(',');
                    foreach (String strip in strips) {
                        int[] vertIxs = Parse3DXMLInts(strip);
                        for (int i = 2; i < vertIxs.Length; i++) {
                            triangles.Add(new MeshSprite.Triangle() {
                                VertexA = offset + vertIxs[i - ((i % 2) == 0 ? 1 : 2)],
                                VertexB = offset + vertIxs[i - ((i % 2) == 0 ? 2 : 1)],
                                VertexC = offset + vertIxs[i]
                            });
                        }
                    }
                } else {
                    throw new ArgumentException("found an unsupported 3dxml mesh surface...");
                }
            }

            /* create mesh sprite */
            sprite = new MeshSprite() {
                Points = points.ToArray(),
                Normals = normals.ToArray(),
                Triangles = triangles.ToArray()
            };
        }

        public MeshSprite GetMesh() {
            return sprite;
        }

        private Vector3[] Parse3DXMLVectors(string str) {
            Vector3[] vectors =
                str
                .Split(',')
                .Select(sPoint => {
                    float[] components = sPoint
                        .Split()
                        .Select(sComponent => float.Parse(sComponent))
                        .ToArray();
                    return new Vector3(components[0], components[1], components[2]);
                })
                .ToArray();
            return vectors;
        }

        private int[] Parse3DXMLInts(string str) {
            var sInts = str.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            var ints = new int[sInts.Length];
            for (int i = 0; i < ints.Length; i++)
                ints[i] = int.Parse(sInts[i]);
            return ints;
        }
    }
}
