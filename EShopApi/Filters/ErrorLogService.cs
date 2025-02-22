namespace EShopApi.Filters
{
    public static class ErrorLogService 
    { 
        public static bool IsInitialized { get; private set; }

        public static void Init()
        {
            if (!IsInitialized)
            { 
                IsInitialized = true;
            }
        } 
    }
}