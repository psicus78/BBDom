using KNXLib;
using log4net;
using System.Threading;
using System.Threading.Tasks;

namespace BBDom.Biz.DotNet
{
    public class Knx
    {
        private static ILog Logger = LogManager.GetLogger(typeof(Knx));

        static KnxConnection _connection { get; set; }
        public void Test()
        {
            KNXLib.Log.Logger.DebugEventEndpoint += Log;
            KNXLib.Log.Logger.InfoEventEndpoint += Log;
            KNXLib.Log.Logger.WarnEventEndpoint += Log;
            KNXLib.Log.Logger.ErrorEventEndpoint += Log;
            //var connection = new KnxConnectionRouting("192.128.1.100");
            _connection = new KnxConnectionTunneling("192.128.1.100", 3671, "192.128.1.1", 3671);
            _connection.Debug = true;
            
            _connection.KnxConnectedDelegate += Connected;
            _connection.KnxDisconnectedDelegate += Disconnected;
            _connection.KnxStatusDelegate += Status;
            _connection.KnxEventDelegate += Event;
            _connection.Connect();
            
            //Task.Run(() =>
            //{
            //    Thread.Sleep(10000);
            //}).Wait();

            //_connection.Action("0/0/6", false);

            //Task.Run(() =>
            //{
            //    Thread.Sleep(10000);
            //}).Wait();

            //_connection.Action("0/0/6", true);

            //Task.Run(() =>
            //{
            //    Thread.Sleep(10000);
            //}).Wait();

            //_connection.RequestStatus("1.1.50");
            //Task.Run(() =>
            //{

            //    Thread.Sleep(10000);
            //}).Wait();
            //_connection.RequestStatus("1.1.51");
            //Task.Run(() =>
            //{

            //    Thread.Sleep(10000);
            //}).Wait();
            //_connection.RequestStatus("1.1.30");
            //Task.Run(() =>
            //{

            //    Thread.Sleep(10000);
            //}).Wait();
            //_connection.RequestStatus("1.1.31");
            //Task.Run(() =>
            //{

            //    Thread.Sleep(10000);
            //}).Wait();
            //_connection.RequestStatus("1.1.32");
            //Task.Run(() =>
            //{

            //    Thread.Sleep(10000);
            //}).Wait();

            Task.Run(() =>
            {
                var _run = true;
                while (_run)
                {
                    Thread.Sleep(100);
                }
            }).Wait();

            _connection.Disconnect();

            Task.Run(() =>
            {

                Thread.Sleep(10000);
            }).Wait();

            //connection.KnxEventDelegate += Event;
            //connection.Action("1/1/50", false);
            //Task.Delay(5000).Wait();
            //connection.Action("1/1/50", true);
            //Task.Delay(5000).Wait();

        }

        void Log(string id, string message)
        {
            Logger.InfoFormat("{0}:{1}", id, message);
        }

        void Connected()
        {
            Logger.Info("Connected");
        }
        void Disconnected()
        {
            Logger.Info("Disconnected");
        }

        void Status(string address, string state)
        {
            Logger.InfoFormat("New Event: device {0} has state {1}", address, state);
            if (address == "1/1/50")
            {
                decimal temp = (decimal)_connection.FromDataPoint("9.001", state);
                Logger.InfoFormat("New Event: device " + address + " has status " + temp);
                return;
            }
            if (address == "1/1/17")
            {
                int perc = (int)_connection.FromDataPoint("5.001", state);
                Logger.InfoFormat("New Event: device " + address + " has status " + perc);
                return;
            }
        }
        
        void Event(string address, string state)
        {
            Logger.InfoFormat("New Event: device {0} has status {1}", address, state);
        }

    }
}
