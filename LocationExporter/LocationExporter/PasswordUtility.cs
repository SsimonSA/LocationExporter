using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocationExporter
{
    public class PasswordUtility
    {
        public void SavePassword(string password)
        {
            byte[] entropy = Encoding.Unicode.GetBytes(Application.ExecutablePath);
            byte[] plainText = Encoding.Unicode.GetBytes(password);
            byte[] cipherText = ProtectedData.Protect(plainText, entropy, DataProtectionScope.CurrentUser);

            Properties.Settings.Default.Password = Convert.ToBase64String(cipherText);
            Properties.Settings.Default.Save();
        }

        public string GetPassword()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Password))
            {
                byte[] cipherText = Convert.FromBase64String(Properties.Settings.Default.Password);
                byte[] entropy = Encoding.Unicode.GetBytes(Application.ExecutablePath);
                byte[] plainText = ProtectedData.Unprotect(cipherText, entropy, DataProtectionScope.CurrentUser);

                return Encoding.Unicode.GetString(plainText);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
