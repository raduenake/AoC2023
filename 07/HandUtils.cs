namespace _07
{
    public static class HandUtil
    {
        public static int CalculateHandWeight(List<string> cardList)
        {
            var groupCards = cardList.GroupBy(c => c);
            var w = 0;
            w = groupCards.Count() switch
            {
                1 => 1000,
                2 => groupCards.Any(g => g.Count() == 4) ? 900 : 800,
                3 => groupCards.Any(g => g.Count() == 3) ? 700 : 600,
                4 => 500,
                _ => 1
            };

            return w;
        }

        public static int CalculateMaxHandWeight(List<string> cardList)
        {
            var cards = new[] { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J' };
            var xCombined = cards.Where(c => c != 'J')
                .Select(c =>
                {
                    var h = cardList.Select(xC => xC == "J" ? "" + c : xC).ToList();
                    return (hand: h, weight: CalculateHandWeight(h));
                });
            return xCombined.Max(hh => hh.weight);
        }
    }

    public class CardComparer : IComparer<List<string>>
    {
        public int Compare(List<string>? x, List<string>? y)
        {
            if (x == null || y == null)
            {
                throw new NullReferenceException();
            }

            int result = HandUtil.CalculateHandWeight(x).CompareTo(HandUtil.CalculateHandWeight(y));
            if (result == 0)
            {
                var cW = (new[] { 'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2' })
                    .Select((c, i) => (card: "" + c, score: 14 - i - 1))
                    .ToDictionary(kSel => kSel.card, vSel => vSel.score);

                // high card in 'order'
                for (int i = 0; i < x.Count(); i++)
                {
                    var xW = cW[x[i]];
                    var yW = cW[y[i]];

                    if (xW != yW)
                    {
                        result = xW.CompareTo(yW);
                        break;
                    }
                }
            }

            return result;
        }
    }

    public class CardComparerJoker : IComparer<List<string>>
    {
        public int Compare(List<string>? x, List<string>? y)
        {
            if (x == null || y == null)
            {
                throw new NullReferenceException();
            }

            int result = HandUtil.CalculateMaxHandWeight(x).CompareTo(HandUtil.CalculateMaxHandWeight(y));
            if (result == 0)
            {
                var cards = new[] { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J' };

                var cW = cards.Select((c, i) => (card: "" + c, score: 14 - i - 1))
                .ToDictionary(kSel => kSel.card, vSel => vSel.score);
                // high card in 'order'
                for (int i = 0; i < x.Count(); i++)
                {
                    var xW = cW[x[i]];
                    var yW = cW[y[i]];

                    if (xW != yW)
                    {
                        result = xW.CompareTo(yW);
                        break;
                    }
                }
            }

            return result;
        }
    }
}
