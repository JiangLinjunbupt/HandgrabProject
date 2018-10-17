using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Assets.Script;


class HandOptimizedDataController: MonoBehaviour
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
    private HandInf data_right;
    private HandInf data_left;
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
    public float PIover180 = 0.0174532925f;
    //【ztp】
    //private RotationExecutor reRThumb3, reRThumb2, reRHand, reRForeArm, reRArm, reRShoulder;
    //private RotationExecutor[] reArray = new RotationExecutor[(data_amount - 12) / 4];
    public ModelType modelType = 0;
    void Start()
    {

        // trans_right 的0 依然是hand
        //               1-3  是小指  ； 4-6 是拇指；7-9是食指指 ； 10-12是中指； 13-16是无名指
        trans_right[0] = right_hand;
        trans_right[1] = right_thumb_1;
        trans_right[2] = right_thumb_2;
        trans_right[3] = right_thumb_3;
        trans_right[4] = right_index_1;
        trans_right[5] = right_index_2;
        trans_right[6] = right_index_3;
        trans_right[7] = right_middle_1;
        trans_right[8] = right_middle_2;
        trans_right[9] = right_middle_3;
        trans_right[10] = right_ring_1;
        trans_right[11] = right_ring_2;
        trans_right[12] = right_ring_3;
        trans_right[13] = right_pinkey_1;
        trans_right[14] = right_pinkey_2;
        trans_right[15] = right_pinkey_3;


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

            sw.Close();
        }

        Mutex = false;
        update_flag = false;
    }

    public void update_data(HandInf result)
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
    void ReLocation(Transform t, float x, float y, float z)
    {
        //以（-15,0，-10）为起始点，其中x范围（-15,15）y范围（0,10）z范围（-10,15）
        Vector3 newposition = new Vector3(-15 + x * 30, 0 + y * 10, -5 + z * 25);
        t.position = Vector3.Lerp(t.position, newposition, 5 * Time.deltaTime);
    }

    void Update()
    {
        if (data_right != null)
        {
            var ex = -data_right.global_roll_x * PIover180 / 2;
            var ey = data_right.global_yaw_z * PIover180 / 2;
            var ez = data_right.global_pitch_y * PIover180 / 2;
            var qqq1 = Quaternion.Euler(new Vector3(0, 0, 0));

            qqq1.w = (Mathf.Cos(ex) * Mathf.Cos(ez) * Mathf.Cos(ey) + Mathf.Sin(ex) * Mathf.Sin(ez) * Mathf.Sin(ey));
            qqq1.x = (Mathf.Sin(ex) * Mathf.Cos(ez) * Mathf.Cos(ey) - Mathf.Cos(ex) * Mathf.Sin(ez) * Mathf.Sin(ey));
            qqq1.y = (Mathf.Cos(ex) * Mathf.Cos(ez) * Mathf.Sin(ey) - Mathf.Sin(ex) * Mathf.Sin(ez) * Mathf.Cos(ey));
            qqq1.z = (Mathf.Cos(ex) * Mathf.Sin(ez) * Mathf.Cos(ey) + Mathf.Sin(ex) * Mathf.Cos(ez) * Mathf.Sin(ey));

            var qq = Quaternion.Euler(0, 90, 0);

            //Debug.Log("qqq1.x: " + qqq1.x + "qqq1.y:" + qqq1.y + "qqq1.z:" + qqq1.z + "qqq1.w:" + qqq1.w);
            RotateFromQ(trans_right[0], qqq1 * qq);
            ReLocation(trans_right[0], data_right.global_x, data_right.global_y, data_right.global_z);



            for (int i = 0; i < 5; i++)
            {
                //thumb
               if(i == 0)
                {
                    int index = i * 3;

                    var Ey = data_right.fingers[i].Mcp_x;
                    var Ez = data_right.fingers[i].Mcp_z;
                    var q = Quaternion.AngleAxis(Ez, new Vector3(0, 0, 1));
                    q *= Quaternion.AngleAxis(Ey, new Vector3(0, 1, 0));
                    //var euler3 = q.eulerAngles;
                    //q = Quaternion.Euler(new Vector3(euler3.x, euler3.y, euler3.z));
                    RotateFromQ(trans_right[index + 1], q);//near


                    var bend1 = -data_right.fingers[i].Pip;
                    q = Quaternion.AngleAxis(bend1, new Vector3(0, 0, 1));
                    RotateFromQ(trans_right[index + 2], q);//near

                    var bend2 = -data_right.fingers[i].Dip;
                    q = Quaternion.AngleAxis(bend2, new Vector3(0, 0, 1));
                    RotateFromQ(trans_right[index + 3], q);//near
                }
                else
                {
                    int index = i* 3;
                    var bend = -data_right.fingers[i].Mcp_x;
                    var strech = data_right.fingers[i].Mcp_z;
                    var q = Quaternion.AngleAxis(strech, new Vector3(0, 1, 0));
                    q *= Quaternion.AngleAxis(bend, new Vector3(0, 0, 1));
                    RotateFromQ(trans_right[index + 1], q);//near

                    var bend1 = -data_right.fingers[i].Pip;
                    q = Quaternion.AngleAxis(bend1, new Vector3(0, 0, 1));
                    RotateFromQ(trans_right[index + 2], q);//near

                    var bend2 = -data_right.fingers[i].Dip;
                    q = Quaternion.AngleAxis(bend2, new Vector3(0, 0, 1));
                    RotateFromQ(trans_right[index + 3], q);//near
                }

            }

        }
    }
}

