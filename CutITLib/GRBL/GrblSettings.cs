using CutIT.Messages;
using CutIT.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutIT.GRBL
{
    public enum GrblStateEnum
    {
        Idle,
        Run,
        Hold,
        Door,
        Home,
        Alarm,
        Check
    }

    public class GrblSettings
    {
        protected ConcurrentDictionary<string, string> Settings { get; set; }
        protected ConcurrentDictionary<string, Coordinate> GCodeParameters { get; set; }
        protected List<Tuple<string, string>> ParserState { get; set; }

        public GrblStateEnum GrblState { get; protected set; }
        public Coordinate MachinePosition { get; protected set; }
        public Coordinate WorkPosition { get; protected set; }
        public bool LastProbeSuccess { get; set; }
        public double ToolLengthOffset { get; protected set; }
        public string StartupBlock1 { get; protected set; }
        public string StartupBlock2 { get; protected set; }
        public string BuildInfo { get; protected set; }

        public GrblSettings()
        {
            Settings = new ConcurrentDictionary<string, string>();
            GCodeParameters = new ConcurrentDictionary<string, Coordinate>();
            ParserState = new List<Tuple<string, string>>();

            GrblState = GrblStateEnum.Idle;
            WorkPosition = new Coordinate();
            MachinePosition = new Coordinate();

            Settings["0"] = "";
            Settings["1"] = "";
            Settings["2"] = "";
            Settings["3"] = "";
            Settings["4"] = "";
            Settings["5"] = "";
            Settings["6"] = "";
            Settings["10"] = "";
            Settings["11"] = "";
            Settings["12"] = "";
            Settings["13"] = "";
            Settings["20"] = "";
            Settings["21"] = "";
            Settings["22"] = "";
            Settings["23"] = "";
            Settings["24"] = "";
            Settings["25"] = "";
            Settings["26"] = "";
            Settings["27"] = "";
            Settings["100"] = "";
            Settings["101"] = "";
            Settings["102"] = "";
            Settings["110"] = "";
            Settings["111"] = "";
            Settings["112"] = "";
            Settings["120"] = "";
            Settings["121"] = "";
            Settings["122"] = "";
            Settings["130"] = "";
            Settings["131"] = "";
            Settings["132"] = "";

            GCodeParameters["G54"] = new Coordinate();
            GCodeParameters["G55"] = new Coordinate();
            GCodeParameters["G56"] = new Coordinate();
            GCodeParameters["G57"] = new Coordinate();
            GCodeParameters["G58"] = new Coordinate();
            GCodeParameters["G59"] = new Coordinate();
            GCodeParameters["G28"] = new Coordinate();
            GCodeParameters["G30"] = new Coordinate();
            GCodeParameters["G92"] = new Coordinate();
            GCodeParameters["PRB"] = new Coordinate();  //Probe

            StartupBlock1 = "";
            StartupBlock2 = "";

            BuildInfo = "";

            ToolLengthOffset = 0.0f;
            LastProbeSuccess = false;
        }

        public bool Parse(GrblResponse response)
        {
            bool result = false;
            if (response != null)
            {
                switch (response.ResponseType)
                {
                    case GrblResponseEnum.Coordinate:
                        if (GCodeParameters.ContainsKey(response.Items[0]))
                        {
                            result = GCodeParameters[response.Items[0]].Relocate(response.Items[1], response.Items[2], response.Items[3]);

                        }
                        break;
                    case GrblResponseEnum.Probe:
                        if (GCodeParameters.ContainsKey("PRB"))
                        {
                            if (GCodeParameters["PRB"].Relocate(response.Items[0], response.Items[1], response.Items[2]))
                            {
                                LastProbeSuccess = response.Items[3].Equals("1");
                                result = true;
                            }
                        }
                        break;
                    case GrblResponseEnum.ParserState:
                        ParserState.Clear();
                        for (int i = 0; i < response.Items.Count; i = i + 2)
                        {
                            ParserState.Add(new Tuple<string, string>(response.Items[i], response.Items[i + 1]));
                        }
                        result = true;
                        break;
                    case GrblResponseEnum.StartupBlock:
                        if (response.Items[0].Equals("0"))
                        {
                            StartupBlock1 = response.Items[1];
                            result = true;
                        }
                        else if (response.Items[0].Equals("1"))
                        {
                            StartupBlock2 = response.Items[1];
                            result = true;
                        }
                        break;
                    case GrblResponseEnum.Setting:
                        if (Settings.ContainsKey(response.Items[0]))
                        {
                            Settings[response.Items[0]] = response.Items[1];
                            result = true;
                        }
                        break;
                    case GrblResponseEnum.Status:
                        GrblState = (GrblStateEnum)Enum.Parse(typeof(GrblStateEnum), response.Items[0], true);
                        MachinePosition.Relocate(response.Items[1], response.Items[2], response.Items[3]);
                        WorkPosition.Relocate(response.Items[4], response.Items[5], response.Items[6]);
                        result = true;
                        break;
                    case GrblResponseEnum.ToolLengthOffset:
                        ToolLengthOffset = double.Parse(response.Items[0]);
                        result = true;
                        break;
                    case GrblResponseEnum.BuildInfo:
                        BuildInfo = response.Items[0];
                        result = true;
                        break;
                }
            }
            return result;
        }

    }
}
