using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using NppPluginNET;

using System.Runtime.InteropServices;
using System.Text;


namespace MusicPlayer
{
    class Main
    {
        #region " Fields "
        internal const string PluginName = "MusicPlayer";
        static string iniFilePath = null;
        static bool someSetting = false;
        static frmMyDlg frmMyDlg = null;
        static int idMyDlg = -1;
        static Bitmap tbBmp = Properties.Resources.sol;
        static Bitmap tbBmp_tbTab = Properties.Resources.sol;
        static Icon tbIcon = null;
        #endregion

        #region " Opciones "
        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");
            someSetting = (Win32.GetPrivateProfileInt("SomeSection", "SomeKey", 0, iniFilePath) != 0);

            int index = 0;
            PluginBase.SetCommand(index, "Open", myFunctionOpen, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(index++, "---", null, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(index++, "Test", myFunctionAuron, new ShortcutKey(false, false, false, Keys.None));
            //PluginBase.SetCommand(index++, "Test", CommandsubMenuInit, new PluginBase());
            PluginBase.SetCommand(index++, "---", null, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(index++, "Donate", myFunctionDonate, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(index++, "About", myFunctionAbout, new ShortcutKey(false, false, false, Keys.None));
            //PluginBase.SetCommand(index++, "About", myDockableDialog); 
            //Id of function on button
            idMyDlg = 0;
        }
        internal static void SetToolBarIcon()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmp.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMyDlg]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }
        internal static void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("SomeSection", "SomeKey", someSetting ? "1" : "0", iniFilePath);
        }
        #endregion

        #region " Mis funciones "

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);
        internal static void myFunctionOpen()
        {

            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            // Configure open file dialog box
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Sound"; // Default file name
            dlg.DefaultExt = ".wav"; // Default file extension
            dlg.Filter = "Music Files |*.wav;*.mp3; *.wma; *.aiff;"; // Filter files by extension 

            // Show open file dialog box
            DialogResult result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == DialogResult.OK && dlg.FileName.Contains("wav")) 
            {
                // Open document 
                string filename = dlg.FileName;
                player.SoundLocation = filename;
                player.Play();
                //player.PlayLooping();
                //player.SoundLocation = ("C:\\Users\\Certiorem\\Desktop\\sm.wav");
                MessageBox.Show("Playing:" + dlg.FileName);
            }
            else
            {
                try
                {
                    // Open document 
                    string filename = dlg.FileName;
                    //type mpegvideo
                    mciSendString("close MediaFile", null, 0, IntPtr.Zero);
                    mciSendString("open \"" + dlg.FileName + "\" alias MediaFile", null, 0, IntPtr.Zero);
                    mciSendString("play MediaFile", null, 0, IntPtr.Zero);
                    MessageBox.Show("Playing: " + dlg.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error playing sound");
                }
            }

        }
        
        internal static void myFunctionAuron()
        {
            System.Media.SoundPlayer player;
            player = new System.Media.SoundPlayer(Properties.Resources.auronplay);
            player.Play();           
        }

        internal static void myFunctionDonate()
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.paypal.com/es/cgi-bin/webscr?cmd=_flow&SESSION=QgrGEt235P4svsgKmSzucbfROxQJSkwwocLUJRB9TVcoVBo8zd266Ahr3fG&dispatch=5885d80a13c0db1f8e263663d3faee8d99e4111b56ef0eae45e68b8988f5b2dd");
            }
            catch { }

        }

        
        internal static void myFunctionAbout()
        {

            MessageBox.Show("MusicPlayer, Copyright 2015\n" +
                      "Created by gallet ( jongalletero@gmail.com )\n\n" +
                      "Enjoy your music without exit your editor!",
                      "About");

        }
        
        internal static void myDockableDialog()
        {
            /*if (frmMyDlg == null)
            {
                frmMyDlg = new frmMyDlg();

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = frmMyDlg.Handle;
                _nppTbData.pszName = "My dockable dialog";
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, frmMyDlg.Handle);
            }*/
        }
        #endregion
    }
}