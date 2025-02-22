namespace EShopModels.Common
{
    public static class QueryablePageListExtensions
    { 
        public static async Task<IList<T>> ToEShopListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, int indexFrom = 0)
        {
            if (indexFrom > (pageNumber-1))
            {
                throw new ArgumentException($"indexFrom: {indexFrom} > pageNumber: {pageNumber}, must indexFrom <= pageNumber");
            }

            var items = source.Skip(((pageNumber - 1) - indexFrom) * pageSize).Take(pageSize);

            List<T> pagedList = new(items);
           
            return pagedList;
        }
    } 
}
