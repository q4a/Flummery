﻿using System;
using System.Collections.Generic;
using ToxicRagers.Stainless.Formats;
using OpenTK;

namespace Flummery.ContentPipeline.Stainless
{
    class MDLImporter : ContentImporter
    {
        public override string GetExtension() { return "mdl"; }

        public override Asset Import(string path)
        {
            MDL mdl = MDL.Load(path);
            Model model = new Model();

            model.Handedness = Model.CoordinateSystem.RightHanded;
            model.Tag = mdl;

            ModelMesh mesh = new ModelMesh();

            for (int i = 0; i < mdl.Meshes.Count; i++)
            {
                ModelMeshPart meshpart;

                var mdlmesh = mdl.GetMesh(i);

                // Process triangle strip
                if (mdlmesh.StripList.Count > 0)
                {
                    meshpart = new ModelMeshPart();
                    meshpart.Material = SceneManager.Current.Content.Load<Material, MaterialImporter>(mdlmesh.Name, path.Substring(0, path.LastIndexOf("\\") + 1), true);
                    meshpart.WindingOrder = OpenTK.Graphics.OpenGL.FrontFaceDirection.Cw;
                    
                    for (int j = 0; j < mdlmesh.StripList.Count; j++)
                    {
                        var v = mdl.Vertices[mdlmesh.StripList[j].Index];
                        meshpart.AddVertex(new Vector3(v.Position.X, v.Position.Y, -v.Position.Z), new Vector3(v.Normal.X, v.Normal.Y, v.Normal.Z), new Vector2(v.UV.X, v.UV.Y));
                    }

                    mesh.AddModelMeshPart(meshpart);
                }

                // Process patch list
                if (mdlmesh.PatchList.Count > 0)
                {
                    meshpart = new ModelMeshPart();
                    meshpart.Material = SceneManager.Current.Content.Load<Material, MaterialImporter>(mdlmesh.Name, path.Substring(0, path.LastIndexOf("\\") + 1), true);
                    meshpart.PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType.Triangles;
                    meshpart.WindingOrder = OpenTK.Graphics.OpenGL.FrontFaceDirection.Cw;

                    for (int j = 0; j < mdlmesh.PatchList.Count; j += 3)
                    {
                        MDLVertex v0, v1, v2;

                        v0 = mdl.Vertices[mdlmesh.PatchList[j + 0].Index];
                        v1 = mdl.Vertices[mdlmesh.PatchList[j + 1].Index];
                        v2 = mdl.Vertices[mdlmesh.PatchList[j + 2].Index];

                        meshpart.AddFace(
                            new Vector3[] {
                                new Vector3(v0.Position.X, v0.Position.Y, -v0.Position.Z),
                                new Vector3(v1.Position.X, v1.Position.Y, -v1.Position.Z),
                                new Vector3(v2.Position.X, v2.Position.Y, -v2.Position.Z)
                            },
                                new Vector3[] {
                                new Vector3(v0.Normal.X, v0.Normal.Y, v0.Normal.Z),
                                new Vector3(v1.Normal.X, v1.Normal.Y, v1.Normal.Z),
                                new Vector3(v2.Normal.X, v2.Normal.Y, v2.Normal.Z)
                            },
                                new Vector2[] {
                                new Vector2(v0.UV.X, v0.UV.Y),
                                new Vector2(v1.UV.X, v1.UV.Y),
                                new Vector2(v2.UV.X, v2.UV.Y)
                            }
                        );
                    }

                    mesh.AddModelMeshPart(meshpart);
                }
            }

            mesh.Name = mdl.Name;
            model.SetName(mdl.Name, model.AddMesh(mesh));

            return model;
        }
    }
}
