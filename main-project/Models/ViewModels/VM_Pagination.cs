namespace main_project.Models.ViewModels
{
    public class VM_Pagination<T> : List<T>
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public string Url { get; set; }

        public VM_Pagination(List<T> items, int page, int itemsPerPage, int totalItems, string url)
        {
            AddRange(items);
            Page = page;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);
            Url = url;
        }

        public bool HasPreviousPage()
        {
            return Page > 1;
        }

        public bool HasNextPage()
        {
            return Page < TotalPages;
        }

        private int StartPage()
        {
            if (Page == 1)
            {
                return 1;
            }
            if (Page == TotalPages)
            {
                return TotalPages - 2 > 0 ? TotalPages - 2 : 1;
            }
            return Page - 1;
        }

        private int EndPage()
        {
            return Math.Min(Page + 2, TotalPages);
        }

        public List<int> GetRange()
        {
            List<int> range = new List<int>();
            for (int i = StartPage(); i <= EndPage(); i += 1)
            {
                range.Add(i);
            }
            return range;
        }

        public static VM_Pagination<T> Create(List<T> items, int page, int itemsPerPage, int totalItems, string url)
        {
            return new VM_Pagination<T>(items, page, itemsPerPage, totalItems, url);
        }

        public static VM_Pagination<T> Create(IQueryable<T> source, int page, int itemsPerPage, string url)
        {
            var totalItems = source.Count();
            var items = source.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
            return new VM_Pagination<T>(items, page, itemsPerPage, totalItems, url);
        }
    }
}