using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SPA
{
    public class Password
    {
        public int id { get; set; }
        public string owner { get; set; }
        public string url { get; set; }
        public string useremail { get; set; }
        public string password { get; set; }
    }

    internal class User
    {
        public static string username { get; set; }
        public static string masterkey { get; set; }
    }

    internal class Encryption
    {
        public static string Encrypt(string input, string key)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] encryptedBytes = new byte[inputBytes.Length];
            for (int i = 0; i < inputBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string encryptedBase64, string key)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedBase64);
            string encryptedText = Encoding.UTF8.GetString(encryptedData);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] decryptedBytes = new byte[encryptedText.Length];
            for (int i = 0; i < encryptedText.Length; i++)
            {
                decryptedBytes[i] = (byte)(encryptedText[i] ^ keyBytes[i % keyBytes.Length]);
            }
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            decryptedText = Regex.Replace(decryptedText, @"\x00", "");
            return decryptedText;
        }

        public static string StringToBase64(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            string base64String = Convert.ToBase64String(bytes);
            return base64String;
        }

        public static string Base64ToString(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            string decodedString = Encoding.UTF8.GetString(bytes);
            return decodedString;
        }
    }

    internal class Security
    {
        private static string mainkey = $"845a443a87bbefae39693724eb82c0d6-{getUnix()}";
        private const string host = "http://localhost:4444";
        public static string error_reason { get; set; }

        private static string getUnix()
        {
            long unixTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int unixHour = (int)(unixTimestamp / 3600) % 24;
            return unixHour.ToString("D2");
        }

        public static bool Register(string username, string password, string masterkey)
        {
            string url = $"{host}/register";
            string jsonContent = Encryption.Encrypt("{\"username\":\"" + username + "\",\"password\":\"" + password + "\", \"key\":\"" + masterkey + "\"}", mainkey);

            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    User.username = username;
                    User.masterkey = masterkey;
                    return true;
                }
                else
                {
                    error_reason = Encryption.Decrypt(response.Content.ReadAsStringAsync().Result, mainkey);
                    if (Security.error_reason != null)
                    {
                        switch (Security.error_reason)
                        {
                            case "INCORRECT_PASS":
                                MessageBox.Show("Error: Incorrect Password!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            case "USERNAME_NOT_FOUND":
                                MessageBox.Show("Error: User not found!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            case "USER_ALREADY_EXIST":
                                MessageBox.Show("Error: User already exists!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            default:
                                MessageBox.Show("Error: " + Security.error_reason, "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: unknown", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }
            }
        }

        public static bool Login(string username, string password, string masterkey)
        {
            string url = $"{host}/login";
            string jsonContent = Encryption.Encrypt("{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}", mainkey);

            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = Encryption.Decrypt(Encryption.Decrypt(response.Content.ReadAsStringAsync().Result, mainkey), Encryption.StringToBase64(Encryption.StringToBase64(Encryption.StringToBase64(masterkey))));
                    User.username = username;
                    User.masterkey = masterkey;
                    return true;
                }
                else
                {
                    error_reason = Encryption.Decrypt(response.Content.ReadAsStringAsync().Result, mainkey);
                    if (Security.error_reason != null)
                    {
                        switch (Security.error_reason)
                        {
                            case "INCORRECT_PASS":
                                MessageBox.Show("Error: Incorrect Password!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            case "USERNAME_NOT_FOUND":
                                MessageBox.Show("Error: User not found!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            case "USER_ALREADY_EXIST":
                                MessageBox.Show("Error: User already exists!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            default:
                                MessageBox.Show("Error: " + Security.error_reason, "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: unknown", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }
            }
        }

        public static string GetPassVal(string password)
        {
            try
            {
                return Encryption.Decrypt(password, Encryption.StringToBase64(Encryption.StringToBase64(Encryption.StringToBase64(User.masterkey))));
            }
            catch
            {
                MessageBox.Show("Could not decrypt passwords! Make sure your Master Key is correct!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "ERROR";
            }
        }

        public static bool AddPass(string username, string url2, string useremail, string password)
        {
            string url = $"{host}/api/add";
            string jsonContent = Encryption.Encrypt("{\"username\":\"" + username + "\", \"url\":\"" + Encryption.Encrypt(url2, Encryption.StringToBase64(Encryption.StringToBase64(Encryption.StringToBase64(User.masterkey)))) + "\", \"useremail\":\"" + Encryption.Encrypt(useremail, Encryption.StringToBase64(Encryption.StringToBase64(Encryption.StringToBase64(User.masterkey)))) + "\",\"password\":\"" + Encryption.Encrypt(password, Encryption.StringToBase64(Encryption.StringToBase64(Encryption.StringToBase64(User.masterkey)))) + "\"}", mainkey);

            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = Encryption.Decrypt(Encryption.Decrypt(response.Content.ReadAsStringAsync().Result, mainkey), Encryption.StringToBase64(Encryption.StringToBase64(Encryption.StringToBase64(User.masterkey))));
                    if (responseBody == "PASSWORD_ADD_SUCCESS")
                    {
                        return true;
                    }
                    return true;
                }
                else
                {
                    error_reason = Encryption.Decrypt(response.Content.ReadAsStringAsync().Result, mainkey);
                    if (Security.error_reason != null)
                    {
                        switch (Security.error_reason)
                        {
                            case "INCORRECT_PASS":
                                MessageBox.Show("Error: Incorrect Password!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            case "USERNAME_NOT_FOUND":
                                MessageBox.Show("Error: User not found!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            case "USER_ALREADY_EXIST":
                                MessageBox.Show("Error: User already exists!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            default:
                                MessageBox.Show("Error: " + Security.error_reason, "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: unknown", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }
            }
        }

        public static List<Password> Passwords(string username)
        {
            string url = $"{host}/api/get";
            string jsonContent = Encryption.Encrypt("{\"username\":\"" + username + "\"}", mainkey);

            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = Encryption.Decrypt(response.Content.ReadAsStringAsync().Result, mainkey);
                    if (responseBody == "NO_PASSWORDS") { return null; }
                    return JsonConvert.DeserializeObject<List<Password>>(responseBody);
                }
                else
                {
                    error_reason = Encryption.Decrypt(response.Content.ReadAsStringAsync().Result, mainkey);
                    if (Security.error_reason != null)
                    {
                        switch (Security.error_reason)
                        {
                            case "INCORRECT_PASS":
                                MessageBox.Show("Error: Incorrect Password!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            case "USERNAME_NOT_FOUND":
                                MessageBox.Show("Error: User not found!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            case "USER_ALREADY_EXIST":
                                MessageBox.Show("Error: User already exists!", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            default:
                                MessageBox.Show("Error: " + Security.error_reason, "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: unknown", "SPA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return null;
                }
            }
        }

    }
}
