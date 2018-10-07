using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Assets.Script;
//using RoExe;

public class HandController : MonoBehaviour
{


    //this is the write lock for data array
    [HideInInspector]
    public bool Mutex;
    //this is the update_flag for animation
    public bool update_flag;
    public int Type = 0;
    //this is the data array
    /*
	 * 0.手w* 1.手x* 2.手y* 3.手z* 
	 * 4.小指远角 5.无名指远角* 6.中指远角* 7.食指远角* 8.小指近角 9.无名近角* 10.中指近角* 11.食指近角
	 * 12.小指左右* 13.无名指左右* 14.中指左右* 15.食指左右
	 * 16-19.拇指第二关节四元数* 20-23.拇指第三关节四元数
	 * 24-27.前臂四元数* 28-31.上臂四元数* 32-35.肩四元数
     */
    private const int data_amount = 36;
    private float[] ori_data = new float[data_amount];
    private float[] data = new float[data_amount];
    private float[] op_data = new float[data_amount];
    private SkeletonJson data_right;
    private SkeletonJson data_left;
    //left hand fingers: pinkey(1),ring(2),middle(3),index(4),thumb(5)
    /*
	public Transform left_pinkey_1,left_pinkey_2,left_pinkey_3,left_ring_1,left_ring_2,left_ring_3,left_middle_1,left_middle_2;
	public Transform left_middle_3,left_index_1,left_index_2,left_index_3,left_thumb_1,left_thumb_2,left_thumb_3,left_hand,left_fore_arm;
	*/

    //right hand fingers: pinkey(6), ring(7), middle(8), index(9), thumb(0)
    public Transform right_pinkey_1, right_pinkey_2, right_pinkey_3, right_ring_1, right_ring_2, right_ring_3, right_middle_1, right_middle_2;
    public Transform right_middle_3, right_index_1, right_index_2, right_index_3, right_thumb_1, right_thumb_2, right_thumb_3;
    public Transform right_hand, right_fore_arm, right_middle_arm, right_upper_arm;
    private Transform[] trans_right = new Transform[Definition.JOINT_COUNT];
    //
    public Transform left_pinkey_1, left_pinkey_2, left_pinkey_3, left_ring_1, left_ring_2, left_ring_3, left_middle_1, left_middle_2;
    public Transform left_middle_3, left_index_1, left_index_2, left_index_3, left_thumb_1, left_thumb_2, left_thumb_3;
    public Transform left_hand, left_fore_arm, left_middle_arm, left_upper_arm;

    private Transform[] trans_left = new Transform[Definition.JOINT_COUNT];
    private bool ini_flag = false;

    //【ztp】
    //private RotationExecutor reRThumb3, reRThumb2, reRHand, reRForeArm, reRArm, reRShoulder;
    //private RotationExecutor[] reArray = new RotationExecutor[(data_amount - 12) / 4];
    public ModelType modelType = 0;
    void Start()
    {

        // trans_right 的0 依然是hand
        //               1-3  是小指  ； 4-6 是拇指；7-9是食指指 ； 10-12是中指； 13-16是无名指
        trans_right[0] = right_hand;
        //trans_right[1] = right_thumb_1;
        //trans_right[2] = right_thumb_2;
        //trans_right[3] = right_thumb_3;
        //trans_right[4] = right_index_1;
        //trans_right[5] = right_index_2;
        //trans_right[6] = right_index_3;
        //trans_right[7] = right_middle_1;
        //trans_right[8] = right_middle_2;
        //trans_right[9] = right_middle_3;
        //trans_right[10] = right_ring_1;
        //trans_right[11] = right_ring_2;
        //trans_right[12] = right_ring_3;
        //trans_right[13] = right_pinkey_1;
        //trans_right[14] = right_pinkey_2;
        //trans_right[15] = right_pinkey_3;


        trans_right[1] = right_pinkey_1;
        trans_right[2] = right_pinkey_2;
        trans_right[3] = right_pinkey_3;
        trans_right[4] = right_thumb_1;
        trans_right[5] = right_thumb_2;
        trans_right[6] = right_thumb_3;
        trans_right[7] = right_index_1;
        trans_right[8] = right_index_2;
        trans_right[9] = right_index_3;
        trans_right[10] = right_middle_1;
        trans_right[11] = right_middle_2;
        trans_right[12] = right_middle_3;
        trans_right[13] = right_ring_1;
        trans_right[14] = right_ring_2;
        trans_right[15] = right_ring_3;



        trans_right[16] = right_fore_arm;
        trans_right[17] = right_middle_arm;
        trans_right[18] = right_upper_arm;






        trans_left[0] = left_hand;
        trans_left[1] = left_thumb_1;
        trans_left[2] = left_thumb_2;
        trans_left[3] = left_thumb_3;
        trans_left[4] = left_index_1;
        trans_left[5] = left_index_2;
        trans_left[6] = left_index_3;
        trans_left[7] = left_middle_1;
        trans_left[8] = left_middle_2;
        trans_left[9] = left_middle_3;
        trans_left[10] = left_ring_1;
        trans_left[11] = left_ring_2;
        trans_left[12] = left_ring_3;
        trans_left[13] = left_pinkey_1;
        trans_left[14] = left_pinkey_2;
        trans_left[15] = left_pinkey_3;
        trans_left[16] = left_fore_arm;
        trans_left[17] = left_middle_arm;
        trans_left[18] = left_upper_arm;
        Mutex = false;
        update_flag = false;
        if (Type == 1)
        {

            StreamWriter sw = new StreamWriter(new FileStream("Export\\" + gameObject.name + ".txt", FileMode.Create));
            //matrix
            for (int i = 0; i < Definition.JOINT_COUNT; i++)
            {
                try
                {
                    sw.WriteLine("{0:f8} {1:f8} {2:f8}", trans_right[i].localPosition.x, trans_right[i].localPosition.y, trans_right[i].localPosition.z);
                    sw.WriteLine("{0:f8} {1:f8} {2:f8} {3:f8}", trans_right[i].localRotation.x, trans_right[i].localRotation.y, trans_right[i].localRotation.z, trans_right[i].localRotation.w);

                }
                catch (System.Exception ex)
                {
                    continue;
                }
            }


            for (int i = 0; i < Definition.JOINT_COUNT; i++)
            {
                try
                {
                    sw.WriteLine("{0:f8} {1:f8} {2:f8}", trans_left[i].localPosition.x, trans_left[i].localPosition.y, trans_left[i].localPosition.z);
                    sw.WriteLine("{0:f8} {1:f8} {2:f8} {3:f8}", trans_left[i].localRotation.x, trans_left[i].localRotation.y, trans_left[i].localRotation.z, trans_left[i].localRotation.w);

                }
                catch (System.Exception ex)
                {
                    continue;
                }
            }
            //Debug.Log(Matrix4x4.TRS(right_hand.localPosition, right_hand.localRotation, right_hand.localScale));

            #region test message
            //string s = string.Format("right_hand.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_hand.localPosition.x, right_hand.localPosition.y, right_hand.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_1.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_thumb_1.localPosition.x, right_thumb_1.localPosition.y, right_thumb_1.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_2.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_thumb_2.localPosition.x, right_thumb_2.localPosition.y, right_thumb_2.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_3.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_thumb_3.localPosition.x, right_thumb_3.localPosition.y, right_thumb_3.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_index_1.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_index_1.localPosition.x, right_index_1.localPosition.y, right_index_1.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_index_2.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_index_2.localPosition.x, right_index_2.localPosition.y, right_index_2.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_index_3.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_index_3.localPosition.x, right_index_3.localPosition.y, right_index_3.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_1.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_middle_1.localPosition.x, right_middle_1.localPosition.y, right_middle_1.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_2.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_middle_2.localPosition.x, right_middle_2.localPosition.y, right_middle_2.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_3.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_middle_3.localPosition.x, right_middle_3.localPosition.y, right_middle_3.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_1.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_ring_1.localPosition.x, right_ring_1.localPosition.y, right_ring_1.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_2.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_ring_2.localPosition.x, right_ring_2.localPosition.y, right_ring_2.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_3.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_ring_3.localPosition.x, right_ring_3.localPosition.y, right_ring_3.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_1.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_pinkey_1.localPosition.x, right_pinkey_1.localPosition.y, right_pinkey_1.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_2.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_pinkey_2.localPosition.x, right_pinkey_2.localPosition.y, right_pinkey_2.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_3.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_pinkey_3.localPosition.x, right_pinkey_3.localPosition.y, right_pinkey_3.localPosition.z);
            //sw.WriteLine(s);
            ////translation
            //s = string.Format("right_hand.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_hand.localPosition.x, right_hand.localPosition.y, right_hand.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_1.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_thumb_1.localPosition.x, right_thumb_1.localPosition.y, right_thumb_1.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_2.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_thumb_2.localPosition.x, right_thumb_2.localPosition.y, right_thumb_2.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_3.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_thumb_3.localPosition.x, right_thumb_3.localPosition.y, right_thumb_3.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_index_1.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_index_1.localPosition.x, right_index_1.localPosition.y, right_index_1.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_index_2.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_index_2.localPosition.x, right_index_2.localPosition.y, right_index_2.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_index_3.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_index_3.localPosition.x, right_index_3.localPosition.y, right_index_3.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_1.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_middle_1.localPosition.x, right_middle_1.localPosition.y, right_middle_1.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_2.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_middle_2.localPosition.x, right_middle_2.localPosition.y, right_middle_2.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_3.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_middle_3.localPosition.x, right_middle_3.localPosition.y, right_middle_3.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_1.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_ring_1.localPosition.x, right_ring_1.localPosition.y, right_ring_1.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_2.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_ring_2.localPosition.x, right_ring_2.localPosition.y, right_ring_2.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_3.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_ring_3.localPosition.x, right_ring_3.localPosition.y, right_ring_3.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_1.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_pinkey_1.localPosition.x, right_pinkey_1.localPosition.y, right_pinkey_1.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_2.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_pinkey_2.localPosition.x, right_pinkey_2.localPosition.y, right_pinkey_2.localPosition.z);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_3.Translate(new Vector3D({0:f7},{1:f7},{2:f7}));", right_pinkey_3.localPosition.x, right_pinkey_3.localPosition.y, right_pinkey_3.localPosition.z);
            //sw.WriteLine(s);
            ////rotation
            //s = string.Format("right_hand.Rotate (new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_hand.localRotation.x, right_hand.localRotation.y, right_hand.localRotation.z, right_hand.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_1.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_thumb_1.localRotation.x, right_thumb_1.localRotation.y, right_thumb_1.localRotation.z, right_thumb_1.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_2.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_thumb_2.localRotation.x, right_thumb_2.localRotation.y, right_thumb_2.localRotation.z, right_thumb_2.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_3.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_thumb_3.localRotation.x, right_thumb_3.localRotation.y, right_thumb_3.localRotation.z, right_thumb_3.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_index_1.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_index_1.localRotation.x, right_index_1.localRotation.y, right_index_1.localRotation.z, right_index_1.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_index_2.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_index_2.localRotation.x, right_index_2.localRotation.y, right_index_2.localRotation.z, right_index_2.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_index_3.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_index_3.localRotation.x, right_index_3.localRotation.y, right_index_3.localRotation.z, right_index_3.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_1.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_middle_1.localRotation.x, right_middle_1.localRotation.y, right_middle_1.localRotation.z, right_middle_1.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_2.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_middle_2.localRotation.x, right_middle_2.localRotation.y, right_middle_2.localRotation.z, right_middle_2.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_3.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_middle_3.localRotation.x, right_middle_3.localRotation.y, right_middle_3.localRotation.z, right_middle_3.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_1.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_ring_1.localRotation.x, right_ring_1.localRotation.y, right_ring_1.localRotation.z, right_ring_1.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_2.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_ring_2.localRotation.x, right_ring_2.localRotation.y, right_ring_2.localRotation.z, right_ring_2.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_3.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_ring_3.localRotation.x, right_ring_3.localRotation.y, right_ring_3.localRotation.z, right_ring_3.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_1.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_pinkey_1.localRotation.x, right_pinkey_1.localRotation.y, right_pinkey_1.localRotation.z, right_pinkey_1.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_2.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_pinkey_2.localRotation.x, right_pinkey_2.localRotation.y, right_pinkey_2.localRotation.z, right_pinkey_2.localRotation.w);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_3.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_pinkey_3.localRotation.x, right_pinkey_3.localRotation.y, right_pinkey_3.localRotation.z, right_pinkey_3.localRotation.w);
            //sw.WriteLine(s);
            ////euler
            //s = string.Format("right_hand.Rotate (new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_hand.localEulerAngles.x, right_hand.localEulerAngles.y, right_hand.localEulerAngles.z, right_hand.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_1.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_thumb_1.localEulerAngles.x, right_thumb_1.localEulerAngles.y, right_thumb_1.localEulerAngles.z, right_thumb_1.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_2.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_thumb_2.localEulerAngles.x, right_thumb_2.localEulerAngles.y, right_thumb_2.localEulerAngles.z, right_thumb_2.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_thumb_3.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_thumb_3.localEulerAngles.x, right_thumb_3.localEulerAngles.y, right_thumb_3.localEulerAngles.z, right_thumb_3.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_index_1.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_index_1.localEulerAngles.x, right_index_1.localEulerAngles.y, right_index_1.localEulerAngles.z, right_index_1.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_index_2.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_index_2.localEulerAngles.x, right_index_2.localEulerAngles.y, right_index_2.localEulerAngles.z, right_index_2.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_index_3.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_index_3.localEulerAngles.x, right_index_3.localEulerAngles.y, right_index_3.localEulerAngles.z, right_index_3.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_1.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_middle_1.localEulerAngles.x, right_middle_1.localEulerAngles.y, right_middle_1.localEulerAngles.z, right_middle_1.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_2.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_middle_2.localEulerAngles.x, right_middle_2.localEulerAngles.y, right_middle_2.localEulerAngles.z, right_middle_2.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_middle_3.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_middle_3.localEulerAngles.x, right_middle_3.localEulerAngles.y, right_middle_3.localEulerAngles.z, right_middle_3.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_1.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_ring_1.localEulerAngles.x, right_ring_1.localEulerAngles.y, right_ring_1.localEulerAngles.z, right_ring_1.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_2.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_ring_2.localEulerAngles.x, right_ring_2.localEulerAngles.y, right_ring_2.localEulerAngles.z, right_ring_2.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_ring_3.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_ring_3.localEulerAngles.x, right_ring_3.localEulerAngles.y, right_ring_3.localEulerAngles.z, right_ring_3.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_1.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_pinkey_1.localEulerAngles.x, right_pinkey_1.localEulerAngles.y, right_pinkey_1.localEulerAngles.z, right_pinkey_1.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_2.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_pinkey_2.localEulerAngles.x, right_pinkey_2.localEulerAngles.y, right_pinkey_2.localEulerAngles.z, right_pinkey_2.localEulerAngles.x);
            //sw.WriteLine(s);
            //s = string.Format("right_pinkey_3.Rotate(new Quaternion({0:f7},{1:f7},{2:f7},{3:f7}));", right_pinkey_3.localEulerAngles.x, right_pinkey_3.localEulerAngles.y, right_pinkey_3.localEulerAngles.z, right_pinkey_3.localEulerAngles.x);
            //sw.WriteLine(s); 
            #endregion

            sw.Close();
        }

        Mutex = false;
        update_flag = false;

        //【ztp】
        //reRThumb3 = new RotationExecutor(right_thumb_3, 0);
        //reRThumb2 = new RotationExecutor(right_thumb_2, 0);
        //reRHand = new RotationExecutor(right_hand, 0);
        //reRForeArm = new RotationExecutor(right_fore_arm, 0);
        //reRArm = new RotationExecutor(right_upper_arm, 0);
        //reRShoulder = new RotationExecutor();

        //reArray[0] = reRHand;
        //reArray[1] = reRThumb2;
        //reArray[2] = reRThumb3;
        //reArray[3] = reRForeArm;
        //reArray[4] = reRArm;
        //reArray[5] = reRShoulder;
    }

    public void update_data(SkeletonJson result)
    {
        Mutex = true;
        if (result != null)
        {
            
            data_right = result;
            
        }

        Mutex = false;
    }

    void RotateFromQ(Transform t, Quaternion q)
    {
        t.localRotation = q;
    }

    float RestrictAngle(float angle, float min, float max)
    {
        angle = angle > 180 ? angle - 360 : angle;
        if (angle <= max && angle >= min)
        {
            return angle;
        }
        var m = (min + max) / 2;//middle point
        if (m > 0)
        {//middle point on right
            if (angle >= m - 180 && angle < min)
            {
                return min;
            }
            else
                return max;
        }
        else
        {
            if (angle <= m + 180 && angle > max)
            {
                return max;
            }
            else
                return min;
        }
    }
    void Update()
    {
        if (data_right != null)
        {

            ////RotateFromQ(trans_right[0], data_right.Joints[0].ToQuaternion());
            //var qq = data_right.Joints[0].ToQuaternion();
            //var euler1 = new Vector3();
            //euler1.z = Mathf.Atan2(2 * (qq.w * qq.z + qq.x * qq.y), 1 - 2 * (qq.z * qq.z + qq.x * qq.x));
            //euler1.x = Mathf.Asin(2 * (qq.w * qq.x - qq.z * qq.y));
            //euler1.y = Mathf.Atan2(2 * (qq.w * qq.y + qq.x * qq.z), 1 - 2 * (qq.x * qq.x + qq.y * qq.y));
            //euler1 = euler1 * Mathf.Rad2Deg;


            //var qqea = qq.eulerAngles;
            //var qqq = Quaternion.Euler(qqea.x, -qqea.y, -qqea.z);
            //RotateFromQ(trans_right[0], qqq);


            ////Debug.Log("this is my euler" + euler1 + "this is unity euler" + qqea);

            //for (int i = 1; i < 2; i++)
            //{
            //    int index = 1;
            //    //near joint
            //    var q = data_right.Joints[index + 1].ToQuaternion();
            //    var ea = q.eulerAngles;
            //    //bend angle
            //    var bend = ea.z;
            //    if (modelType == ModelType.HandOnly)
            //    {
            //        //new model
            //        bend = RestrictAngle(bend, -98, 10);
            //    }
            //    else
            //    {
            //        //old model
            //        bend = RestrictAngle(bend, -10, 90);
            //    }
            //    //strech angle
            //    var strech = ea.y;

            //    strech = RestrictAngle(strech, -30, 10);

            //    if (bend > -30)
            //    {
            //        strech = strech;
            //    }
            //    else if (bend < -50)
            //    {
            //        strech = 0;
            //    }
            //    else
            //    {
            //        strech *= (bend + 50) / 20.0f;
            //    }
            //    q = Quaternion.AngleAxis(strech, new Vector3(0, 1, 0));
            //    q *= Quaternion.AngleAxis(bend, new Vector3(0, 0, 1));
            //    RotateFromQ(trans_right[index], q);//near

            //    //middle joint
            //    q = data_right.Joints[index + 2].ToQuaternion();
            //    if (i == 1)
            //    {
            //        //Debug.Log (q.eulerAngles);
            //    }
            //    float bend1 = 0;
            //    if (modelType == ModelType.HandOnly)
            //    {
            //        //new model
            //        bend1 = RestrictAngle(q.eulerAngles.z, -90, 0);
            //    }
            //    else
            //    {
            //        //old model
            //        bend1 = RestrictAngle(q.eulerAngles.z, 0, 90);
            //    }
            //    q = Quaternion.AngleAxis(bend1, new Vector3(0, 0, 1));
            //    RotateFromQ(trans_right[index + 1], q);

            //    //far joint
            //    q = data_right.Joints[index + 2].ToQuaternion();
            //    float bend2 = 0;
            //    //double hand 
            //    if (modelType == ModelType.HandOnly)
            //    {
            //        //new model
            //        bend2 = RestrictAngle(q.eulerAngles.z, -90, 0);
            //    }
            //    else
            //    {
            //        //old model
            //        bend2 = RestrictAngle(q.eulerAngles.z, 0, 90);
            //    }
            //    //old model
            //    q = Quaternion.AngleAxis(bend2, new Vector3(0, 0, 1));
            //    RotateFromQ(trans_right[index + 2], q);

            //}

            //for (int i = 4; i < 7; i++)
            //{
            //    RotateFromQ(trans_right[i], data_right.Joints[i].ToQuaternion());
            //}

            //for (int i = 1; i < 4; i++)
            //{
            //    int index = i * 3 + 4;
            //    //near joint
            //    var q = data_right.Joints[index].ToQuaternion();
            //    var ea = q.eulerAngles;
            //    //bend angle
            //    var bend = ea.z;
            //    if (modelType == ModelType.HandOnly)
            //    {
            //        //new model
            //        bend = RestrictAngle(bend, -98, 10);
            //    }
            //    else
            //    {
            //        //old model
            //        bend = RestrictAngle(bend, -10, 90);
            //    }
            //    //strech angle
            //    var strech = ea.y;
            //    if (i == 1)
            //    {//index finger
            //        strech = RestrictAngle(strech, -10, 30);
            //    }
            //    else if (i == 2)
            //    { //middle finger
            //        strech = RestrictAngle(strech, -10, 10);
            //    }
            //    else
            //    {//ring and little
            //        strech = RestrictAngle(strech, -30, 10);
            //    }
            //    if (bend > -30)
            //    {
            //        strech = strech;
            //    }
            //    else if (bend < -50)
            //    {
            //        strech = 0;
            //    }
            //    else
            //    {
            //        strech *= (bend + 50) / 20.0f;
            //    }
            //    q = Quaternion.AngleAxis(strech, new Vector3(0, 1, 0));
            //    q *= Quaternion.AngleAxis(bend, new Vector3(0, 0, 1));
            //    RotateFromQ(trans_right[index], q);//near

            //    //middle joint
            //    q = data_right.Joints[index + 1].ToQuaternion();
            //    if (i == 1)
            //    {
            //        //Debug.Log (q.eulerAngles);
            //    }
            //    float bend1 = 0;
            //    if (modelType == ModelType.HandOnly)
            //    {
            //        //new model
            //        bend1 = RestrictAngle(q.eulerAngles.z, -90, 0);
            //    }
            //    else
            //    {
            //        //old model
            //        bend1 = RestrictAngle(q.eulerAngles.z, 0, 90);
            //    }
            //    q = Quaternion.AngleAxis(bend1, new Vector3(0, 0, 1));
            //    RotateFromQ(trans_right[index + 1], q);

            //    //far joint
            //    q = data_right.Joints[index + 2].ToQuaternion();
            //    float bend2 = 0;
            //    //double hand 
            //    if (modelType == ModelType.HandOnly)
            //    {
            //        //new model
            //        bend2 = RestrictAngle(q.eulerAngles.z, -90, 0);
            //    }
            //    else
            //    {
            //        //old model
            //        bend2 = RestrictAngle(q.eulerAngles.z, 0, 90);
            //    }
            //    //old model
            //    q = Quaternion.AngleAxis(bend2, new Vector3(0, 0, 1));
            //    RotateFromQ(trans_right[index + 2], q);
            //}
            ////right  arm
            //RotateFromQ(trans_right[16], data_right.Joints[16].ToQuaternion());
            //RotateFromQ(trans_right[17], data_right.Joints[17].ToQuaternion());
            //RotateFromQ(trans_right[18], data_right.Joints[18].ToQuaternion());


            //RotateFromQ(trans_right[0], data_right.Joints[0].ToQuaternion());
            var qq = data_right.Joints[0].ToQuaternion();
            //Debug.Log("qq.x: " + qq.x + "qqq1.y:" + qq.y + "qq.z:" + qq.z + "qq.w:" + qq.w);
            var euler1 = new Vector3();
            euler1.z = Mathf.Atan2(2 * (qq.w * qq.z + qq.x * qq.y), 1 - 2 * (qq.z * qq.z + qq.x * qq.x));
            euler1.x = Mathf.Asin(2 * (qq.w * qq.x - qq.z * qq.y));
            euler1.y = Mathf.Atan2(2 * (qq.w * qq.y + qq.x * qq.z), 1 - 2 * (qq.x * qq.x + qq.y * qq.y));
            euler1 = euler1 * Mathf.Rad2Deg;


            var qqea = qq.eulerAngles;
            var qqq = Quaternion.Euler(qqea.x, -qqea.y, -qqea.z);
            RotateFromQ(trans_right[0], qqq);

            //Debug.Log("qqq is: " + qqq);
            //Debug.Log("this is my euler" + euler1 + "this is unity euler" + qqea);

            for (int i = 1; i < 2; i++)
            {
                int index = 1;
                //near joint
                var q = data_right.Joints[index + 1].ToQuaternion();
                var ea = q.eulerAngles;
                //bend angle
                var bend = ea.z;
                
                bend = RestrictAngle(bend, -98, 10);
                
                
                //strech angle
                var strech = ea.y -180;

                strech = RestrictAngle(strech, -10, 30);

                if (bend > -30)
                {
                    strech = strech;
                }
                else if (bend < -50)
                {
                    strech = 0;
                }
                else
                {
                    strech *= (bend + 50) / 20.0f;
                }
                q = Quaternion.AngleAxis(-strech, new Vector3(0, 1, 0));
                q *= Quaternion.AngleAxis(bend, new Vector3(0, 0, 1));
                RotateFromQ(trans_right[index], q);//near

                //middle joint
                q = data_right.Joints[index + 2].ToQuaternion();    
                float bend1 = 0;  
                bend1 = RestrictAngle(q.eulerAngles.z, 0, 90);
                q = Quaternion.AngleAxis(-bend1, new Vector3(0, 0, 1));
                RotateFromQ(trans_right[index + 1], q);

                //far joint
                q = data_right.Joints[index + 2].ToQuaternion();
                float bend2 = 0;
                bend2 = RestrictAngle(q.eulerAngles.z, 0, 90);
                q = Quaternion.AngleAxis(-bend2, new Vector3(0, 0, 1));
                RotateFromQ(trans_right[index + 2], q);

            }

            //RotateFromQ(trans_right[4], data_right.Joints[13].ToQuaternion());
            
            
            var qqqea = data_right.Joints[13].ToQuaternion().eulerAngles;
            var qqqq = Quaternion.Euler(qqqea.x, -qqqea.y + 15, -qqqea.z);
            var qy = Quaternion.Euler(0, 180, 0);
            RotateFromQ(trans_right[4], qqqq*qy);
            //Debug.Log("thumb euler is: " + qqqea);

            for (int i = 5; i < 7; i++)
            {
                //RotateFromQ(trans_right[i], data_right.Joints[9 + i].ToQuaternion());
                var q = data_right.Joints[14].ToQuaternion();
                float bend1 = 0;
                bend1 = RestrictAngle(q.eulerAngles.z, 0, 90);
                q = Quaternion.AngleAxis(-bend1, new Vector3(0, 0, 1));
                RotateFromQ(trans_right[i], q);
            }

            for (int i = 1; i < 4; i++)
            {
                int index = i * 3 + 4;
                //near joint
                var q = data_right.Joints[13-3*i].ToQuaternion();
                var ea = q.eulerAngles;
                //bend angle
                var bend = ea.z;
                
                bend = RestrictAngle(bend, -98, 10);

                //strech angle
                var strech = ea.y -180 ;
                if (i == 1)
                {//index finger
                    strech = RestrictAngle(strech, -30, 10);
                }
                else if (i == 2)
                { //middle finger
                    strech = RestrictAngle(strech, -10, 10);
                }
                else
                {//ring and little
                    strech = RestrictAngle(strech, -10, 20);
                }
                if (bend > -30)
                {
                    strech = strech;
                }
                else if (bend < -50)
                {
                    strech = 0;
                }
                else
                {
                    strech *= (bend + 50) / 20.0f;
                }
                q = Quaternion.AngleAxis(-strech, new Vector3(0, 1, 0));
                q *= Quaternion.AngleAxis(bend, new Vector3(0, 0, 1));
                RotateFromQ(trans_right[index], q);//near

                //middle joint
                q = data_right.Joints[14-3*i].ToQuaternion();
                float bend1 = 0;
                bend1 = RestrictAngle(q.eulerAngles.z, 0, 90);
                q = Quaternion.AngleAxis(-bend1, new Vector3(0, 0, 1));
                RotateFromQ(trans_right[index + 1], q);
                RotateFromQ(trans_right[index + 2], q);
            }
            //right  arm
            RotateFromQ(trans_right[16], data_right.Joints[16].ToQuaternion());
            RotateFromQ(trans_right[17], data_right.Joints[17].ToQuaternion());
            RotateFromQ(trans_right[18], data_right.Joints[18].ToQuaternion());

        }

        //left
        if (data_left != null)
        {
            for (int i = 0; i < 4; i++)
            {
                RotateFromQ(trans_left[i], data_left.Joints[i].ToQuaternion());
            }
            for (int i = 0; i < 4; i++)
            {
                int index = i * 3 + 4;
                //near joint
                var q = data_left.Joints[index].ToQuaternion();
                var ea = q.eulerAngles;
                //bend angle
                var bend = ea.z;
                bend = RestrictAngle(bend, -98, 10);
                //strech angle
                var strech = ea.y;
                if (i == 0)
                {//index finger
                    strech = RestrictAngle(strech, -10, 30);
                }
                else if (i == 1)
                { //middle finger
                    strech = RestrictAngle(strech, -10, 10);
                }
                else
                {//ring and little
                    strech = RestrictAngle(strech, -30, 10);
                }
                if (bend > -30)
                {
                    strech = strech;
                }
                else if (bend < -50)
                {
                    strech = 0;
                }
                else
                {
                    strech *= (bend + 50) / 20.0f;
                }
                q = Quaternion.AngleAxis(strech, new Vector3(0, 1, 0));
                q *= Quaternion.AngleAxis(bend, new Vector3(0, 0, 1));
                RotateFromQ(trans_left[index], q);//near

                //middle joint
                q = data_left.Joints[index + 1].ToQuaternion();
                var bend1 = RestrictAngle(q.eulerAngles.z, -94, 0);
                q = Quaternion.AngleAxis(bend1, new Vector3(0, 0, 1));
                RotateFromQ(trans_left[index + 1], q);
                //far joint
                q = data_left.Joints[index + 2].ToQuaternion();
                var bend2 = RestrictAngle(q.eulerAngles.z, -94, 0);
                q = Quaternion.AngleAxis(bend2, new Vector3(0, 0, 1));
                RotateFromQ(trans_left[index + 2], q);
            }
            //left fore arm
            //RotateFromQ(trans_left[16], data_left.Joints[16].ToQuaternion());
        }
    }



}




