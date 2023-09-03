using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betfair.ESAClient.Protocols
{
    internal class ChangeMessage <T>
    {
        private readonly DateTime _receivedTime;

        public ChangeMessage()
        {
            _receivedTime = DateTime.UtcNow;
        }

        public long? Pt { get; set; }
        public int Id { get; set; }
        public string Clk { get; set; }
        public string InitialClk { get; set; }
        public long? Heartbeatms { get; set; }
        public long ConflateMs { get; set; }

        public List<T> Items { get; set; }
        public SegmentType _segmentType { get; set; }

        public ChangeType _changeType { get; set; }

        public bool IsStartOfNewSubscription
        {
            get
            {
                return _changeType == ChangeType.SUB_IMAGE &&
                    (_segmentType == SegmentType.NONE || _segmentType == SegmentType.SEG_START);
            }
        }

        public bool IsStartOfRecovery
        { 
            get
            {
                return (_changeType == ChangeType.SUB_IMAGE || _changeType == ChangeType.RESUB_DELTA) &&
                    (_segmentType == SegmentType.NONE || _segmentType == SegmentType.SEG_START);
            }
        }

        public bool IsEndOfRecovery
        {
            get
            {
                return (_changeType == ChangeType.SUB_IMAGE || _changeType == ChangeType.RESUB_DELTA) &&
                    (_segmentType == SegmentType.NONE || _segmentType == SegmentType.SEG_END);
            }
        }

        public DateTime ArrivalTime
        {
            get
            {
                return _receivedTime;
            }
        }
    }
}
