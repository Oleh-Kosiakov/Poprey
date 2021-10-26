using System.Collections.Generic;
using Poprey.Core.Models.Instagram;

namespace Poprey.Core.Models.Bag
{
    public class Order
    {
        public string SystemName { get; set; }

        public string ServiceName { get; set; }

        public string TariffType { get; set; }

        public int TariffPlan { get; set; }

        public double Price { get; set; }

        public int Discount { get; set; }

        public string UserEmail { get; set; }

        public string UserNickname { get; set; }

        public List<InstagramPost> Posts { get; set; }

        public int NewPostsCount { get; set; }

        public string ResourceUrl { get; set; }

        public int? Impressions { get; set; }

        public List<string> CommentsList { get; set; }
        public bool IsAdditionalService { get; set; }
    }
}