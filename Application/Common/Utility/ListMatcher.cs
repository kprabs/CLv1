namespace CoreLib.Application.Common.Utility
{
    public class ListMatch<Type1, Type2>
    {
        public Type1? Obj1 { get; set; }
        public Type2? Obj2 { get; set; }
    }

    public class ListMatcher<Type1, Type2>
    {
        public IList<Type1> OnlyIn1 { get; set; }
        public IList<Type2> OnlyIn2 { get; set; }

        public IList<ListMatch<Type1, Type2>> InBoth { get; set; }

        public ListMatcher(ICollection<Type1> list1, ICollection<Type2> list2, Func<Type1, Type2, bool> AreMatch)
            : this(list1, list2, AreMatch, false)
        {

        }
        public ListMatcher(ICollection<Type1> list1, ICollection<Type2> list2, Func<Type1, Type2, bool> AreMatch, bool throwExceptionOnDuplicate)
        {
            OnlyIn1 = new List<Type1>();
            OnlyIn2 = new List<Type2>();
            InBoth = new List<ListMatch<Type1, Type2>>();

            List<Type2> remaining2 = new List<Type2>(list2);
            if (list1 != null)
            {
                foreach (var obj1 in list1)
                {
                    var obj2Set = list2.Where(x => AreMatch(obj1, x)).ToList();
                    if (obj2Set.Any())
                    {

                        if (throwExceptionOnDuplicate && obj2Set.Count > 1)
                        {
                            throw new Exception("Duplicate Items Matched");
                        }

                        var obj2 = obj2Set.First();
                        InBoth.Add(new ListMatch<Type1, Type2> { Obj1 = obj1, Obj2 = obj2 });
                        remaining2.Remove(obj2);
                    }
                    else
                    {
                        OnlyIn1.Add(obj1);
                    }
                }
            }

            foreach (var obj2 in remaining2)
            {
                OnlyIn2.Add(obj2);
            }
        }
    }
}
