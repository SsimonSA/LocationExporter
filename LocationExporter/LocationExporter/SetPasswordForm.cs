using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocationExporter
{
    public partial class PasswordInputForm : Form
    {
        public PasswordInputForm()
        {
            InitializeComponent();
        }

        private void PasswordInputForm_Load(object sender, EventArgs e)
        {
            PasswordUtility passwordUtility = new PasswordUtility();
            string password = passwordUtility.GetPassword();
            txtPassword.Text = password;
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                PasswordUtility passwordUtility = new PasswordUtility();
                passwordUtility.SavePassword(txtPassword.Text);

                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a password.");
            }
        }

        private void button_Quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
