namespace UHtml.Tests.WPF.Utility
{
    public static class TestFiles
    {
        public static string GetTestFile(string fileName, string fileNumber)
        {
            return string.Format(@"{0}\{0}_{1}\{0}_{1}", fileName, fileNumber);
        }
    }
}
