using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CutIT.Messages
{
    public enum GCommandTypeEnum
    {
        NonModal,
        MotionModes,
        FeedRateModes,
        UnitModes,
        DistanceModes,
        ArcIJKDistanceModes,
        PlaneSelectModes,
        ToolLengthOffsetModes,
        CutterCompensationModes,
        CoordinateSystemModes,
        ControlModes,
        ProgramFlow,
        CoolantControl,
        SpindleControl,
        ValidNonCommandWord,
        Unknown = 99
    }

    public class GCommand:GrblRequest
    {
        static Dictionary<GCommandTypeEnum, string[]> GCODE_GROUPS = new Dictionary<GCommandTypeEnum, string[]>()
        {
            {GCommandTypeEnum.NonModal                  , new string[] {"G4", "G10L2", "G10L20", "G28", "G30", "G28.1", "G30.1", "G53", "G92", "G92.1"} },
            {GCommandTypeEnum.MotionModes               , new string[] {"G0", "G1", "G2", "G3", "G38.2", "G38.3", "G38.4", "G38.5", "G80"} },
            {GCommandTypeEnum.FeedRateModes             , new string[] {"G93", "G94"} },
            {GCommandTypeEnum.UnitModes                 , new string[] {"G20", "G21"} },
            {GCommandTypeEnum.DistanceModes             , new string[] {"G90", "G91"} },
            {GCommandTypeEnum.ArcIJKDistanceModes       , new string[] {"G91.1"} },
            {GCommandTypeEnum.PlaneSelectModes          , new string[] {"G17", "G18", "G19"} },
            {GCommandTypeEnum.ToolLengthOffsetModes     , new string[] {"G43.1", "G49"} },
            {GCommandTypeEnum.CutterCompensationModes   , new string[] {"G40"} },
            {GCommandTypeEnum.CoordinateSystemModes     , new string[] {"G54", "G55", "G56", "G57", "G58", "G59"} },
            {GCommandTypeEnum.ControlModes              , new string[] {"G61"} },
            {GCommandTypeEnum.ProgramFlow               , new string[] {"M0", "M1", "M2", "M30"} },
            {GCommandTypeEnum.CoolantControl            , new string[] {"M7", "M8", "M9"} },
            {GCommandTypeEnum.SpindleControl            , new string[] {"M3", "M4", "M5"} }
        };

        static string[] VALID_COMMANDS = new string[] { "G4", "G10L2", "G10L20", "G28", "G30", "G28.1", "G30.1", "G53", "G92", "G92.1",
                                                "G0", "G1", "G2", "G3", "G38.2", "G38.3", "G38.4", "G38.5", "G80", "G93", "G94",
                                                "G20", "G21", "G90", "G91", "G91.1", "G17", "G18", "G19", "G43.1", "G49", "G40",
                                                "G54", "G55", "G56", "G57", "G58", "G59", "G61", "M0", "M1", "M2", "M30", "M7",
                                                "M8", "M9", "M3", "M4", "M5" };

        static string[] VALID_LETTERS = new string[] { "F", "I", "J", "K", "L", "N", "P", "R", "S", "T", "X", "Y", "Z" };

        static string GCODE_PATTERN = "^[ \t]*([GMFIJKLNPRSTXYZgmfijklnprstxyz])([0-9]+(?:\\.)?[0-9]*)(?:(L)(20?))?";

        public static GCommandTypeEnum GetCommandType(string command)
        {
            GCommandTypeEnum result = GCommandTypeEnum.Unknown;
            foreach (var group in GCODE_GROUPS)
            {
                foreach (var item in group.Value)
                {
                    if (item.Equals(command))
                    {
                        result = group.Key;
                        break;
                    }
                }
                if (result != GCommandTypeEnum.Unknown)
                    break;
            }
            return result;
        }

        GCommandTypeEnum _commandType = GCommandTypeEnum.Unknown;
        string _letter = "";
        string _number = "";
        string _command = "";
        bool _isLetter = false;
        bool _isCommand = false;
        bool _addNewline = true;
        GCommand _chainedCommand = null;

        public override GrblRequestEnum RequestType
        {
            protected set
            {
                base.RequestType = GrblRequestEnum.GCode;
            }
        }

        public virtual string Letter { get { return _letter; } protected set { _letter = value; } }
        public virtual string Number { get { return _number; } protected set { _number = value; } }
        public virtual string Command { get { return _command; } protected set { _command = value; } }
        public virtual bool IsLetter { get { return _isLetter; } protected set { _isLetter = value; } }
        public virtual bool IsCommand { get { return _isCommand; } protected set { _isCommand = value; } }
        public override bool IsValid { get { return IsCommand || IsLetter && base.IsValid; } }
        public virtual bool AddNewline { get { return _addNewline; } protected set { _addNewline = value; } }
        public virtual GCommandTypeEnum CommandType { get { return _commandType; } protected set { _commandType = value; } }
        public virtual GCommand ChainedCommand { get { return _chainedCommand; } protected set { _chainedCommand = value; } }

        public GCommand()
        {
        }

        public GCommand(string command, bool ParseCoupled = false) : this()
        {
            Parse(command, ParseCoupled);
        }

        public virtual int Parse(string input)
        {
            if (!IsValid)
            {
                Match match = Regex.Match(input, GCODE_PATTERN);
                if (match.Success)
                {
                    string command;
                    string letter = match.Groups[1].Value.ToUpper();
                    string number = match.Groups[2].Value;
                    while (number.StartsWith("0") && number.Length > 1)
                        number = number.Substring(1);
                    command = letter + number;
                    if (match.Groups.Count == 5)
                        command += match.Groups[3].Value.ToUpper() + match.Groups[4].Value;
                    Clear();
                    if (VALID_COMMANDS.Contains(command))
                    {
                        CommandType = GetCommandType(command);
                        IsCommand = true;
                        Command = command;
                        Items.Add(command);
                    }
                    else if (VALID_LETTERS.Contains(letter))
                    {
                        CommandType = GCommandTypeEnum.ValidNonCommandWord;
                        Command = command;
                        IsLetter = true;
                    }
                    else
                    {
                        CommandType = GCommandTypeEnum.Unknown;
                        Command = command;
                    }
                    Items.Add(letter);
                    Items.Add(number);
                    if (command.StartsWith("G10L2"))
                    {
                        Items.Add(match.Groups[3].Value.ToUpper());
                        Items.Add(match.Groups[4].Value);
                    }
                    else
                    {
                        Letter = letter;
                        Number = number;
                    }
                    Content = input;
                    return match.Length;
                }
            }
            return -1;
        }

        public virtual int Parse(string input, bool parseCoupled)
        {
            int result = Parse(input);
            if (parseCoupled)
            {
                if (result >= 0)
                {
                    int length = 0;
                    input = input.Substring(result);

                    GCommand coupledCommand = new GCommand();
                    length = coupledCommand.Parse(input, parseCoupled);
                    if (length >= 0)
                    {
                        result += length;
                        ChainedCommand = coupledCommand;
                    }
                    base.SetContent(ToString());
                }
            }            
            return result;
        }

        public override bool SetContent(string content)
        {
            if (Parse(content, true) >= 0)
            {                
                return true;
            }
            return false;
        }

        public override void Clear()
        {
            _letter = "";
            _number = "";
            _command = "";
            _isLetter = false;
            _isCommand = false;
            _commandType = GCommandTypeEnum.Unknown;
            base.Clear();
        }

        public override string ToString()
        {
            string result = _command;
            if (ChainedCommand != null)
                result = result + " " + ChainedCommand.ToString();
            return result;
        }

        public virtual GrblRequest ToGrblRequest()
        {
            GrblRequest result = new GrblRequest();
            if (!ToGrblRequest(result))
                result = null;
            return result;
        }

        public virtual bool ToGrblRequest(GrblRequest request)
        {
            return request.SetContent(ToString());
        }

        public override bool Stamp()
        {
            if (base.Stamp())
            {
                if (ChainedCommand != null)
                    ChainedCommand.Stamp();
                return true;
            }
            return false;
        }
    }
}
