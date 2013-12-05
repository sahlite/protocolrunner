using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;

namespace ProtocolTest
{
    public partial class Form1 : Form
    {
        public const String VER_STR = "Protocol Runner V0.1.12";
        private byte[] buffer = new byte[ProtocolStruct.COMMAND_MAX_SIZE];
        private int buffer_len = 0;
        private byte[] command = new byte[ProtocolStruct.COMMAND_CORE_SIZE];
        private byte[] sn = new byte[ProtocolStruct.SN_LEN];
        private byte[] data = new byte[ProtocolStruct.WRITE_FLASH_ADDRESS_LEN+ProtocolStruct.WRITE_FLASH_DATA_LEN_MAX];
        private byte id = ProtocolStruct.DEFAULT_ID;
        private char[] tttt_sn = {'Y', 'L', '0', '1' ,'2', '0', '1', '3', '0', '0', '0', '2'};
        private byte[] status = new byte[ProtocolStruct.PS_LEN + ProtocolStruct.CS_LEN];

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < sn.Length; i++)
            {
                sn[i] = (byte)ProtocolStruct.DEFAULT_SN[i];
                test_sn[i] = (byte) ProtocolStruct.DEFAULT_SN[i];
            }

            
            SerialPort comm = new SerialPort();

            foreach (string s in SerialPort.GetPortNames())
            {
                try
                {
                    comm.PortName = s;
                    comm.Open();

                    if (comm.IsOpen)
                    {
                        //testComPort.Items.Add(s);
                        comPort.Items.Add(s);
                        comm.Close();
                    }
                }
                catch (Exception e)
                {
                }
            }
            try
            {
                comPort.SelectedIndex = 0;
            }
            catch (Exception e)
            {
            }

            FileStream configFile = new FileStream("config.bin", FileMode.OpenOrCreate);
            StreamReader wConfig = new StreamReader(configFile);
            String data = wConfig.ReadLine();
            if (null != data)
            {
                comBaud.Text = data;
                comByteSize.Text = wConfig.ReadLine();
                comParity.Text = wConfig.ReadLine();
                comStopBit.Text = wConfig.ReadLine();
            }
            else
            {
                comBaud.Text = "9600";
                comByteSize.Text = "8";
                comParity.Text = "NONE";
                comStopBit.Text = "1";
            }
            wConfig.Close();
            wConfig = null;
            configFile.Close();
            configFile = null;

            this.Text = VER_STR;

            listBoxDevices.Items.Clear();

            devices_id = new byte[ProtocolStruct.PROBE_NUM + 1];
            devices_sn = new byte[ProtocolStruct.PROBE_NUM + 1][];

            /*  
            testPort = new MyPort("COM8", 9600, 8, Parity.None, StopBits.One);
            testPort.mReader += TestFlowDataReader;
            //testPort.mReader += VIBarometerReader;
            testPort.Open();
            */ 
            txtTemperature.Text = string.Format("{0:0.######}", testcal(0x33c8, 2+9+13));
        }

        private float testcal(int data, int shit)
        {
            double tmp = 0d;
            tmp += data;
            return (float)(tmp / (1 << shit));
        }

        private String covArrayToString(byte[] array, int len)
        {
            String str = "";
            for (int i = 0; i < len; i++)
            {
                str += string.Format("{0:d3} ", array[i]);
            }
            return str;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            test_set_status();         // 1
            test_get_status();         // 2
            test_get_id_sn();          // 3
            test_erase();              // 4
            test_write_flash();        // 5
            test_write_flash_finish(); // 6
            test_get_sample_everage(); // 7
            test_get_phase();          // 8
            test_get_tangent();       // 9
            test_get_real_imag();      // 10
            test_get_temperature();    // 11
            test_get_t0();             // 12
            test_set_t0();             // 13
            test_get_cali_co();        // 14
            test_set_cali_co();        // 15
            test_get_temp_co();        // 16
            test_set_temp_co();        // 17
            test_get_baro_co();        // 18
            test_set_baro_co();        // 19
            test_get_led_drive();      // 20
            test_get_usr_co();         // 21
            test_set_usr_co();         // 22
            test_detect_do();          // 23
            test_set_led_drive();      // 24
        }

        private void test_set_status()
        {
            Int32 len = 0;
            Int32 result = 0;
            textBox1.AppendText("---1---set status---\n");
            // 请求发送
            len = ProtocolStruct.PackageSetStatus(buffer, ProtocolStruct.STATUS_APP, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len)+"\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + ProtocolStruct.GetDataStatus(data) + "\n");
            // 响应发送
            len = ProtocolStruct.PackageSetStatusResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
        }

        private void test_get_status()
        {
            Int32 len = 0;
            Int32 result = 0;
            textBox1.AppendText("---2---get status---\n");
            // 请求发送
            len = ProtocolStruct.PackageGetStatus(buffer, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            len = ProtocolStruct.PackageGetStatusResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, ProtocolStruct.STATUS_APP);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + ProtocolStruct.GetDataStatus(data) + "\n");
        }

        private void test_get_id_sn()
        {
            Int32 len = 0;
            Int32 result = 0;
            textBox1.AppendText("---3---get id and sn---\n");
            // 请求发送
            len = ProtocolStruct.PackageGetIdSN(buffer, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            len = ProtocolStruct.PackageGetIdSNResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            id = ProtocolStruct.GetDataIdSN(data, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + id + "," + covArrayToString(sn, ProtocolStruct.SN_LEN) + "\n");
        }

        private void test_erase()
        {
            Int32 len = 0;
            Int32 result = 0;
            textBox1.AppendText("---4---set erase---\n");
            // 请求发送
            len = ProtocolStruct.PackageSetErase(buffer, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            len = ProtocolStruct.PackageSetEraseResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
        }

        private void test_write_flash()
        {
            textBox1.AppendText("---5---set erase---\n");
        }

        private void test_write_flash_finish()
        {
            Int32 len = 0;
            Int32 result = 0;
            textBox1.AppendText("---6---set flash write finish---\n");
            // 请求发送
            len = ProtocolStruct.PackageSetFinish(buffer, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            len = ProtocolStruct.PackageSetFinishResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
        }

        private void test_get_sample_everage()
        {
            Int32 len = 0;
            Int32 result = 0;
            textBox1.AppendText("---7---get sample---\n");
            // 请求发送
            len = ProtocolStruct.PackageGetSampleEverage(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.LED_RED);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            byte led = ProtocolStruct.GetDataSampleLed(data);
            //byte number = ProtocolStruct.GetDataSampleNumber(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + ", " + led + "\n");
            // 响应发送
            float sample_first = (11f);
            float sample_second = (1f);
            float sample_third = (0.1f);
            float sample_forth = (9999.9999f);

            len = ProtocolStruct.PackageGetSampleEverageResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, sample_first, sample_second, sample_third, sample_forth);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            sample_first = ProtocolStruct.GetDataSampleFirst(data);
            sample_second = ProtocolStruct.GetDataSampleSecond(data);
            sample_third = ProtocolStruct.GetDataSampleThird(data);
            sample_forth = ProtocolStruct.GetDataSampleForth(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + ",");
            textBox1.AppendText(string.Format("{0:F6}", sample_first));
            textBox1.AppendText(",");
            textBox1.AppendText(string.Format("{0:F6}", sample_second));
            textBox1.AppendText(",");
            textBox1.AppendText(string.Format("{0:F6}", sample_third));
            textBox1.AppendText(",");
            textBox1.AppendText(string.Format("{0:F6}", sample_forth));
            textBox1.AppendText("\n");
            
        }

        private void test_get_phase()
        {
            /*
            Int32 len = 0;
            Int32 result = 0;
            textBox1.AppendText("---8---get phase---\n");
            // 请求发送
            len = ProtocolStruct.PackageGetPhase(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.LED_RED);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, ProtocolStruct.DEFAULT_ID, sn);
            byte led = ProtocolStruct.GetDataPhaseLed(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + ", " + led +  "\n");
            // 响应发送
            float sample_value = 2f;

            len = ProtocolStruct.PackageGetPhaseResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, sample_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, ProtocolStruct.DEFAULT_ID, sn);
            sample_value = ProtocolStruct.GetDataPhase(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + sample_value + "\n");
             */
        }

        private void test_get_tangent()
        {
            textBox1.AppendText("---9---get tangent---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            len = ProtocolStruct.PackageGetTangent(buffer, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            float sample_value = 708.48f;

            len = ProtocolStruct.PackageGetTangentResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, sample_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            sample_value = ProtocolStruct.GetDataTangent(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + sample_value + "\n");
        }

        private void test_get_real_imag()
        {
            textBox1.AppendText("---10---get real imag---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            len = ProtocolStruct.PackageGetRealImag(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.REAL_IMAGE_DATA_REF);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            byte select = ProtocolStruct.GetDataRealImagSelect(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            float real_value = 508.48f;
            float imag_value = 408.48f;

            len = ProtocolStruct.PackageGetRealImagResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, real_value, imag_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            real_value = ProtocolStruct.GetDataReal(data);
            imag_value = ProtocolStruct.GetDataImag(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + real_value + "," + imag_value + "\n");
        }

        private void test_get_temperature()
        {
            Int32 len = 0;
            Int32 result = 0;
            textBox1.AppendText("---11---get temperature---\n");
            // 请求发送
            len = ProtocolStruct.PackageGetTemperature(buffer, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            float sample_value = 708.48f;

            len = ProtocolStruct.PackageGetTemperatureResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, sample_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            sample_value = ProtocolStruct.GetDataTemperature(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + sample_value + "\n");
        }

        private void test_get_t0()
        {
            textBox1.AppendText("---12---get T0---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            len = ProtocolStruct.PackageGetT0(buffer, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            float sample_value = 708.48f;

            len = ProtocolStruct.PackageGetT0Response(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, sample_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            sample_value = ProtocolStruct.GetDataT0(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + sample_value + "\n");
        }

        private void test_set_t0()
        {
            textBox1.AppendText("---13---set T0---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            float sample_value = 708.48f;
            len = ProtocolStruct.PackageSetT0(buffer, command, ProtocolStruct.DEFAULT_ID, sn, sample_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            sample_value = ProtocolStruct.GetDataT0(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + sample_value + "\n");
            // 响应发送
            len = ProtocolStruct.PackageSetT0(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
        }

        private void test_get_cali_co()
        {
            textBox1.AppendText("---14---get calibration cooperator---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            len = ProtocolStruct.PackageGetCaliCo(buffer, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            float l0_value = 408.48f;
            float l1_value = 508.48f;
            float l2_value = 608.48f;
            float l3_value = 708.48f;
            float l4_value = 808.48f;

            len = ProtocolStruct.PackageGetCaliCoResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, l0_value, l1_value, l2_value, l3_value, l4_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            l0_value = ProtocolStruct.GetDataCaliCoL0(data);
            l1_value = ProtocolStruct.GetDataCaliCoL1(data);
            l2_value = ProtocolStruct.GetDataCaliCoL2(data);
            l3_value = ProtocolStruct.GetDataCaliCoL3(data);
            l4_value = ProtocolStruct.GetDataCaliCoL4(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + l0_value + "," + l1_value + "," + l2_value + "," + l3_value + "," + l4_value + "\n");
        }

        private void test_set_cali_co()
        {
            textBox1.AppendText("---15---set calibration cooperator---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            float l0_value = 408.48f;
            float l1_value = 508.48f;
            float l2_value = 608.48f;
            float l3_value = 708.48f;
            float l4_value = 808.48f;

            len = ProtocolStruct.PackageSetCaliCo(buffer, command, ProtocolStruct.DEFAULT_ID, sn, l0_value, l1_value, l2_value, l3_value, l4_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            l0_value = ProtocolStruct.GetDataCaliCoL0(data);
            l1_value = ProtocolStruct.GetDataCaliCoL1(data);
            l2_value = ProtocolStruct.GetDataCaliCoL2(data);
            l3_value = ProtocolStruct.GetDataCaliCoL3(data);
            l4_value = ProtocolStruct.GetDataCaliCoL4(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + l0_value + "," + l1_value + "," + l2_value + "," + l3_value + "," + l4_value + "\n");
            
            // 响应发送
            len = ProtocolStruct.PackageSetCaliCoResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
        }

        private void test_get_temp_co()
        {
            textBox1.AppendText("---16---get temperature cooperator---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            len = ProtocolStruct.PackageGetTempCo(buffer, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            float l0_value = 1f;
            float l1_value = 19.999999f;
            float l2_value = 16f;


            len = ProtocolStruct.PackageGetTempCoResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, l0_value, l1_value, l2_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            l0_value = ProtocolStruct.GetDataTempCoL0(data);
            l1_value = ProtocolStruct.GetDataTempCoL1(data);
            l2_value = ProtocolStruct.GetDataTempCoL2(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + l0_value + "," + l1_value + "," + l2_value + "\n");
        }

        private void test_set_temp_co()
        {
            textBox1.AppendText("---17---set temperature cooperator---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            float l0_value = 9999.999f;
            float l1_value = 10000.001f;
            float l2_value = 5555.555f;

            len = ProtocolStruct.PackageSetTempCo(buffer, command, ProtocolStruct.DEFAULT_ID, sn, l0_value, l1_value, l2_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            l0_value = ProtocolStruct.GetDataTempCoL0(data);
            l1_value = ProtocolStruct.GetDataTempCoL1(data);
            l2_value = ProtocolStruct.GetDataTempCoL2(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + l0_value + "," + l1_value + "," + l2_value + "\n");

            // 响应发送
            len = ProtocolStruct.PackageSetTempCoResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
        }

        private void test_get_baro_co()
        {
            textBox1.AppendText("---18---get barometric cooperator---\n");
        }

        private void test_set_baro_co()
        {
            textBox1.AppendText("---19---set barometric cooperator---\n");
        }

        private void test_get_led_drive()
        {
            textBox1.AppendText("---20---get led drive---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            len = ProtocolStruct.PackageGetLedDrive(buffer, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            int red_dac = 405;
            int blue_dac = 193;
            int depth = 10;
            len = ProtocolStruct.PackageGetLedDriveResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, red_dac, blue_dac, depth);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            red_dac = ProtocolStruct.GetDataRedDAC(data);;
            blue_dac = ProtocolStruct.GetDataBlueDAC(data);
            depth = ProtocolStruct.GetDataDepth(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + red_dac + "," + blue_dac + "," + depth + "\n");
        }

        private void test_get_usr_co()
        {
            textBox1.AppendText("---21---get user cooperator---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            len = ProtocolStruct.PackageGetUserCo(buffer, command, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            float l0_value = 408.48f;
            float l1_value = 508.48f;

            len = ProtocolStruct.PackageGetUserCoResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, l0_value, l1_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            l0_value = ProtocolStruct.GetDataUserCoL0(data);
            l1_value = ProtocolStruct.GetDataUserCoL1(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + l0_value + "," + l1_value + "\n");
        }

        private void test_set_usr_co()
        {
            textBox1.AppendText("---22---set user cooperator---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            float l0_value = 408.48f;
            float l1_value = 508.48f;

            len = ProtocolStruct.PackageSetUserCo(buffer, command, ProtocolStruct.DEFAULT_ID, sn, l0_value, l1_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            l0_value = ProtocolStruct.GetDataUserCoL0(data);
            l1_value = ProtocolStruct.GetDataUserCoL1(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + l0_value + "," + l1_value + "\n");

            // 响应发送
            len = ProtocolStruct.PackageSetUserCoResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
        }

        private void test_detect_do()
        {
            textBox1.AppendText("---23---get do---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            len = ProtocolStruct.PackageGetDO(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.DO_DATA_UNCOMPENSATED);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            byte select = ProtocolStruct.GetDataDOSelect(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
            // 响应发送
            float real_value = 508.48f;

            len = ProtocolStruct.PackageGetDOResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK, real_value);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            real_value = ProtocolStruct.GetDataDO(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + real_value + "\n");
        }

        private void test_set_led_drive()
        {
            textBox1.AppendText("---24---get led drive---\n");
            Int32 len = 0;
            Int32 result = 0;
            // 请求发送
            
            int red_dac = 105;
            int blue_dac = 300;
            int depth = 10;

            len = ProtocolStruct.PackageSetLedDrive(buffer, command, ProtocolStruct.DEFAULT_ID, sn, red_dac, blue_dac, depth);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 请求解析
            result = ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            red_dac = ProtocolStruct.GetDataRedDAC(data);
            blue_dac = ProtocolStruct.GetDataBlueDAC(data);
            depth = ProtocolStruct.GetDataDepth(data);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "," + red_dac + "," + blue_dac + "," + depth  + "\n");
            // 响应发送
            len = ProtocolStruct.PackageSetLedDriveResponse(buffer, command, ProtocolStruct.DEFAULT_ID, sn, ProtocolStruct.RESULT_OK);
            textBox1.AppendText(covArrayToString(buffer, len) + "\n");
            // 响应解析
            ProtocolStruct.PackageCommandDown(len, buffer, command, data, status, ProtocolStruct.DEFAULT_ID, sn);
            textBox1.AppendText(covArrayToString(command, ProtocolStruct.COMMAND_CORE_SIZE) + ", " + result + "\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int len;
            len = ProtocolStruct.PackageGetIdSN(buffer, command, id, sn);
            write(buffer, len);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {

            if (null != mPort)
            {
                mPort.Close();
                mPort = null;
            }

            mPort = new MyPort(comPort.Text,
                        Convert.ToInt32(comBaud.Text),
                        Convert.ToInt32(comByteSize.Text),
                        StringToParity(comParity.Text),
                        StringToStopBits(comStopBit.Text));
            mPort.Open();
            textBox1.AppendText(comPort.Text + " open\n");

            mPort.mReader += FlowDataReader;
            //mPort.mReader += VIThermometerReader;
            mPort.mReporter += FlowErrorReporter;

            button2.PerformClick();
        }

        public static Parity StringToParity(string parity)
        {
            if ("ODD" == parity)
            {
                return Parity.Odd;
            }
            else if ("EVEN" == parity)
            {
                return Parity.Even;
            }
            return Parity.None;
        }

        public static StopBits StringToStopBits(string stopbits)
        {
            return ("TWO" == stopbits) ? StopBits.Two : StopBits.One;
        }

        private MyPort mPort;

        private void button7_Click(object sender, EventArgs e)
        {
            if (null != mPort)
            {
                mPort.Close();
                mPort = null;
                textBox1.AppendText(comPort.Text + " close\n");
            }
        }

        private void write(byte[] buffer, int len)
        {
            if ((null != mPort) /*&& mPort.IsOpen()*/)
            {
                buffer_len = len;
                mPort.write(len, buffer);
                update_message("SEND :" + FromUnicodeByteArray(buffer, 0, len) + "\n");
                //ReadProtectStart();
            }
            else
            {
                /*  
                byte[] test_command = new byte[ProtocolStruct.COMMAND_CORE_SIZE];
                test_command[ProtocolStruct.HEAD_TYPE] = ProtocolStruct.HEAD_REQ;
                test_command[ProtocolStruct.OP_TYPE] = command[ProtocolStruct.OP_TYPE];
                test_command[ProtocolStruct.OB_TYPE] = command[ProtocolStruct.OB_TYPE];
                test_command[ProtocolStruct.LEN_TYPE_H] = command[ProtocolStruct.LEN_TYPE_H];
                test_command[ProtocolStruct.LEN_TYPE_H] = command[ProtocolStruct.LEN_TYPE_H];
                TestHandle(buffer, len, test_command, id, sn);
                */
                update_message("port is not opend! \n");
            }
        }

        public static string FromUnicodeByteArray(byte[] characters, Int32 start_pos, Int32 len)
        {
            String result = "";
            for (int i = 0; i < len; i++)
                result += string.Format("{0:X2}", characters[start_pos + i]);
            return result;
        }

        private void HandleResponse()
        {
            //ReadProtectStop();
            int result = ProtocolStruct.PackageCommandDown(rbuf_len, rbuf, command, data, status, id, sn);
            update_message("GET : " + FromUnicodeByteArray(rbuf, 0, rbuf_len));
            if (ProtocolStruct.RESULT_OK == result)
            {
                resend_count = 0;
                if (ProtocolStruct.OP_GET == command[ProtocolStruct.OP_TYPE])
                {
                    switch (command[ProtocolStruct.OB_TYPE])
                    {
                        case ProtocolStruct.OB_ID_SN:
                            id = ProtocolStruct.GetDataIdSN(data, sn);
                            byte id_in_sn = ProtocolStruct.GetIdInSN(sn);
                            if (true)//id == id_in_sn)
                            {
                                String strID = string.Format("{0:D}", id);
                                char[] char_sn = new char[ProtocolStruct.SN_LEN];
                                for (int i = 0; i < ProtocolStruct.SN_LEN; i++)
                                {
                                    char_sn[i] = (char)sn[i];
                                }
                                String strSN = new String(char_sn);//FromUnicodeByteArray(sn, 0, ProtocolStruct.SN_LEN);
                                update_id_sn(strID, strSN);
                                update_message("GET ID AND SN SUCCESS ! ");

                                int len = ProtocolStruct.PackageGetHWSW(buffer, command, id, sn);
                                write(buffer, len);
                            }
                            else
                            {
                                update_message("ID not match SN ! ");
                            }
                            break;

                        case ProtocolStruct.OB_REAL_IMAG:
                            float real_value = ProtocolStruct.GetDataReal(data);
                            float imag_value = ProtocolStruct.GetDataImag(data);
                            //String strReal= string.Format("{0:F6}", real_value);
                            //String strImage = string.Format("{0:F6}", imag_value);
                            update_real_iamge(real_value, imag_value);
                            update_message("GET REAL AND IMAGE SUCCESS ! ");
                            break;

                        case ProtocolStruct.OB_SAMPLE:
                            float first_value = ProtocolStruct.GetDataSampleFirst(data);
                            float second_value = ProtocolStruct.GetDataSampleSecond(data);
                            float third_value = ProtocolStruct.GetDataSampleThird(data);
                            float forth_value = ProtocolStruct.GetDataSampleForth(data);
                            update_sample(first_value, second_value, third_value, forth_value);
                            update_message("GET SAMPLE SUCCESS !");
                            break;

                        case ProtocolStruct.OB_TANGENT:
                            if (isRunningTest)
                            {
                                handleTest(data);
                            }
                            else
                            {
                                if (ProtocolStruct.HEAD_RSP == command[ProtocolStruct.HEAD_TYPE])
                                {
                                    float tangent = ProtocolStruct.GetDataTangent(data);
                                    update_tangent(tangent);
                                    update_message("GET TANGENT SUCCESS !");
                                }
                                else
                                {
                                    update_message("REQUEST TANGENT !");
                                }
                            }
                            
                            
                            break;

                        case ProtocolStruct.OB_TEMPERATURE:
                            if (isRunningTest)
                            {
                                handleTest(data);
                            }
                            else
                            {
                                if (is_getting_temperature)
                                {
                                    float temperature = ProtocolStruct.GetDataTemperature(data);
                                    update_message(string.Format("{0:f}", temperature));

                                    Thread.Sleep(80);

                                    int len = ProtocolStruct.PackageGetTemperature(buffer, command, id, sn);
                                    write(buffer, len);
                                }
                                else
                                {
                                    float temperature = ProtocolStruct.GetDataTemperature(data);
                                    update_temperature(temperature);
                                    update_message("GET TEMPERATURE SUCCESS !");
                                }
                            }
                            
                            break;

                        case ProtocolStruct.OB_LED_DRIVE:
                            int red_dac = ProtocolStruct.GetDataRedDAC(data);
                            int blue_dac = ProtocolStruct.GetDataBlueDAC(data);
                            int depth = ProtocolStruct.GetDataDepth(data);
                            update_led_drive(red_dac, blue_dac, depth);
                            break;

                        case ProtocolStruct.OB_CALI_CO:
                            float dc0 = ProtocolStruct.GetDataCaliCoL0(data);
                            float dc1 = ProtocolStruct.GetDataCaliCoL1(data);
                            float dc2 = ProtocolStruct.GetDataCaliCoL2(data);
                            float dc3 = ProtocolStruct.GetDataCaliCoL3(data);
                            float dc4 = ProtocolStruct.GetDataCaliCoL4(data);
                            float[] dc_array = { dc0, dc1, dc2, dc3, dc4};
                            update_something(0, dc_array);
                            update_message("GET CALIBRATION COEFFICIENT SUCCESS !");
                            break;
                        case ProtocolStruct.OB_TEMP_CO:
                            float tc0 = ProtocolStruct.GetDataTempCoL0(data);
                            float tc1 = ProtocolStruct.GetDataTempCoL1(data);
                            float tc2 = ProtocolStruct.GetDataTempCoL2(data);
                            float[] tc_array = { tc0, tc1, tc2};
                            update_something(1, tc_array);
                            update_message("GET TEMPERATURE COEFFICIENT SUCCESS !");
                            break;
                        case ProtocolStruct.OB_USR_CO:
                            float uc0 = ProtocolStruct.GetDataUserCoL0(data);
                            float uc1 = ProtocolStruct.GetDataUserCoL1(data);
                            float[] uc_array = { uc0, uc1};
                            update_something(2, uc_array);
                            update_message("GET USER COEFFICIENT SUCCESS !");
                            break;
                        case ProtocolStruct.OB_T0:
                            float t0 = ProtocolStruct.GetDataT0(data);
                            float[] t0_array = { t0 };
                            update_something(4, t0_array);
                            update_message("GET T0 SUCCESS !");
                            break;
                        case ProtocolStruct.OB_DO:
                            if (isRunningTest)
                            {
                                handleTest(data);
                            }
                            else
                            {
                                float mydo = ProtocolStruct.GetDataDO(data);
                                float[] do_array = { mydo };
                                update_something(3, do_array);
                                update_message("GET DO SUCCESS !");
                            }
                            break;

                        case ProtocolStruct.OB_HW_SW:
                            byte hw_major = ProtocolStruct.GetDataHWMaj(data);
                            byte hw_min = ProtocolStruct.GetDataHWMin(data);
                            byte sw_major = ProtocolStruct.GetDataSWMaj(data);
                            byte sw_min = ProtocolStruct.GetDataSWMin(data);
                            update_hw_sw(hw_major, hw_min, sw_major, sw_min);
                            update_message("GET HW SW SUCCESS !");

                            //update_probe_num();
                            break;

                        case ProtocolStruct.OB_PROBE_NUM:
                            byte num = ProtocolStruct.GetDataProbeState(data);
                            update_probe_state(num);
                            update_message("GET PROBE NUM SUCCESS!");

                            device_index = 0;
                            update_devices();
                            break;

                        case ProtocolStruct.OB_PROBE_SN:
                            byte[] probe_sn = new byte[ProtocolStruct.SN_LEN];
                            byte index = ProtocolStruct.GetDataProbeIndexSN(data, probe_sn);
                            update_probe_index_sn(index, probe_sn);
                            update_message("GET PROBE SN SUCCESS");
                            break;

                        case ProtocolStruct.OB_PRESSURE:
                            float pressure = ProtocolStruct.GetDataPressure(data);
                            update_pressure(pressure);
                            update_message("GET PRESSURE SUCCESS");
                            break;

                        case ProtocolStruct.OB_DEBUG_INFO:
                            update_data(FromUnicodeByteArray(data, 0, ProtocolStruct.DEBUG_INFO_BLOCK_SIZE));
                            update_message("GET CRITICAL SUCCESS");
                            break;

                        default:
                            update_message("Wrong OB CODE IN GET! ");
                            break;
                    }
                }
                else if (ProtocolStruct.OP_SET == command[ProtocolStruct.OP_TYPE])
                {
                    switch (command[ProtocolStruct.OB_TYPE])
                    {
                        case ProtocolStruct.OB_LED_DRIVE:
                            update_message("SET LED DRIVE SUCCESS !");
                            break;

                        case ProtocolStruct.OB_DETECT:
                            if (isRunningTest)
                            {
                                handleTest(data);
                            }
                            else
                            {
                                update_message("ACTIVE LED SUCCESS !");
                            }
                            break;

                        case ProtocolStruct.OB_CALI_CO:
                            update_message("SET CALIBRATION COEFFICIENT SUCCESS !");
                            break;
                        case ProtocolStruct.OB_TEMP_CO:
                            update_message("GET TEMPERATURE COEFFICIENT SUCCESS !");
                            break;
                        case ProtocolStruct.OB_USR_CO:
                            update_message("GET USER COEFFICIENT SUCCESS !");
                            break;
                        case ProtocolStruct.OB_T0:
                            update_message("GET T0 SUCCESS !");
                            break;
                        case ProtocolStruct.OB_ID_SN:
                            update_message("SET ID SN SUCCESS !");
                            break;

                        case ProtocolStruct.OB_SALINITY:
                            update_message("SET SALINITY SUCCESS !");
                            break;

                        default:
                            update_message("Wrong OB CODE IN SET!");
                            break;
                    }
                }
                else
                {
                    update_message("Wrong OP CODE !");
                }
                update_status(status[ProtocolStruct.STATUS_C], status[ProtocolStruct.STATUS_P]);
            }
            else
            {
                if ((ProtocolStruct.OP_SET == command[ProtocolStruct.OP_TYPE])
                    && (ProtocolStruct.OB_ID_SN == command[ProtocolStruct.OB_TYPE]))
                {
                    for (int i = 0; i < ProtocolStruct.SN_LEN; i++)
                    {
                        sn[i] = old_sn[i];
                    }
                    id = ProtocolStruct.GetIdInSN(sn);
                    
                    String strID = string.Format("{0:D}", id);
                    char[] char_sn = new char[ProtocolStruct.SN_LEN];
                    for (int i = 0; i < ProtocolStruct.SN_LEN; i++)
                    {
                        char_sn[i] = (char)sn[i];
                    }
                    String strSN = new String(char_sn);//FromUnicodeByteArray(sn, 0, ProtocolStruct.SN_LEN);
                    update_id_sn(strID, strSN);
                     /**/
                }
                else
                {
                    if (3 > resend_count++)
                    {
                        write(buffer, buffer_len);
                        update_message("Resend !");
                    }  
                }
                

            }
            
        }

        private int resend_count = 0;

        private void ReadProtectStart()
        {
            Thread read_protect_t = new Thread(delegate()
            {
                Thread.Sleep(800);
                isCompeletePackage = true;
                rbuf_len = in_buf_index;
                HandleResponse();
            });
            read_protect_t.Start();
            read_protect_list.Add(read_protect_t);
        }

        private void ReadProtectStop()
        {
            while (0 != read_protect_list.Count)
            {
                Thread read_protect_t = read_protect_list.First<Thread>();
                read_protect_t.Abort();
                read_protect_list.Remove(read_protect_t);
            }
        }

        private List<Thread> read_protect_list = new List<Thread>();
        int data_count = 0;
        byte sample_old_data = 0;
        bool is_old_sample = false;
        string my_data_message = "";
        byte[] input_data = new byte[108];
        public void FlowDataReader(object sender, byte[] data, Int32 data_len)
        {
            //Int32 package_len = 0;

            //update_message(FromUnicodeByteArray(data, 0, data_len));
            /*
            if (ProtocolStruct.OB_DETECT == command[ProtocolStruct.OB_TYPE])
            {
                int i = 0;
                
                for (i = 0; (i < data_len) && (108 != data_count); i++, data_count++)
                {
                    input_data[data_count] = data[i];
                }
                
                if (108 == data_count)
                {
                    my_data_message = "****data start**** \r\n";
                    for (int j = 24; j < 108; j+=4)
                    {
                        float tangent = ProtocolStruct.combineBytesToFloat(input_data, j);
                        double angle = Math.Atan(tangent) * 180 / Math.PI;
                        my_data_message += string.Format("{0:f6} \r\n", angle);
                    }
                    my_data_message += "****data end**** \r\n";
                    update_data(my_data_message);
                    my_data_message = "";
                    data_count = 0;
                }
            }
             */
            //else
            {

                int start_index = 0;
                for (start_index = 0; (0 == in_buf_index)&&(start_index < data_len); start_index++)
                {
                    if (ProtocolStruct.IsRSPHead(data[start_index])||ProtocolStruct.IsREQHead(data[start_index]))
                    {
                        break;
                    }
                }

                isCompeletePackage = false;
                FillReadBuffer(data, start_index, data_len-start_index);

                if (ProtocolStruct.getPackageLen(rbuf, in_buf_index) <= in_buf_index)
                {
                    ReadProtectStop();
                    isCompeletePackage = true;
                    rbuf_len = ProtocolStruct.getPackageLen(rbuf, in_buf_index);
                    HandleResponse();
                    if (isCompeletePackage)
                    {
                        ClearReadBuffer();
                        in_buf_index = 0;
                    }
                }
            }
            /*
            if (isCompeletePackage)
            {
                ClearReadBuffer();
                in_buf_index = 0;
            }

            Int32 expect_len = ProtocolStruct.combineByteToInt(command, ProtocolStruct.LEN_TYPE_H);

            expect_len = (0 == expect_len) ? data_len : expect_len;
            package_len = (expect_len > in_buf_index + data_len) ? data_len : expect_len - in_buf_index;

            if (expect_len > in_buf_index + package_len)
            {
                rbuf_len = 0;
                isCompeletePackage = false;
                FillReadBuffer(data, package_len);
                if (!ProtocolStruct.IsHead(rbuf))
                {
                    update_message("!!! head error :" + FromUnicodeByteArray(data, 0, data_len));
                    update_message("----!!!head error!!!---");
                    rbuf_len = 0;
                    isCompeletePackage = true;
                    HandleResponse();
                    return;
                }
                update_message("!!!incomplete package :" + FromUnicodeByteArray(data, 0, data_len));

            }
            else
            {
                isCompeletePackage = true;
                rbuf_len = in_buf_index + package_len;
                FillReadBuffer(data, data_len);
                //update_message("----full package---");
                HandleResponse();
            }
             * */
        }

        public void VIThermometerReader(object sender, byte[] data, Int32 data_len)
        {
            if (5 == data_len)
            {
                if ((2 == data[0]) && (3 == data[4]))
                {
                    byte[] tmp = new byte[6];
                    tmp[0] = 2;
                    tmp[5] = 3;
                    if (1 == data[2])
                    {
                        ProtocolStruct.splitFloatToBytes(27.8f, tmp, 1);
                    }
                    else if (2 == data[3])
                    {
                        ProtocolStruct.splitFloatToBytes(770f, tmp, 1);
                    }
                  
                    write(tmp, 6);
                }
            }
        }

        public void VIBarometerReader(object sender, byte[] data, Int32 data_len)
        {
            if (5 == data_len)
            {
                if ((2 == data[0]) && (3 == data[4]))
                {
                    byte[] tmp = new byte[6];
                    tmp[0] = 2;
                    tmp[5] = 3;
                    ProtocolStruct.splitFloatToBytes(770f, tmp, 1);
                    test_write(tmp, 6);
                }
            }
        }

        public void TestFlowDataReader(object sender, byte[] data, Int32 data_len)
        {
            byte[] tmp = new byte[ProtocolStruct.COMMAND_CORE_SIZE];
            ProtocolStruct.FindResponseFor(command, tmp);
            TestHandle(data, data_len, tmp, test_id, test_sn);
        }

        private byte test_id = 0xff;
        private byte[] test_sn = new byte[ProtocolStruct.SN_LEN];
        private Boolean is_first_get_id_sn = true;

        private void FillReadBuffer(byte[] data, int data_start_index, Int32 data_len)
        {
            for (int i = 0; i < data_len; i++, in_buf_index++)
            {
                if (ProtocolStruct.COMMAND_MAX_SIZE <= in_buf_index)
                {
                    in_buf_index = 0;
                }

                rbuf[in_buf_index] = data[i + data_start_index];

                
            }
        }

        public void FlowErrorReporter(object sender, string error_message)
        {
            update_message(error_message);
        }

        private void ClearReadBuffer()
        {
            for (int i = 0; i < ProtocolStruct.COMMAND_MAX_SIZE; i++)
            {
                rbuf[i] = 0;
            }
        }

        private byte[] rbuf = new byte[ProtocolStruct.COMMAND_MAX_SIZE];
        private Int32 rbuf_len;
        private int in_buf_index = 0;
        private Boolean isCompeletePackage = true;

        private void button3_Click(object sender, EventArgs e)
        {
            int red_dac = int.Parse(txtRedDAC.Text);
            
            int blue_dac = int.Parse(txtBlueDAC.Text);
            int depth = int.Parse(txtDepth.Text);
            int len = ProtocolStruct.PackageSetLedDrive(buffer, command, id, sn, red_dac, blue_dac, depth);
            write(buffer, len);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte selectLed = ProtocolStruct.DETECT_LED_RED_BLUE;
            if ("red" == comLed.Text)
            {
                selectLed = ProtocolStruct.DETECT_LED_RED;
            }
            else if ("blue" == comLed.Text)
            {
                selectLed = ProtocolStruct.DETECT_LED_BLUE;
            }
            float pressure = float.Parse(txtPressure.Text);
            int len = ProtocolStruct.PackageSetLedActive(buffer, command, id, sn, selectLed, pressure);
            write(buffer, len);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte selectData = ProtocolStruct.REAL_IMAGE_DATA_ZERO;
            if ("ref" == comRealImage.Text)
            {
                selectData = ProtocolStruct.REAL_IMAGE_DATA_REF;
            }
            else if ("signal" == comRealImage.Text)
            {
                selectData = ProtocolStruct.REAL_IMAGE_DATA_SIGNAL;
            }
            int len = ProtocolStruct.PackageGetRealImag(buffer, command, id, sn, selectData);
            write(buffer, len);
        }

        private int tangent_index = 0;
        private float[] tangent_value={12f, 6f, 3f, 4f};
        private float[] do_value = { 190.5f, 180.7f, 106.8f, 130.4f};
        private byte probe_id = 1;
        private char[] probe_sn = {'Y', 'L', '0', '1', '1', '3', '0', '9', '0', '0', '0', '0' };

        private void TestHandle(byte[] in_buff, int in_len, byte[] in_command, byte in_id, byte[] in_sn)
        {
            if (ProtocolStruct.IsREQHead(in_buff[ProtocolStruct.POS_HEAD]))
            {
                if (ProtocolStruct.IsHasLength(in_buff, in_len)){
                    int len = ProtocolStruct.getLength(in_buff, in_len);
                    if (in_len >= len + ProtocolStruct.HEAD_LEN + ProtocolStruct.LEN_LEN)
                    {
                        byte[] inner_data = new byte[ProtocolStruct.COMMAND_MAX_SIZE];
                        byte[] tmp_sn = new byte[ProtocolStruct.SN_LEN];
                        for (int k = 0; k < ProtocolStruct.SN_LEN; k++)
                        {
                            tmp_sn[k] = (byte)probe_sn[k];
                        }
                        int result = ProtocolStruct.PackageCommandDown(in_len, in_buff, in_command, inner_data, status, in_id, in_sn);
                        if (ProtocolStruct.RESULT_ID_ERR == result)
                        {

                            result = ProtocolStruct.PackageCommandDown(in_len, in_buff, in_command, inner_data, status, probe_id, tmp_sn);
                        }
                        if ((ProtocolStruct.RESULT_OK == result))
                        {
                            int wlen = 0;
                            byte[] inner_buff = new byte[ProtocolStruct.COMMAND_MAX_SIZE];
                            if (ProtocolStruct.OP_GET == in_command[ProtocolStruct.OP_TYPE])
                            {
                                switch (in_command[ProtocolStruct.OB_TYPE])
                                {
                                    case ProtocolStruct.OB_ID_SN:
                                        if (is_first_get_id_sn)
                                        {
                                            /**/
                                            byte new_id = 0xFF;
                                            byte[] new_sn = new byte[ProtocolStruct.SN_LEN];
                                            new_sn[0] = (byte)'Y';
                                            new_sn[1] = (byte)'L';
                                            new_sn[2] = (byte)'F';
                                            new_sn[3] = (byte)'F';
                                            new_sn[4] = (byte)'F';
                                            new_sn[5] = (byte)'F';
                                            new_sn[6] = (byte)'F';
                                            new_sn[7] = (byte)'F';
                                            new_sn[8] = (byte)'F';
                                            new_sn[9] = (byte)'F';
                                            new_sn[10] = (byte)'F';
                                            new_sn[11] = (byte)'F';
                                             
     
                                            wlen = ProtocolStruct.PackageGetIdSNResponse(inner_buff, in_command, id, sn, ProtocolStruct.RESULT_OK, new_id, new_sn);
                                            test_write(inner_buff, wlen);
                                            test_id = new_id;
                                            for (int k = 0; k < ProtocolStruct.SN_LEN; k++)
                                            {
                                                test_sn[k] = new_sn[k];
                                            }
                                            is_first_get_id_sn = false;
                                        }
                                        else
                                        {
                                            wlen = ProtocolStruct.PackageGetIdSNResponse(inner_buff, in_command, id, sn, ProtocolStruct.RESULT_OK, id, sn);
                                            test_write(inner_buff, wlen);
                                        }
                                        
                                        break;

                                    case ProtocolStruct.OB_REAL_IMAG:
                                        byte selectLed = ProtocolStruct.GetDataRealImagSelect(inner_data);
                                        float r = 100.4f;
                                        float i = 203.91f;
                                        wlen = ProtocolStruct.PackageGetRealImagResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK, r, i);
                                        test_write(inner_buff, wlen);
                                        break;

                                    case ProtocolStruct.OB_SAMPLE:
                                        float first = 809.378f;
                                        float second = 809.3781f;
                                        float third = 809.377f;
                                        float forth = 809.379f;
                                        wlen = ProtocolStruct.PackageGetSampleEverageResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK, first, second, third, forth);
                                        test_write(inner_buff, wlen);
                                        break;
                                        
                                    case ProtocolStruct.OB_LED_DRIVE:
                                        //int red_cycle = 49;
                                        int red_dac = 405;
                                        //int blue_cycle = 10;
                                        int blue_dac = 193;
                                        int depth = 10;
                                        wlen = ProtocolStruct.PackageGetLedDriveResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK, red_dac, blue_dac, depth);
                                        test_write(inner_buff, wlen);
                                        break;

                                    case ProtocolStruct.OB_TEMPERATURE:
                                        float temp = 27.8f;
                                        wlen = ProtocolStruct.PackageGetTemperatureResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK, temp);
                                        //test_write(inner_buff, wlen);
                                        break;

                                    case ProtocolStruct.OB_TANGENT:
                                        wlen = ProtocolStruct.PackageGetTangentResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK, tangent_value[tangent_index++]);
                                        test_write(inner_buff, wlen);
                                        tangent_index = tangent_index % 4;
                                        break;

                                    case ProtocolStruct.OB_PRESSURE:
                                        float pressure = 102.8f;
                                        wlen = ProtocolStruct.PackageGetPressureResponse(inner_buff, in_command, in_id, in_sn, ProtocolStruct.RESULT_OK, pressure);
                                        test_write(inner_buff, wlen);
                                        break;

                                    case ProtocolStruct.OB_CALI_CO:
                                        float dc0 = 100.3f;
                                        float dc1 = 120.3f;
                                        float dc2 = 104.3f;
                                        float dc3 = 105.5f;
                                        float dc4 = 270.2f;
                                        wlen = ProtocolStruct.PackageGetCaliCoResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK, dc0, dc1, dc2, dc3, dc4);
                                        test_write(inner_buff, wlen);
                                        break;
                                    case ProtocolStruct.OB_TEMP_CO:
                                        float tc0 = 100.3f;
                                        float tc1 = 120.3f;
                                        float tc2 = 104.3f;

                                        wlen = ProtocolStruct.PackageGetTempCoResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK, tc0, tc1, tc2);
                                        test_write(inner_buff, wlen);
                                        break;
                                    case ProtocolStruct.OB_USR_CO:
                                        float uc0 = 100.3f;
                                        float uc1 = 120.3f;

                                        wlen = ProtocolStruct.PackageGetUserCoResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK, uc0, uc1);
                                        test_write(inner_buff, wlen);
                                        break;
                                    case ProtocolStruct.OB_T0:
                                        float t0 = 90.2f;
                                        wlen = ProtocolStruct.PackageGetT0Response(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK, t0);
                                        test_write(inner_buff, wlen);
                                        break;
                                    case ProtocolStruct.OB_DO:
                                        float mydo = do_value[tangent_index++];
                                        tangent_index = tangent_index % 4;
                                        wlen = ProtocolStruct.PackageGetDOResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK, mydo);
                                        test_write(inner_buff, wlen);
                                        break;

                                    case ProtocolStruct.OB_HW_SW:
                                        byte hw_maj = 0x01;
                                        byte hw_min = 0x01;
                                        byte sw_maj = 0x02;
                                        byte sw_min = 0x02;
                                        wlen = ProtocolStruct.PackageGetHWSWResponse(inner_buff, in_command, in_id, in_sn, ProtocolStruct.RESULT_OK, hw_maj, hw_min, sw_maj, sw_min);
                                        test_write(inner_buff, wlen);
                                        break;

                                    case ProtocolStruct.OB_PROBE_NUM:
                                        byte probe_state = 0x8D;
                                        wlen = ProtocolStruct.PackageGetProbeNumResponse(inner_buff, in_command, in_id, in_sn, ProtocolStruct.RESULT_OK, probe_state);
                                        test_write(inner_buff, wlen);
                                        break;

                                    case ProtocolStruct.OB_PROBE_SN:
                                        {
                                            byte probe_index = ProtocolStruct.GetDataProbeState(inner_data);

                                            wlen = ProtocolStruct.PackageGetProbeSNResponse(inner_buff, in_command, in_id, in_sn, ProtocolStruct.RESULT_OK, probe_index, tmp_sn);
                                            test_write(inner_buff, wlen);
                                            break;
                                        }

                                    default:
                                        
                                        break;
                                }
                            }
                            else if (ProtocolStruct.OP_SET == in_command[ProtocolStruct.OP_TYPE])
                            {
                                switch (in_command[ProtocolStruct.OB_TYPE])
                                {
                                    case ProtocolStruct.OB_LED_DRIVE:
                                        //int red_cycle = ProtocolStruct.GetDataRedCycle(inner_data);
                                        int red_dac = ProtocolStruct.GetDataRedDAC(inner_data);
                                        //int blue_cycle = ProtocolStruct.GetDataBlueCycle(inner_data);
                                        int blue_dac = ProtocolStruct.GetDataBlueDAC(inner_data);
                                        int depth = ProtocolStruct.GetDataDepth(inner_data);
                                        wlen = ProtocolStruct.PackageSetLedDriveResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK);
                                        test_write(inner_buff, wlen);
                                        break;

                                    case ProtocolStruct.OB_DETECT:
                                        int ledselected = ProtocolStruct.GetDataLedActiveSelected(inner_data);
                                        float pressure = ProtocolStruct.GetDataLedActivePressure(inner_data);
                                        wlen = ProtocolStruct.PackageSetLedActiveResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK);
                                        test_write(inner_buff, wlen);
                                        break;

                                    case ProtocolStruct.OB_CALI_CO:
                                        float dc0 = ProtocolStruct.GetDataCaliCoL0(inner_data);
                                        float dc1 = ProtocolStruct.GetDataCaliCoL1(inner_data);
                                        float dc2 = ProtocolStruct.GetDataCaliCoL2(inner_data);
                                        float dc3 = ProtocolStruct.GetDataCaliCoL3(inner_data);
                                        float dc4 = ProtocolStruct.GetDataCaliCoL4(inner_data);

                                        wlen = ProtocolStruct.PackageSetCaliCoResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK);
                                        test_write(inner_buff, wlen);
                                        break;
                                    case ProtocolStruct.OB_TEMP_CO:
                                        float tc0 = ProtocolStruct.GetDataTempCoL0(inner_data);
                                        float tc1 = ProtocolStruct.GetDataTempCoL1(inner_data);
                                        float tc2 = ProtocolStruct.GetDataTempCoL2(inner_data);

                                        wlen = ProtocolStruct.PackageSetTempCoResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK);
                                        test_write(inner_buff, wlen);
                                        break;
                                    case ProtocolStruct.OB_USR_CO:
                                        float uc0 = ProtocolStruct.GetDataUserCoL0(inner_data);
                                        float uc1 = ProtocolStruct.GetDataUserCoL1(inner_data);
                                        wlen = ProtocolStruct.PackageSetUserCoResponse(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK);
                                        test_write(inner_buff, wlen);
                                        break;
                                    case ProtocolStruct.OB_T0:
                                        float t0 = ProtocolStruct.GetDataT0(inner_data);
                                        wlen = ProtocolStruct.PackageSetT0Response(inner_buff, in_command, probe_id, tmp_sn, ProtocolStruct.RESULT_OK);
                                        test_write(inner_buff, wlen);
                                        break;
                                    case ProtocolStruct.OB_ID_SN:
                                        
                                        test_id = ProtocolStruct.GetIdInSN(inner_data);
                                        for (int m = 0; m < ProtocolStruct.SN_LEN; m++)
                                        {
                                            test_sn[m] = inner_data[m];
                                        }
                                        wlen = ProtocolStruct.PackageSetIdSNResponse(inner_buff, in_command, in_id, in_sn, ProtocolStruct.RESULT_OK);
                                        test_write(inner_buff, wlen);
                                         /*
                                        wlen = ProtocolStruct.PackageSetIdSNResponse(inner_buff, in_command, in_id, in_sn, ProtocolStruct.RESULT_CHKSUM_ERR);
                                        test_write(inner_buff, wlen);*/
                                        break;

                                    default:
                                        
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void test_write(byte[] buffer, int len)
        {
            //FlowDataReader(null, buffer, len);
            if ((null != testPort) && testPort.IsOpen())
            {
                /* 
                int temp_len = len + 20;
                byte[] temp_buff = new byte[temp_len];
                for (int ti = 0; ti < len; ti++)
                {
                    temp_buff[20 + ti] = buffer[ti];
                }
                //test_write(temp_buff, 20 + wlen);

                byte[] hwbuf = new byte[3];
                for (int i = 0; i < temp_len; )
                {
                    int j;
                    for (j = 0; j < 3; i++, j++)
                    {
                        if (i < temp_len)
                        {
                            hwbuf[j] = buffer[i];
                        }
                        else
                        {
                            break;
                        }
                    }
                    testPort.write(j, hwbuf);
                }
                * */
                testPort.write(len, buffer);
                update_message("TEST SEND :" + FromUnicodeByteArray(buffer, 0, len) + "\n");
            }
            else
            {
                /*  */
                update_message("testPort is not opend! \n");
            }
        }

        void update_message(String message)
        {
            if (textBox1.InvokeRequired)
            {
                update_delegate dd = new update_delegate(update_message);
                textBox1.BeginInvoke(dd, message);
                return;
            }
            textBox1.AppendText(message);
            textBox1.AppendText("\r\n");
        }

        void update_data(String data)
        {
            if (textBox2.InvokeRequired)
            {
                update_delegate dd = new update_delegate(update_data);
                textBox1.BeginInvoke(dd, data);
                return;
            }
            textBox2.AppendText(data);
            textBox2.AppendText("\r\n");
        }

        void update_id_sn(String id, String sn)
        {
            if (txtID.InvokeRequired)
            {
                update_id_sn_delegate dd = new update_id_sn_delegate(update_id_sn);
                txtID.BeginInvoke(dd, id, sn);
                return;
            }
            txtID.Text = id;
            txtSN.Text = sn;
        }

        void update_real_iamge(float real, float image)
        {
            if (txtReal.InvokeRequired)
            {
                update_real_image_delegate dd = new update_real_image_delegate(update_real_iamge);
                txtReal.BeginInvoke(dd, real, image);
                return;
            }
            txtReal.Text = string.Format("{0:f6}", real);
            txtImage.Text = string.Format("{0:f6}", image);
            float scale = float.Parse(txtScale.Text);
            txtAmplitude.Text = string.Format("{0:f6}",Math.Sqrt(real*real+image*image)*scale);
        }

        void update_sample(float first, float second, float third, float forth)
        {
            if (txtFirst.InvokeRequired)
            {
                update_sample_delegate dd = new update_sample_delegate(update_sample);
                txtFirst.BeginInvoke(dd, first, second, third, forth);
                return;
            }
            txtFirst.Text = string.Format("{0:f6}", first);
            txtSecond.Text = string.Format("{0:f6}", second);
            txtThird.Text = string.Format("{0:f6}", third);
            txtForth.Text = string.Format("{0:f6}", forth);
        }

        void update_tangent(float tangent)
        {
            if (txtTangent.InvokeRequired)
            {
                update_tangent_delegate dd = new update_tangent_delegate(update_tangent);
                txtTangent.BeginInvoke(dd, tangent);
                return;
            }
            txtTangent.Text = string.Format("{0:f6}", tangent);
            txtDegree.Text = string.Format("{0:f6}", Math.Atan(tangent)*180/Math.PI);
        }

        void update_temperature(float temperature)
        {
            if (txtTemperature.InvokeRequired)
            {
                update_temperature_delegate dd = new update_temperature_delegate(update_temperature);
                txtTemperature.BeginInvoke(dd, temperature);
                return;
            }
            txtTemperature.Text = string.Format("{0:f6}", temperature);
        }

        void update_pressure(float pressure)
        {
            if (textBox3.InvokeRequired)
            {
                update_pressure_delegate dd = new update_pressure_delegate(update_pressure);
                textBox3.BeginInvoke(dd, pressure);
                return;
            }
            textBox3.Text = string.Format("{0:f6}", pressure);
        }

        private delegate void update_delegate(String message);
        private delegate void update_id_sn_delegate(String id, String sn);
        private delegate void update_real_image_delegate(float real, float image);
        private delegate void update_sample_delegate(float first, float second, float third, float forth);
        private delegate void update_tangent_delegate(float tangent);
        private delegate void update_temperature_delegate(float temperature);
        private delegate void update_pressure_delegate(float temperature);
        private delegate void stop_test_delegate();
        private delegate void update_led_drive_delegate(int red_dac, int blue_dac, int depth);
        private delegate void update_data_delegate(String message);
        private delegate void update_something_delegate(int some, float[] thing);
        private delegate void update_hw_sw_delegate(byte hw_maj, byte hw_min, byte sw_maj, byte sw_min);
        private delegate void update_probe_state_delegate(byte state);
        private delegate void update_probe_index_sn_delegate(byte index, byte[] sn);
        private delegate void update_general_delegate();
        private delegate void update_status_delegate(byte cs, byte ps);

        void update_status(byte cs, byte ps)
        {
            if (labProbeStatus.InvokeRequired)
            {
                update_status_delegate dd = new update_status_delegate(update_status);
                labProbeStatus.BeginInvoke(dd, cs, ps);
                return;
            }
            labConvertorStatus.Text = string.Format("{0:x}", cs);
            labProbeStatus.Text = string.Format("{0:x}", ps);
        }

        void update_probe_index_sn(byte index, byte[] sn)
        {
            if (listBoxDevices.InvokeRequired)
            {
                update_probe_index_sn_delegate dd = new update_probe_index_sn_delegate(update_probe_index_sn);
                listBoxDevices.BeginInvoke(dd, index, sn);
                return;
            }
            devices_id[index + 1] = ProtocolStruct.GetIdInSN(sn);
            char[] char_sn = new char[ProtocolStruct.SN_LEN];
            for (int i = 0; i < ProtocolStruct.SN_LEN; i++)
            {
                devices_sn[index + 1][i] = sn[i];
                char_sn[i] = (char)sn[i];
            }
            String device_info = string.Format("probe #{0:d} ", index + 1);
            device_info += string.Format("{0:d} ", devices_id[index + 1]) + new String(char_sn);
            listBoxDevices.Items.Add(device_info);

            device_index++;
            update_devices();
        }

        void update_probe_state(byte state)
        {
            if (txtState.InvokeRequired)
            {
                update_probe_state_delegate dd = new update_probe_state_delegate(update_probe_state);
                txtState.BeginInvoke(dd, state);
                return;
            }
            txtState.Text = string.Format("{0:d}", state);
            mProbeState = state;
        }

        private byte mProbeState = 0x00;

        void update_something(int some, float[] thing)
        {
            if (txtDOCO0.InvokeRequired)
            {
                update_something_delegate dd = new update_something_delegate(update_something);
                txtDOCO0.BeginInvoke(dd, some, thing);
                return;
            }
            switch (some)
            {
                case 0:
                    txtDOCO0.Text = string.Format("{0:f6}", thing[0]);
                    txtDOCO1.Text = string.Format("{0:f6}", thing[1]);
                    txtDOCO2.Text = string.Format("{0:f6}", thing[2]);
                    txtDOCO3.Text = string.Format("{0:f6}", thing[3]);
                    txtDOCO4.Text = string.Format("{0:f6}", thing[4]);
                    break;
                case 1:
                    txtTECO0.Text = string.Format("{0:f6}", thing[0]);
                    txtTECO1.Text = string.Format("{0:f6}", thing[1]);
                    txtTECO2.Text = string.Format("{0:f6}", thing[2]);
                    break;
                case 2:
                    txtUSCO0.Text = string.Format("{0:f6}", thing[0]);
                    txtUSCO1.Text = string.Format("{0:f6}", thing[1]);
                    break;
                case 3:
                    txtDOv.Text = string.Format("{0:f6}", thing[0]);
                    break;
                case 4:
                    txtT0.Text = string.Format("{0:f6}", thing[0]);
                    break;
            }
        }

        void update_led_drive(int red_dac, int blue_dac, int depth)
        {
            if (txtRedDAC.InvokeRequired)
            {
                update_led_drive_delegate dd = new update_led_drive_delegate(update_led_drive);
                txtRedDAC.BeginInvoke(dd, red_dac, blue_dac, depth);
                return;
            }
            
            txtRedDAC.Text = string.Format("{0:d}", red_dac);
            
            txtBlueDAC.Text = string.Format("{0:d}", blue_dac);
            txtDepth.Text = string.Format("{0:d}", depth);
        }

        void update_hw_sw(byte hw_maj, byte hw_min, byte sw_maj, byte sw_min)
        {
            if (labHW.InvokeRequired)
            {
                update_hw_sw_delegate dd = new update_hw_sw_delegate(update_hw_sw);
                labHW.BeginInvoke(dd, hw_maj, hw_min, sw_maj, sw_min);
                return;
            }

            labHW.Text = "V" + string.Format("{0:x}", hw_maj) + "." + string.Format("{0:x}", hw_min);
            labSW.Text = "V" + string.Format("{0:x}", sw_maj) + "." + string.Format("{0:x}", sw_min);
        }

        private MyPort testPort;

        private void button9_Click(object sender, EventArgs e)
        {
            byte selectData = ("red" == comSample.Text) ? ProtocolStruct.LED_RED : ProtocolStruct.LED_BLUE;

            int len = ProtocolStruct.PackageGetSampleEverage(buffer, command, id, sn, selectData);
            write(buffer, len);
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (null != mPort)
            {
                mPort.Close();
                mPort = null;
            }

            if (null != testPort)
            {
                testPort.Close();
                testPort = null;
            }
            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int len = ProtocolStruct.PackageGetTangent(buffer, command, id, sn);
            write(buffer, len);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Z:
                        button4.PerformClick();
                        break;
                    case Keys.X:
                        button10.PerformClick();
                        break;
                    case Keys.S:
                        button2.PerformClick();
                        break;
                    case Keys.Q:
                        button15.PerformClick();
                        break;
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            byte[] tmp = new byte[6];
            tmp[0] = 2;
            tmp[5] = 3;
            ProtocolStruct.splitFloatToBytes(26f, tmp, 1);
            write(tmp, 6);

            tmp[0] = 2;
            tmp[5] = 3;
            ProtocolStruct.splitFloatToBytes(770f, tmp, 1);
            test_write(tmp, 6);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            int len = ProtocolStruct.PackageGetAllSample(buffer, command, id, sn);
            write(buffer, len);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (is_getting_temperature)
            {
                is_getting_temperature = false;
                button14.Text = "start";
            }
            else
            {
                is_getting_temperature = true;
                button14.Text = "stop";
                int len = ProtocolStruct.PackageGetTemperature(buffer, command, id, sn);
                write(buffer, len);
            }
        }

        private bool is_getting_temperature = false;

        private void button15_Click(object sender, EventArgs e)
        {
            int len = ProtocolStruct.PackageGetTemperature(buffer, command, id, sn);
            write(buffer, len);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            String testData = "正切差\t相位差\t温度";
            update_data(testData);
            totaltimes = int.Parse(txtTimes.Text);
            isRunningTest = true;
            button16.Enabled = false;
            button34.Enabled = false;
            testtype = 1;
            doTest();
        }

        private void handleTest(byte[] data)
        {
            switch (testtype)
            {
                case 1:
                switch (testStep)
                {
                    case 1:
                        temperature = ProtocolStruct.GetDataTemperature(data);
                        
                        break;
                    case 2:
                        /** */
                        float tangent = ProtocolStruct.GetDataTangent(data);
                        double angle = Math.Atan(tangent) * 180 / Math.PI;
                        String testData = string.Format("{0:f6}", tangent);
                        testData += "\t";
                        testData = string.Format("{0:f6}", angle);
                        testData += "\t";
                        testData += string.Format("{0:f6}", temperature);
                        update_data(testData);
                         
                        break;
                }
                    /**/
                testStep++;
                testStep = testStep % 3;
                     
                break;
                case 2:
                switch (testStep)
                {
                    case 1:
                        temperature = ProtocolStruct.GetDataTemperature(data);
                        break;
                    case 2:
                        float calcdo = ProtocolStruct.GetDataDO(data);
                        String testData = string.Format("{0:f6}", calcdo);
                        testData += "\t";
                        testData += string.Format("{0:f6}", temperature);
                        update_data(testData);
                        break;
                }
                testStep++;
                testStep = testStep % 3;
                break;
            }

            

            if (times >= totaltimes)
            {
                stop_test();
            }
            else
            {

                    Thread read_protect_t = new Thread(delegate()
                    {
                        int i = int.Parse(txtInterval1.Text);
                        Thread.Sleep(i);
                        rbuf_len = 0;
                        doTest();
                    });
                    read_protect_t.Start();
                
                
            }
            
        }

        private void doTest()
        {
            int len = 0;

            switch (testtype)
            {
                case 1:
                    switch (testStep)
                    {
                        case 0:
                            len = ProtocolStruct.PackageSetLedActive(buffer, command, id, sn, ProtocolStruct.DETECT_LED_RED_BLUE, 100.35f);
                            break;
                        case 1:
                            len = ProtocolStruct.PackageGetTemperature(buffer, command, id, sn);
                            break;
                        case 2:
                            len = ProtocolStruct.PackageGetTangent(buffer, command, id, sn);
                            times++;
                            break;
                    }
                    break;
                case 2:
                    switch (testStep)
                    {
                        case 0:
                            len = ProtocolStruct.PackageSetLedActive(buffer, command, id, sn, ProtocolStruct.DETECT_LED_RED_BLUE, 100.35f);
                            break;
                        case 1:
                            len = ProtocolStruct.PackageGetTemperature(buffer, command, id, sn);
                            break;
                        case 2:
                            len = ProtocolStruct.PackageGetDO(buffer, command, id, sn, 0x04);
                            times++;
                            break;
                    }
                    break;
            }
            
            write(buffer, len);
            
        }

        public void stop_test()
        {
            if (button16.InvokeRequired)
            {
                stop_test_delegate dd = new stop_test_delegate(stop_test);
                button16.BeginInvoke(dd);
                return;
            }
            button16.Enabled = true;
            button34.Enabled = true;
            totaltimes = 0;
            times = 0;
            temperature = 0f;
            isRunningTest = false;
        }

        private int totaltimes = 0;
        private float temperature = 0f;
        //private float angle = 0f;
        private int testStep = 0;
        private int times = 0;
        private bool isRunningTest = false;

        private void button17_Click(object sender, EventArgs e)
        {
            int wlen = ProtocolStruct.PackageGetLedDrive(buffer, command, id, sn);
            write(buffer, wlen);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void button19_Click(object sender, EventArgs e)
        {
            float dc0 = float.Parse(txtDOCO0.Text);
            float dc1 = float.Parse(txtDOCO1.Text);
            float dc2 = float.Parse(txtDOCO2.Text);
            float dc3 = float.Parse(txtDOCO3.Text);
            float dc4 = float.Parse(txtDOCO4.Text);
            int len = ProtocolStruct.PackageSetCaliCo(buffer, command, id, sn, dc0, dc1, dc2, dc3, dc4);
            write(buffer, len);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            int len = ProtocolStruct.PackageGetCaliCo(buffer, command, id, sn);
            write(buffer, len);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            float uc0 = float.Parse(txtUSCO0.Text);
            float uc1 = float.Parse(txtUSCO1.Text);
            int len = ProtocolStruct.PackageSetUserCo(buffer, command, id, sn, uc0, uc1);
            write(buffer, len);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            int len = ProtocolStruct.PackageGetUserCo(buffer, command, id, sn);
            write(buffer, len);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            float tc0 = float.Parse(txtTECO0.Text);
            float tc1 = float.Parse(txtTECO1.Text);
            float tc2 = float.Parse(txtTECO2.Text);
            int len = ProtocolStruct.PackageSetTempCo(buffer, command, id, sn, tc0, tc1, tc2);
            write(buffer, len);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            int len = ProtocolStruct.PackageGetTempCo(buffer, command, id, sn);
            write(buffer, len);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            byte data = 0x04;
            if (comDos.Text == "uncompensated")
            {
                data = 0x01;
            }
            else if (comDos.Text == "temperature")
            {
                data = 0x02;
            }
            else if (comDos.Text == "pressure")
            {
                data = 0x03;
            }
            
            int len = ProtocolStruct.PackageGetDO(buffer, command, id, sn, data);
            write(buffer, len);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            float t0 = float.Parse(txtT0.Text);
            int len = ProtocolStruct.PackageSetT0(buffer, command, id, sn, t0);
            write(buffer, len);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            int len = ProtocolStruct.PackageGetT0(buffer, command, id, sn);
            write(buffer, len);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            char[] str_sn = txtSN.Text.ToCharArray();
            for (int i = 0; i < ProtocolStruct.SN_LEN; i++)
            {
                the_set_sn[i] = (byte)str_sn[i];
            }

            int len = ProtocolStruct.PackageSetIdSN(buffer, command, id, sn, the_set_sn);
            write(buffer, len);

            for (int i = 0; i < ProtocolStruct.SN_LEN; i++)
            {
                old_sn[i] = (byte)sn[i];
                sn[i] = the_set_sn[i];
            }
            id = ProtocolStruct.GetIdInSN(sn);
        }

        private byte[] old_sn = new byte[ProtocolStruct.SN_LEN];
        private byte[] the_set_sn = new byte[ProtocolStruct.SN_LEN];

        private void button30_Click(object sender, EventArgs e)
        {
            
        }

        private void button31_Click(object sender, EventArgs e)
        {
            id =  ProtocolStruct.DEFAULT_ID;
            String strID = string.Format("{0:D}",id);
            for (int i = 0; i < ProtocolStruct.SN_LEN; i++)
            {
                sn[i] = (byte)ProtocolStruct.DEFAULT_SN[i];
            }
            String strSN = new String(ProtocolStruct.DEFAULT_SN);
            update_id_sn(strID, strSN);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            update_probe_num();
        }



        private void update_probe_num()
        {
            if (listBoxDevices.InvokeRequired)
            {
                update_general_delegate dd = new update_general_delegate(update_probe_num);
                listBoxDevices.BeginInvoke(dd);
                return;
            }

            devices_id[0] = id;
            devices_sn[0] = new byte[ProtocolStruct.SN_LEN];
            listBoxDevices.Items.Clear();
            char[] char_sn = new char[ProtocolStruct.SN_LEN];
            for (int i = 0; i < ProtocolStruct.SN_LEN; i++)
            {
                char_sn[i] = (char)sn[i];
                devices_sn[0][i] = sn[i];
            }
            listBoxDevices.Items.Add(string.Format("smarter {0:d} ", id) + new String(char_sn));

            int len = ProtocolStruct.PackageGetProbeNum(buffer, command, id, sn);
            write(buffer, len);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            device_index = 0;
            update_devices();
        }

        private byte device_index = 0;
        private byte[] devices_id;
        private byte[][] devices_sn;

        private void update_devices()
        {
            if (listBoxDevices.InvokeRequired)
            {
                update_general_delegate dd = new update_general_delegate(update_devices);
                listBoxDevices.BeginInvoke(dd);
                return;
            }

            if (ProtocolStruct.PROBE_NUM <= device_index)
            {
                return;
            }

            
            devices_sn[device_index+1] = new byte[ProtocolStruct.SN_LEN];
            if (0 == (mProbeState & (0x01 << device_index)))
            {
                devices_id[device_index + 1] = 0xff;
                for (int i = 0; i < ProtocolStruct.SN_LEN; i++)
                {
                    devices_sn[device_index + 1][i] = (byte)ProtocolStruct.DEFAULT_SN[i];
                }
                listBoxDevices.Items.Add(string.Format("probe #{0:d} ------------------", device_index+1));
                device_index++;
                update_devices();
            }
            else
            {
                int len = ProtocolStruct.PackageGetProbeSN(buffer, command, id, sn, device_index);
                write(buffer, len);
            }
        }

        private void listBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the currently selected item in the ListBox.
            string curItem = listBoxDevices.SelectedItem.ToString();

            // Find the string in ListBox2.
            int index = listBoxDevices.FindString(curItem);

            id = devices_id[index];
            char[] char_sn = new char[ProtocolStruct.SN_LEN];
            for (int i = 0; i < ProtocolStruct.SN_LEN; i++)
            {
                sn[i] = devices_sn[index][i];
                char_sn[i] = (char)devices_sn[index][i];
            }

            txtID.Text = string.Format("{0:d}", id);
            txtSN.Text = new string(char_sn);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            String testData = "溶氧\t温度";
            update_data(testData);
            totaltimes = int.Parse(txtTimes.Text);
            isRunningTest = true;
            button34.Enabled = false;
            button16.Enabled = false;
            testtype = 2;
            doTest();
        }

        private int testtype = 0;

        private void button35_Click(object sender, EventArgs e)
        {
            int wlen = ProtocolStruct.PackageGetPressure(buffer, command, id, sn);
            write(buffer, wlen);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            byte select = byte.Parse(comCriticalSelected.Text);
            int wlen = ProtocolStruct.PackageGetDebugInfo(buffer, command, id, sn, select);
            write(buffer, wlen);
        }

        private void button37_Click(object sender, EventArgs e)
        {
            byte select = byte.Parse(comCriticalSelected.Text);
            byte[] data = new byte[ProtocolStruct.DEBUG_INFO_BLOCK_SIZE];
            for (int i = 0; i < ProtocolStruct.DEBUG_INFO_BLOCK_SIZE; i++)
            {
                data[i] = 0x00;
            }
            int wlen = ProtocolStruct.PackageSetDebugInfo(buffer, command, id, sn, select, data);
            write(buffer, wlen);
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            float salinity = float.Parse(txtsalinity.Text);
            int wlen = ProtocolStruct.PackageSetSalinity(buffer, command, id, sn, salinity);
            write(buffer, wlen);
        }

    }

    class MyPort
    {
        public MyPort()
        {
        }

        public MyPort(String name, Int32 BaudRate, Int32 databits, Parity parity, StopBits stopbits)
        {
            comm.PortName = name;
            comm.BaudRate = BaudRate;
            comm.DataBits = databits;
            comm.Parity = parity;
            comm.StopBits = stopbits;

            comm.DataReceived += read;
        }

        public bool IsOpen()
        {
            return comm.IsOpen;
        }

        public void Open()
        {
            try
            {
                comm.Open();
            }
            catch (Exception e)
            {
                Report(e.ToString());
            }
        }

        public void write(Int32 data_len, byte[] data)
        {
            try
            {
                comm.Write(data, 0, data_len);
            }
            catch (Exception e)
            {
                Report(e.ToString());
            }
        }

        void read(object sender, SerialDataReceivedEventArgs ev)
        {
            int n = comm.BytesToRead;
            if (0 != n)
            {
                byte[] buf = new byte[n];
                try
                {
                    comm.Read(buf, 0, n);
                }
                catch (Exception e)
                {
                    Report(e.ToString());
                }

                if (null != mReader)
                {
                    mReader(this, buf, n);
                }
            }

        }


        public void Close()
        {
            try
            {
                comm.Close();
            }
            catch (Exception e)
            {
                Report(e.ToString());
            }
        }

        private void Report(string message)
        {
            if (null != mReporter)
            {
                mReporter(this, message);
            }
        }

        private SerialPort comm = new SerialPort();

        public event DataReader mReader;
        public event ErrorReporter mReporter;
    }

    public delegate void ProgressReporter(object sender, int progress);
    public delegate void DataReader(object sender, byte[] data, Int32 data_len);
    public delegate void ErrorReporter(object sender, string error_message);
    public delegate void StatusReporter(object sender, Int32 status);
}
