using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CutIT.Messages
{
    public enum GrblRequestEnum
    {
        GCode = 0,                       //G ..
        CurrentStatus = 1,               //?
        FeedHold = 2,                    //!
        CycleStart = 3,                  //~
        Reset = 4,                       //ctrl-x
        GrblSettings = 5,                //$$
        ParserSettings = 6,              //$#
        ParserState = 7,                 //$G
        GrblSetting = 8,                 //$1= ..
        StartupBlocks = 9,               //$N
        StartupBlock = 10,               //$N1= ..            
        GetBuildInfo = 11,               //$I
        SetBuildInfo = 12,               //$I= ..
        CheckMode = 13,                  //$C
        KillAlarmLock = 14,              //$X
        Homing = 15,                     //$H
        Unknown = 99                     //other
    }

    public class GrblRequest : Message
    {
        public static Dictionary<GrblRequestEnum, string> RequestRegex =
            new Dictionary<GrblRequestEnum, string>()
            {
                {GrblRequestEnum.GCode,             "^(?:(?:[ ]*([A-Z])[ ]*([-]?[0-9]+[/.]?[0-9]*))+)" },
                {GrblRequestEnum.CurrentStatus,     "^[?]"},
                {GrblRequestEnum.FeedHold,          "^[!]"},
                {GrblRequestEnum.CycleStart,        "^[~]"},
                {GrblRequestEnum.Reset,             "^[\x18]"},
                {GrblRequestEnum.GrblSettings,      "^[$]{2}"},
                {GrblRequestEnum.ParserSettings,    "^[$][#]"},
                {GrblRequestEnum.ParserState,       "^[$][G]"},
                {GrblRequestEnum.GrblSetting,       "^[$](?<p1>[0-9]+)[=](?<p2>[0-9]+[.]?[0-9]*)"},
                {GrblRequestEnum.StartupBlocks,     "^[$][N]"},
                {GrblRequestEnum.StartupBlock,      "^[$][N](?<p1>[12])[=](?<p2>[^\n]*)"},
                {GrblRequestEnum.GetBuildInfo,      "^[$][I]"},
                {GrblRequestEnum.SetBuildInfo,      "^[$][I]=(?<p1>[^\n]{1,79})"},
                {GrblRequestEnum.CheckMode,         "^[$][C]"},
                {GrblRequestEnum.KillAlarmLock,     "^[$][X]"},
                {GrblRequestEnum.Homing,            "^[$][H]"}
            };

        GrblRequestEnum _requestType = GrblRequestEnum.Unknown;
        List<string> _items = new List<string>();

        public virtual GrblRequestEnum RequestType
        {
            get { return _requestType; }
            protected set { _requestType = value; }
        }

        GrblResponse _response = null;
        public virtual GrblResponse Response
        {
            get { return _response; }
            protected set { _response = value; }
        }

        public virtual List<string> Items { get { return _items; } }

        public virtual bool IsRequestType(GrblRequestEnum compare)
        {
            return RequestType == compare;
        }

        public virtual bool IsSpecial
        {
            get
            {
                return IsRequestType(GrblRequestEnum.CycleStart) ||
                       IsRequestType(GrblRequestEnum.FeedHold) ||
                       IsRequestType(GrblRequestEnum.Reset) ||
                       IsRequestType(GrblRequestEnum.CurrentStatus);
            }
        }

        public override bool IsValid
        {
            get
            {
                return base.IsValid && !IsRequestType(GrblRequestEnum.Unknown);
            }
        }

        public override bool IsFinished
        {
            get
            {
                if (IsSpecial && !IsRequestType(GrblRequestEnum.CurrentStatus))
                    return base.IsFinished;
                return !IsRequestType(GrblRequestEnum.Unknown) && Response != null && base.IsFinished;
            }
        }

        public virtual long Duration { get { if (Response != null) return Response.Timestamp - Timestamp; return -1; } }

        public override bool SetContent(string content)
        {
            content = content.ToUpper();
            bool result = false;
            MatchCollection resultMatches = null;
            RequestType = GrblRequestEnum.Unknown;
            foreach (var pattern in RequestRegex)
            {
                MatchCollection match = Regex.Matches(content, pattern.Value);
                if (match.Count > 0)
                {
                    if (IsRequestType(GrblRequestEnum.Unknown))
                    {
                        resultMatches = match;
                        RequestType = (GrblRequestEnum)Enum.Parse(typeof(GrblRequestEnum), pattern.Key.ToString());
                        result = true;
                    }
                    else
                    {
                        resultMatches = null;
                        result = false;
                        RequestType = GrblRequestEnum.Unknown;
                        break;
                    }
                }
            }
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

            if (result)
            {
                base.SetContent(content);
            }
            return result;
        }

        public virtual bool SetResponse(GrblResponse response)
        {
            if (response.IsStamped)
            {
                if (IsRequestType(GrblRequestEnum.GetBuildInfo))
                {
                    if (response.IsResponseType(GrblResponseEnum.Parameter))
                    {
                        Response = response;
                        Response.ResponseType = GrblResponseEnum.BuildInfo;
                        return true;
                    }
                }else if (IsRequestType(GrblRequestEnum.CurrentStatus))
                {
                    if (response.IsResponseType(GrblResponseEnum.Status))
                    {
                        Response = response;
                        return true;
                    }
                }
                if (response.IsResponseType(GrblResponseEnum.Ok) ||
                   response.IsResponseType(GrblResponseEnum.Error) ||
                   response.IsResponseType(GrblResponseEnum.Alarm))
                {
                    Response = response;
                    return true;
                }
            }
            return false;
        }

    }
}
