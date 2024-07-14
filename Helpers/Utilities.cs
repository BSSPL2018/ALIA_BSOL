using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BSOL.Helpers
{
    public class Utilities
    {
        #region Cryptography

        public static string Encrypt(string message, string key = Constants.CRYPTO_KEY, bool disableSpecialChars = false)
        {
            try
            {
                byte[] keyArray = null;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(message);

                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice
                hashmd5.Clear();

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
                {
                    //set the secret key for the tripleDES algorithm
                    Key = keyArray,
                    //mode of operation. there are other 4 modes.
                    //We choose ECB(Electronic code Book)
                    Mode = CipherMode.ECB,
                    //padding mode(if any extra byte added)

                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                //transform the specified region of bytes array to resultArray
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                //Release resources held by TripleDes Encryption
                tdes.Clear();
                hashmd5.Dispose();
                tdes.Dispose();
                //Return the encrypted data into unreadable string format
                if (disableSpecialChars)
                    return ConvertBytesToHex(resultArray);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch
            {
                //TODO: log error
                return message;
            }
        }

        public static string Decrypt(string cipherString, string key = Constants.CRYPTO_KEY, bool disableSpecialChars = false)
        {
            try
            {
                byte[] keyArray = null;
                //get the byte code of the string
                byte[] toEncryptArray = disableSpecialChars ? ConvertHexToBytes(cipherString) : Convert.FromBase64String(cipherString);

                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider
                hashmd5.Clear();


                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
                {
                    //set the secret key for the tripleDES algorithm
                    Key = keyArray,
                    //mode of operation. there are other 4 modes. 
                    //We choose ECB(Electronic code Book)

                    Mode = CipherMode.ECB,
                    //padding mode(if any extra byte added)
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                //Release resources held by TripleDes Encryptor                
                tdes.Clear();
                //return the Clear decrypted TEXT
                hashmd5.Dispose();
                tdes.Dispose();
                return Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                //TODO: log error
                return cipherString;
            }
        }

        public static string ConvertBytesToHex(byte[] stringBytes)
        {
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public static byte[] ConvertHexToBytes(string hexInput)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static string EncryptParam(object paramValue, long? employeeId = null)
        {
            return Convert.ToString(paramValue) == "0" ? null : ("P" + Encrypt(Convert.ToString(paramValue), (employeeId.HasValue ? employeeId.Value.ToString() : ApplicationContext.Current.User.FindFirstValue(ClaimType.EmployeeId.ToString())) + Constants.CRYPTO_KEY_FOR_ID, true));
        }

        public static string DecryptParam(object paramValue, long? employeeId = null)
        {
            paramValue = paramValue != null && paramValue.ToString().Length > 1 ? paramValue.ToString().Substring(1, paramValue.ToString().Length - 1) : "";
            return Decrypt(Convert.ToString(paramValue), (employeeId.HasValue ? employeeId.Value.ToString() : ApplicationContext.Current.User.FindFirstValue(ClaimType.EmployeeId.ToString())) + Constants.CRYPTO_KEY_FOR_ID, true);
        }
        public static string FormatBytes(long byteCount)
        {
            string[] suf = { "bytes", "kb", "mb", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0 " + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }
        public static string GetRandomFileName(string directory, string extension)
        {
            string fileName = string.Format("{0}.{1}", Guid.NewGuid().ToString(), extension.Replace(".", ""));
            if (File.Exists(Path.Combine(directory, fileName)))
                return GetRandomFileName(directory, extension);
            else
                return fileName;
        }
        #endregion
        #region Enums

        public static List<T> EnumToList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static List<string> EnumToString<T>() where T : Enum
        {
            return EnumToList<T>().Select(x => x.ToString()).ToList();
        }

        public static List<string> EnumToDescription<T>() where T : Enum
        {
            return EnumToList<T>().Select(x => x.Description()).ToList();
        }
        


        #endregion
    }
}
