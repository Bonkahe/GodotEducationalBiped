using Godot;
using System;
using System.IO;
using System.Linq;
using System.Resources;

[Tool]
public partial class GearExtractor : EditorScenePostImport
{
    public override GodotObject _PostImport(Node scene)
    {
        string truncatedPath = GetSourceFile();
        truncatedPath = truncatedPath.Remove(truncatedPath.LastIndexOf("/") + 1);
        Iterate(scene, truncatedPath);
        return scene;
    }

    public void Iterate(Node node,string basePath)
    {
        if (node != null)
        {
            if (node.Name == "GeneralSkeleton")
            {
                foreach (Node child in node.GetChildren())
                {
                    var scene = new PackedScene();
                    Error result = scene.Pack(child);
                    if (result == Error.Ok)
                    {
                        using var dir = DirAccess.Open(basePath);
                        if (dir != null)
                        {
                            Error errordel = dir.Remove(child.Name + ".tscn");
                        }

                        Error error = ResourceSaver.Save(scene, basePath + child.Name + ".tscn", ResourceSaver.SaverFlags.ReplaceSubresourcePaths);
                        if (error != Error.Ok)
                        {
                            GD.PushError("An error occurred while saving the scene to disk.");
                        }
                    }
                }
            }
            else
            {
                foreach (Node child in node.GetChildren())
                {
                    Iterate(child, basePath);
                }
            }
        }
    }
}
