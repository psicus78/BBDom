using BBDom.Data;
using BBDom.Data.Dtos;
using BBDom.Data.Models;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using KNXLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BBDom.Web
{
    public class KnxConnectionManager
    {

        private readonly ILogger<KnxConnectionManager> _logger;
        private readonly System.IServiceProvider _serviceProvider;
        private KnxConnection _connection;
        private bool _connected = false;
        private bool _run = false;
        private object _runLock = new Boolean();

        Dictionary<string, KnxGroupWithStateDto> Groups = new Dictionary<string, KnxGroupWithStateDto>();

        public KnxConnectionManager(ILogger<KnxConnectionManager> logger, System.IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            KNXLib.Log._logger.DebugEventEndpoint += Log;
            KNXLib.Log._logger.InfoEventEndpoint += Log;
            KNXLib.Log._logger.WarnEventEndpoint += Log;
            KNXLib.Log._logger.ErrorEventEndpoint += Log;

        }

        public void Init()
        {
            lock (_runLock)
            {
                if (_run)
                {
                    return;
                }

                _run = true;
            }
            _connection = new KnxConnectionTunneling("192.168.1.100", 3671, "192.168.1.101", 3671);
            _connection.Debug = true;
            _connection.KnxDisconnectedDelegate += Disconnected;
            _connection.KnxConnectedDelegate += Connected;
            _connection.KnxStatusDelegate += Status;
            _connection.KnxEventDelegate += Event;

            #region Never ending loop
            Task.Factory.StartNew(() =>
            {
                //var doAction = true;
                //var address = "0/0/1";
                //var state = true;

                //Repeat daily
                var rrule = new RecurrencePattern(FrequencyType.Daily, 1);
                double offsetOn = 1; //SP should become 21
                double offsetOff = -2; //SP should become 18
                var calendar = new Ical.Net.Calendar();
                List<MyCalendarEvent> myCalendarEvents = new List<MyCalendarEvent>();
                //on
                var e_17__18_30 = new MyCalendarEvent
                {
                    Start = new CalDateTime(new DateTime(2021, 1, 4, 17, 0, 0)),
                    End = new CalDateTime(new DateTime(2021, 1, 4, 18, 30, 0)),
                    RecurrenceRules = new List<RecurrencePattern> { rrule },
                    Offset = offsetOn,
                };
                calendar.Events.Add(e_17__18_30);
                myCalendarEvents.Add(e_17__18_30);
                //off
                var e_18_30__20_30 = new MyCalendarEvent
                {
                    Start = new CalDateTime(new DateTime(2021, 1, 4, 18, 30, 0)),
                    End = new CalDateTime(new DateTime(2021, 1, 4, 20, 30, 0)),
                    RecurrenceRules = new List<RecurrencePattern> { rrule },
                    Offset = offsetOff,
                };
                calendar.Events.Add(e_18_30__20_30);
                myCalendarEvents.Add(e_18_30__20_30);
                //on
                var e_20_30__22 = new MyCalendarEvent
                {
                    Start = new CalDateTime(new DateTime(2021, 1, 4, 20, 30, 0)),
                    End = new CalDateTime(new DateTime(2021, 1, 4, 22, 0, 0)),
                    RecurrenceRules = new List<RecurrencePattern> { rrule },
                    Offset = offsetOn,
                };
                calendar.Events.Add(e_20_30__22);
                myCalendarEvents.Add(e_20_30__22);
                //off
                var e_22__00 = new MyCalendarEvent
                {
                    Start = new CalDateTime(new DateTime(2021, 1, 4, 22, 0, 0)),
                    End = new CalDateTime(new DateTime(2021, 1, 5, 0, 0, 0)),
                    RecurrenceRules = new List<RecurrencePattern> { rrule },
                    Offset = offsetOff,
                };
                calendar.Events.Add(e_22__00);
                myCalendarEvents.Add(e_22__00);
                //off
                var e_00__6_30 = new MyCalendarEvent
                {
                    Start = new CalDateTime(new DateTime(2021, 1, 4, 0, 0, 0)),
                    End = new CalDateTime(new DateTime(2021, 1, 4, 6, 30, 0)),
                    RecurrenceRules = new List<RecurrencePattern> { rrule },
                    Offset = offsetOff,
                };
                calendar.Events.Add(e_00__6_30);
                myCalendarEvents.Add(e_00__6_30);
                //on
                var e_6_30__9 = new MyCalendarEvent
                {
                    Start = new CalDateTime(new DateTime(2021, 1, 4, 6, 30, 0)),
                    End = new CalDateTime(new DateTime(2021, 1, 4, 9, 0, 0)),
                    RecurrenceRules = new List<RecurrencePattern> { rrule },
                    Offset = offsetOn,
                };
                calendar.Events.Add(e_6_30__9);
                myCalendarEvents.Add(e_6_30__9);
                //off
                var e_9__12 = new MyCalendarEvent
                {
                    Start = new CalDateTime(new DateTime(2021, 1, 4, 9, 0, 0)),
                    End = new CalDateTime(new DateTime(2021, 1, 4, 12, 0, 0)),
                    RecurrenceRules = new List<RecurrencePattern> { rrule },
                    Offset = offsetOff,
                };
                calendar.Events.Add(e_9__12);
                myCalendarEvents.Add(e_9__12);
                //on
                var e_12__14 = new MyCalendarEvent
                {
                    Start = new CalDateTime(new DateTime(2021, 1, 4, 12, 0, 0)),
                    End = new CalDateTime(new DateTime(2021, 1, 4, 14, 0, 0)),
                    RecurrenceRules = new List<RecurrencePattern> { rrule },
                    Offset = offsetOn,
                };
                calendar.Events.Add(e_12__14);
                myCalendarEvents.Add(e_12__14);
                //off
                var e_14__17 = new MyCalendarEvent
                {
                    Start = new CalDateTime(new DateTime(2021, 1, 4, 14, 0, 0)),
                    End = new CalDateTime(new DateTime(2021, 1, 4, 17, 0, 0)),
                    RecurrenceRules = new List<RecurrencePattern> { rrule },
                    Offset = offsetOff,
                };
                calendar.Events.Add(e_14__17);
                myCalendarEvents.Add(e_14__17);

                var serializer = new CalendarSerializer();
                var serializedCalendar = serializer.SerializeToString(calendar);

                var searchStart = DateTime.Now.Date;
                var searchEnd = searchStart.AddDays(1);
                List<EventOccurrency> events = GetEventOccurrencies(myCalendarEvents, calendar, searchStart, searchEnd);
                foreach (var myEvent in events)
                {
                    _logger.LogInformation("Event: {0} - {1}, offset: {2}", myEvent.Start, myEvent.End, myEvent.Offset);
                }
                EventOccurrency currentEvent = null;
                while (_run)
                {
                    if (!_connected)
                    {
                        InitGroups();

                        _logger.LogInformation("Connecting");
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
                        //_connection.Action("2/1/11", true);
                        //Task.Delay(2000).GetAwaiter().GetResult();
                        //_connection.RequestStatus("2/1/11");
                        //if (_connected)
                        //{
                        //    var dpValue = _connection.ToDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.HVAC], "3");
                        //    _connection.Action("2/1/4", dpValue);
                        //    var dpTempValue = _connection.ToDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.TEMPERATURE], "1");
                        //    _connection.Action("2/1/13", dpTempValue);
                        //    var _2_1_xAddresses = Groups.Values.Where(v => v.Address.StartsWith("2/1/")).ToList();
                        //    foreach (var _2_1_xAddress in _2_1_xAddresses)
                        //    {
                        //        _connection.RequestStatus(_2_1_xAddress.Address);
                        //        Task.Delay(200).GetAwaiter().GetResult();
                        //    }
                        //    //Task.Delay(2000).GetAwaiter().GetResult();
                        //    //_connection.RequestStatus("2/1/4");
                        //}
                    }

                    var now = DateTime.Now;

                    if (now >= searchEnd)
                    {
                        searchStart = now.Date;
                        searchEnd = searchStart.AddDays(1);
                        _logger.LogInformation("Changing day: {0} - {1}", searchStart, searchEnd);
                        events = GetEventOccurrencies(myCalendarEvents, calendar, searchStart, searchEnd);
                        foreach (var myEvent in events)
                        {
                            _logger.LogInformation("Event: {0} - {1}, offset: {2}", myEvent.Start, myEvent.End, myEvent.Offset);
                        }
                    }
                    //now < searchEnd
                    else if (currentEvent == null || currentEvent.End <= now)
                    {
                        _logger.LogInformation("Set current event: {0}", now);
                        events.RemoveAll(e => e.End <= now);
                        if (events.Any())
                        {
                            currentEvent = events.OrderBy(e => e.Start).First();
                            _logger.LogInformation("Event: {0} - {1}, offset: {2}", currentEvent.Start, currentEvent.End, currentEvent.Offset);
                            var offsetString = currentEvent.Offset.ToString(CultureInfo.InvariantCulture);
                            var dpTempValue = _connection.ToDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.TEMPERATURE], offsetString);
                            _connection.Action("2/1/13", dpTempValue);
                            Task.Delay(200).GetAwaiter().GetResult();
                            _connection.Action("2/2/13", dpTempValue);
                            Task.Delay(200).GetAwaiter().GetResult();
                            _connection.Action("2/3/13", dpTempValue);
                            Task.Delay(200).GetAwaiter().GetResult();
                            _connection.Action("2/4/13", dpTempValue);
                            Task.Delay(200).GetAwaiter().GetResult();
                            _connection.Action("2/5/13", dpTempValue);
                            Task.Delay(200).GetAwaiter().GetResult();
                            _connection.Action("2/6/13", dpTempValue);
                            Task.Delay(200).GetAwaiter().GetResult();
                        }
                    }
                    Task.Delay(100).GetAwaiter().GetResult();
                }

            });
            #endregion

        }

        private List<EventOccurrency> GetEventOccurrencies(List<MyCalendarEvent> myCalendarEvents, Ical.Net.Calendar calendar, DateTime searchStart, DateTime searchEnd)
        {
            var occurrencies = calendar.GetOccurrences(searchStart, searchEnd);
            var events = new List<EventOccurrency>();
            foreach (var occurrency in occurrencies)
            {
                var myCalendarEvent = myCalendarEvents.FirstOrDefault(e => e.Start.Value.TimeOfDay == occurrency.Period.StartTime.Value.TimeOfDay);
                if (myCalendarEvent != null)
                {
                    events.Add(new EventOccurrency
                    {
                        Offset = myCalendarEvent.Offset,
                        Start = occurrency.Period.StartTime.Value,
                        End = occurrency.Period.EndTime.Value,
                    });
                }
            }
            return events.OrderBy(e => e.Start).ToList();
        }

        public void Shutdown()
        {
            lock (_runLock)
            {
                if (!_run)
                {
                    return;
                }
                _run = false;
            }
            _connection.Disconnect();
            Task.Delay(10000).GetAwaiter().GetResult();
            _connection.KnxDisconnectedDelegate -= Disconnected;
            _connection.KnxConnectedDelegate -= Connected;
            _connection.KnxStatusDelegate -= Status;
            _connection.KnxEventDelegate -= Event;
            _connection = null;
        }

        private void RequestStateChange(KnxGroupWithStateDto group)
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

        private string PrintCommands()
        {
            Console.WriteLine("1 - Request status");
            Console.WriteLine("2 - Set state");
            Console.WriteLine("99 - Quit");
            Console.WriteLine("Comando: ");
            return Console.In.ReadLine();
        }

        private void InitGroups()
        {
            List<KnxGroupWithStateDto> groups = null;
            using (var scope = _serviceProvider.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetService<BBDomDbContext>();
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
        private string PrintRequestReadAddress()
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

        private string PrintRequestWriteAddress()
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

        void Log(string id, string message)
        {
            _logger.LogInformation("{0}:{1}", id, message);
        }

        void Connected()
        {
            _logger.LogInformation("Connected");
            _connected = true;
        }
        void Disconnected()
        {
            _logger.LogInformation("Disconnected");
            _connected = false;
        }

        void Status(string address, string state)
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
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var ctx = scope.ServiceProvider.GetService<BBDomDbContext>();
                        ctx.KnxGroups.Add(new KnxGroup
                        {
                            Address = address,
                            Name = address,
                        });
                        ctx.SaveChanges();
                    }
                    Groups[address] = knxGroup;
                    _logger.LogInformation("New Status: device {0} has state {1}", address, state);
                }
                else
                {
                    var knxGroup = Groups[address];
                    LogValue(knxGroup, state, false);
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var utcNow = DateTime.UtcNow;
                        var ctx = scope.ServiceProvider.GetService<BBDomDbContext>();
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
                _logger.LogError("Status, address {0}", address);
                _logger.LogError("Status, exception: {0}", string.Format("{0}{1}{2}", ex.Message, System.Environment.NewLine, ex.StackTrace));
            }
        }

        void Event(string address, string state)
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
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var utcNow = DateTime.UtcNow;
                        var ctx = scope.ServiceProvider.GetService<BBDomDbContext>();
                        ctx.KnxGroups.Add(new KnxGroup
                        {
                            Address = address,
                            Name = address,
                        });
                        ctx.SaveChanges();
                    }
                    Groups[address] = knxGroup;
                    _logger.LogInformation("New Event: device {0} has state {1}", address, state);
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
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var utcNow = DateTime.UtcNow;
                        var ctx = scope.ServiceProvider.GetService<BBDomDbContext>();
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
                _logger.LogError("Event, address {0}", address);
                _logger.LogError("Event, exception: {0}", string.Format("{0}{1}{2}", ex.Message, System.Environment.NewLine, ex.StackTrace));
            }
        }

        private void DoSonno()
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

        private void DoIngressoCasaGiorno()
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

        private void DoUscitaCasaGiorno()
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

        private void DoUscitaCasaNotte()
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

        private void StartupCaldaia()
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

        private void LogValue(KnxGroupWithStateDto knxGroup, string state, bool isEvent)
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
                    case KnxDPTEnum.HEAT_COOL:
                        bool heatCoolVal = (bool)_connection.FromDataPoint(KnxDPT.KnxDPTs[KnxDPTEnum.HEAT_COOL], state);
                        knxGroup.State = heatCoolVal ? 1 : 0;
                        stringFormat = string.Format("knxGroup {0}, openClose: {1}", knxGroup.Name, heatCoolVal ? "heating" : "cooling");
                        break;
                    default:
                        stringFormat = string.Format("New {0}: knxGroup {1} has state {2}", isEvent ? "Event" : "Status", knxGroup.Address, state);
                        break;
                }
            }
            _logger.LogInformation(stringFormat);
            Console.Out.WriteLine(stringFormat);
        }

    }

    public class MyCalendarEvent : CalendarEvent
    {
        public double Offset { get; set; }

    }

    public class EventOccurrency
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double Offset { get; set; }

    }
}

