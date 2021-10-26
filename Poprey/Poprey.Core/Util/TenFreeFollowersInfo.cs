using System;

namespace Poprey.Core.Util
{
    public class TenFreeFollowersInfo
    {
        public string InstagramNickname { get; set; }

        public string Email { get; set; }

        public long SerializedDateOfUsageInTicks { get; set; }

        public void SetDate(DateTime dateTime)
        {
            SerializedDateOfUsageInTicks = dateTime.Ticks;
        }

        public DateTime GetDate()
        {
            return new DateTime(SerializedDateOfUsageInTicks);
        }
    }
}