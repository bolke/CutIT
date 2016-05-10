using CutIT.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutIT.Messages
{
    public abstract class Message
    {
        long _timestamp = 0;
        string _content = "";

        public virtual bool IsFinished { get { return IsStamped; } }
        public virtual long Timestamp { get { return _timestamp; } protected set { _timestamp = value; } }
        public virtual bool IsStamped { get { return Timestamp != 0; } }
        public virtual string Content { get { return _content; } protected set { _content = value; } }
        public virtual bool IsValid { get { return Content.Length > 0; } }

        public Message()
        {
        }

        public virtual bool Stamp()
        {
            {
                if (Timestamp == 0)
                {
                    Timestamp = TimeUtility.GetCurrentUnixTimestampMillis();
                    return true;
                }
                return false;
            }
        }

        public virtual void Clear()
        {
            Timestamp = 0;
            _content = "";
        }

        public virtual bool SetContent(string content)
        {
            if (_content.Length == 0)
            {
                _content = content;
                return true;
            }
            return false;
        }

    }
}
