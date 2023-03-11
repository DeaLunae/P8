using UnityEngine;

namespace HandTrackingData.Data
{
    public class LegeacyHandAspect
    {
        public const float comparisonThreshold = 0.1f;
        public static readonly double[] maxes = new double[21]{0.09021042,0.1111464,0.09921884,0.1323981,0.06017102,0.07532546,0.1339429,0.06814501,0.03681857,0.1253064,0.06339972,0.06077478,0.1132728,0.04930152,0.147451,0.9428114,0.9908907,0.9999788,0.30046,1.184354,0.4404907 };
        public static readonly double[] mines = new double[21]{0.06838486,0.08991423,0.01402679,0.1115697,0.03685664,0.01539352,0.1057315,0.04198776,0.01321025,0.09662139,0.03922486,0.01349914,0.09491879,0.03493643,0.04432947,-2.75817E-05,3.92234E-06,0.000278436,-0.4935929,0.7348156,0.008642531 };

        public float[,] values = new float[5, 3];
        public Vector3 wristPosition;
        public Quaternion wristRotation;
        private LegacyHandPostionsForCalc originalPositionData;
        public LegeacyHandAspect(LegacyHandPostionsForCalc data)
        {
            originalPositionData = data;
            SetAllAspects(data);
        }
        
        
        #region SetValues

        /// <summary>
        /// Sets the KnuckleAngle of a given finger by taken the distance from the corner of the finger to the wrist. Make sure wristPosition is updated before calling this.
        /// </summary>
        private void SetFingerKnuckleAngle(int finger,Vector3 corner)
        {
            values[finger,0] = Vector3.Distance(corner, wristPosition);
        }
        private void SetThumbKnuckleAngle(Vector3 thumbKnuckle,Vector3 pinkyKnuckle)
        {
            values[0,0] = Vector3.Distance(thumbKnuckle , pinkyKnuckle);
        }
        
        /// <summary>
        /// Sets the corner angle of a finger by taking the lenght from the tip of the finger to the knuckle
        /// </summary>
        private void SetCornerAngle(int finger,Vector3 tip,Vector3 knuckle)
        {
            values[finger,1] = Vector3.Distance(tip , knuckle);
        }
        public void SetOffset(int finger,Vector3 tip,Vector3 nextTip)
        {
            values[finger, 2] = Vector3.Distance(tip,nextTip);
        }
        public void SetAllAspects(LegacyHandPostionsForCalc data)
        {
            wristPosition = data.wristPosition;
            wristRotation = data.wristRotation;
            
            SetOffset(0,data.tips[0],data.tips[1]);
            SetOffset(1,data.tips[1],data.tips[2]);
            SetOffset(2,data.tips[2],data.tips[3]);
            SetOffset(3,data.tips[3],data.tips[4]);
            SetOffset(4,data.tips[4],data.tips[0]);
            SetCornerAngle(0,data.tips[0],wristPosition);
            SetCornerAngle(1,data.tips[1],data.knuckles[1]);
            SetCornerAngle(2,data.tips[2],data.knuckles[2]);
            SetCornerAngle(3,data.tips[3],data.knuckles[3]);
            SetCornerAngle(4,data.tips[4],data.knuckles[4]);
            SetThumbKnuckleAngle(data.knuckles[0],data.knuckles[4]);
            SetFingerKnuckleAngle(1,data.corners[1]);
            SetFingerKnuckleAngle(2,data.corners[2]);
            SetFingerKnuckleAngle(3,data.corners[3]);
            SetFingerKnuckleAngle(4,data.corners[4]);
        }

        #endregion

        #region GetValues

        public static float GetFingerKnuckleAngle(Vector3 corner,Vector3 wristPosition)
        {
            return Vector3.Distance(corner, wristPosition);
        }

        public static float GetThumbKnuckleAngle(Vector3 thumbKnuckle,Vector3 pinkyKnuckle)
        {
            return Vector3.Distance(thumbKnuckle , pinkyKnuckle);
        }
    
        public static float GetCornerAngle(Vector3 tip,Vector3 knuckle)
        {
            return Vector3.Distance(tip , knuckle);
        }
    
        public static float GetOffset(Vector3 tip,Vector3 nextTip)
        {
            return Vector3.Distance(tip,nextTip);
        }

        #endregion
        public int CompareTo(LegeacyHandAspect other)
        {
            int diffs = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Mathf.Abs(values[i, j] - other.values[i, j]) > comparisonThreshold)
                    {
                        diffs++;
                    }
                }
            }

            return diffs;
        }
        public static void DrawDebugLines(LegacyHandPostionsForCalc data)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(data.knuckles[0], data.knuckles[4]);
            Gizmos.DrawLine(data.wristPosition, data.corners[1]);
            Gizmos.DrawLine(data.wristPosition, data.corners[2]);
            Gizmos.DrawLine(data.wristPosition, data.corners[3]);
            Gizmos.DrawLine(data.wristPosition, data.corners[4]);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(data.tips[0], data.wristPosition);
            Gizmos.DrawLine(data.tips[1], data.knuckles[1]);
            Gizmos.DrawLine(data.tips[2], data.knuckles[2]);
            Gizmos.DrawLine(data.tips[3], data.knuckles[3]);
            Gizmos.DrawLine(data.tips[4], data.knuckles[4]);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(data.tips[0], data.tips[1]);
            Gizmos.DrawLine(data.tips[1], data.tips[2]);
            Gizmos.DrawLine(data.tips[2], data.tips[3]);
            Gizmos.DrawLine(data.tips[3], data.tips[4]);
            Gizmos.DrawLine(data.tips[4], data.tips[0]);
        }
        public LegacyHandPostionsForCalc GetData()
        {
            return originalPositionData;
        }

        public double[] GetHandInputs()
        {
            double[] handInputs = new double[18];
            int i = 0;
            foreach (float aspect in values)
            {
                handInputs[i] = InverseLerp(mines[i],maxes[i],aspect);
                i++;
            }
            Vector3 rote = wristRotation.eulerAngles.normalized;
            handInputs[15] = InverseLerp(mines[i],maxes[i],rote.x);
            handInputs[16] = InverseLerp(mines[i],maxes[i],rote.y);
            handInputs[17] = InverseLerp(mines[i],maxes[i],rote.z);
            return handInputs;
        }
        public static double InverseLerp(double a, double b, double value)
        { 
            return (value - a) / (b - a);
        }

        public static double[] ConvertHandInputs(LegacyHandPostionsForCalc data)
        {
            double[] handInputs = new double[18];
            handInputs[0]=InverseLerp(mines[0],maxes[0],GetThumbKnuckleAngle(data.knuckles[0],data.knuckles[4]));
            handInputs[1]=InverseLerp(mines[1],maxes[1],GetCornerAngle(data.tips[0],data.wristPosition));
            handInputs[2]=InverseLerp(mines[2],maxes[2],GetOffset(data.tips[0],data.tips[1]));
            handInputs[3]=InverseLerp(mines[3],maxes[3],GetFingerKnuckleAngle(data.corners[1],data.wristPosition));
            handInputs[4]=InverseLerp(mines[4],maxes[4],GetCornerAngle(data.tips[1],data.knuckles[1]));
            handInputs[5]=InverseLerp(mines[5],maxes[5],GetOffset(data.tips[1],data.tips[2]));
            handInputs[6]=InverseLerp(mines[6],maxes[6],GetFingerKnuckleAngle(data.corners[2],data.wristPosition));
            handInputs[7]=InverseLerp(mines[7],maxes[7],GetCornerAngle(data.tips[2],data.knuckles[2]));
            handInputs[8]=InverseLerp(mines[8],maxes[8],GetOffset(data.tips[2],data.tips[3]));
            handInputs[9]=InverseLerp(mines[9],maxes[9],GetFingerKnuckleAngle(data.corners[3],data.wristPosition));
            handInputs[10]=InverseLerp(mines[10],maxes[10],GetCornerAngle(data.tips[3],data.knuckles[3]));
            handInputs[11]=InverseLerp(mines[11],maxes[11],GetOffset(data.tips[3],data.tips[4]));
            handInputs[12]=InverseLerp(mines[12],maxes[12],GetFingerKnuckleAngle(data.corners[4],data.wristPosition));
            handInputs[13]=InverseLerp(mines[13],maxes[13],GetCornerAngle(data.tips[4],data.knuckles[4]));
            handInputs[14]=InverseLerp(mines[14],maxes[14],GetOffset(data.tips[4],data.tips[0]));
            
            Vector3 rote = data.wristRotation.eulerAngles.normalized;
            handInputs[15] = InverseLerp(mines[15],maxes[15],rote.x);
            handInputs[16] = InverseLerp(mines[16],maxes[16],rote.y);
            handInputs[17] = InverseLerp(mines[17],maxes[17],rote.z);
            return handInputs;
        }
    }
}