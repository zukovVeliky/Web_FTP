namespace Identity
{
    public class CodeDecode
    {
        /// <summary>
        /// zakodovani
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            string vystup = "";
            foreach (var arg in plainText.Reverse())
            {
                vystup += arg;
            }
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(vystup);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// dekodovani
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string Base64Decode(string base64EncodedData)
        {
            string vystup = "";
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            foreach (var arg in System.Text.Encoding.UTF8.GetString(base64EncodedBytes).Reverse())
            {
                vystup += arg;
            }

            return vystup;
        }
    }
}
