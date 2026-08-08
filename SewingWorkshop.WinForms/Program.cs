namespace SewingWorkshop.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();
            using var apiClient = new Api.ApiClient();
            Application.Run(new MainForm(apiClient));
        }
    }
}
