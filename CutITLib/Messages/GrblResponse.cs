using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CutIT.Messages
{
    public enum GrblResponseEnum
    {
        Ok = 0,
        Error = 1,
        Alarm = 2,
        Setting = 3,
        StartupBlock = 4,
        Probe = 5,
        Coordinate = 6,
        ToolLengthOffset = 7,
        ParserState = 8,
        Parameter = 9,
        Status = 10,
        BuildInfo = 11,
        Unknown = 99
    }
    public class GrblResponse : Message
    {
        public static Dictionary<GrblResponseEnum, string> ResponseRegex =
            new Dictionary<GrblResponseEnum, string>()
            {
                {GrblResponseEnum.Ok,"^(?:OK)" },
                {GrblResponseEnum.Error,"^(?:ERROR:)([^\n]*)" },
                {GrblResponseEnum.Alarm,"^(?:ALARM:)([^\n]*)" },
                {GrblResponseEnum.Setting,"^[$]([0-9]+)=([-]?[0-9]+[/.]?[0-9]*)" },
                {GrblResponseEnum.StartupBlock,"^[$][N]([01])=([^\n]{0,79})" },
                {GrblResponseEnum.Probe,@"^[\[]PRB:([-]?[0-9]+[/.]?[0-9]*),([-]?[0-9]+[/.]?[0-9]*),([-]?[0-9]+[/.]?[0-9]*):([01])[\]]" },
                {GrblResponseEnum.Coordinate,@"^[\[]([\w+]+):([-]?[0-9]+[/.]?[0-9]*),([-]?[0-9]+[/.]?[0-9]*),([-]?[0-9]+[/.]?[0-9]*)[\]]" },
                {GrblResponseEnum.ToolLengthOffset,@"^[\[]TLO:([-]?[0-9]+[/.]?[0-9]*)[\]]" },
                {GrblResponseEnum.ParserState,@"^\[(?:(?:[ ]*([A-Z])[ ]*([-]?[0-9]+[/.]?[0-9]*))+)\]" },
                {GrblResponseEnum.Parameter,@"^[\[]([^\]]+)[\]]" },
                {GrblResponseEnum.Status, @"^<(\w+),MPOS:(-?[0-9]+[.][0-9]*),(-?[0-9]+[.][0-9]*),(-?[0-9]+[.][0-9]*),WPOS:(-?[0-9]+[.][0-9]*),(-?[0-9]+[.][0-9]*),(-?[0-9]+[.][0-9]*)>" }
            };

        GrblResponseEnum _responseType = GrblResponseEnum.Unknown;
        GrblRequest _request = null;
        List<string> _items = new List<string>();

        public virtual GrblResponseEnum ResponseType { get { return _responseType; } set { _responseType = value; } }
        public long Duration { get { return (Request != null && Request.IsStamped) ? Timestamp - Request.Timestamp: -1;} }
        public List<string> Items { get { return _items; } }
        public override bool IsFinished { get { return base.IsFinished || (IsResponseType(GrblResponseEnum.Unknown) && IsStamped); } }
        public GrblRequest Request { get { return _request; } protected set { _request = value; } }

        public virtual bool IsResponseType(GrblResponseEnum compare)
        {
            return ResponseType == compare;
        }

        //REFACTOR
        public override bool SetContent(string line)
        {
            bool result = false;
            line = line.ToUpper();            

            MatchCollection resultMatches = null;
            ResponseType = GrblResponseEnum.Unknown;
            foreach (var pattern in ResponseRegex)
            {
                MatchCollection match = Regex.Matches(line, pattern.Value);
                if (match.Count > 0)
                {
                    if (IsResponseType(GrblResponseEnum.Unknown))
                    {
                        resultMatches = match;
                        ResponseType = pattern.Key;
                        result = true;
                    }
                    else
                    {
                        if ((IsResponseType(GrblResponseEnum.Coordinate) || IsResponseType(GrblResponseEnum.ToolLengthOffset) ||
                             IsResponseType(GrblResponseEnum.Probe) || IsResponseType(GrblResponseEnum.ParserState)) &&
                             pattern.Key == GrblResponseEnum.Parameter)
                            continue;
                        result = false;
                        break;
                    }
                }
            }

            if (result)
            {
                ParseRegexMatches(resultMatches);
                base.SetContent(line);
                if (IsValid)
                    Stamp();
            }
            return result;
        }

        private void ParseRegexMatches(MatchCollection resultMatches)
        {
            if (resultMatches != null)
            {
                foreach (Match resultMatch in resultMatches)
                {
                    for (int i = 1; i < resultMatch.Groups.Count; i++)
                    {
                        for (int j = 0; j < resultMatch.Groups[i].Captures.Count; j++)
                        {
                            _items.Add(null);
                        }
                    }
                    for (int i = 1; i < resultMatch.Groups.Count; i++)
                    {
                        int groupCnt = resultMatch.Groups.Count - 1;
                        for (int j = 0; j < resultMatch.Groups[i].Captures.Count; j++)
                        {
                            _items[groupCnt * j + i - 1] = resultMatch.Groups[i].Captures[j].Value;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return base.ToString() + ":" + ResponseType.ToString();
        }

        public virtual bool SetRequest(GrblRequest request)
        {
            if (IsResponseType(GrblResponseEnum.Status) && request.IsRequestType(GrblRequestEnum.CurrentStatus))
            {
                Request = request;
                return true;
            }
            else if(request.IsValid)
            {
                Request = request;
                return true;
            }
            return false;
        }     
    }

}
