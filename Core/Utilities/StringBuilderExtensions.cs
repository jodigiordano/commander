namespace EphemereGames.Core.Utilities
{
    using System.Text;


    public static class StringBuilderExtension
    {
        private static char[] charToInt = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static void Swap(this StringBuilder sb, int startIndex, int endIndex)
        {
            // Swap the integers
            int count = (endIndex - startIndex + 1) / 2;
            for (int i = 0; i < count; ++i)
            {
                char temp = sb[startIndex + i];
                sb[startIndex + i] = sb[endIndex - i];
                sb[endIndex - i] = temp;
            }
        }


        public static void AppendNumber(this StringBuilder sb, int number)
        {
            // Save the current length as starting index
            int startIndex = sb.Length;

            // Handle negative
            bool isNegative;
            uint unumber;
            if (number < 0)
            {
                unumber = (uint) ((number == int.MinValue) ? number : -number);
                isNegative = true;
            }
            else
            {
                unumber = (uint) number;
                isNegative = false;
            }

            // Convert 
            do
            {
                sb.Append(charToInt[unumber % 10]);
                unumber /= 10;
            } while (unumber != 0);

            if (isNegative)
                sb.Append('-');

            sb.Swap(startIndex, sb.Length - 1);
        }


        public static void AppendNumber(this StringBuilder sb, uint unumber)
        {
            // Save the current length as starting index
            int startIndex = sb.Length;

            // Convert 
            do
            {
                sb.Append(charToInt[unumber % 10]);
                unumber /= 10;
            } while (unumber != 0);

            sb.Swap(startIndex, sb.Length - 1);
        }
    }
}
