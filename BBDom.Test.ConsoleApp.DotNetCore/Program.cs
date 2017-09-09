using BBDom.Data;
using BBDom.Data.Dtos;
using BBDom.Data.Models;
using KNXLib;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BBDom.Test.ConsoleApp.DotNetCore
{
    class Program
    {

        private static ILog Logger = LogManager.GetLogger(typeof(Program));

        static KnxConnection _connection { get; set; }
        static bool _connected = false;

        static Dictionary<string, KnxGroupWithStateDto> Groups = new Dictionary<string, KnxGroupWithStateDto>();

        static void Main(string[] args)
        {
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
                while (_run)
                {
                    if (!_connected)
                    {
                        _connection.Connect();
                        Task.Run(() =>
                        {
                            Thread.Sleep(5000);
                        }).Wait();

                        List<KnxGroupWithStateDto> groups = null;
                        using (var ctx = new BBDomDbContext())
                        {
                            groups = ctx.KnxGroups
                                //.Where(g => !g.State.HasValue)
                                .Select(g => new KnxGroupWithStateDto
                                {
                                    Address = g.Address,
                                    DPT = g.DPT,
                                    Name = g.Name,
                                    Direction = g.Direction,
                                })
                                .OrderBy(g => g.Address).ToList();
                        }
                        Groups = groups.ToDictionary(g => g.Address, g => g);

                        _connection.Action("0/0/1", false);
                        Task.Run(() =>
                        {
                            Thread.Sleep(2000);
                        }).Wait();

                        var readOnlyGroupAddresses = Groups.Values.Where(g => g.Direction == KnxGroupDirection.OUTPUT).Select(g => g.Address).ToList();
                        foreach (var readOnlyGroupAddress in readOnlyGroupAddresses)
                        {
                            if (!_connected)
                            {
                                break;
                            }
                            _connection.RequestStatus(readOnlyGroupAddress);
                            Task.Run(() =>
                            {
                                Thread.Sleep(1000);
                            }).Wait();
                        }
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
                        knxGroup.State = switchVal ? 1: 0;
                        stringFormat = string.Format("knxGroup {0}, switch: {1}", knxGroup.Name, switchVal);
                        break;
                    case KnxDPTEnum.UP_DOWN:
                        bool upDownVal = (bool)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.UP_DOWN], state);
                        knxGroup.State = upDownVal ? 1 : 0;
                        stringFormat = string.Format("knxGroup {0}, upDown: {1}", knxGroup.Name, upDownVal);
                        break;
                    case KnxDPTEnum.OPEN_CLOSE:
                        bool openCloseVal = (bool)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.OPEN_CLOSE], state);
                        knxGroup.State = openCloseVal ? 1 : 0;
                        stringFormat = string.Format("knxGroup {0}, openClose: {1}", knxGroup.Name, openCloseVal);
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
