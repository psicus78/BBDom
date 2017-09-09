using KNXLib;
using log4net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BBDom.Test.ConsoleApp
{
    class Program
    {
        private static bool APRI = false;
        private static bool CHIUDI = true;

        private static ILog Logger = LogManager.GetLogger(typeof(Program));

        static KnxConnection _connection { get; set; }
        static bool _connected = false;


        static void Main(string[] args)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
            }
            catch (Exception) { }

            KNXLib.Log.Logger.DebugEventEndpoint += Log;
            KNXLib.Log.Logger.InfoEventEndpoint += Log;
            KNXLib.Log.Logger.WarnEventEndpoint += Log;
            KNXLib.Log.Logger.ErrorEventEndpoint += Log;

            _connection = new KnxConnectionTunneling("192.168.1.100", 3671, "192.168.1.101", 3671);
            _connection.Debug = true;

            _connection.KnxDisconnectedDelegate += Disconnected;
            _connection.KnxConnectedDelegate += Connected;
            _connection.KnxStatusDelegate += Status;
            _connection.KnxEventDelegate += Event;

                        
            #region Never ending loop
            Task.Run(() =>
            {
                var _run = true;
                while (_run)
                {
                    if (!_connected)
                    {
                        _connection.Connect();
                        Task.Run(() =>
                        {
                            Thread.Sleep(5000);
                        }).Wait();
                        //_connection.Action("0/0/1", true);
                        //Task.Run(() =>
                        //{
                        //    Thread.Sleep(2000);
                        //}).Wait();

                        _connection.RequestStatus("2/1/1");
                        Task.Run(() =>
                        {
                            Thread.Sleep(1000);
                        }).Wait();
                        _connection.RequestStatus("2/1/2");
                        Task.Run(() =>
                        {
                            Thread.Sleep(1000);
                        }).Wait();
                    }
                    Thread.Sleep(100);
                }
            }).Wait();
            #endregion
            
            #region Disconnessione
            _connection.Disconnect();
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();
            #endregion
            
        }

        private static void Apri(string tapparella, string tapparella_stop)
        {
            var operazione = APRI;
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(3500);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(6300);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(5500);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(5000);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();

            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(4000);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();

            _connection.Action(tapparella, operazione);

        }
        private static void Chiudi(string tapparella, string tapparella_stop)
        {
            var operazione = CHIUDI;
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(4500);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(4500);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(5500);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(5500);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();

            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(4500);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();

            _connection.Action(tapparella, operazione);

        }
        
        private static void Apri_10(string tapparella, string tapparella_stop)
        {
            var operazione = APRI;
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(3500);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(6300);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(6300);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();
            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(5000);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();

            _connection.Action(tapparella, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(4000);
            }).Wait();
            _connection.Action(tapparella_stop, operazione);
            Task.Run(() =>
            {
                Thread.Sleep(10000);
            }).Wait();

            _connection.Action(tapparella, operazione);

        }

        static void Log(string id, string message)
        {
            Logger.InfoFormat("{0}:{1}", id, message);
        }

        static void Connected()
        {
            Logger.Info("Connected");
            _connected = true;
        }
        static void Disconnected()
        {
            Logger.Info("Disconnected");
            _connected = false;
        }

        static void Status(string address, string state)
        {
            if (address == "2/1/1" || address == "2/1/2")
            {
                decimal temp = (decimal)_connection.FromDataPoint("9.001", state);
                Logger.InfoFormat("New temp: device {0} has state {1}, temp: {2}", address, state, temp);
            }
            else
            {
                Logger.InfoFormat("New Status: device {0} has state {1}", address, state);
            }            
        }

        static void Event(string address, string state)
        {
            if (address == "2/1/1" || address == "2/1/2")
            {
                decimal temp = (decimal)_connection.FromDataPoint("9.001", state);
                Logger.InfoFormat("New temp: device {0} has state {1}, temp: {2}", address, state, temp);
            }
            else
            {
                Logger.InfoFormat("New Event: device {0} has state {1}", address, state);
            }
        }

        //var operazione = CHIUDI;

        //#region Chiudi tutte tapparelle
        //if (false)
        //{
        //    _connection.Action("1/1/1", operazione);
        //    _connection.Action("1/1/2", operazione);
        //    _connection.Action("1/1/4", operazione);
        //    _connection.Action("1/1/5", operazione);
        //    _connection.Action("1/1/6", operazione);
        //    _connection.Action("1/1/7", operazione);
        //    _connection.Action("1/1/8", operazione);
        //    _connection.Action("1/1/9", operazione);
        //    _connection.Action("1/1/10", operazione);
        //}
        //#endregion

        //#region Apri/chiudi tapparella per test tempistiche
        //if (false)
        //{
        //    var tapparella = "1/1/4";
        //    var tapparella_stop = "1/2/4";
        //    _connection.Action(tapparella, operazione);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(16000);
        //    }).Wait();
        //    Chiudi(tapparella, tapparella_stop);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(10000);
        //    }).Wait();
        //    Apri(tapparella, tapparella_stop);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(10000);
        //    }).Wait();
        //    Chiudi(tapparella, tapparella_stop);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(10000);
        //    }).Wait();
        //    Apri(tapparella, tapparella_stop);
        //}
        //#endregion

        //#region Accendi tutte le luci
        //bool _turnOn = false;
        //if (_turnOn)
        //{
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/1", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/3", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/4", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/5", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/6", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/7", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/10", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/11", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/12", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/13", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/14", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/16", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/17", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/19", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/20", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/21", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/22", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //    _connection.Action("0/0/23", true);
        //    Task.Run(() =>
        //    {
        //        Thread.Sleep(2000);
        //    }).Wait();
        //}
        //#endregion

    }
}
