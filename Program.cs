using HedgeLib.IO;
using KnuxLib.Engines.Hedgehog;
using Marathon.IO;
using System.Diagnostics;

foreach (string arg in args)
{
    if (arg.EndsWith(".pcmodel"))
    {
        string workDir = Path.GetDirectoryName(arg);
        PointCloud pcmodel = new(arg);
        foreach (var instance in pcmodel.Data)
        {
            string modelpath = Path.Combine(workDir, $"{instance.AssetName}.terrain-model");
            if (!File.Exists(modelpath))
            { continue;}
            BinaryReaderEx modelReader = new(new FileStream(modelpath, FileMode.Open, FileAccess.Read));

            Console.WriteLine($"modelpath: {modelpath}");
            Console.WriteLine($"InstanceName: {instance.InstanceName}");
            Console.WriteLine($"ModelName: {instance.AssetName}");
            Console.WriteLine($"Position: {instance.Position}");
            Console.WriteLine($"Rotation: {instance.Rotation}");
            Console.WriteLine($"Scale: {instance.Scale}");

            bool isLODmodel = modelReader.ReadInt32() == 0x4144454E;
            Console.WriteLine($"Has LOD: {isLODmodel}");
            modelReader.Close();
            modelReader.Dispose();
            if (isLODmodel)
            {
                var HedgeNeedleProcess = Process.Start("HedgeNeedle.exe", $"\"{modelpath}\"");
                HedgeNeedleProcess.WaitForExit();

                modelpath = Path.Combine(workDir, instance.AssetName, $"{instance.AssetName}.0.terrain-model");
            }

            var modelFBXProcess = Process.Start("ModelFBX.exe", $"\"{modelpath}\"");
            modelFBXProcess.WaitForExit();
            Console.WriteLine("Finished converting to FBX.");

            if (isLODmodel)
            {
                string fbxmodelpath = $"{modelpath}.fbx";
                if (!File.Exists(fbxmodelpath))
                { continue;}
                File.Move(fbxmodelpath, Path.Combine(workDir, Path.GetFileName(fbxmodelpath)), true);
                Console.WriteLine("FBX moved to work directory.");
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine("Finished converting all models, starting instance transforms...");
        Console.WriteLine();
        //Console.Read();

        foreach (var instance in pcmodel.Data)
        {
            Console.WriteLine($"InstanceName: {instance.InstanceName}");
            Console.WriteLine($"ModelName: {instance.AssetName}");
            Console.WriteLine($"Position: {instance.Position}");
            Console.WriteLine($"Rotation: {instance.Rotation}");
            Console.WriteLine($"Scale: {instance.Scale}");

            Console.WriteLine("doing shit here");

            Console.WriteLine();
        }

        Console.WriteLine();

    }
}

Console.WriteLine("Finished all instances.");
Console.Read();
