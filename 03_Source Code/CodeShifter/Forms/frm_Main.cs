using CodeShift;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

/****************************************************************         
    *                                                           *   
    * Author:  J.Anish Dev                                      *   
    *                                                           *   
    * About :  Solution to NSA's CodeBreaker Challenge 2016     *   
    *          I'd love to hear from you if: You just want to   * 
    *          say hi, if you found this useful, if you         *
    *          use my code, if you solved task 6, or if         * 
    *          you want to team up for 2017's challenge ;)      *    
    *                                                           *
    * Contact me:                                               *
    *          www.dreamersion.com                              *
    *          anishdev@ufl.edu                                 *   
    ************************************************************/
    /*The code (at places) might not be too "clean", I'm sorry for that.
    After all, I made this program just to "get things done",
    and not for demonstration of efficient coding ;)
    BE SURE TO COPY THE BINARY/KEY FILES BEFORE YOU RUN THIS!!!*/

namespace CodeShifter
{

    public partial class frm_Main : Form
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        c_ParellelBrute tBruteForcer = new c_ParellelBrute();
        c_OTP_Generator tOTPgen = new c_OTP_Generator();

        Thread thread;
        public frm_Main()
        {
            InitializeComponent();
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            byte[] result;

            result= BitConverter.GetBytes(2057307114);
            c_ParellelBrute.deviceSerial = result;

            result = System.Text.Encoding.ASCII.GetBytes("5GAQU6KI54PHWWCDXWHIMLIK456AWFZ3UH4CYZJPLSRK2ULBE4WQ");
            c_ParellelBrute.TargetSecretKey = result;
        }

        public void updateProgress(string parr_Status)
        {
            st_Lbl.Text = parr_Status;
            if (st_Lbl.Text.StartsWith("Master file written. Password cracked:") == true)
            { st_Lbl.ForeColor = Color.Red; }
        }

        private void tmr_Status_Tick(object sender, EventArgs e)
        {
            updateProgress(tBruteForcer.currentDate);
        }

        private void cmd_Generate_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txt_OTP_S.Text))
            {
                MessageBox.Show(this, "Master Key file missing", "Error");
                return;
            }

            int[] OTP_N = new int[3];

            tOTPgen.CalculateOtpValues(txt_OTP_S.Text, int.Parse(txt_OTP_S.Text), ref OTP_N);

            txt_OTP1.Text = OTP_N[0].ToString();
            txt_OTP2.Text = OTP_N[1].ToString();
            txt_OTP3.Text = OTP_N[2].ToString();

        }

        private void cmd_Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txt_OTP3.Text);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cmd_Exe_Click(object sender, EventArgs e)
        {
            if (!File.Exists("hwsim.exe") || !File.Exists("server.exe") || !File.Exists("client.exe"))
            {
                string Present_hwsim = "";
                string Present_server = "";
                string Present_client = "";

                Present_hwsim = "Hwsim present: " + File.Exists("hwsim.exe");
                Present_server = "Server present: " + File.Exists("server.exe");
                Present_client = "Client key present: " + File.Exists("client.exe");

                MessageBox.Show(this, Present_hwsim + "\n" + Present_server + "\n" + Present_client, "Required file(s) missing");
                return;
            }


            if (!((chk_1.Checked) == true && (chk_2.Checked) == true && (chk_3.Checked == true)))
            {
                if (chk_1.Checked == true)
                {
                    Process procT = new Process();
                    procT.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath)
                                           + @"\hwsim.exe";
                    procT.StartInfo.UseShellExecute = true;
                    procT.Start();
                }

                if (chk_2.Checked == true)
                {
                    Process procT2 = new Process();
                    procT2.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath)
                                           + @"\server.exe";
                    procT2.StartInfo.Arguments = "--key "+ txt_AUTH_ENC.Text;
                    procT2.StartInfo.UseShellExecute = true;
                    procT2.StartInfo.RedirectStandardInput = false;
                    procT2.Start();
                }

                System.Threading.Thread.Sleep(500);

                if (chk_3.Checked == true)
                {
                    Process procT3 = new Process();
                    procT3.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath)
                                           + @"\client";
                    procT3.StartInfo.UseShellExecute = true;
                  
                    if (chk_authenticate.Checked)
                    {
                        cmd_Generate_Click(this, e);

                        procT3.StartInfo.Arguments = "--otp " + txt_OTP3.Text;

                        txt_OTP1.Text = "-";
                        txt_OTP2.Text = "-";
                        txt_OTP3.Text = "-";
                    }

                    procT3.Start();
                    System.Threading.Thread.Sleep(500);
                }

                return;
            }
        
            cmd_Generate_Click(this, e);
            int pauseTime = 300;

            Process proc = new Process();
            proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath)
                                   + @"\hwsim.exe";
            proc.StartInfo.UseShellExecute = true;
            proc.Start();

            System.Threading.Thread.Sleep(pauseTime);


            Process proc2 = new Process();
            proc2.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath)
                                   + @"\server.exe";
            proc2.StartInfo.Arguments = "--key " + txt_AUTH_ENC.Text;
            proc2.StartInfo.UseShellExecute = true;
            proc2.StartInfo.RedirectStandardInput = false;
            proc2.Start();

            System.Threading.Thread.Sleep(pauseTime);

            Process proc3 = new Process();
            proc3.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath)
                                   + @"\client";
            proc3.StartInfo.UseShellExecute = true;

            if (chk_authenticate.Checked)
            {
                cmd_Generate_Click(this, e);

                proc3.StartInfo.Arguments = "--otp " + txt_OTP3.Text;

                txt_OTP1.Text = "-";
                txt_OTP2.Text = "-";
                txt_OTP3.Text = "-";
            }

            proc3.Start();

        }
        void cleanOlder()
        {
            foreach (var process in Process.GetProcessesByName("client"))
            {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("server"))
            {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("hwsim"))
            {
                process.Kill();
            }
        }

        public class ProcessHelper
        {
            public static void SetFocusToExternalApp(string strProcessName)
            {
                Process[] arrProcesses = Process.GetProcessesByName(strProcessName);
                if (arrProcesses.Length > 0)
                {

                    IntPtr ipHwnd = arrProcesses[0].MainWindowHandle;
                    Thread.Sleep(100);
                    SetForegroundWindow(ipHwnd);

                }
            }

        }

        private void cmd_reset_Click(object sender, EventArgs e)
        {
            cleanOlder();

            cmd_Exe_Click(this, e);
        }

        private void cmd_Close_Click(object sender, EventArgs e)
        {
            cleanOlder();
        }

        private void cmd_SCRIPT_Execute(object sender, EventArgs e)
        {
            bool isRunning = false;
            foreach (var process in Process.GetProcessesByName("client"))
            {
                isRunning = true;
            }

            if (!isRunning) return;

            ProcessHelper.SetFocusToExternalApp("client");

            var textLines = txt_Script.Text.Split('\n');
            foreach (var line in textLines)
            {
                SendKeys.Send(line);
                SendKeys.Send("~");
            }
        }

        private void cmd_Start_Click(object sender, EventArgs e)
        {
            thread = new Thread(() => tBruteForcer.doBruteParallel(dt_P.Value.ToUniversalTime()));
            c_ParellelBrute.deviceSerialNumber = txt_MKC_DeviceTarget.Text;
            thread.Start();
            tmr_Status.Enabled = true;
            st_Lbl.Text = "";
            cmd_Start.Enabled = false;
            cmd_Stop.Enabled = true;
        }

        private void cmd_Stop_Click(object sender, EventArgs e)
        {
            tmr_Status.Enabled = false;
            thread.Abort();
            if (st_Lbl.Text != "")
                st_Lbl.Text = "Aborted. Last trial: " + st_Lbl.Text.Remove(0, 9);
            else
                st_Lbl.Text = "Ready";
            cmd_Stop.Enabled = false;
            cmd_Start.Enabled = true;
        }

        private void cm_Alter_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Alter serial", "Enter new Device Serial", "2057307114");
            int serial;

            if (input=="") return;

            if (!Int32.TryParse(input.Trim(), out serial))
            {
                MessageBox.Show(this, "Invalid entry", "Error");
                return;
            }
 
            byte[] result = BitConverter.GetBytes(serial);

            string hexValue = BitConverter.ToString(result).Replace("-", ", 0x");
            hexValue = "0x" + hexValue;

            txt_MKC_DeviceTarget .Text = input;
            txt_MKC_DeviceTargetHex.Text = hexValue;

            c_ParellelBrute.deviceSerial = result;
        }

        private void txt_MKC_SecretKey_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmd_Alter_SecretKey_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Alter Secret Key", "Enter new Secret", "5GAQU6KI54PHWWCDXWHIMLIK456AWFZ3UH4CYZJPLSRK2ULBE4WQ");

            if (input == "") return;
            Regex rGX_AlphaNumeric = new Regex("^[a-zA-Z0-9]*$");
            if (!rGX_AlphaNumeric.IsMatch(input))
            {
                MessageBox.Show(this, "Invalid entry", "Error");
                return;
            }

            byte[] result = System.Text.Encoding.ASCII.GetBytes(input);

            string hexValue = BitConverter.ToString(result).Replace("-", ", 0x");
            hexValue = "0x" + hexValue;

            txt_MKC_SecretKey.Text = input;
            txt_MKC_SecretKeyHex.Text = hexValue;

            c_ParellelBrute.TargetSecretKey = result;

        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private void cmd_Alter_OTP_Target_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Alter OTP Target", "Enter new Target", "2057307114");
            int serial;

            if (input == "") return;

            if (!Int32.TryParse(input.Trim(), out serial))
            {
                MessageBox.Show(this,"Invalid entry","Error");
                return;
            }

            if (!File.Exists(input))
            {
                MessageBox.Show(this, "Corresponding Master Key file missing!  \n\nPlace Master Key file in Application Directory (File named as the Device Serial number with no extension)", "Error");
                return;
            }

            txt_OTP_S.Text = serial.ToString();

            BinaryReader reader = new BinaryReader(new FileStream(serial.ToString(), FileMode.Open, FileAccess.Read, FileShare.None));
            reader.BaseStream.Position = 0x0;     
            byte[] data = reader.ReadBytes(32); 
            reader.Close();

            string hexValue = BitConverter.ToString(data).Replace("-", ", 0x");
            hexValue = "0x" + hexValue;

            txt_MasterKeyHex.Text = (hexValue); 
        }

        private void cmd_MKG_MK_Click(object sender, EventArgs e)
        {
            string path;
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
                path = file.FileName;
            else
                return;

            if ((new FileInfo(path).Length)!=32)
            {
                MessageBox.Show(this, "Master Key file has to be exactly 32 bytes in size. The one you chose isn't", "Error");
                return;
            }

            txt_MKG_MK.Text = System.IO.Path.GetFileNameWithoutExtension(path);
        }



        private void cmd_MKG_PRPEM_Click(object sender, EventArgs e)
        {
            string path;
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
                path = file.FileName;
            else
                return;

            txt_MKG_PRV.Text = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        private void cmd_MKG_PBPEM_Click(object sender, EventArgs e)
        {
            string path;
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
                path = file.FileName;
            else
                return;

            txt_MKG_PBK.Text = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        private void cmd_MKG_DVS_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Alter Key Target", "Enter new Key", "2057307114");
            int serial;

            if (input == "") return;

            if (!Int32.TryParse(input.Trim(), out serial))
            {
                MessageBox.Show(this, "Invalid entry", "Error");
                return;
            }

            txt_MKG_DVS.Text = input.Trim();

        }

        private void cmd_MKG_Generate_Click(object sender, EventArgs e)
        {
            if (!File.Exists("keygen.exe"))
            {
                MessageBox.Show(this, "Binary: keygen.exe missing! Place the file in the application startup path", "Required Binary missing");
                return;
            }

            if (!File.Exists("openssl.exe"))
            {
                MessageBox.Show(this, "Binary: Openssl.exe missing! Place the file in the application startup path", "Required Binary missing");
                return;
            }

            if (!File.Exists(txt_MKG_PRV.Text) || !File.Exists(txt_MKG_PBK.Text) || !File.Exists(txt_MKG_MK.Text) )
            {
                string Present_Privatekey="";
                string Present_Pubkey = "";
                string Present_Masterkey = "";

                Present_Privatekey = "Private key present: " + File.Exists(txt_MKG_PRV.Text).ToString();
                Present_Pubkey = "Public key present: " + File.Exists(txt_MKG_PBK.Text).ToString();
                Present_Masterkey = "Master key present: " + File.Exists(txt_MKG_MK.Text).ToString();

                MessageBox.Show(this, Present_Privatekey + "\n" + Present_Pubkey + "\n" + Present_Masterkey, "Required key(s) missing");
                return;
            }

            string keygenParameters;
            keygenParameters = " -m " + txt_MKG_MK.Text + " -k " + txt_MKG_DVS.Text;
            string openSSLParameters;
            openSSLParameters= "rsautl -encrypt -inkey " + txt_MKG_PBK.Text+ "  -pubin -in " + txt_MKG_DVS.Text + ".key " + "-out " + txt_MKG_DVS.Text + ".key.enc";

            System.Diagnostics.Process pProcessKG = new System.Diagnostics.Process();
            pProcessKG.StartInfo.FileName = @"keygen.exe";
            pProcessKG.StartInfo.Arguments = keygenParameters;
            pProcessKG.StartInfo.UseShellExecute = false;
            pProcessKG.StartInfo.RedirectStandardOutput = true;
            pProcessKG.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcessKG.StartInfo.CreateNoWindow = true;
            pProcessKG.Start();
            pProcessKG.WaitForExit();

            System.Diagnostics.Process pProcessOP = new System.Diagnostics.Process();
            pProcessOP.StartInfo.FileName = @"openssl.exe";
            pProcessOP.StartInfo.Arguments = openSSLParameters;
            pProcessOP.StartInfo.UseShellExecute = false;
            pProcessOP.StartInfo.RedirectStandardOutput = true;
            pProcessOP.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcessOP.StartInfo.CreateNoWindow = true;
            pProcessOP.Start();
            pProcessOP.WaitForExit();

            MessageBox.Show("Key successfully generated and encrpted (for Server), using specified master key.");

        }

        private void cmd_MKG_DECRYPT_Click(object sender, EventArgs e)
        {
            if (!File.Exists("openssl.exe"))
            {
                MessageBox.Show(this, "Binary: Openssl.exe missing! Place the file in the application startup path", "Required Binary missing");
                return;
            }


            if (!File.Exists(txt_MKG_PRV.Text))
            {
                string Present_Privatekey = "Private key present: " + File.Exists(txt_MKG_PRV.Text).ToString();
                MessageBox.Show(this, Present_Privatekey, "Required key(s) missing");
                return;
            }

            string path;
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
                path = file.FileName;
            else
                return;

            string fileToDecrypt= System.IO.Path.GetFileName(path);

            string openSSLParameters;
            openSSLParameters = "rsautl -decrypt -inkey " + txt_MKG_PRV.Text + " -in " + fileToDecrypt + " -out " + System.IO.Path.GetFileNameWithoutExtension(path).Replace(" ", "") + ".key";

            System.Diagnostics.Process pProcessOP = new System.Diagnostics.Process();
            pProcessOP.StartInfo.FileName = @"openssl.exe";
            pProcessOP.StartInfo.Arguments = openSSLParameters;
            pProcessOP.StartInfo.UseShellExecute = false;
            pProcessOP.StartInfo.RedirectStandardOutput = true;
            pProcessOP.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcessOP.StartInfo.CreateNoWindow = true;
            pProcessOP.Start();
            pProcessOP.WaitForExit();

            MessageBox.Show("Key successfully decrypted");
            Clipboard.SetText(openSSLParameters);
        }

        private void cmd_AUTH_ALTER_Click(object sender, EventArgs e)
        {
            string path;
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
                path = file.FileName;
            else
                return;

            txt_AUTH_ENC.Text = System.IO.Path.GetFileName(path);
        }

        private void cmd_MKC_Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To find the master key, you'll have to enter the device serial number that you were given in task 3. You'll then have to enter the secret key from its decrypted contents (You can decrypt .key.enc files using the 'Key Maker' Tab. \n\nOnce you're ready, choose a relevant start date to begin brute forcing (To be safe, choose the date after you finished task 2)", "Help");
        }

        private void cmd_OTP_Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To generate OTPs, you'll need the master key that was used to create the .key file given to you in task 3 (Obtain this using the 'Master-Key cracker' tab)", "Help");
        }

        private void cmd_MKG_Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To be able to generate a valid .key.enc file, you'll need to provide the master key used and the target device serial.\n\nYou can decrypt a .key.enc file, too. But, both these operations require you to extract/specify the private and public key from the assembly (server.exe). I don't know if they used the same private/public key for everyone's binaries, so in case they did, you can just use the keys I extracted from my assembly", "Help");
        }

        private void cmd_Important_Click(object sender, EventArgs e)
        {
            {
                MessageBox.Show("THIS WILL NOT FIND THE SOLUTION IF COMPILED/RUN IN 64 BITS. COMPILE AND RUN X86 ONLY! ", "IMPORTANT");
            }

        }
    }
}