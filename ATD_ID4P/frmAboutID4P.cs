using System;
using System.Windows.Forms;
using System.Reflection;

namespace ATD_ID4P
{
    public partial class frmAboutID4P : Form
    {
        public frmAboutID4P()
        {
            InitializeComponent();
            // Assembly หมายถึง ไฟล์ทั้งหมดใน Project (ทั้ง โค้ด และ ข้อมูล) ที่ถูก Compile รวมอยู่ในไฟล์เดียว
            // Compile Program ได้ ไฟล์ .exe
            // Compile Program ที่ไม่มี Method main() จะได้ ไฟล์ .dll (ได้เป็น Library)
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.lblProductName.Text = AssemblyProduct;
            this.lblVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.lblCopyright.Text = AssemblyCopyright;
            this.lblCompanyName.Text = AssemblyCompany;
            this.txbDescription.Text = AssemblyDescription;
            this.txbDescription.Text = this.txbDescription.Text + " Application path : " + Application.StartupPath;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                // rightclick Properties > add > assemblyinfofile

                // Assembly is the compiled output of your code, typically a DLL, but your EXE is also an assembly.
                // the smallest unit of deployment for any .NET project
                // Typically contain .NET code in MSIL(Microsoft Intermediate language) that will be compiled to native code the 1st time it is executed on a given machine.
                // That compiled code will also be stored in the assembly and reused on subsequent calls

                // GetExecutingAssembly(): get the assembly that contains the code that is currently executing
                // GetCustomAttributes(type,boolean): get custom attributes for the assembly as specified by type
                //  object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                //  Equal
                //   Assembly currentAssem = Assembly.GetExecutingAssembly();
                //   object[] attributes = currentAssem.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

                // Store AssemblyTitle Attribute in variable attributes
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

                // Check if attributes contains any element
                // Length : total number of element in array
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }


        #endregion

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
