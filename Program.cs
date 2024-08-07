using TrafficMonitor.MonitorApp;

namespace TrafficMonitor;

class Program
{
    static void Main(string[] args)
    {
        try 
        {
            NetworkMonitor networkMonitor = new NetworkMonitor();
            networkMonitor.InitializeDevice();
            networkMonitor.StartCapture();

            Console.WriteLine("Capturing traffic. Press enter to cancel..");
            Console.ReadLine();
        } 
        catch (Exception ex)
        {
            Console.WriteLine("Error: ", ex.Message);
        }
        
    }
}
