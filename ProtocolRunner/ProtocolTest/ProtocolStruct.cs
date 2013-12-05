using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtocolTest
{
    class ProtocolStruct
    {
        public const byte MIN_LEN = (HEAD_LEN + LEN_LEN + ID_LEN + SN_LEN + OP_LEN + OB_LEN + RESERVE_LEN + CHKSUM_LEN);
        public const Int32 COMMAND_MAX_SIZE = 64;

        /*
         * 协议的固定辨析部分包括
         * 头部，辨析请求和应答
         * 操作，辨析写入和读取
         * 对象，辨析操作的目标
         * 长度，辨析当前帧的长度
         */
        public const Int32 HEAD_TYPE = 0;
        public const Int32 OP_TYPE = 1;
        public const Int32 OB_TYPE = 2;
        public const Int32 LEN_TYPE_H = 3;
        public const Int32 LEN_TYPE_L = 4;
        /*
         * 协议的固定辨析部分的总长度
         */
        public const Int32 COMMAND_CORE_SIZE = 5;

        /*
         * 协议固定长度部分的值
         * 包括头部，长度部分，id，sn，操作，对象，保留位，校验位，结果位
         */
        public const byte HEAD_LEN = 1;
        public const byte LEN_LEN = 2;
        public const byte ID_LEN = 1;
        public const byte SN_LEN = 12;
        public const byte OP_LEN = 1;
        public const byte OB_LEN = 1;
        public const byte CS_LEN = 1;
        public const byte PS_LEN = 1;
        public const byte RESERVE_LEN = 2;
        public const byte CHKSUM_LEN = 1;
        public const byte RESULT_LEN = 1;

        /*
         * 仅有固定位的请求帧（无结果位）长度，即最短帧
         */
        public const Int32 PURE_FRAME_LEN = (HEAD_LEN + LEN_LEN + ID_LEN + SN_LEN + OP_LEN + OB_LEN + RESERVE_LEN + CHKSUM_LEN);

        /*
         * 可确定的帧元素起始位置
         * 除了校验位，其他元素的起始位置均为确定的
         * 请求帧中，如果有数据，则数据位位POS_DATA_S，
         *           反之，则POS_DATA_S为校验位
         * 应答帧中，POS_DATA_S为结果位，
         *          如果有数据元素，POS_DATA_S+1为数据位起始位置
         *          反之，该位置为校验位
         */
        public const byte POS_HEAD = 0;
        const byte POS_LEN = (POS_HEAD + HEAD_LEN);
        //const Int32 POS_LEN_L = POS_LEN_H + 1);
        const byte POS_ID = (POS_LEN + LEN_LEN);
        const byte POS_SN = (POS_ID + ID_LEN);
        const byte POS_OP = (POS_SN + SN_LEN);
        const byte POS_OB = (POS_OP + OP_LEN);
        const byte POS_CS = (POS_OB + OB_LEN);
        const byte POS_PS = (POS_CS + CS_LEN);
        const byte POS_RESERVE = (POS_PS + PS_LEN);
        const byte POS_DATA_S = (POS_RESERVE + RESERVE_LEN);

        /*
         * 元素中，为有限集的元素值定义
         */
        /*
         * 0x7E 为请求
         * 0xE7 为应答
         */
        public const byte HEAD_REQ = 0x7E;
        public const byte HEAD_RSP = 0xE7;

        /*
         * 0x01 为读取
         * 0x02 为写入
         */
        public const byte OP_GET = 0x01;
        public const byte OP_SET = 0x02;

        /*
         * 0x01 目标设备的状态
         * 0x02 擦除数据
         * 0x03 写入数据
         * 0x04 写入结束
         * 0x05 控制红，蓝灯亮灭
         * 0x06 
         * 0x07 相位
         * 0x08 温度
         * 0x09 id和sn
         * 0x0A
         * 0x0B 
         * 0x0C 
         * 0x0D T0即无氧，
         * 0x0E
         * 0x0F 无补偿溶氧校准参数
         * 0x10 温度补偿参数
         * 0x11 用户调整参数
         * 0x12 
         */
        public const byte OB_STATUS = 0x01;
        public const byte OB_ERASE = 0x02;
        public const byte OB_WRITE = 0x03;
        public const byte OB_FINISH = 0x04;
        public const byte OB_LED_DRIVE = 0x05;
        public const byte OB_SAMPLE = 0x06;
        public const byte OB_HW_SW = 0x07;
        public const byte OB_TEMPERATURE = 0x08;
        public const byte OB_ID_SN = 0x09;
        public const byte OB_DETECT = 0x0A;
        public const byte OB_TANGENT = 0x0B;
        public const byte OB_REAL_IMAG = 0x0C;
        public const byte OB_T0 = 0x0D;
        public const byte OB_PROBE_NUM = 0x0E;
        public const byte OB_CALI_CO = 0x0F;
        public const byte OB_TEMP_CO = 0x10;
        public const byte OB_USR_CO = 0x11;
        public const byte OB_DO = 0x12;
        public const byte OB_PRESSURE = 0x13;
        public const byte OB_PROBE_SN = 0x14;
        public const byte OB_SALINITY = 0x15;
        public const byte OB_DEBUG_INFO = 0x16;

        public const int STATUS_C = 0;
        public const int STATUS_P = 1;

        /*
         * 带有数据信息的帧中，有具体格式的数据格式及相关值
         */
        public const Int32 DATA_BYTE = 1;
        public const Int32 DATA_INT = 2;
        public const Int32 DATA_FLOAT = 4;
        public const Int32 DATA_BYTE_FIRST = 0;
        public const Int32 DATA_INT_FIRST = 0;
        public const Int32 DATA_FLOAT_FIRST = 0;

        /*
         * 读取id和sn的应答帧中数据id和sn的起始位置
         */
        public const Int32 ID_SN_DATA_ID_POS = 0;
        public const Int32 ID_SN_DATA_SN_POS = 1;

        /*
         * 读取状态响应帧中数据位状态值
         *
         * 写入状态请求帧中数据位状态值
         */
        public const byte STATUS_BOOT = 0x01;
        public const byte STATUS_APP = 0x02;
        public const byte STATUS_LEN = 1;


        /*
         * 写入flash请求帧数据位地址与数据值
         * 写入flash请求帧数据位地址值
         */
        public const Int32 WRITE_FLASH_ADDRESS_POS = 0;
        public const Int32 WRITE_FLASH_ADDRESS_LEN = 2;
        public const Int32 WRITE_FLASH_DATA_POS = (WRITE_FLASH_ADDRESS_POS + WRITE_FLASH_ADDRESS_LEN);
        public const Int32 WRITE_FLASH_DATA_LEN_MAX = 39;

        /*
         *  获取采样平均值
         */
        /*
         *  V7.2
        public const Int32 SAMPLE_DATA_LED = 0;
        public const Int32 SAMPLE_DATA_NUM = 1;
        public const Int32 SAMPLE_DATA_LEN = 2;

        public const Int32 SAMPLE_DATA_VALUE = 0;
        public const byte SAMPLE_FIRST = 0x01;
        public const byte SAMPLE_SECOND = 0x02;
        public const byte SAMPLE_THIRD = 0x03;
        public const byte SAMPLE_FOURTH = 0x04;
         */
        public const Int32 SAMPLE_DATA_FIRST = DATA_FLOAT_FIRST;
        public const Int32 SAMPLE_DATA_SECOND = (SAMPLE_DATA_FIRST + DATA_FLOAT);
        public const Int32 SAMPLE_DATA_THIRD = (SAMPLE_DATA_SECOND + DATA_FLOAT);
        public const Int32 SAMPLE_DATA_FORTH = (SAMPLE_DATA_THIRD + DATA_FLOAT);
        public const Int32 SAMPLE_DATA_LEN = (SAMPLE_DATA_FORTH + DATA_FLOAT);

        /*
         *  获取相位值
         */
        public const Int32 PHASE_DATA_LEN = 1;

        /*
        *  获取real和imag
        */
        public const Int32 REAL_IMAGE_DATA_REAL = 0;
        public const Int32 REAL_IMAGE_DATA_IMAG = DATA_FLOAT;
        public const Int32 REAL_IMAG_DATA_LEN = (REAL_IMAGE_DATA_IMAG + DATA_FLOAT);

        public const byte REAL_IMAGE_DATA_REF = 0x01;
        public const byte REAL_IMAGE_DATA_SIGNAL = 0x02;
        public const byte REAL_IMAGE_DATA_ZERO = 0x03;

        /*
        *  获取未补偿校准参数
        */
        public const Int32 CALI_CO_DATA_L0 = 0;
        public const Int32 CALI_CO_DATA_L1 = DATA_FLOAT;
        public const Int32 CALI_CO_DATA_L2 = (CALI_CO_DATA_L1 + DATA_FLOAT);
        public const Int32 CALI_CO_DATA_L3 = (CALI_CO_DATA_L2 + DATA_FLOAT);
        public const Int32 CALI_CO_DATA_L4 = (CALI_CO_DATA_L3 + DATA_FLOAT);
        public const Int32 CALI_CO_DATA_LEN = (CALI_CO_DATA_L4 + DATA_FLOAT);

        /*
        *  获取温度补偿参数
        */
        public const Int32 TEMP_CO_DATA_L0 = 0;
        public const Int32 TEMP_CO_DATA_L1 = DATA_FLOAT;
        public const Int32 TEMP_CO_DATA_L2 = (CALI_CO_DATA_L1 + DATA_FLOAT);
        public const Int32 TEMP_CO_DATA_LEN = (TEMP_CO_DATA_L2 + DATA_FLOAT);

        /*
        *  获取用户校准参数
        */
        public const Int32 USR_CO_DATA_L0 = 0;
        public const Int32 USR_CO_DATA_L1 = DATA_FLOAT;
        public const Int32 USR_CO_DATA_LEN = (USR_CO_DATA_L1 + DATA_FLOAT);

        /*
         * 控制红蓝灯闪烁的模式
         */
        public const byte LED_RED = 0x01;
        public const byte LED_BLUE = 0x02;
        public const byte LED_RED_BLUE = 0x03;
        public const Int32 LED_DRIVE_RED_DAC_LEN = 2;
        public const Int32 LED_DRIVE_BLUE_DAC_LEN = 2;
        public const Int32 LED_DRIVE_DEPTH_LEN = 2;
        public const Int32 LED_DRIVE_RED_DAC_POS = 0;
        public const Int32 LED_DRIVE_BLUE_DAC_POS = (LED_DRIVE_RED_DAC_POS + LED_DRIVE_RED_DAC_LEN);
        public const Int32 LED_DRIVE_DEPTH_POS = (LED_DRIVE_BLUE_DAC_POS + LED_DRIVE_BLUE_DAC_LEN);
        public const Int32 LED_DRIVE_LEN = (LED_DRIVE_RED_DAC_LEN + LED_DRIVE_BLUE_DAC_LEN + LED_DRIVE_DEPTH_LEN);
        /*
        public const Int32 EXCITE_RED_CYCLE_LEN = 2;
        public const Int32 EXCITE_RED_BRIGHTNESS_LEN = 2;
        public const Int32 EXCITE_BLUE_CYCLE_LEN = 2;
        public const Int32 EXCITE_BLUE_BRIGHTNESS_LEN = 2;
        public const Int32 EXCITE_LEN = (EXCITE_RED_CYCLE_LEN + EXCITE_RED_BRIGHTNESS_LEN + EXCITE_BLUE_CYCLE_LEN + EXCITE_BLUE_BRIGHTNESS_LEN);
        public const Int32 EXCITE_RED_CYCLE_POS = 0;
        public const Int32 EXCITE_RED_BRIGHTNESS_POS = (EXCITE_RED_CYCLE_POS + EXCITE_RED_CYCLE_LEN);
        public const Int32 EXCITE_BLUE_CYCLE_POS = (EXCITE_RED_BRIGHTNESS_POS + EXCITE_RED_BRIGHTNESS_LEN);
        public const Int32 EXCITE_BLUE_BRIGHTNESS_POS = (EXCITE_BLUE_CYCLE_POS + EXCITE_BLUE_CYCLE_LEN);
         * /

        /*
        *  获取用户校准参数
        */
        public const Int32 DETECT_DATA_LED = 0;
        public const Int32 DETECT_DATA_PRESSURE = DATA_BYTE;
        public const Int32 DETECT_DATA_LEN = (DETECT_DATA_PRESSURE + DATA_FLOAT);
        public const byte DETECT_LED_RED = 0x01;
        public const byte DETECT_LED_BLUE = 0x02;
        public const byte DETECT_LED_RED_BLUE = 0x03;

        /*
        *  获取溶氧值
        */
        public const byte DO_DATA_UNCOMPENSATED = 0x01;
        public const byte DO_DATA_TEMPERATURE = 0x02;
        public const byte DO_DATA_PRESSURE = 0x03;
        public const byte DO_DATA_CALCULATED = 0x04;

        public const byte PROBE_NUM = 8;

        /*
         *  获取调试信息
         */
        public const int DEBUG_INFO_BLOCK_SIZE = 16;

        /*
         * 解析出现的错误
         * 并非目标设备返回的错误
         */
        public const byte RESULT_OK = 0x00;
        public const byte RESULT_CHKSUM_ERR = 0x01;
        public const byte RESULT_HEAD_ERR = 0x02;
        public const byte RESULT_OP_ERR = 0x03;
        public const byte RESULT_OB_ERR = 0x04;
        public const byte RESULT_ID_ERR = 0x05;
        public const byte RESULT_SN_ERR = 0x06;
        public const byte RESULT_RESERVE_ERR = 0x07;
        public const byte RESULT_SIZE_ERR = 0x08;
        public const byte RESULT_UNFINISHED_PACKAGE = 0x09;
        public const byte RESULT_COMMAND_ERR = 0x0A;
        public const byte RESULT_ID_SN_ERR = 0x0B;

        /*
        * 默认id与sn
        * 保留位的值
        */
        public const byte DEFAULT_ID = 0xFF;
        public const byte DEFAULT_RESERVE = 0xFF;
        public static char[] DEFAULT_SN = { 'Y', 'L', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F' };

        public const Int32 EXPECT_LEN_GET_STATUS_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_GET_STATUS_RSP = (PURE_FRAME_LEN + RESULT_LEN + STATUS_LEN);
        public const Int32 EXPECT_LEN_GET_ID_SN_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_GET_ID_SN_RSP = (PURE_FRAME_LEN + RESULT_LEN + ID_LEN + SN_LEN);
        public const Int32 EXPECT_LEN_SET_STATUS_REQ = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_SET_STATUS_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_SET_ERASE_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_SET_ERASE_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_SET_WRITE_REQ = COMMAND_MAX_SIZE;
        public const Int32 EXPECT_LEN_SET_WRITE_RSP = (PURE_FRAME_LEN + RESULT_LEN + WRITE_FLASH_ADDRESS_LEN);
        public const Int32 EXPECT_LEN_SET_FINISH_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_SET_FINISH_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_GET_SAMPLE_REQ = (PURE_FRAME_LEN + DATA_BYTE);
        public const Int32 EXPECT_LEN_GET_SAMPLE_RSP = (PURE_FRAME_LEN + RESULT_LEN + SAMPLE_DATA_LEN);
        //public const Int32 EXPECT_LEN_GET_PHASE_REQ = (PURE_FRAME_LEN + DATA_BYTE);
        //public const Int32 EXPECT_LEN_GET_PHASE_RSP = (PURE_FRAME_LEN + RESULT_LEN + DATA_FLOAT);
        public const Int32 EXPECT_LEN_GET_TEMPERATURE_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_GET_TEMPERATURE_RSP = (PURE_FRAME_LEN + RESULT_LEN + DATA_FLOAT);
        public const Int32 EXPECT_LEN_GET_TANGENT_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_GET_TANGENT_RSP = (PURE_FRAME_LEN + RESULT_LEN + DATA_FLOAT);
        public const Int32 EXPECT_LEN_GET_REAL_IMAG_REQ = (PURE_FRAME_LEN + DATA_BYTE);
        public const Int32 EXPECT_LEN_GET_REAL_IMAG_RSP = (PURE_FRAME_LEN + RESULT_LEN + REAL_IMAG_DATA_LEN);
        public const Int32 EXPECT_LEN_GET_DO_REQ = (PURE_FRAME_LEN + DATA_BYTE);
        public const Int32 EXPECT_LEN_GET_DO_RSP = (PURE_FRAME_LEN + RESULT_LEN + DATA_FLOAT);
        public const Int32 EXPECT_LEN_GET_CALI_CO_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_GET_CALI_CO_RSP = (PURE_FRAME_LEN + RESULT_LEN + CALI_CO_DATA_LEN);
        public const Int32 EXPECT_LEN_GET_TEMP_CO_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_GET_TEMP_CO_RSP = (PURE_FRAME_LEN + RESULT_LEN + TEMP_CO_DATA_LEN);
        public const Int32 EXPECT_LEN_GET_USR_CO_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_GET_USR_CO_RSP = (PURE_FRAME_LEN + RESULT_LEN + USR_CO_DATA_LEN);
        public const Int32 EXPECT_LEN_SET_DETECT_REQ = (PURE_FRAME_LEN + DETECT_DATA_LEN);
        public const Int32 EXPECT_LEN_SET_DETECT_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_GET_T0_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_GET_T0_RSP = (PURE_FRAME_LEN + RESULT_LEN + DATA_FLOAT);
        public const Int32 EXPECT_LEN_SET_T0_REQ = (PURE_FRAME_LEN + DATA_FLOAT);
        public const Int32 EXPECT_LEN_SET_T0_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_SET_CALI_CO_REQ = (PURE_FRAME_LEN + CALI_CO_DATA_LEN);
        public const Int32 EXPECT_LEN_SET_CALI_CO_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_SET_TEMP_CO_REQ = (PURE_FRAME_LEN + TEMP_CO_DATA_LEN);
        public const Int32 EXPECT_LEN_SET_TEMP_CO_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_SET_USR_CO_REQ = (PURE_FRAME_LEN + USR_CO_DATA_LEN);
        public const Int32 EXPECT_LEN_SET_USR_CO_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_GET_LED_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_GET_LED_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_SET_LED_REQ = (PURE_FRAME_LEN + LED_DRIVE_LEN);//EXCITE_LEN);
        public const Int32 EXPECT_LEN_SET_LED_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_SET_ID_SN_REQ = (PURE_FRAME_LEN + SN_LEN);
        public const Int32 EXPECT_LEN_SET_ID_SN_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_GET_HW_SW_REQ = (PURE_FRAME_LEN);
        public const Int32 EXPECT_LEN_GET_HW_SW_RSP = (PURE_FRAME_LEN + RESULT_LEN + DATA_INT + DATA_INT);
        public const Int32 EXPECT_LEN_GET_PROBE_NUM_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_GET_PROBE_NUM_RSP = (PURE_FRAME_LEN + RESULT_LEN + DATA_BYTE);
        public const Int32 EXPECT_LEN_GET_PROBE_SN_REQ = (PURE_FRAME_LEN + DATA_BYTE);
        public const Int32 EXPECT_LEN_GET_PROBE_SN_RSP = (PURE_FRAME_LEN + RESULT_LEN + DATA_BYTE + SN_LEN);
        public const Int32 EXPECT_LEN_GET_PRESSURE_REQ = PURE_FRAME_LEN;
        public const Int32 EXPECT_LEN_GET_PRESSURE_RSP = (PURE_FRAME_LEN + RESULT_LEN + DATA_FLOAT);
        public const Int32 EXPECT_LEN_GET_DEBUG_INFO_REQ = (PURE_FRAME_LEN + DATA_BYTE);
        public const Int32 EXPECT_LEN_GET_DEBUG_INFO_RSP = (PURE_FRAME_LEN + RESULT_LEN + DEBUG_INFO_BLOCK_SIZE);
        public const Int32 EXPECT_LEN_SET_DEBUG_INFO_REQ = (PURE_FRAME_LEN + DATA_BYTE + DEBUG_INFO_BLOCK_SIZE);
        public const Int32 EXPECT_LEN_SET_DEBUG_INFO_RSP = (PURE_FRAME_LEN + RESULT_LEN);
        public const Int32 EXPECT_LEN_SET_SALINITY_REQ = (PURE_FRAME_LEN + DATA_FLOAT);
        public const Int32 EXPECT_LEN_SET_SALINITY_RSP = (PURE_FRAME_LEN + RESULT_LEN);

        public static Int32 combineByteToInt(byte[] data, Int32 pos)
        {
            Int32 result = data[pos + 1];
            result = (result << 8) | data[pos];
            return result;
        }

        public static void splitIntToByte(Int32 data, byte[] byteData, Int32 pos)
        {
            byteData[pos + 1] = (byte)((data >> 8) & 0xFF);
            byteData[pos] = (byte)(data & 0xFF);
        }

        public static void splitFloatToBytes(float f_value, byte[] b_array, int start_p)
        {
            byte[] tmp = BitConverter.GetBytes(f_value);
            Buffer.BlockCopy(tmp, 0, b_array, start_p, DATA_FLOAT);
        }

        public static float combineBytesToFloat(byte[] b_array, int start_p)
        {

            return BitConverter.ToSingle(b_array, start_p);
        }

        public static Int32 getPackageLen(byte[] data, int data_len)
        {
            if (LEN_LEN + HEAD_LEN > data_len)
                return 65535;
            int base_len = combineByteToInt(data, POS_LEN);
            return (base_len + LEN_LEN + HEAD_LEN);
        }

        public static bool IsRSPHead(byte head)
        {
            return (HEAD_RSP == head);
        }

        public static bool IsREQHead(byte head)
        {
            return (HEAD_REQ == head);
        }

        public static bool IsHead(byte[] head)
        {
            return ((HEAD_REQ == head[POS_HEAD]) || (HEAD_RSP == head[POS_HEAD]));
        }

        public static bool IsHasLength(byte[] data, Int32 len)
        {
            return (3 < len);
        }

        public static Int32 getLength(byte[] data, Int32 len)
        {
            if (3 < len)
            {
                Int32 result = combineByteToInt(data, POS_LEN);
                return result;
            }
            else
            {
                return 0;
            }
        }
        public static bool isConnectedProbe(byte state, int index)
        {
            byte mask = (byte)(0x01 << index);
            return (0x00 != (state & mask));
        }

        public static byte CalcChksum(byte[] command, Int32 offset, Int32 len)
        {
            Int32 i = 0;
            byte chksum = 0;
            for (; i < len; i++)
            {
                chksum += command[offset + i];
            }
            //chksum = (byte)~chksum;
            //chksum += 1;
            return chksum;
        }

        public static byte CharToHex(byte char_data)
        {
            byte result;
            if (65 < char_data)
            {
                result = (byte)(char_data - 55);
            }
            else if (97 < char_data)
            {
                result = (byte)(char_data - 87);
            }
            else
            {
                result = (byte)(char_data - 48);
            }
            return result;
        }

        public static byte GetIdInSN(byte[] sn)
        {
            byte high_id = CharToHex(sn[2]);
            byte low_id = CharToHex(sn[3]);
            byte id_in_sn = (byte)((high_id << 4) | low_id);
            return id_in_sn;
        }

        public static Int32 PackageCommandUp(byte[] command, Int32 in_data_len, byte[] in_data, byte in_result, byte[] out_data, byte id, byte[] sn)
        {
            Int32 cmd_len = POS_DATA_S - POS_ID; //default len is the sum of len of id, sn, op, ob, reserve = 19
            Int32 data_start = POS_DATA_S;

            out_data[POS_HEAD] = command[HEAD_TYPE];
            out_data[POS_OP] = command[OP_TYPE];
            out_data[POS_OB] = command[OB_TYPE];

            out_data[POS_ID] = id;
            for (Int32 i = 0; i < SN_LEN; i++)
            {
                out_data[POS_SN + i] = sn[i];
            }

            out_data[POS_CS] = DEFAULT_RESERVE;
            out_data[POS_PS] = DEFAULT_RESERVE;

            for (Int32 i = 0; i < RESERVE_LEN; i++)
            {
                out_data[POS_RESERVE + i] = DEFAULT_RESERVE;
            }

            cmd_len += in_data_len;

            if (HEAD_RSP == command[HEAD_TYPE])
            {
                out_data[data_start] = in_result;
                data_start += RESULT_LEN;
                cmd_len += RESULT_LEN;
            }

            cmd_len += CHKSUM_LEN; //check sum count in len

            splitIntToByte(cmd_len, out_data, POS_LEN);

            if (0 < in_data_len)
            {
                for (Int32 i = 0; i < in_data_len; i++)
                {
                    out_data[data_start + i] = in_data[i];
                }
                //cmd_len += in_data_len;
            }
            out_data[data_start + in_data_len] = CalcChksum(out_data, POS_ID, cmd_len - CHKSUM_LEN);
            return HEAD_LEN + LEN_LEN + cmd_len;
        }

        public static Int32 PackageCommandDown(Int32 in_data_len, byte[] in_data, byte[] command, byte[] out_data, byte[] out_status, byte id, byte[] sn)
        {
            Int32 cmd_len;
            byte chksum;
            Int32 data_start = POS_DATA_S;
            Int32 result = RESULT_OK;

            command[HEAD_TYPE] = in_data[POS_HEAD];
            command[OP_TYPE] = in_data[POS_OP];
            command[OB_TYPE] = in_data[POS_OB];

            if ((HEAD_REQ != command[HEAD_TYPE]) && (HEAD_RSP != command[HEAD_TYPE]))
                return RESULT_HEAD_ERR;

            if ((OP_GET != command[OP_TYPE]) && (OP_SET != command[OP_TYPE]))
                return RESULT_OP_ERR;

            if ((OB_STATUS != command[OP_TYPE]) && (OB_ERASE != command[OP_TYPE]) && (OB_WRITE != command[OP_TYPE]) && (OB_FINISH != command[OP_TYPE]))
                return RESULT_OB_ERR;

            if (id != in_data[POS_ID])
            {
                if ((OP_GET != command[OP_TYPE]) && (OB_SALINITY != command[OB_TYPE]))
                {
                    return RESULT_ID_ERR;
                }
            }
                

            out_status[STATUS_C] = in_data[POS_CS];
            out_status[STATUS_P] = in_data[POS_PS];

            if ((OP_GET != command[OP_TYPE]) && (OB_SALINITY != command[OB_TYPE]))
            {
                for (Int32 i = 0; i < SN_LEN; i++)
                {
                    if (in_data[POS_SN + i] != sn[i])
                        return RESULT_SN_ERR;
                }
            }
            

            cmd_len = combineByteToInt(in_data, POS_LEN);
            if ((cmd_len + HEAD_LEN + LEN_LEN) > in_data_len)
            {
                return RESULT_UNFINISHED_PACKAGE;
            }

            int out_data_len = cmd_len - (POS_DATA_S - POS_ID + CHKSUM_LEN);

            if (ProtocolStruct.COMMAND_MAX_SIZE < cmd_len + HEAD_LEN + LEN_LEN)
            {
                return RESULT_SIZE_ERR;
            }

            if (HEAD_RSP == command[HEAD_TYPE])
            {
                result = in_data[data_start];
                if (RESULT_OK != result)
                {
                    return result;
                }
                data_start += RESULT_LEN;
                out_data_len -= RESULT_LEN;
            }

            for (Int32 i = 0; i < out_data_len; i++)
            {
                out_data[i] = in_data[data_start + i];
            }

            chksum = CalcChksum(in_data, POS_ID, cmd_len - CHKSUM_LEN);

            if (chksum != in_data[data_start + out_data_len])
            {
                result = RESULT_CHKSUM_ERR;
            }
            return result;
        }

        public static bool IsResponseFor(byte[] expect_command, byte[] command)
        {
            return (HEAD_RSP == command[HEAD_TYPE])
                && (expect_command[OP_TYPE] == command[OP_TYPE])
                && (expect_command[OB_TYPE] == command[OB_TYPE]);
        }

        public static bool IsRequestFor(byte[] expect_command, byte[] command)
        {
            return (HEAD_REQ == command[HEAD_TYPE])
                && (expect_command[OP_TYPE] == command[OP_TYPE])
                && (expect_command[OB_TYPE] == command[OB_TYPE]);
        }

        public static void FindResponseFor(byte[] req_command, byte[] rsp_command)
        {
            rsp_command[HEAD_TYPE] = HEAD_RSP;
            rsp_command[OP_TYPE] = req_command[OP_TYPE];
            rsp_command[OB_TYPE] = req_command[OB_TYPE];
        }

        /*
        * 主端使用的帧封装 
        */

        public static Int32 PackageGetStatus(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {

            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_STATUS;
            splitIntToByte(EXPECT_LEN_GET_STATUS_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageSetStatus(byte[] out_data, byte status, byte[] out_command, byte id, byte[] sn)
        {

            byte[] data = new byte[] { status };
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_STATUS;
            splitIntToByte(EXPECT_LEN_SET_STATUS_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, STATUS_LEN, data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageSetErase(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {

            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_ERASE;
            splitIntToByte(EXPECT_LEN_SET_ERASE_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageSetWrite(byte[] out_data, Int32 address, Int32 data_len, byte[] data, byte[] out_command, byte id, byte[] sn)
        {
            Int32 write_data_len = data_len + WRITE_FLASH_ADDRESS_LEN;
            byte[] write_data = new byte[write_data_len];
            splitIntToByte(address, write_data, WRITE_FLASH_ADDRESS_POS);
            for (int i = 0; i < data_len; i++)
            {
                write_data[WRITE_FLASH_DATA_POS + i] = data[i];
            }
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_WRITE;
            splitIntToByte(EXPECT_LEN_SET_WRITE_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, write_data_len, write_data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageSetFinish(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {

            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_FINISH;
            splitIntToByte(EXPECT_LEN_SET_FINISH_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetIdSN(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_ID_SN;
            splitIntToByte(EXPECT_LEN_GET_ID_SN_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetSampleEverage(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte led)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_SAMPLE;
            splitIntToByte(EXPECT_LEN_GET_SAMPLE_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_BYTE];
            data[DATA_BYTE_FIRST] = led;

            return PackageCommandUp(out_command, DATA_BYTE, data, RESULT_OK, out_data, id, sn);
        }

        /*
        public static Int32 PackageGetPhase(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte led)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_PHASE;
            splitIntToByte(EXPECT_LEN_GET_PHASE_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_BYTE];
            data[DATA_BYTE_FIRST] = led;

            return PackageCommandUp(out_command, DATA_BYTE, data, RESULT_OK, out_data, id, sn);
        }
         */

        public static Int32 PackageGetTemperature(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_TEMPERATURE;
            splitIntToByte(EXPECT_LEN_GET_TEMPERATURE_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetTangent(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_TANGENT;
            splitIntToByte(EXPECT_LEN_GET_TANGENT_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetRealImag(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte select)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_REAL_IMAG;
            splitIntToByte(EXPECT_LEN_GET_REAL_IMAG_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_BYTE];
            data[DATA_BYTE_FIRST] = select;

            return PackageCommandUp(out_command, DATA_BYTE, data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetDO(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte select)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_DO;
            splitIntToByte(EXPECT_LEN_GET_DO_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_BYTE];
            data[DATA_BYTE_FIRST] = select;

            return PackageCommandUp(out_command, DATA_BYTE, data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetCaliCo(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_CALI_CO;
            splitIntToByte(EXPECT_LEN_GET_CALI_CO_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetTempCo(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_TEMP_CO;
            splitIntToByte(EXPECT_LEN_GET_TEMP_CO_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetUserCo(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_USR_CO;
            splitIntToByte(EXPECT_LEN_GET_USR_CO_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageSetCaliCo(byte[] out_data, byte[] out_command, byte id, byte[] sn, float l0, float l1, float l2, float l3, float l4)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_CALI_CO;
            splitIntToByte(EXPECT_LEN_SET_CALI_CO_REQ, out_command, LEN_TYPE_H);

            byte[] data = new byte[CALI_CO_DATA_LEN];
            splitFloatToBytes(l0, data, CALI_CO_DATA_L0);
            splitFloatToBytes(l1, data, CALI_CO_DATA_L1);
            splitFloatToBytes(l2, data, CALI_CO_DATA_L2);
            splitFloatToBytes(l3, data, CALI_CO_DATA_L3);
            splitFloatToBytes(l4, data, CALI_CO_DATA_L4);

            return PackageCommandUp(out_command, CALI_CO_DATA_LEN, data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageSetTempCo(byte[] out_data, byte[] out_command, byte id, byte[] sn, float l0, float l1, float l2)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_TEMP_CO;
            splitIntToByte(EXPECT_LEN_SET_TEMP_CO_REQ, out_command, LEN_TYPE_H);

            byte[] data = new byte[TEMP_CO_DATA_LEN];
            splitFloatToBytes(l0, data, TEMP_CO_DATA_L0);
            splitFloatToBytes(l1, data, TEMP_CO_DATA_L1);
            splitFloatToBytes(l2, data, TEMP_CO_DATA_L2);

            return PackageCommandUp(out_command, TEMP_CO_DATA_LEN, data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageSetUserCo(byte[] out_data, byte[] out_command, byte id, byte[] sn, float l0, float l1)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_USR_CO;
            splitIntToByte(EXPECT_LEN_SET_USR_CO_REQ, out_command, LEN_TYPE_H);

            byte[] data = new byte[USR_CO_DATA_LEN];
            splitFloatToBytes(l0, data, USR_CO_DATA_L0);
            splitFloatToBytes(l1, data, USR_CO_DATA_L1);

            return PackageCommandUp(out_command, USR_CO_DATA_LEN, data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetT0(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_T0;
            splitIntToByte(EXPECT_LEN_GET_T0_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetLedDrive(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_LED_DRIVE;
            splitIntToByte(EXPECT_LEN_GET_LED_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageSetT0(byte[] out_data, byte[] out_command, byte id, byte[] sn, float t0)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_T0;
            splitIntToByte(EXPECT_LEN_GET_T0_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_FLOAT];
            splitFloatToBytes(t0, data, DATA_FLOAT_FIRST);

            return PackageCommandUp(out_command, DATA_FLOAT, data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageSetLedDrive(byte[] out_data, byte[] out_command, byte id, byte[] sn, int red_dac, int blue_dac, int depth)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_LED_DRIVE;
            splitIntToByte(EXPECT_LEN_GET_LED_RSP, out_command, LEN_TYPE_H);

            /*
            byte[] data = new byte[EXCITE_LEN];
            splitIntToByte(red_cycle, data, EXCITE_RED_CYCLE_POS);
            splitIntToByte(red_dac, data, EXCITE_RED_BRIGHTNESS_POS);
            splitIntToByte(blue_cycle, data, EXCITE_BLUE_CYCLE_POS);
            splitIntToByte(blue_dac, data, EXCITE_BLUE_BRIGHTNESS_POS);

            return PackageCommandUp(out_command, EXCITE_LEN, data, RESULT_OK, out_data, id, sn);
             */
            byte[] data = new byte[LED_DRIVE_LEN];
            splitIntToByte(red_dac, data, LED_DRIVE_RED_DAC_POS);
            splitIntToByte(blue_dac, data, LED_DRIVE_BLUE_DAC_POS);
            splitIntToByte(depth, data, LED_DRIVE_DEPTH_POS);

            return PackageCommandUp(out_command, LED_DRIVE_LEN, data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageSetLedActive(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte select, float pressure)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_DETECT;
            splitIntToByte(EXPECT_LEN_SET_DETECT_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DETECT_DATA_LEN];
            data[DETECT_DATA_LED] = select;
            splitFloatToBytes(pressure, data, DETECT_DATA_PRESSURE);

            return PackageCommandUp(out_command, DETECT_DATA_LEN, data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageSetIdSN(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte[] new_sn)
        {
            byte[] data = new byte[SN_LEN];

            for (Int32 i = 0; i < SN_LEN; i++)
            {
                data[i] = new_sn[i];
            }

            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_ID_SN;
            splitIntToByte(EXPECT_LEN_SET_ID_SN_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, SN_LEN, data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetHWSW(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_HW_SW;
            splitIntToByte(EXPECT_LEN_GET_HW_SW_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static int PackageGetProbeNum(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_PROBE_NUM;
            splitIntToByte(EXPECT_LEN_GET_PROBE_NUM_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static int PackageGetProbeSN(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte probe_index)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_PROBE_SN;
            splitIntToByte(EXPECT_LEN_GET_PROBE_SN_RSP, out_command, LEN_TYPE_H);
            byte[] data = new byte[DATA_BYTE];
            data[DATA_BYTE_FIRST] = probe_index;

            return PackageCommandUp(out_command, DATA_BYTE, data, RESULT_OK, out_data, id, sn);
        }

        public static Int32 PackageGetAllSample(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = 0x13;

            return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }
        
        public static int PackageGetPressure(byte[] out_data, byte[] out_command, byte id, byte[] sn)
        {
        	out_command[HEAD_TYPE] = HEAD_REQ;
        	out_command[OP_TYPE] = OP_GET;
        	out_command[OB_TYPE] = OB_PRESSURE;
        	splitIntToByte(EXPECT_LEN_GET_PRESSURE_RSP, out_command, LEN_TYPE_H);
        	
        	return PackageCommandUp(out_command, 0, null, RESULT_OK, out_data, id, sn);
        }

        public static int PackageGetDebugInfo(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte part)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_DEBUG_INFO;
            splitIntToByte(EXPECT_LEN_GET_DEBUG_INFO_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_BYTE];
            data[DATA_BYTE_FIRST] = part;

            return PackageCommandUp(out_command, DATA_BYTE, data, RESULT_OK, out_data, id, sn);
        }

        public static int PackageSetDebugInfo(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte part, byte[] block)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_DEBUG_INFO;
            splitIntToByte(EXPECT_LEN_SET_DEBUG_INFO_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_BYTE + DEBUG_INFO_BLOCK_SIZE];
            data[DATA_BYTE_FIRST] = part;
            for (int i = 0; i < DEBUG_INFO_BLOCK_SIZE; i++)
            {
                data[1 + i] = block[i];
            }

            return PackageCommandUp(out_command, DATA_BYTE + DEBUG_INFO_BLOCK_SIZE, data, RESULT_OK, out_data, id, sn);
        }

        public static int PackageSetSalinity(byte[] out_data, byte[] out_command, byte id, byte[] sn, float salinity)
        {
            out_command[HEAD_TYPE] = HEAD_REQ;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_SALINITY;
            splitIntToByte(EXPECT_LEN_SET_SALINITY_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_FLOAT];
            splitFloatToBytes(salinity, data, 0);

            return PackageCommandUp(out_command, DATA_FLOAT, data, RESULT_OK, out_data, id, sn);
        }
        /*
         * 从端响应使用的帧封装 
         */

        public static Int32 PackageGetStatusResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, byte status)
        {
            byte[] data = new byte[] { status };
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_STATUS;
            splitIntToByte(EXPECT_LEN_GET_STATUS_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 1, data, result, out_data, id, sn);
        }

        public static Int32 PackageSetStatusResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {


            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_STATUS;
            splitIntToByte(EXPECT_LEN_SET_STATUS_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static Int32 PackageSetEraseResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {

            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_ERASE;
            splitIntToByte(EXPECT_LEN_SET_ERASE_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static Int32 PackageSetWriteResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, Int32 address)
        {

            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_WRITE;
            splitIntToByte(EXPECT_LEN_SET_WRITE_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static Int32 PackageSetFinishResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {

            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_FINISH;
            splitIntToByte(EXPECT_LEN_SET_FINISH_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static Int32 PackageGetIdSNResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, byte new_id, byte[] new_sn)
        {
            byte[] data = new byte[ID_SN_DATA_SN_POS + SN_LEN];

            data[ID_SN_DATA_ID_POS] = new_id;
            for (Int32 i = 0; i < SN_LEN; i++)
            {
                data[ID_SN_DATA_SN_POS + i] = new_sn[i];
            }

            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_ID_SN;
            splitIntToByte(EXPECT_LEN_GET_ID_SN_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, ID_SN_DATA_SN_POS + SN_LEN, data, result, out_data, id, sn);
        }

        public static Int32 PackageGetSampleEverageResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, float first, float second, float third, float forth)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_SAMPLE;
            splitIntToByte(EXPECT_LEN_GET_SAMPLE_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[SAMPLE_DATA_LEN];
            splitFloatToBytes(first, data, SAMPLE_DATA_FIRST);
            splitFloatToBytes(second, data, SAMPLE_DATA_SECOND);
            splitFloatToBytes(third, data, SAMPLE_DATA_THIRD);
            splitFloatToBytes(forth, data, SAMPLE_DATA_FORTH);

            return PackageCommandUp(out_command, SAMPLE_DATA_LEN, data, result, out_data, id, sn);
        }
        /*
        public static Int32 PackageGetPhaseResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, float value)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_PHASE;
            splitIntToByte(EXPECT_LEN_GET_PHASE_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_FLOAT];
            splitFloatToBytes(value, data, DATA_FLOAT_FIRST);

            return PackageCommandUp(out_command, DATA_FLOAT, data, result, out_data, id, sn);
        }
         */

        public static Int32 PackageGetTemperatureResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, float value)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_TEMPERATURE;
            splitIntToByte(EXPECT_LEN_GET_TEMPERATURE_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_FLOAT];
            splitFloatToBytes(value, data, DATA_FLOAT_FIRST);

            return PackageCommandUp(out_command, DATA_FLOAT, data, result, out_data, id, sn);
        }

        public static Int32 PackageGetTangentResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, float value)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_TANGENT;
            splitIntToByte(EXPECT_LEN_GET_TANGENT_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_FLOAT];
            splitFloatToBytes(value, data, DATA_FLOAT_FIRST);

            return PackageCommandUp(out_command, DATA_FLOAT, data, result, out_data, id, sn);
        }

        public static Int32 PackageGetRealImagResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, float real, float imag)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_REAL_IMAG;
            splitIntToByte(EXPECT_LEN_GET_REAL_IMAG_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[REAL_IMAG_DATA_LEN];
            splitFloatToBytes(real, data, REAL_IMAGE_DATA_REAL);
            splitFloatToBytes(imag, data, REAL_IMAGE_DATA_IMAG);

            return PackageCommandUp(out_command, REAL_IMAG_DATA_LEN, data, result, out_data, id, sn);
        }

        public static Int32 PackageGetDOResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, float value)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_DO;
            splitIntToByte(EXPECT_LEN_GET_DO_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_FLOAT];
            splitFloatToBytes(value, data, DATA_FLOAT_FIRST);

            return PackageCommandUp(out_command, DATA_FLOAT, data, result, out_data, id, sn);
        }

        public static Int32 PackageGetCaliCoResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, float l0, float l1, float l2, float l3, float l4)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_CALI_CO;
            splitIntToByte(EXPECT_LEN_GET_CALI_CO_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[CALI_CO_DATA_LEN];
            splitFloatToBytes(l0, data, CALI_CO_DATA_L0);
            splitFloatToBytes(l1, data, CALI_CO_DATA_L1);
            splitFloatToBytes(l2, data, CALI_CO_DATA_L2);
            splitFloatToBytes(l3, data, CALI_CO_DATA_L3);
            splitFloatToBytes(l4, data, CALI_CO_DATA_L4);

            return PackageCommandUp(out_command, CALI_CO_DATA_LEN, data, result, out_data, id, sn);
        }

        public static Int32 PackageGetTempCoResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, float l0, float l1, float l2)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_TEMP_CO;
            splitIntToByte(EXPECT_LEN_GET_TEMP_CO_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[TEMP_CO_DATA_LEN];
            splitFloatToBytes(l0, data, TEMP_CO_DATA_L0);
            splitFloatToBytes(l1, data, TEMP_CO_DATA_L1);
            splitFloatToBytes(l2, data, TEMP_CO_DATA_L2);

            return PackageCommandUp(out_command, TEMP_CO_DATA_LEN, data, result, out_data, id, sn);
        }

        public static Int32 PackageGetUserCoResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, float l0, float l1)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_USR_CO;
            splitIntToByte(EXPECT_LEN_GET_USR_CO_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[USR_CO_DATA_LEN];
            splitFloatToBytes(l0, data, USR_CO_DATA_L0);
            splitFloatToBytes(l1, data, USR_CO_DATA_L1);

            return PackageCommandUp(out_command, USR_CO_DATA_LEN, data, result, out_data, id, sn);
        }

        public static Int32 PackageSetCaliCoResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {


            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_CALI_CO;
            splitIntToByte(EXPECT_LEN_SET_CALI_CO_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static Int32 PackageSetTempCoResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {


            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_TEMP_CO;
            splitIntToByte(EXPECT_LEN_SET_TEMP_CO_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static Int32 PackageSetUserCoResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {


            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_USR_CO;
            splitIntToByte(EXPECT_LEN_SET_USR_CO_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static Int32 PackageSetT0Response(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_T0;
            splitIntToByte(EXPECT_LEN_GET_T0_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static Int32 PackageSetLedDriveResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_LED_DRIVE;
            splitIntToByte(EXPECT_LEN_SET_LED_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static Int32 PackageGetT0Response(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, float t0)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_T0;
            splitIntToByte(EXPECT_LEN_GET_T0_RSP, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_FLOAT];
            splitFloatToBytes(t0, data, DATA_FLOAT_FIRST);

            return PackageCommandUp(out_command, DATA_FLOAT, data, result, out_data, id, sn);
        }

        public static Int32 PackageGetLedDriveResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, int red_dac, int blue_dac, int depth)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_LED_DRIVE;
            splitIntToByte(EXPECT_LEN_GET_LED_RSP, out_command, LEN_TYPE_H);

            /*
            byte[] data = new byte[EXCITE_LEN];
            splitIntToByte(red_cycle, data, EXCITE_RED_CYCLE_POS);
            splitIntToByte(red_dac, data, EXCITE_RED_BRIGHTNESS_POS);
            splitIntToByte(blue_cycle, data, EXCITE_BLUE_CYCLE_POS);
            splitIntToByte(blue_dac, data, EXCITE_BLUE_BRIGHTNESS_POS);

            return PackageCommandUp(out_command, EXCITE_LEN, data, result, out_data, id, sn);
             */
            byte[] data = new byte[LED_DRIVE_LEN];
            splitIntToByte(red_dac, data, LED_DRIVE_RED_DAC_POS);
            splitIntToByte(blue_dac, data, LED_DRIVE_BLUE_DAC_POS);
            splitIntToByte(depth, data, LED_DRIVE_DEPTH_POS);

            return PackageCommandUp(out_command, LED_DRIVE_LEN, data, result, out_data, id, sn);
        }

        public static Int32 PackageSetLedActiveResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_DETECT;
            splitIntToByte(EXPECT_LEN_SET_DETECT_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static Int32 PackageSetIdSNResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_ID_SN;
            splitIntToByte(EXPECT_LEN_SET_ID_SN_RSP, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static Int32 PackageGetHWSWResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, byte hw_maj, byte hw_min, byte sw_maj, byte sw_min)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_HW_SW;
            splitIntToByte(EXPECT_LEN_SET_DETECT_RSP, out_command, LEN_TYPE_H);
            byte[] data = { hw_maj, hw_min, sw_maj, sw_min };



            return PackageCommandUp(out_command, DATA_BYTE * 4, data, result, out_data, id, sn);
        }

        public static int PackageGetProbeNumResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, byte probe_state)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_PROBE_NUM;
            splitIntToByte(EXPECT_LEN_GET_PROBE_NUM_REQ, out_command, LEN_TYPE_H);
            byte[] data = { probe_state };

            return PackageCommandUp(out_command, DATA_BYTE, data, result, out_data, id, sn);
        }

        public static int PackageGetProbeSNResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, byte probe_index, byte[] probe_sn)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_PROBE_SN;
            splitIntToByte(EXPECT_LEN_GET_PROBE_SN_REQ, out_command, LEN_TYPE_H);
            byte[] data = new byte[DATA_BYTE + SN_LEN];
            data[DATA_BYTE_FIRST] = probe_index;
            for (int i = 0; i < SN_LEN; i++)
                data[DATA_BYTE_FIRST + DATA_BYTE + i] = probe_sn[i];

            return PackageCommandUp(out_command, DATA_BYTE + SN_LEN, data, result, out_data, id, sn);
        }
    
        public static int PackageGetPressureResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, float pressure)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_PRESSURE;
            splitIntToByte(EXPECT_LEN_GET_PRESSURE_REQ, out_command, LEN_TYPE_H);

            byte[] data = new byte[DATA_FLOAT];
            splitFloatToBytes(pressure, data, DATA_FLOAT_FIRST);

            return PackageCommandUp(out_command, DATA_FLOAT, data, result, out_data, id, sn);
        }

        public static int PackageGetCriticalResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result, byte[] info)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_GET;
            out_command[OB_TYPE] = OB_DEBUG_INFO;
            splitIntToByte(EXPECT_LEN_GET_DEBUG_INFO_REQ, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, DEBUG_INFO_BLOCK_SIZE, info, result, out_data, id, sn);
        }

        public static int PackageGetCriticalResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_DEBUG_INFO;
            splitIntToByte(EXPECT_LEN_SET_DEBUG_INFO_REQ, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }

        public static int PackageSetSalinityResponse(byte[] out_data, byte[] out_command, byte id, byte[] sn, byte result)
        {
            out_command[HEAD_TYPE] = HEAD_RSP;
            out_command[OP_TYPE] = OP_SET;
            out_command[OB_TYPE] = OB_SALINITY;
            splitIntToByte(EXPECT_LEN_SET_SALINITY_REQ, out_command, LEN_TYPE_H);

            return PackageCommandUp(out_command, 0, null, result, out_data, id, sn);
        }
        //请求帧数据解析

        //响应帧数据解析
        public static byte GetDataStatus(byte[] data)
        {
            return data[0];
        }

        public static byte GetDataIdSN(byte[] data, byte[] sn)
        {
            for (int i = 0; i < SN_LEN; i++)
            {
                sn[i] = data[ID_SN_DATA_SN_POS + i];
            }
            return data[ID_SN_DATA_ID_POS];
        }

        public static byte GetDataSampleLed(byte[] data)
        {
            return data[DATA_BYTE_FIRST];
        }
        /*
        public static byte GetDataSampleNumber(byte[] data)
        {
            return data[SAMPLE_DATA_NUM];
        }
        
        public static float GetDataSampleValue(byte[] data)
        {
            return combineBytesToFloat(data, SAMPLE_DATA_VALUE);
        }
        */
        public static float GetDataSampleFirst(byte[] data)
        {
            return combineBytesToFloat(data, SAMPLE_DATA_FIRST);
        }

        public static float GetDataSampleSecond(byte[] data)
        {
            return combineBytesToFloat(data, SAMPLE_DATA_SECOND);
        }

        public static float GetDataSampleThird(byte[] data)
        {
            return combineBytesToFloat(data, SAMPLE_DATA_THIRD);
        }

        public static float GetDataSampleForth(byte[] data)
        {
            return combineBytesToFloat(data, SAMPLE_DATA_FORTH);
        }

        public static byte GetDataPhaseLed(byte[] data)
        {
            return data[DATA_BYTE_FIRST];
        }

        public static float GetDataPhase(byte[] data)
        {
            return combineBytesToFloat(data, DATA_FLOAT_FIRST);
        }

        public static float GetDataTemperature(byte[] data)
        {
            return combineBytesToFloat(data, DATA_FLOAT_FIRST);
        }

        public static float GetDataTangent(byte[] data)
        {
            return combineBytesToFloat(data, DATA_FLOAT_FIRST);
        }

        public static byte GetDataRealImagSelect(byte[] data)
        {
            return data[DATA_BYTE_FIRST];
        }

        public static float GetDataReal(byte[] data)
        {
            return combineBytesToFloat(data, REAL_IMAGE_DATA_REAL);
        }

        public static float GetDataImag(byte[] data)
        {
            return combineBytesToFloat(data, REAL_IMAGE_DATA_IMAG);
        }

        public static byte GetDataDOSelect(byte[] data)
        {
            return data[DATA_BYTE_FIRST];
        }

        public static float GetDataDO(byte[] data)
        {
            return combineBytesToFloat(data, DATA_FLOAT_FIRST);
        }

        public static float GetDataCaliCoL0(byte[] data)
        {
            return combineBytesToFloat(data, CALI_CO_DATA_L0);
        }

        public static float GetDataCaliCoL1(byte[] data)
        {
            return combineBytesToFloat(data, CALI_CO_DATA_L1);
        }

        public static float GetDataCaliCoL2(byte[] data)
        {
            return combineBytesToFloat(data, CALI_CO_DATA_L2);
        }

        public static float GetDataCaliCoL3(byte[] data)
        {
            return combineBytesToFloat(data, CALI_CO_DATA_L3);
        }

        public static float GetDataCaliCoL4(byte[] data)
        {
            return combineBytesToFloat(data, CALI_CO_DATA_L4);
        }



        public static float GetDataTempCoL0(byte[] data)
        {
            return combineBytesToFloat(data, TEMP_CO_DATA_L0);
        }

        public static float GetDataTempCoL1(byte[] data)
        {
            return combineBytesToFloat(data, TEMP_CO_DATA_L1);
        }

        public static float GetDataTempCoL2(byte[] data)
        {
            return combineBytesToFloat(data, TEMP_CO_DATA_L2);
        }

        public static float GetDataUserCoL0(byte[] data)
        {
            return combineBytesToFloat(data, USR_CO_DATA_L0);
        }

        public static float GetDataUserCoL1(byte[] data)
        {
            return combineBytesToFloat(data, USR_CO_DATA_L1);
        }

        public static float GetDataT0(byte[] data)
        {
            return combineBytesToFloat(data, DATA_FLOAT_FIRST);
        }
        /*
        public static int GetDataRedCycle(byte[] data)
        {
            return combineByteToInt(data, EXCITE_RED_CYCLE_POS);
        }

        public static int GetDataRedDAC(byte[] data)
        {
            return combineByteToInt(data, EXCITE_RED_BRIGHTNESS_POS);
        }

        public static int GetDataBlueCycle(byte[] data)
        {
            return combineByteToInt(data, EXCITE_BLUE_CYCLE_POS);
        }

        public static int GetDataBlueDAC(byte[] data)
        {
            return combineByteToInt(data, EXCITE_BLUE_BRIGHTNESS_POS);
        }
         */
        public static int GetDataRedDAC(byte[] data)
        {
            return combineByteToInt(data, LED_DRIVE_RED_DAC_POS);
        }

        public static int GetDataBlueDAC(byte[] data)
        {
            return combineByteToInt(data, LED_DRIVE_BLUE_DAC_POS);
        }

        public static int GetDataDepth(byte[] data)
        {
            return combineByteToInt(data, LED_DRIVE_DEPTH_POS);
        }

        public static int GetDataAddress(byte[] data)
        {
            return combineByteToInt(data, WRITE_FLASH_ADDRESS_POS);
        }

        public static byte GetDataLedActiveSelected(byte[] data)
        {
            return data[DETECT_DATA_LED];
        }

        public static float GetDataLedActivePressure(byte[] data)
        {
            return combineBytesToFloat(data, DETECT_DATA_PRESSURE);
        }

        public static byte GetDataHWMaj(byte[] data)
        {
            return data[DATA_BYTE_FIRST];
        }

        public static byte GetDataHWMin(byte[] data)
        {
            return data[DATA_BYTE_FIRST + DATA_BYTE];
        }

        public static byte GetDataSWMaj(byte[] data)
        {
            return data[DATA_BYTE_FIRST + DATA_BYTE * 2];
        }

        public static byte GetDataSWMin(byte[] data)
        {
            return data[DATA_BYTE_FIRST + DATA_BYTE * 3];
        }

        public static byte GetDataProbeState(byte[] data)
        {
            return data[DATA_BYTE_FIRST];
        }

        public static byte GetDataProbeIndexSN(byte[] data, byte[] sn)
        {
            return GetDataIdSN(data, sn);
        }

        public static float GetDataPressure(byte[] data)
        {
    	    return combineBytesToFloat(data, DATA_FLOAT_FIRST);
        }

    }
}
