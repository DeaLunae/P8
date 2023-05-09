using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;
using Object = UnityEngine.Object;


namespace Recording
{
    public class CSVReader : EditorWindow
    {
        [MenuItem("Tools/Read CSV Transform Columns")]
        public static void ReadCSVTransformColumns()
        {
            Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            List<string> csvFilePaths = new List<string>();
            GestureList gesturelist = null;
            Object selectedGestureObject = null;
            // Find all selected CSV files and store their file paths in a list
            foreach (Object selectedObject in selectedObjects)
            {
                string filePath = AssetDatabase.GetAssetPath(selectedObject);
                if (filePath.EndsWith(".csv"))
                {
                    csvFilePaths.Add(filePath);
                }
                else if (filePath.EndsWith(".asset"))
                {
                    gesturelist = AssetDatabase.LoadAssetAtPath<GestureList>(filePath);
                    selectedGestureObject = selectedObject;
                }
            }
            if (gesturelist == null)
            {
                Debug.LogError("No GestureList selected");
                return;
            }
            // Read data from transform columns in each selected CSV file
            foreach (string csvFilePath in csvFilePaths)
            {
                string[] filename = Path.GetFileNameWithoutExtension(csvFilePath).Split('-');
                Enum.TryParse(filename[1], out Gesture.GestureName gestureName);
                Handedness handedness = filename[2] == "Right" ? Handedness.Right : Handedness.Left;
                var appropriateGesture = gesturelist.list.FirstOrDefault(e => e.Name == gestureName && e.Handedness == handedness);
                if (appropriateGesture == null)
                {
                    Debug.LogError($"No gesture with name {gestureName} and handedness {handedness} found in GestureList");
                    return;
                }
                appropriateGesture.demonstrationData.Clear();
                using (var reader = new StreamReader(csvFilePath))
                {
                    List<string> colHeaders = new List<string>();
                    Dictionary<string, List<float>> colData = new Dictionary<string, List<float>>();

                    // Read column headers from the first row
                    string[] headers = reader.ReadLine().Split(';');
                    for (int i = 0; i < headers.Length; i++)
                    {
                        colHeaders.Add(headers[i]);
                        colData[headers[i]] = new List<float>();
                    }
                

                    // Read data from transform columns
                    int j = 0;
                    while (!reader.EndOfStream)
                    {
                        string[] row = reader.ReadLine().Split(';');

                        for (int i = 0; i < row.Length; i++)
                        {
                            if (!colHeaders[i].ToLower().Contains("transform") &&
                                !colHeaders[i].ToLower().Contains("head") &&
                                !colHeaders[i].ToLower().Contains("timestamp"))
                                continue;
                            if (float.TryParse(row[i], out var value))
                            {
                                colData[colHeaders[i]].Add(value);
                            }
                        }

                        var headPos = new Vector3(colData["Head.pos.x"][j], colData["Head.pos.y"][j],
                            colData["Head.pos.z"][j]);
                        var headRot = new Quaternion(colData["Head.rot.x"][j], colData["Head.rot.y"][j],
                            colData["Head.rot.z"][j], colData["Head.rot.w"][j]);
                        var timestamp = colData["Timestamp"][j];
                        Gesture.PoseFrameData pfd = new Gesture.PoseFrameData
                        {
                            positions = GeneratePositions(colData, j),
                            rotations = GenerateRotations(colData, j),
                            headPosition = headPos,
                            headRotation = headRot,
                            time = timestamp
                        };
                        appropriateGesture.AddPoseFrame(Gesture.PoseDataType.Demonstration,pfd);
                        j += 1;
                    }
                }
            }
            EditorUtility.SetDirty(selectedGestureObject);
            AssetDatabase.SaveAssetIfDirty(selectedGestureObject);
            AssetDatabase.Refresh();
        }

        private static List<Vector3> GeneratePositions(Dictionary<string, List<float>> colData, int i)
        {
            List<Vector3> positions = new List<Vector3>();
            for (int j = 0; j < 22; j++)
            {
                positions.Add(new Vector3(
                    colData[$"Transform.Index.{j}.pos.x"][i],
                    colData[$"Transform.Index.{j}.pos.y"][i],
                    colData[$"Transform.Index.{j}.pos.z"][i]));
            }
            return positions;
        }
    
        private static List<Quaternion> GenerateRotations(Dictionary<string, List<float>> colData, int i)
        {
            List<Quaternion> rotations = new List<Quaternion>();
            for (int j = 0; j < 22; j++)
            {
                rotations.Add(new Quaternion(
                    colData[$"Transform.Index.{j}.rot.x"][i],
                    colData[$"Transform.Index.{j}.rot.y"][i],
                    colData[$"Transform.Index.{j}.rot.z"][i],
                    colData[$"Transform.Index.{j}.rot.w"][i]));
            }
            return rotations;
        }
    }
}