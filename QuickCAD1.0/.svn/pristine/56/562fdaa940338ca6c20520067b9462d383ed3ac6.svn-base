using Max.BaseKit.Customs;

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Test1();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            System.Windows.Forms.Application.Run(new MainForm());
        }


        private static void Test1()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {

                    Thread.Sleep(5 * 1000);
                }
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(() =>
            {
                while (true)
                {

                    Thread.Sleep(5 * 1000);
                }
            }, TaskCreationOptions.LongRunning);


            //List<string> bag = new List<string>();
            //bag.Add("a");
            //bag.Add("b");
            //bag.Add("c");
            //var obj1 = bag.OrderBy(p => p).ToList().FirstOrDefault();
            //bag.Remove(obj1);

            CustomConcurrentQueue<string> queue = new CustomConcurrentQueue<string>();
            queue.OnItemAdded += (p1, p2) =>
            {

            };
            queue.OnItemRemoved += (p1, p2) =>
            {

            };
            queue.Enqueue("c");
            queue.Enqueue("b");
            queue.Enqueue("a");
            //var queue1 = queue.OrderBy(p => p).ToList();
            //queue = (ConcurrentQueue<string>)queue.OrderBy(p => p);
            queue.TryDequeue(out var lll);
        }
    }
}