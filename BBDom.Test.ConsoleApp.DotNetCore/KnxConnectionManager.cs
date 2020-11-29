using BBDom.Data;
using BBDom.Data.Dtos;
using BBDom.Data.Models;
using KNXLib;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BBDom.Test.ConsoleApp.DotNetCore
{
    public class KnxConnectionManager
    {

        private static ILog Logger = LogManager.GetLogger(typeof(KnxConnectionManager));

        static KnxConnection _connection { get; set; }
        static bool _connected = false;

        static Dictionary<string, KnxGroupWithStateDto> Groups = new Dictionary<string, KnxGroupWithStateDto>();

        public void Init()
        {

            //try
            //{
            //    log4net.Config.XmlConfigurator.Configure();
            //}
            //catch (Exception) { }

            //var knx = new BBDom.Biz.DotNet.Knx();

            //knx.Test();

            try
            {
                var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo("log4net.config"));
            }
            catch (Exception) { }

            //KNXLib.Log.Logger.DebugEventEndpoint += Log;
            //KNXLib.Log.Logger.InfoEventEndpoint += Log;
            KNXLib.Log.Logger.WarnEventEndpoint += Log;
            KNXLib.Log.Logger.ErrorEventEndpoint += Log;

            _connection = new KnxConnectionTunneling("192.168.1.100", 3671, "192.168.1.101", 3671);
            //_connection.Debug = true;

            _connection.KnxDisconnectedDelegate += Disconnected;
            _connection.KnxConnectedDelegate += Connected;
            _connection.KnxStatusDelegate += Status;
            _connection.KnxEventDelegate += Event;

            #region Never ending loop
            Task.Run(() =>
            {
                var _run = true;
                //var doAction = true;
                //var address = "0/0/1";
                //var state = true;

                while (_run)
                {
                    if (!_connected)
                    {
                        InitGroups();

                        Console.Out.WriteLine("Connecting");
                        _connection.Connect();
                        Task.Delay(5000).GetAwaiter().GetResult();

                        //StartupCaldaia();

                        //var readOnlyGroupAddresses = Groups.Values.Where(g => g.Direction == KnxGroupDirection.OUTPUT).Select(g => g.Address).ToList();
                        //foreach (var readOnlyGroupAddress in readOnlyGroupAddresses)
                        //{
                        //    if (!_connected)
                        //    {
                        //        break;
                        //    }
                        //    _connection.RequestStatus(readOnlyGroupAddress);
                        //    Task.Run(() =>
                        //    {
                        //        Thread.Sleep(1000);
                        //    }).Wait();
                        //}
                    }

                    var cmd = PrintCommands();
                    switch (cmd)
                    {
                        case "1":
                            {
                                var address = PrintRequestReadAddress();
                                if (Groups.ContainsKey(address))
                                {
                                    var group = Groups[address];
                                    if (!group.Read)
                                    {
                                        Console.Out.WriteLine("Indirizzo non valido");
                                    }
                                    else
                                    {
                                        _connection.RequestStatus(address);
                                    }
                                }
                                else
                                {
                                    Console.Out.WriteLine("Indirizzo non valido");
                                }
                            }
                            break;
                        case "2":
                            {
                                var address = PrintRequestWriteAddress();
                                if (Groups.ContainsKey(address))
                                {
                                    var group = Groups[address];
                                    if (!group.Write)
                                    {
                                        Console.Out.WriteLine("Indirizzo non valido");
                                    }
                                    else
                                    {
                                        RequestStateChange(group);
                                    }
                                }
                                else
                                {
                                    Console.Out.WriteLine("Indirizzo non valido");
                                }
                            }
                            break;
                        case "99":
                            _run = false;
                            break;
                    }

                    Task.Delay(3000).GetAwaiter().GetResult();
                }
            }).Wait();
            #endregion

            #region Disconnessione
            _connection.Disconnect();

            Task.Delay(10000).GetAwaiter().GetResult();
            #endregion

        }

        private static void RequestStateChange(KnxGroupWithStateDto group)
        {
            switch (group.DPT.Value)
            {
                case KnxDPTEnum.BOOLEAN:
                    {
                        Console.Write("true/false: ");
                        var boolString = Console.In.ReadLine();
                        bool boolState;
                        if (bool.TryParse(boolString, out boolState))
                        {
                            _connection.Action(group.Address, boolState);
                        }
                        else
                        {
                            Console.Out.WriteLine("Stato non valido");
                        }
                    }
                    break;
                case KnxDPTEnum.SWITCH:
                    {
                        Console.Write("on/off: ");
                        var onOffString = Console.In.ReadLine();
                        if (onOffString.ToLower() == "on".ToLower())
                        {
                            _connection.Action(group.Address, true);
                        }
                        else if (onOffString.ToLower() == "off".ToLower())
                        {
                            _connection.Action(group.Address, false);
                        }
                        else
                        {
                            Console.Out.WriteLine("Stato non valido");
                        }
                    }
                    break;
                case KnxDPTEnum.UP_DOWN:
                    {
                        Console.Write("up/down: ");
                        var onOffString = Console.In.ReadLine();
                        if (onOffString.ToLower() == "up".ToLower())
                        {
                            _connection.Action(group.Address, false);
                        }
                        else if (onOffString.ToLower() == "down".ToLower())
                        {
                            _connection.Action(group.Address, true);
                        }
                        else
                        {
                            Console.Out.WriteLine("Stato non valido");
                        }
                    }
                    break;
                case KnxDPTEnum.TEMPERATURE:
                    {
                        Console.Write("5..28: ");
                        var tempString = Console.In.ReadLine();
                        double tempState;
                        if (double.TryParse(tempString, NumberStyles.Any, CultureInfo.InvariantCulture, out tempState))
                        {
                            _connection.Action(group.Address, _connection.ToDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.TEMPERATURE], tempString));
                        }
                        else
                        {
                            Console.Out.WriteLine("Stato non valido");
                        }
                    }
                    break;
                case KnxDPTEnum.OPEN_CLOSE:
                    {
                        Console.Write("open/closed: ");
                        var openClosedString = Console.In.ReadLine();
                        if (openClosedString.ToLower() == "open".ToLower())
                        {
                            _connection.Action(group.Address, false);
                        }
                        else if (openClosedString.ToLower() == "closed".ToLower())
                        {
                            _connection.Action(group.Address, true);
                        }
                        else
                        {
                            Console.Out.WriteLine("Stato non valido");
                        }
                    }
                    break;
                case KnxDPTEnum.COUNTER_PULSES:
                case KnxDPTEnum.DIMMING_CONTROL:
                    Console.Out.WriteLine("DPT non gestito");
                    break;
                case KnxDPTEnum.PERCENTAGE:
                    {
                        Console.Write("0..100: ");
                        var percString = Console.In.ReadLine();
                        double percState;
                        if (double.TryParse(percString, NumberStyles.Any, CultureInfo.InvariantCulture, out percState) &&
                                percState >= 0 && percState <= 100)
                        {
                            _connection.Action(group.Address, _connection.ToDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.PERCENTAGE], percString));
                        }
                        else
                        {
                            Console.Out.WriteLine("Stato non valido");
                        }
                    }
                    break;
                case KnxDPTEnum.HVAC:
                    {
                        Console.Write("0..4: ");
                        var intString = Console.In.ReadLine();
                        int intHVAC;
                        if (int.TryParse(intString, NumberStyles.Any, CultureInfo.InvariantCulture, out intHVAC) &&
                                intHVAC >= 0 && intHVAC <= 4)
                        {
                            _connection.Action(group.Address, _connection.ToDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.HVAC], intString));
                        }
                        else
                        {
                            Console.Out.WriteLine("Stato non valido");
                        }
                    }
                    break;
                default:
                    break;
            }


        }

        private static string PrintCommands()
        {
            Console.WriteLine("1 - Request status");
            Console.WriteLine("2 - Set state");
            Console.WriteLine("99 - Quit");
            Console.WriteLine("Comando: ");
            return Console.In.ReadLine();
        }

        private static void InitGroups()
        {
            List<KnxGroupWithStateDto> groups = null;
            using (var ctx = new BBDomDbContext())
            {
                groups = ctx.KnxGroups
                            .Select(g => new KnxGroupWithStateDto
                            {
                                Address = g.Address,
                                DPT = g.DPT,
                                Name = g.Name,
                                Read = g.Read,
                                Write = g.Write,
                            })
                            .OrderBy(g => g.Address).ToList();
            }
            Groups = groups.ToDictionary(g => g.Address, g => g);
        }
        private static string PrintRequestReadAddress()
        {
            int lineSize = 4;
            int index = 1;
            Groups.ToList().Where(g => g.Value.Read).ToList().ForEach(g =>
            {
                if (index % lineSize != 0)
                {
                    Console.Out.Write(string.Format("{0}: {1}\t", g.Key, g.Value.Name));
                }
                else
                {
                    Console.Out.WriteLine(string.Format("{0}: {1}", g.Key, g.Value.Name));
                }
                index++;
            });
            if (index % lineSize != 1)
            {
                Console.Out.WriteLine();
            }
            Console.WriteLine("Indirizzo: ");
            return Console.In.ReadLine();
        }

        private static string PrintRequestWriteAddress()
        {
            int lineSize = 4;
            int index = 1;

            Groups.ToList().Where(g => g.Value.Write).ToList().ForEach(g =>
            {
                if (index % lineSize != 0)
                {
                    Console.Out.Write(string.Format("{0}: {1}\t", g.Key, g.Value.Name));
                }
                else
                {
                    Console.Out.WriteLine(string.Format("{0}: {1}", g.Key, g.Value.Name));
                }
                index++;
            });
            if (index % lineSize != 1)
            {
                Console.Out.WriteLine();
            }
            Console.WriteLine("Indirizzo: ");
            return Console.In.ReadLine();
        }

        static void Log(string id, string message)
        {
            Logger.InfoFormat("{0}:{1}", id, message);
        }

        static void Connected()
        {
            Console.Out.WriteLine("Connected");
            _connected = true;
        }
        static void Disconnected()
        {
            Console.Out.WriteLine("Disconnected");
            _connected = false;
        }

        static void Status(string address, string state)
        {
            try
            {
                if (!Groups.ContainsKey(address))
                {
                    var knxGroup = new KnxGroupWithStateDto
                    {
                        Address = address,
                        Name = address,
                    };
                    LogValue(knxGroup, state, false);
                    using (var ctx = new BBDomDbContext())
                    {
                        ctx.KnxGroups.Add(new KnxGroup
                        {
                            Address = address,
                            Name = address,
                        });
                        ctx.SaveChanges();
                    }
                    Groups[address] = knxGroup;
                    Logger.InfoFormat("New Status: device {0} has state {1}", address, state);
                }
                else
                {
                    var knxGroup = Groups[address];
                    LogValue(knxGroup, state, false);
                    using (var ctx = new BBDomDbContext())
                    {
                        var utcNow = DateTime.UtcNow;
                        ctx.KnxStates.Add(new KnxState
                        {
                            Address = knxGroup.Address,
                            State = knxGroup.State,
                            UTCTicks = utcNow.Ticks,
                            UTCTimestamp = utcNow,
                            StateSource = StateSource.STATUS,
                        });
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Status, address {0}", address);
                Logger.Error("Status", ex);
            }
        }

        static void Event(string address, string state)
        {
            try
            {
                if (!Groups.ContainsKey(address))
                {
                    var knxGroup = new KnxGroupWithStateDto
                    {
                        Address = address,
                        Name = address,
                    };
                    LogValue(knxGroup, state, true);
                    using (var ctx = new BBDomDbContext())
                    {
                        ctx.KnxGroups.Add(new KnxGroup
                        {
                            Address = address,
                            Name = address,
                        });
                        ctx.SaveChanges();
                    }
                    Groups[address] = knxGroup;
                    Logger.InfoFormat("New Event: device {0} has state {1}", address, state);
                }
                else
                {
                    var knxGroup = Groups[address];
                    switch (knxGroup.Address)
                    {
                        case "3/0/1":
                            DoUscitaCasaNotte();
                            break;
                        case "3/0/2":
                            DoUscitaCasaGiorno();
                            break;
                        case "3/0/3":
                            DoIngressoCasaGiorno();
                            break;
                        case "3/0/5":
                            DoSonno();
                            break;
                        default:
                            break;
                    }
                    LogValue(knxGroup, state, true);
                    using (var ctx = new BBDomDbContext())
                    {
                        var utcNow = DateTime.UtcNow;
                        ctx.KnxStates.Add(new KnxState
                        {
                            Address = knxGroup.Address,
                            State = knxGroup.State,
                            UTCTicks = utcNow.Ticks,
                            UTCTimestamp = utcNow,
                            StateSource = StateSource.EVENT,
                        });
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Status, address {0}", address);
                Logger.Error("Status", ex);
            }
        }

        private static void DoSonno()
        {
            _connection.Action("1/1/1", true);
            Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/2", true);
            Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/3", true);
            Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/4", true);
            Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/5", true);
            Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/6", true);
            Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/7", true);
            Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/8", true);
            Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/9", true);
            Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/10", true);
        }

        private static void DoIngressoCasaGiorno()
        {
            //G1,G2,G3 = P1
            _connection.Action("1/7/1", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/7/2", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/7/3", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/7/4", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/7/5", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/7/6", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/7/7", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/7/8", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/9", false);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/7/10", true);
             Task.Delay(200).GetAwaiter().GetResult();
        }

        private static void DoUscitaCasaGiorno()
        {
            //G1,G2,G3 = P0
            _connection.Action("1/6/1", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/6/2", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/6/3", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/6/4", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/6/5", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/6/6", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/6/7", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/6/8", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/6/9", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/6/10", true);
        }

        private static void DoUscitaCasaNotte()
        {
            //G1,G3 = 0%, G2 = P0
            _connection.Action("1/1/1", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/2", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/3", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/4", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/5", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/6", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/7", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/8", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/9", true);
             Task.Delay(200).GetAwaiter().GetResult();
            _connection.Action("1/1/10", true);
        }

        private static void StartupCaldaia()
        {
            //set stato caldaia a on e poi off, perché quando si riavvia l'impianto elettrico lo switch non funziona,
            //inoltre, se dovesse capitare un black-out elettrico con la caldaia a on al ritorno della corrente quest'ultima resterebbe accesa
            Console.Out.WriteLine("StartupCaldaia, on, lascio accesa per 30 secondi, per non accendere e spegnere troppo in fretta...");
            _connection.Action("2/0/1", true);
            Task.Delay(30000).GetAwaiter().GetResult();
            Console.Out.WriteLine("StartupCaldaia, off");
            _connection.Action("2/0/1", false);
             Task.Delay(200).GetAwaiter().GetResult();
        }

        private static void LogValue(KnxGroupWithStateDto knxGroup, string state, bool isEvent)
        {
            string stringFormat = string.Empty;
            if (!knxGroup.DPT.HasValue)
            {
                stringFormat = string.Format("New {0}: knxGroup {1} has state {2}", isEvent ? "Event" : "Status", knxGroup.Address, state);
            }
            else
            {
                switch (knxGroup.DPT.Value)
                {
                    case KnxDPTEnum.PERCENTAGE:
                        decimal perc = (decimal)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.PERCENTAGE], state);
                        knxGroup.State = (double)perc;
                        stringFormat = string.Format("knxGroup {0}, perc: {1}", knxGroup.Name, perc);
                        break;
                    case KnxDPTEnum.TEMPERATURE:
                        decimal temp = (decimal)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.TEMPERATURE], state);
                        knxGroup.State = (double)temp;
                        stringFormat = string.Format("knxGroup {0}, temp: {1}", knxGroup.Name, temp);
                        break;
                    case KnxDPTEnum.SWITCH:
                        bool switchVal = (bool)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.SWITCH], state);
                        knxGroup.State = switchVal ? 1 : 0;
                        stringFormat = string.Format("knxGroup {0}, switch: {1}", knxGroup.Name, switchVal ? "on" : "off");
                        break;
                    case KnxDPTEnum.BOOLEAN:
                        bool booleanVal = (bool)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.SWITCH], state);
                        knxGroup.State = booleanVal ? 1 : 0;
                        stringFormat = string.Format("knxGroup {0}, boolean: {1}", knxGroup.Name, booleanVal);
                        break;
                    case KnxDPTEnum.UP_DOWN:
                        bool upDownVal = (bool)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.UP_DOWN], state);
                        knxGroup.State = upDownVal ? 1 : 0;
                        stringFormat = string.Format("knxGroup {0}, upDown: {1}", knxGroup.Name, upDownVal ? "down" : "up");
                        break;
                    case KnxDPTEnum.OPEN_CLOSE:
                        bool openCloseVal = (bool)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.OPEN_CLOSE], state);
                        knxGroup.State = openCloseVal ? 1 : 0;
                        stringFormat = string.Format("knxGroup {0}, openClose: {1}", knxGroup.Name, openCloseVal ? "opened" : "closed");
                        break;
                    case KnxDPTEnum.COUNTER_PULSES:
                        int counterPulsesVal = (int)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.COUNTER_PULSES], state);
                        knxGroup.State = (double)counterPulsesVal;
                        stringFormat = string.Format("knxGroup {0}, counterPulsesVal: {1}", knxGroup.Name, counterPulsesVal);
                        break;
                    case KnxDPTEnum.DIMMING_CONTROL:
                        int step = (int)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.DIMMING_CONTROL], state);
                        knxGroup.State = (double)step;
                        stringFormat = string.Format("knxGroup {0}, step: {1}", knxGroup.Name, step);
                        break;
                    case KnxDPTEnum.HVAC:
                        int hvac = (int)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.HVAC], state);
                        knxGroup.State = hvac;
                        switch (hvac)
                        {
                            case 0:
                                stringFormat = string.Format("knxGroup {0}, hvac: Auto", knxGroup.Name);
                                break;
                            case 1:
                                stringFormat = string.Format("knxGroup {0}, hvac: Comfort", knxGroup.Name);
                                break;
                            case 2:
                                stringFormat = string.Format("knxGroup {0}, hvac: Standby", knxGroup.Name);
                                break;
                            case 3:
                                stringFormat = string.Format("knxGroup {0}, hvac: Economy", knxGroup.Name);
                                break;
                            case 4:
                                stringFormat = string.Format("knxGroup {0}, hvac: Building Protection", knxGroup.Name);
                                break;
                            default:
                                stringFormat = string.Format("knxGroup {0}, hvac: {1}", knxGroup.Name, hvac);
                                break;
                        }
                        break;
                    default:
                        stringFormat = string.Format("New {0}: knxGroup {1} has state {2}", isEvent ? "Event" : "Status", knxGroup.Address, state);
                        break;
                }
            }
            Logger.InfoFormat(stringFormat);
            Console.Out.WriteLine(stringFormat);
        }
        
    }
}
