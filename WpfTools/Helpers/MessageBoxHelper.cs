using System.Windows;

namespace WpfTools.Helpers
{
    public static class MessageBoxHelper
    {
        //dissabled per default delivers default Result
        public static bool Enabled { get; set; } = false;



        public static string Text { get; set; } = "MyText";
        public static string Title { get; set; } = "MyTitle";
        public static MessageBoxButton Button { get; set; } = MessageBoxButton.OK;
        public static MessageBoxImage Image { get; set; } = MessageBoxImage.Information;
        public static MessageBoxResult Result { get; set; } = MessageBoxResult.OK;

        public static MessageBoxResult ShowMessageBox()
        {
            if (Enabled)
                Result = MessageBox.Show(Text, Title, Button, Image, Result);
            return Result;
        }

    }
}
