using KNXLib;
using System.Threading;
using System.Threading.Tasks;

namespace BBDom.Biz.DotNetStandard
{
    public class Knx
    {

        KnxConnectionTunneling _connection { get; set; }

        public void Connect()
        {
            _connection = new KnxConnectionTunneling("192.168.1.100", 3671, "192.168.1.101", 3671);
            _connection.Debug = true;
            _connection.KnxConnectedDelegate += Connected;
            _connection.KnxDisconnectedDelegate += Disconnected;
            _connection.KnxStatusDelegate += Status;
            _connection.KnxEventDelegate += Event;

            _connection.Connect();

            Task.Run(() =>
            {
                Thread.Sleep(5000);
            }).Wait();
;
            var _run = true;
            while (_run)
            {
                _connection.RequestStatus("2/1/1");
                Task.Run(() =>
                {
                    Thread.Sleep(5000);
                }).Wait();
                _connection.RequestStatus("2/1/2");
                Task.Run(() =>
                {
                    Thread.Sleep(5000);
                }).Wait();
            }


            #region Disconnessione
            _connection.Disconnect();
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();
            #endregion
        }
        void Status(string address, string state)
        {
            //Logger.InfoFormat("New Status: device {0} has state {1}", address, state);
            if (address == "2/1/1")
            {
                decimal temp = (decimal)_connection.FromDataPoint("9.001", state);
            }
            if (address == "2/1/2")
            {
                decimal temp = (decimal)_connection.FromDataPoint("9.001", state);
            }
            if (address == "1/1/50")
            {
                decimal temp = (decimal)_connection.FromDataPoint("9.001", state);
                //Logger.InfoFormat("New Event: device " + address + " has status " + temp);
                return;
            }
            if (address == "1/1/17")
            {
                int perc = (int)_connection.FromDataPoint("5.001", state);
                //Logger.InfoFormat("New Event: device " + address + " has status " + perc);
                return;
            }
        }

        void Event(string address, string state)
        {
            //Logger.InfoFormat("New Event: device {0} has status {1}", address, state);
        }
        void Connected()
        {
            //Logger.Info("Connected");
        }
        void Disconnected()
        {
            //Logger.Info("Disconnected");
        }

    }
}
